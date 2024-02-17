using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Turret_Horizontal_CS))]
	public class Turret_Horizontal_CSEditor : Editor
	{
	
		SerializedProperty Limit_FlagProp;
		SerializedProperty Max_RightProp;
		SerializedProperty Max_LeftProp;
		SerializedProperty Speed_MagProp;
		SerializedProperty Acceleration_TimeProp;
		SerializedProperty Deceleration_TimeProp;

		void OnEnable ()
		{
			Limit_FlagProp = serializedObject.FindProperty ("Limit_Flag");
			Max_RightProp = serializedObject.FindProperty ("Max_Right");
			Max_LeftProp = serializedObject.FindProperty ("Max_Left");
			Speed_MagProp = serializedObject.FindProperty ("Speed_Mag");
			Acceleration_TimeProp = serializedObject.FindProperty ("Acceleration_Time");
			Deceleration_TimeProp = serializedObject.FindProperty ("Deceleration_Time");
		}

		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Turret Rotation settings", MessageType.None, true);
			if (EditorApplication.isPlaying == false) {
				Limit_FlagProp.boolValue = EditorGUILayout.Toggle ("Limit", Limit_FlagProp.boolValue);
				if (Limit_FlagProp.boolValue) {
					EditorGUI.indentLevel++;
					EditorGUILayout.Slider (Max_RightProp, 0.0f, 180.0f, "Max Rigth Angle");
					EditorGUILayout.Slider (Max_LeftProp, 0.0f, 180.0f, "Max Left Angle");
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.Slider (Speed_MagProp, 1.0f, 360.0f, "Speed");
			EditorGUILayout.Slider (Acceleration_TimeProp, 0.01f, 5.0f, "Acceleration Time");
			EditorGUILayout.Slider (Deceleration_TimeProp, 0.01f, 5.0f, "Deceleration Time");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}