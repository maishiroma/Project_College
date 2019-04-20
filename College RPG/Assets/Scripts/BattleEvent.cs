/*  This script handles taking the player from the main gameplay to the battle scene
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MattScripts {

    [RequireComponent(typeof(TransitionArea))]
    public class BattleEvent : BaseEvent
    {
        // TODO: Determine how EXP and gold are rewarded after fight
        [Header("Sub Variables")]
        public Transform respawnPlayerLocation;
        public List<EnemyData> listOfEnemiesInFight;

        // Private Variables
        private int origSceneIndex;
        private Vector3 origCameraLocation;
        private Quaternion origCameraRotation;

        private float origCameraMinX;                
        private float origCameraMaxX;
        private float origCameraMinY;
        private float origCameraMaxY;
        private float origCameraMinZ;
        private float origCameraMaxZ;

        // Upon activating, we start up the battle.
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

            DontDestroyOnLoad(gameObject);
		}

        // We conclude the event and take the player back to the original scene
        public override void EventOutcome()
        {
            // TODO: We first reward the player with their rewards 

            // And then warp them back using the saved coords and deactivate this event.
            gameObject.transform.parent = GameManager.Instance.gameObject.transform;
            StartCoroutine(gameObject.GetComponent<TransitionArea>().ReturnToOrigScene(origSceneIndex));
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