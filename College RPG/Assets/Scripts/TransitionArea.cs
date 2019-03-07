/*  This covers the code used to move player from one area to another
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace MattScripts {
    
    public class TransitionArea : MonoBehaviour {

        public bool toNewScene;
        public int newSceneIndex;
        public float fadeTime = 1f;
        public float lerpValue = 0.1f;

        public float cameraMinX;
        public float cameraMaxX;
        public float cameraMinY;
        public float cameraMaxY;
        public float cameraMinZ;
        public float cameraMaxZ;

        public BoxCollider activateArea;
        public Transform travelSpot;
        public Image fadeOverlay;

        // Private Variables
        private float alpha = 0f;
        private bool isTraveling = false;
        private bool isFading = false;

		// Handles the core logic of the transition cutscene
		private void Update()
        {
            if(isTraveling == true)
            {
                // Fading to black
                if(isFading == true)
                {
                    alpha = Mathf.Lerp(alpha, 1, lerpValue);
                }
                // Fading to transparent
                else
                {
                    alpha = Mathf.Lerp(alpha, 0, lerpValue);
                }
                // Updating the alpha of the fadeout
                Color newColor = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, alpha);
                fadeOverlay.color = newColor;
            }
        }

        // While the player entered the trigger zone, they will travel to another point
        private void OnTriggerEnter(Collider collision)
        {
            if(isTraveling == false && collision.CompareTag("Player"))
            {
                isTraveling = true;
                StartCoroutine("TravelCutscene", collision.gameObject);
            }
        }

        // This is used to time the transition
        private IEnumerator TravelCutscene(GameObject player)
        {
            // Getting the proper components
            CharacterController playerController = player.GetComponent<CharacterController>();

            // We stop the player from moving and fade to black
            GameManager.Instance.CurrentState = GameStates.TRAVEL;
            playerController.DisableController();
            isFading = true;
            yield return new WaitForSeconds(fadeTime);

            if(toNewScene)
            {
                // We load to the new scene and exit
                LoadingScreenManager.LoadScene(newSceneIndex);
                yield return null;
            }
            else
            {
                // We then teleport the player, change their respawn point
                playerController.WarpPlayer(travelSpot.position);
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
                isFading = false;
                yield return new WaitForSeconds(fadeTime);

                // We tell this invoke we are done!
                playerController.EnableController();
                isTraveling = false;
                GameManager.Instance.CurrentState = GameStates.NORMAL;
                yield return null;
            }
        }
    }

    // This is used to make a custom editor so that this class can be more flexible in displaying information
    [CustomEditor(typeof(TransitionArea))]
    public class TransitionAreaEditor : Editor {

        public override void OnInspectorGUI()
        {
            TransitionArea myScript = target as TransitionArea;

            // This toggles showing specific variables depending on the boolean passed
            GUILayout.Space(5f);
            myScript.toNewScene = EditorGUILayout.Toggle("Warp To New Scene?", myScript.toNewScene);
            EditorGUI.indentLevel++;
            if(myScript.toNewScene == true)
            {                
                myScript.newSceneIndex = EditorGUILayout.IntField("New Scene Index", myScript.newSceneIndex);
                EditorGUI.indentLevel--;
            }
            else
            {
                myScript.travelSpot = (Transform)EditorGUILayout.ObjectField("Travel Spot", myScript.travelSpot, typeof(GameObject), true);

                GUILayout.Space(5f);
                EditorGUILayout.PrefixLabel("Camera Variables");
                GUILayout.Space(3f);

                EditorGUI.indentLevel++;
                myScript.cameraMinX = EditorGUILayout.FloatField("Min X", myScript.cameraMinX);
                myScript.cameraMaxX = EditorGUILayout.FloatField("Max X", myScript.cameraMaxX);
                myScript.cameraMinY = EditorGUILayout.FloatField("Min Y", myScript.cameraMinY);
                myScript.cameraMaxY = EditorGUILayout.FloatField("Max Y", myScript.cameraMaxY);
                myScript.cameraMinZ = EditorGUILayout.FloatField("Min Z", myScript.cameraMinZ);
                myScript.cameraMaxZ = EditorGUILayout.FloatField("Max Z", myScript.cameraMaxZ);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(5f);
            EditorGUILayout.PrefixLabel("General Variables");
            GUILayout.Space(3f);

            EditorGUI.indentLevel++;
            myScript.fadeTime = EditorGUILayout.Slider("Fade Time", myScript.fadeTime, 1f, 2f);
            myScript.lerpValue = EditorGUILayout.Slider("Lerp Value", myScript.lerpValue, 0.1f, 1f);
            EditorGUI.indentLevel--;

            GUILayout.Space(5f);
            EditorGUILayout.PrefixLabel("External References");
            GUILayout.Space(3f);

            EditorGUI.indentLevel++;
            myScript.activateArea = (BoxCollider)EditorGUILayout.ObjectField("Activate Area", myScript.activateArea, typeof(GameObject), true);
            myScript.fadeOverlay = (Image)EditorGUILayout.ObjectField("Fade Overlay", myScript.fadeOverlay, typeof(GameObject), true);
            EditorGUI.indentLevel--;
        }
    }
}