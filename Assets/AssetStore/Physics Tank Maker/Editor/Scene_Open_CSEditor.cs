using UnityEngine;
using UnityEditor;


namespace ChobiAssets.PTM
{

    [CustomEditor(typeof(Scene_Open_CS))]
    public class Scene_Open_CSEditor : Editor
    {

        SerializedProperty Scene_TypeProp;
        SerializedProperty Scene_NameProp;


        string[] typeNames = { "Input Manually", "Current Scene", "Menu Scene", "Battle Scene" };


        void OnEnable()
        {
            Scene_TypeProp = serializedObject.FindProperty("Scene_Type");
            Scene_NameProp = serializedObject.FindProperty("Scene_Name");
        }


        public override void OnInspectorGUI()
        {
            if (Application.isPlaying == false)
            {
                Set_Inspector();
            }
        }


        void Set_Inspector()
        {
            GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Open Scene Settings", MessageType.None, true);
            EditorGUILayout.Space();


            Scene_TypeProp.intValue = EditorGUILayout.Popup("Scene Type", Scene_TypeProp.intValue, typeNames);
            if (Scene_TypeProp.intValue == 0)
            { // Input the scene name manually.
                Scene_NameProp.stringValue = EditorGUILayout.TextField("Scene Name", Scene_NameProp.stringValue);
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
