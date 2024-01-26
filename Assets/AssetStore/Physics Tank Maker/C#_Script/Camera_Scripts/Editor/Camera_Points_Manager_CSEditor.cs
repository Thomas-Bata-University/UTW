using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Camera_Points_Manager_CS))]
	public class Camera_Points_Manager_CSEditor : Editor
	{
		
		SerializedProperty Main_CameraProp;
		SerializedProperty Main_AudioListenerProp;
		SerializedProperty Camera_Points_NumProp;
		SerializedProperty Camera_PointsProp;


		void OnEnable ()
		{
			Main_CameraProp = serializedObject.FindProperty ("Main_Camera");
			Main_AudioListenerProp = serializedObject.FindProperty ("Main_AudioListener");
			Camera_Points_NumProp = serializedObject.FindProperty ("Camera_Points_Num");
			Camera_PointsProp = serializedObject.FindProperty ("Camera_Points");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Camera Points settings", MessageType.None, true);
			Main_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Main Camera", Main_CameraProp.objectReferenceValue, typeof(Camera), true);
			Main_AudioListenerProp.objectReferenceValue = EditorGUILayout.ObjectField ("Main AudioListener", Main_AudioListenerProp.objectReferenceValue, typeof(AudioListener), true);
			EditorGUILayout.Space ();

			EditorGUILayout.IntSlider(Camera_Points_NumProp, 1, 10, "Number of Camera Points");
			Camera_PointsProp.arraySize = Camera_Points_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Camera_PointsProp.arraySize; i++) {
				EditorGUILayout.PropertyField(Camera_PointsProp.GetArrayElementAtIndex(i), true);
				EditorGUILayout.Space ();
			}
			EditorGUI.indentLevel--;


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}