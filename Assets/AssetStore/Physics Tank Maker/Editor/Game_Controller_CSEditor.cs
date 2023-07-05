using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

    [CustomEditor(typeof(Game_Controller_CS))]
    public class Game_Controller_CSEditor : Editor
    {

        SerializedProperty Fix_Frame_RateProp;
        SerializedProperty Target_Frame_RateProp;
        SerializedProperty Fixed_TimeStepProp;
        SerializedProperty Sleep_ThresholdProp;
        SerializedProperty Pause_CanvasProp;


        void OnEnable()
        {
            Fix_Frame_RateProp = serializedObject.FindProperty("Fix_Frame_Rate");
            Target_Frame_RateProp = serializedObject.FindProperty("Target_Frame_Rate");
            Fixed_TimeStepProp = serializedObject.FindProperty("Fixed_TimeStep");
            Sleep_ThresholdProp = serializedObject.FindProperty("Sleep_Threshold");
            Pause_CanvasProp = serializedObject.FindProperty("Pause_Canvas");
        }


        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
            serializedObject.Update();

            // Physics settings.
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Frame Rate settings", MessageType.None, true);
            Fix_Frame_RateProp.boolValue = EditorGUILayout.Toggle("Fix Frame Rate", Fix_Frame_RateProp.boolValue);
            if (Fix_Frame_RateProp.boolValue)
            {
                if (GUILayout.Button("Set according to 'Fixed TimeStep'", GUILayout.Width(250)))
                {
                    Target_Frame_RateProp.intValue = (int)(1.0f / Fixed_TimeStepProp.floatValue);
                }
                EditorGUILayout.IntSlider(Target_Frame_RateProp, 30, 120, "Target Frame_Rate");
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Physics settings", MessageType.None, true);
            EditorGUILayout.Slider(Fixed_TimeStepProp, 0.005f, 0.05f, "Fixed TimeStep");
            EditorGUILayout.Slider(Sleep_ThresholdProp, 0.00f, 10.0f, "Sleep Threshold");

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Pause settings", MessageType.None, true);
            Pause_CanvasProp.objectReferenceValue = EditorGUILayout.ObjectField("Pause Canvas", Pause_CanvasProp.objectReferenceValue, typeof(Canvas), true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

    }

}