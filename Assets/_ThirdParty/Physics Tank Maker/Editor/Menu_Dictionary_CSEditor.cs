using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Menu_Dictionary_CS))]
	public class Menu_Dictionary_CSEditor : Editor
	{

        SerializedProperty Scene_TypeProp;
        SerializedProperty Battle_Scene_NameProp;

        string[] typeNames = { "Input Manually", "Battle Scene" };


        void OnEnable ()
		{
            Scene_TypeProp = serializedObject.FindProperty("Scene_Type");
            Battle_Scene_NameProp = serializedObject.FindProperty ("Battle_Scene_Name");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Dictionary settings", MessageType.None, true);
            Scene_TypeProp.intValue = EditorGUILayout.Popup("Scene Type", Scene_TypeProp.intValue, typeNames);
            if (Scene_TypeProp.intValue == 0)
            { // Input the scene name manually.
                Battle_Scene_NameProp.stringValue = EditorGUILayout.TextField("Battle Scene Name", Battle_Scene_NameProp.stringValue);
            }

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}