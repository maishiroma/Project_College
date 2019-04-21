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

    // This is used to make a custom editor so that this class can be more flexible in displaying information
    // Refer to https://docs.unity3d.com/ScriptReference/Editor.html for help on this
    [CustomEditor(typeof(TransitionArea))]
    [CanEditMultipleObjects]
    public class TransitionAreaEditor : Editor {

        SerializedProperty showTriggerArea_Prop;
        SerializedProperty toNewScene_Prop;
        SerializedProperty newSceneIndex_Prop;
        SerializedProperty fadeTime_Prop;
        SerializedProperty lerpValue_Prop;

        SerializedProperty cameraMinX_Prop;
        SerializedProperty cameraMaxX_Prop;
        SerializedProperty cameraMinY_Prop;
        SerializedProperty cameraMaxY_Prop;
        SerializedProperty cameraMinZ_Prop;
        SerializedProperty cameraMaxZ_Prop;

        SerializedProperty activateArea_Prop;
        SerializedProperty travelSpot_Prop;
        SerializedProperty fadeOverlay_Prop;

        // Sets up all of the serialized properties
		private void OnEnable()
		{
            showTriggerArea_Prop = serializedObject.FindProperty("showTriggerArea");
            toNewScene_Prop = serializedObject.FindProperty("toNewScene");
            newSceneIndex_Prop = serializedObject.FindProperty("newSceneIndex");
            fadeTime_Prop = serializedObject.FindProperty("fadeTime");
            lerpValue_Prop = serializedObject.FindProperty("lerpValue");

            cameraMinX_Prop = serializedObject.FindProperty("cameraMinX");
            cameraMaxX_Prop = serializedObject.FindProperty("cameraMaxX");
            cameraMinY_Prop = serializedObject.FindProperty("cameraMinY");
            cameraMaxY_Prop = serializedObject.FindProperty("cameraMaxY");
            cameraMinZ_Prop = serializedObject.FindProperty("cameraMinZ");
            cameraMaxZ_Prop = serializedObject.FindProperty("cameraMaxZ");

            activateArea_Prop = serializedObject.FindProperty("activateArea");
            travelSpot_Prop = serializedObject.FindProperty("travelSpot");
            fadeOverlay_Prop = serializedObject.FindProperty("fadeOverlay");
		}

        // We override the Editor GUI on how these values are seen with our own logic
		public override void OnInspectorGUI()
        {
            // ALWAYS CALL THIS FIRST here!
            serializedObject.Update();

            // This toggles showing specific variables depending on the boolean passed
            GUILayout.Space(5f);
            toNewScene_Prop.boolValue = EditorGUILayout.Toggle("Warp To New Scene?", toNewScene_Prop.boolValue);
            EditorGUI.indentLevel++;
            if(toNewScene_Prop.boolValue == true)
            {                
                newSceneIndex_Prop.intValue = EditorGUILayout.IntField("New Scene Index", newSceneIndex_Prop.intValue);
                EditorGUI.indentLevel--;
            }
            else
            {
                travelSpot_Prop.objectReferenceValue = EditorGUILayout.ObjectField("Travel Spot", travelSpot_Prop.objectReferenceValue, typeof(Transform), true);

                GUILayout.Space(5f);
                EditorGUILayout.PrefixLabel("Camera Variables");
                GUILayout.Space(3f);

                EditorGUI.indentLevel++;
                cameraMinX_Prop.floatValue = EditorGUILayout.FloatField("Min X", cameraMinX_Prop.floatValue);
                cameraMaxX_Prop.floatValue = EditorGUILayout.FloatField("Max X", cameraMaxX_Prop.floatValue);

                cameraMinY_Prop.floatValue = EditorGUILayout.FloatField("Min Y", cameraMinY_Prop.floatValue);
                cameraMaxY_Prop.floatValue = EditorGUILayout.FloatField("Max Y", cameraMaxY_Prop.floatValue);

                cameraMinZ_Prop.floatValue = EditorGUILayout.FloatField("Min Z", cameraMinZ_Prop.floatValue);
                cameraMaxZ_Prop.floatValue = EditorGUILayout.FloatField("Max Z", cameraMaxZ_Prop.floatValue);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(5f);
            EditorGUILayout.PrefixLabel("General Variables");
            GUILayout.Space(3f);

            EditorGUI.indentLevel++;
            showTriggerArea_Prop.boolValue = EditorGUILayout.Toggle("Show Trigger?", showTriggerArea_Prop.boolValue);
            fadeTime_Prop.floatValue = EditorGUILayout.Slider("Fade Time", fadeTime_Prop.floatValue, 0.5f, 2f);
            lerpValue_Prop.floatValue = EditorGUILayout.Slider("Lerp Value", lerpValue_Prop.floatValue, 0.1f, 1f);
            EditorGUI.indentLevel--;

            GUILayout.Space(5f);
            EditorGUILayout.PrefixLabel("External References");
            GUILayout.Space(3f);

            EditorGUI.indentLevel++;
            activateArea_Prop.objectReferenceValue = EditorGUILayout.ObjectField("Activate Area", activateArea_Prop.objectReferenceValue, typeof(BoxCollider), true);
            fadeOverlay_Prop.objectReferenceValue = EditorGUILayout.ObjectField("Fade Overlay", fadeOverlay_Prop.objectReferenceValue, typeof(Image), true);
            EditorGUI.indentLevel--;

            // ALWAYS END THE CALL WITH THIS
            serializedObject.ApplyModifiedProperties();
        }
    }
}