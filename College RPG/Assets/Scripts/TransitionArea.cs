/*  This covers the code used to move player from one area to another
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MattScripts {
    
    public class TransitionArea : MonoBehaviour {

        public bool showTriggerArea;            // Should this script show the box gizmo on how large the triggerbox of this is?
        public bool toNewScene;                 // Should we go to a new scene?

        public int newSceneIndex;               // What is the index number of the new scene?

        public float fadeTime = 1f;             // How long does the fade transition last?
        public float lerpValue = 0.1f;          // How quickly does the fade in/out act?

        public float cameraMinX;                // All of these vars change the binding box of the main camera
        public float cameraMaxX;
        public float cameraMinY;
        public float cameraMaxY;
        public float cameraMinZ;
        public float cameraMaxZ;

        public BoxCollider activateArea;        // Hard references to external components
        public Transform travelSpot;
        public Image fadeOverlay;

        // While the player entered the trigger zone, they will travel to another point
        private void OnTriggerEnter(Collider collision)
        {
            if(collision.CompareTag("Player") && GameManager.Instance.CurrentState != GameStates.TRAVEL)
            {
                GameManager.Instance.CurrentState = GameStates.TRAVEL;
                StartCoroutine("TravelCutscene", collision.gameObject);
            }
        }

        // When selected and when enabled, allows for the hitbox to be visualized
        private void OnDrawGizmos()
        {
            if(showTriggerArea == true)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(activateArea.gameObject.transform.position, activateArea.size);
            }
        }

        // This is used to time the transition
        private IEnumerator TravelCutscene(GameObject player)
        {
            // Getting the proper components
            CharacterController playerController = player.GetComponent<CharacterController>();

            // We stop the player from moving and fade to black
            fadeOverlay.CrossFadeAlpha(1, fadeTime, true);
            yield return new WaitForSeconds(fadeTime);

            if(toNewScene == true)
            {
                // We load to the new scene and exit
                LoadingScreenManager.LoadScene(newSceneIndex);
                yield return null;
            }
            else
            {
                // We then teleport the player, change their respawn point
                playerController.WarpCharacter(travelSpot.position);
                yield return new WaitForFixedUpdate();

                // We set the camera to have new bounds so that it will properly show the player
                GameManager.Instance.MainCamera.minXPos = cameraMinX;
                GameManager.Instance.MainCamera.maxXPos = cameraMaxX;
                GameManager.Instance.MainCamera.minYPos = cameraMinY;
                GameManager.Instance.MainCamera.maxYPos = cameraMaxY;
                GameManager.Instance.MainCamera.minZPos = cameraMinZ;
                GameManager.Instance.MainCamera.maxZPos = cameraMaxZ;
                yield return new WaitForFixedUpdate();

                // We then tell the transition to fade back in
                fadeOverlay.CrossFadeAlpha(0, fadeTime, true);
                yield return new WaitForSeconds(fadeTime);

                // We tell this invoke we are done!
                GameManager.Instance.CurrentState = GameStates.NORMAL;
                yield return null;
            }
        }
    
        // This is called from outside to return the player to the specified scene
        public IEnumerator GoToSpecificScene(int sceneIndex)
        {
            if(fadeOverlay == null)
            {
                fadeOverlay = GameObject.FindWithTag("FadeUI").GetComponent<Image>();
            }

            fadeOverlay.CrossFadeAlpha(1, fadeTime, true);
            yield return new WaitForSeconds(fadeTime);

            // We load to the new scene
            LoadingScreenManager.LoadScene(sceneIndex);
        }
    
        // Used to transition into the app quitting
        public IEnumerator QuitGame()
        {
            if(fadeOverlay == null)
            {
                fadeOverlay = GameObject.FindWithTag("FadeUI").GetComponent<Image>();
            }

            fadeOverlay.CrossFadeAlpha(1, fadeTime, true);
            yield return new WaitForSeconds(fadeTime);

            Debug.Log("We quit");
            Application.Quit();
        }
    }
}