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
        [Header("Sub Variables")]
        public int gameOverIndex = 5;                       // The scene index that the game over scene is at. 
        public Transform respawnPlayerLocation;             // The location that the player will respawn at when the battle is over
        public List<EnemyData> listOfEnemiesInFight;        // A list of all of the enemies that the event will spawn in the battle

        // Private Variables
        private int origSceneIndex;                         // A reference to the original scene that the player was originally in

        private Vector3 origCameraLocation;                 // Hard references to the player camera and values that the camera was before the battle
        private Quaternion origCameraRotation;
        private float origCameraMinX;                
        private float origCameraMaxX;
        private float origCameraMinY;
        private float origCameraMaxY;
        private float origCameraMinZ;
        private float origCameraMaxZ;

        // When this event is activated, we start up the battle.
		private void OnEnable()
		{
            if(hasActivated == false)
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
                    // TODO: We first reward the player with their rewards as well as mark this event being completed permenatly.

                    // And then warp them back using the saved coords and deactivate this event.
                    StartCoroutine(gameObject.GetComponent<TransitionArea>().GoToSpecificScene(origSceneIndex));
                }
            }
        }
    
        // Saves the new information to the player's party and rewards them exp and gold
        public void PostBattleActions(List<BattleStats> currentPlayerPartyInBattle)
        {
            PlayerInventory currentParty = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();

            foreach(BattleStats currentPartyMember in currentPlayerPartyInBattle)
            {
                CharacterData comparedPartyMember = (CharacterData)currentPartyMember.battleData;
                for(int currPartyIndex = 0; currPartyIndex < currentParty.GetPartyInvetorySize(); currPartyIndex++)
                {
                    // We check if we are looking at the same character
                    if(comparedPartyMember.characterName == currentParty.GetInventoryCharacterAtIndex(currPartyIndex).SpecifiedCharacter.characterName)
                    {
                        // We save the HP/SP that the character has to the inventory
                        currentPartyMember.SaveCurrentHPSP(currentParty.GetInventoryCharacterAtIndex(currPartyIndex));
                    }
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
            player.WarpCharacter(respawnPlayerLocation.position);
            mainCamera.transform.position = origCameraLocation;
            mainCamera.transform.rotation = origCameraRotation;
        }
    }
}