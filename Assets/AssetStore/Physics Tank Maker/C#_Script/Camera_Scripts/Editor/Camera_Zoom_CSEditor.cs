using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Camera_Zoom_CS))]
	public class Camera_Zoom_CSEditor : Editor
	{
		
		SerializedProperty Main_CameraProp;
		SerializedProperty Min_FOVProp;
		SerializedProperty Max_FOVProp;


		void OnEnable ()
		{
			Main_CameraProp = serializedObject.FindProperty ("Main_Camera");
			Min_FOVProp = serializedObject.FindProperty ("Min_FOV");
			Max_FOVProp = serializedObject.FindProperty ("Max_FOV");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Camera Pop Up settings", MessageType.None, true);
			Main_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Main Camera", Main_CameraProp.objectReferenceValue, typeof(Camera), true);
			EditorGUILayout.Slider (Min_FOVProp, 1.0f, 100.0f, "Min FOV");
			EditorGUILayout.Slider (Max_FOVProp, 1.0f, 100.0f, "Max FOV");


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}