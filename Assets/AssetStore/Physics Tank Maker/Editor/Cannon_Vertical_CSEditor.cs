using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Cannon_Vertical_CS))]
	public class Cannon_Vertical_CSEditor : Editor
	{
	
		SerializedProperty Max_ElevationProp;
		SerializedProperty Max_DepressionProp;
		SerializedProperty Speed_MagProp;
		SerializedProperty Acceleration_TimeProp;
		SerializedProperty Deceleration_TimeProp;
		SerializedProperty Upper_CourseProp;


		void  OnEnable ()
		{
			Max_ElevationProp = serializedObject.FindProperty ("Max_Elevation");
			Max_DepressionProp = serializedObject.FindProperty ("Max_Depression");
			Speed_MagProp = serializedObject.FindProperty ("Speed_Mag");
			Acceleration_TimeProp = serializedObject.FindProperty ("Acceleration_Time");
			Deceleration_TimeProp = serializedObject.FindProperty ("Deceleration_Time");
			Upper_CourseProp = serializedObject.FindProperty ("Upper_Course");
		}


		public override void  OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Cannon Elevation settings", MessageType.None, true);
			if (EditorApplication.isPlaying == false) {
				EditorGUILayout.Slider (Max_ElevationProp, 0.0f, 180.0f, "Max Angle");
				EditorGUILayout.Slider (Max_DepressionProp, 0.0f, 180.0f, "Min Angle");
			}
			EditorGUILayout.Slider (Speed_MagProp, 1.0f, 360.0f, "Speed");
			EditorGUILayout.Slider (Acceleration_TimeProp, 0.01f, 5.0f, "Acceleration Time");
			EditorGUILayout.Slider (Deceleration_TimeProp, 0.01f, 5.0f, "Deceleration Time");
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox ("Only when 'Use_Auto_Elevation' is enabled in 'Aiming_Control_CS'.", MessageType.None, true);
			Upper_CourseProp.boolValue = EditorGUILayout.Toggle ("Upper Course", Upper_CourseProp.boolValue);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}