using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTS
{

	[ CustomEditor (typeof(PTS_Drive_Control_CS))]
	public class Drive_Control_CSEditor : Editor
	{
	
		SerializedProperty TorqueProp;
		SerializedProperty Max_SpeedProp;
		SerializedProperty MaxAngVelocity_LimitProp;

		SerializedProperty ParkingBrake_VelocityProp;
		SerializedProperty ParkingBrake_LagProp;
		SerializedProperty Fix_Useless_RotaionProp;

		SerializedProperty Use_AntiSlipProp;
		SerializedProperty Ray_DistanceProp;
		SerializedProperty Support_VelocityProp;


		void OnEnable ()
		{
			TorqueProp = serializedObject.FindProperty ("Torque");
			Max_SpeedProp = serializedObject.FindProperty ("Max_Speed");
			MaxAngVelocity_LimitProp = serializedObject.FindProperty ("MaxAngVelocity_Limit");

			ParkingBrake_VelocityProp = serializedObject.FindProperty ("ParkingBrake_Velocity");
			ParkingBrake_LagProp = serializedObject.FindProperty ("ParkingBrake_Lag");
			Fix_Useless_RotaionProp = serializedObject.FindProperty ("Fix_Useless_Rotaion");

			Use_AntiSlipProp = serializedObject.FindProperty ("Use_AntiSlip");
			Ray_DistanceProp = serializedObject.FindProperty ("Ray_Distance");
			Support_VelocityProp = serializedObject.FindProperty ("Support_Velocity");
		}

		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
			if (EditorApplication.isPlaying == false) {
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Driving Wheels Settings", MessageType.None, true);
				EditorGUILayout.Slider (TorqueProp, 0.0f, 100000.0f, "Torque");
				EditorGUILayout.Slider (Max_SpeedProp, 0.0f, 30.0f, "Maximum Speed");
				EditorGUILayout.Slider (MaxAngVelocity_LimitProp, 1.0f, 100.0f, "MaxAngularVelocity Limit");
			
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Parking Brake Settings", MessageType.None, true);
				EditorGUILayout.Slider (ParkingBrake_VelocityProp, 0.0f, 10.0f, "Work Velocity");
				EditorGUILayout.Slider (ParkingBrake_LagProp, 0.0f, 5.0f, "Lag Time");
				Fix_Useless_RotaionProp.boolValue = EditorGUILayout.Toggle ("Fix Useless Rotation", Fix_Useless_RotaionProp.boolValue);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Anti Slipping Settings", MessageType.None, true);
				Use_AntiSlipProp.boolValue = EditorGUILayout.Toggle ("Use Anti-Slipping", Use_AntiSlipProp.boolValue);
				if (Use_AntiSlipProp.boolValue) {
					EditorGUILayout.Slider(Ray_DistanceProp, 0.0f, 10.0f, "Ray Distance");
					EditorGUILayout.Slider(Support_VelocityProp, 0.0f, 0.1f, "Support Velocity");
				}
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
			}
				
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}