using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Camera_Avoid_Obstacle_CS))]
	public class Camera_Avoid_Obstacle_CSEditor : Editor
	{
		
		SerializedProperty Main_CameraProp;


		void OnEnable ()
		{
			Main_CameraProp = serializedObject.FindProperty ("Main_Camera");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Camera Pop Up settings", MessageType.None, true);
			Main_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Main Camera", Main_CameraProp.objectReferenceValue, typeof(Camera), true);


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}