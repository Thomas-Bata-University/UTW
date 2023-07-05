using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Camera_Rotation_CS))]
	public class Camera_Rotation_CSEditor : Editor
	{
		
		SerializedProperty Main_CameraProp;
        /*
		SerializedProperty Horizontal_SpeedProp;
		SerializedProperty Vertical_SpeedProp;
		SerializedProperty Invert_FlagProp;
        SerializedProperty Use_ClampProp;
        SerializedProperty Simulate_Head_PhysicsProp;
        */
        SerializedProperty Use_Demo_CameraProp;


		void OnEnable ()
		{
			Main_CameraProp = serializedObject.FindProperty ("Main_Camera");
            /*
			Horizontal_SpeedProp = serializedObject.FindProperty ("Horizontal_Speed");
			Vertical_SpeedProp = serializedObject.FindProperty ("Vertical_Speed");
			Invert_FlagProp = serializedObject.FindProperty ("Invert_Flag");
            Use_ClampProp = serializedObject.FindProperty("Use_Clamp");
            Simulate_Head_PhysicsProp = serializedObject.FindProperty("Simulate_Head_Physics");
            */
            Use_Demo_CameraProp = serializedObject.FindProperty ("Use_Demo_Camera");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Camera Rotaion settings", MessageType.None, true);
			Main_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Main Camera", Main_CameraProp.objectReferenceValue, typeof(Camera), true);
            /*
            EditorGUILayout.Slider (Horizontal_SpeedProp, 1.0f, 10.0f, "Horizontal Speed");
			EditorGUILayout.Slider (Vertical_SpeedProp, 1.0f, 10.0f, "Vertical Speed");
			Invert_FlagProp.boolValue = EditorGUILayout.Toggle ("Invert Vertical Rotation", Invert_FlagProp.boolValue);
            Use_ClampProp.boolValue = EditorGUILayout.Toggle("Clamp Rotation", Use_ClampProp.boolValue);
            Simulate_Head_PhysicsProp.boolValue = EditorGUILayout.Toggle("Simulate Head Physics", Simulate_Head_PhysicsProp.boolValue);
            */        
            Use_Demo_CameraProp.boolValue = EditorGUILayout.Toggle ("Use Demo Camera", Use_Demo_CameraProp.boolValue);


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}