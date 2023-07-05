using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Gun_Camera_CS))]
	public class Gun_Camera_CSEditor : Editor
	{

		SerializedProperty Gun_CameraProp;
		SerializedProperty Camera_Manager_ScriptProp;
		SerializedProperty Minimum_FOVProp;
		SerializedProperty Maximum_FOVProp;
	

		void OnEnable ()
		{
			Gun_CameraProp = serializedObject.FindProperty ("Gun_Camera");
            Camera_Manager_ScriptProp = serializedObject.FindProperty ("Camera_Manager_Script");
            Minimum_FOVProp = serializedObject.FindProperty ("Minimum_FOV");
            Maximum_FOVProp = serializedObject.FindProperty ("Maximum_FOV");
		}


		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Gun Camera settings", MessageType.None, true);
			Gun_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Gun Camera", Gun_CameraProp.objectReferenceValue, typeof(Camera), true);
            Camera_Manager_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Camera Points Manager Script", Camera_Manager_ScriptProp.objectReferenceValue, typeof(Camera_Points_Manager_CS), true);
			EditorGUILayout.Slider (Minimum_FOVProp, 1.0f, 100.0f, "Min FOV");
			EditorGUILayout.Slider (Maximum_FOVProp, 1.0f, 100.0f, "Max FOV");


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}