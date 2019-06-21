/*  This covers the code used to move player from one area to another
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace MattScripts {
    
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