using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Damage_Control_Center_CS))]
	public class Damage_Control_Center_CSEditor : Editor
	{
	
		SerializedProperty MainBody_HPProp;
		SerializedProperty Left_Track_HPProp;
		SerializedProperty Right_Track_HPProp;

		SerializedProperty MainBody_Damage_ThresholdProp;
		SerializedProperty Track_Damage_ThresholdProp;
        SerializedProperty Repairable_TrackProp;
        SerializedProperty Track_Repairing_TimeProp;
        SerializedProperty Track_Repairing_Velocity_LimitProp;

        SerializedProperty Destroyed_EffectProp;
        SerializedProperty Destroyed_Effect_OffsetProp;

        SerializedProperty Turret_NumProp;
		SerializedProperty Turret_PropsProp;


		void OnEnable ()
		{
			MainBody_HPProp = serializedObject.FindProperty ("MainBody_HP");
			Left_Track_HPProp = serializedObject.FindProperty ("Left_Track_HP");
			Right_Track_HPProp = serializedObject.FindProperty ("Right_Track_HP");

			MainBody_Damage_ThresholdProp = serializedObject.FindProperty ("MainBody_Damage_Threshold");
			Track_Damage_ThresholdProp = serializedObject.FindProperty ("Track_Damage_Threshold");

            Repairable_TrackProp = serializedObject.FindProperty("Repairable_Track");
            Track_Repairing_TimeProp = serializedObject.FindProperty("Track_Repairing_Time");
            Track_Repairing_Velocity_LimitProp = serializedObject.FindProperty("Track_Repairing_Velocity_Limit");

            Destroyed_EffectProp = serializedObject.FindProperty("Destroyed_Effect");
            Destroyed_Effect_OffsetProp = serializedObject.FindProperty("Destroyed_Effect_Offset");

            Turret_NumProp = serializedObject.FindProperty ("Turret_Num");
			Turret_PropsProp = serializedObject.FindProperty ("Turret_Props");
		}


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
		
			EditorGUILayout.Space ();	
			EditorGUILayout.HelpBox("Hit Point settings.", MessageType.None, true);
			EditorGUILayout.Slider(MainBody_HPProp, 1.0f, 10000.0f, "MainBody Hit Point");
			EditorGUILayout.Slider(Left_Track_HPProp, 1.0f, 10000.0f, "Left Track Hit Point");
			EditorGUILayout.Slider(Right_Track_HPProp, 1.0f, 10000.0f, "Right Track Hit Point");

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox("Damage Threshold settings.", MessageType.None, true);
			EditorGUILayout.Slider(MainBody_Damage_ThresholdProp, 1.0f, 10000.0f, "MainBody Damage Threshold");
			EditorGUILayout.Slider(Track_Damage_ThresholdProp, 1.0f, 10000.0f, "Track Damage Threshold");

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Track Repairing settings.", MessageType.None, true);
            Repairable_TrackProp.boolValue = EditorGUILayout.Toggle("Repairable Tracks", Repairable_TrackProp.boolValue);
            if (Repairable_TrackProp.boolValue)
            {
                EditorGUILayout.Slider(Track_Repairing_TimeProp, 1.0f, 180.0f, "Repairing Time");
                EditorGUILayout.Slider(Track_Repairing_Velocity_LimitProp, 0.01f, 10.0f, "Velocity Limit");
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Destroyed Effect settings.", MessageType.None, true);
            Destroyed_EffectProp.objectReferenceValue = EditorGUILayout.ObjectField("Destroyed Effect Prefab", Destroyed_EffectProp.objectReferenceValue, typeof(GameObject), false);
            Destroyed_Effect_OffsetProp.vector3Value = EditorGUILayout.Vector3Field("Destroyed Effect Offset", Destroyed_Effect_OffsetProp.vector3Value);

            EditorGUILayout.Space ();
			EditorGUILayout.HelpBox("Turret settings.", MessageType.None, true);
			EditorGUILayout.IntSlider(Turret_NumProp, 1, 16, "Number of Turrets");
			Turret_PropsProp.arraySize = Turret_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Turret_PropsProp.arraySize; i++) {
				EditorGUILayout.PropertyField(Turret_PropsProp.GetArrayElementAtIndex(i), true);
				EditorGUILayout.Space ();
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}

	}

}