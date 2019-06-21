/*  This script handles taking the player from the main gameplay to the battle scene and vice verse.
 *  By default, the gameobject/script should be deactivated
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MattScripts {

    [RequireComponent(typeof(TransitionArea))]
    public class BattleEvent : BaseEvent
    {
        public static BaseEvent Instance;                   // Used to keep a reference to the current Battle Event Instance

        [Header("Sub Variables")]
        public int gameOverIndex = 5;                       // The scene index that the game over scene is at. 
        public List<EnemyData> listOfEnemiesInFight;        // A list of all of the enemies that the event will spawn in the battle

        // Private Variables
        private int origSceneIndex;                         // A reference to the original scene that the player was originally in
        private int goldReward;                 
        private int expReward;

        private Vector3 origCameraLocation;                 // Hard references to the player camera and values that the camera was before the battle
        private Quaternion origCameraRotation;
        private float origCameraMinX;                
        private float origCameraMaxX;
        private float origCameraMinY;
        private float origCameraMaxY;
        private float origCameraMinZ;
        private float origCameraMaxZ;
        private Vector3 respawnPlayerLocation;             // The location that the player will respawn at when the battle is over

        public int GoldReward {
            get { return goldReward; }
        }

        public int ExpReward {
            get { return expReward; }
        }

        // IMPORTANT
        // Makes sure that when the player returns to the main scene after a battle, the enemy that was hosting this is destroyed
		private void Awake()
		{
            if(Instance != null && Instance.name == this.name)
            {
                gameObject.transform.parent.gameObject.SetActive(false);
                Destroy(gameObject.transform.parent.gameObject);
                Instance = null;
            }
		}

		// Sets up the private variables
		private void Start()
		{
            origSceneIndex = 0;

            // Calculates how much EXP and gold this fight gives
            foreach(EnemyData currEnemy in listOfEnemiesInFight)
            {
                expReward += currEnemy.expDrop;
                goldReward += currEnemy.goldDrop;
            }
		}

		// When this event is activated, we start up the battle.
		private void OnEnable()
		{
            // NOTE! If we want to have the behavior of having the battle triggered by a trigger box, we just need to make sure Activate Area and object to interact is NULL
            if(hasActivated == false && activateArea == null && objectToInteract == null)
            {
                EventSetup();
                hasActivated = true;
            }
		}

        // We transition to the battle scene using the required component. 
		public override void EventSetup()
		{
            // We save all of the information we will need to get back
            CameraController mainCamera = GameManager.Instance.MainCamera;

            origCameraLocation = mainCamera.gameObject.transform.position; 
            origCameraRotation = mainCamera.gameObject.transform.rotation;
            origCameraMinX = mainCamera.minXPos;
            origCameraMinY = mainCamera.minYPos;
            origCameraMinZ = mainCamera.minZPos;
            origCameraMaxX = mainCamera.maxXPos;
            origCameraMaxY = mainCamera.maxYPos;
            origCameraMaxZ = mainCamera.maxZPos;
            origSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // We make this object the parent of the GameManager so that we can have an easier time finding it
            gameObject.transform.parent = GameManager.Instance.gameObject.transform;
            respawnPlayerLocation = GameManager.Instance.PlayerReference.gameObject.transform.position;
            Instance = this;
		}

        // We conclude the event and take the player back to the original scene
        public override void EventOutcome()
        {
            if(FindObjectOfType<BattleController>() != null)
            {
                BattleController controller = FindObjectOfType<BattleController>();

                if(controller.CurrentState == BattleStates.ENEMY_WIN)
                {
                    // We go to the Game Over Scene
                    GameManager.Instance.CurrentState = GameStates.GAME_OVER;
                    StartCoroutine(gameObject.GetComponent<TransitionArea>().GoToSpecificScene(gameOverIndex));

                    // Since we do not need this anymore, we will queue a destroy on this
                    Destroy(gameObject, 4f);
                }
                else if(controller.CurrentState == BattleStates.PLAYER_WIN)
                {
                    // We warp them back using the saved coords and deactivate this event.
                    StartCoroutine(gameObject.GetComponent<TransitionArea>().GoToSpecificScene(origSceneIndex));
                }
            }
        }

        // We reset the player and camera to what they were based on what we saved prior
        public void ResetPlayerAndCamera()
        {
            CharacterController player = GameManager.Instance.PlayerReference.GetComponent<CharacterController>();
            CameraController mainCamera = GameManager.Instance.MainCamera;

            mainCamera.maxXPos = origCameraMaxX;
            mainCamera.maxYPos = origCameraMaxY;
            mainCamera.maxZPos = origCameraMaxZ;
            mainCamera.minXPos = origCameraMinX;
            mainCamera.minYPos = origCameraMinY;
            mainCamera.minZPos = origCameraMinZ;
            player.WarpCharacter(respawnPlayerLocation);
            mainCamera.transform.position = origCameraLocation;
            mainCamera.transform.rotation = origCameraRotation;
        }
    }
}