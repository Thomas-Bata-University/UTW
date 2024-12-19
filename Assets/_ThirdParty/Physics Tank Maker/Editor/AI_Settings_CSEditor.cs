using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(AI_Settings_CS))]
	public class AI_Settings_CSEditor : Editor
	{
		
		SerializedProperty WayPoint_PackProp;
		SerializedProperty Patrol_TypeProp;
		SerializedProperty Follow_TargetProp;
		SerializedProperty No_AttackProp;
		SerializedProperty BreakthroughProp;
		SerializedProperty CommanderProp;
		SerializedProperty Visibility_RadiusProp;
		SerializedProperty Approach_DistanceProp;
		SerializedProperty OpenFire_DistanceProp;
		SerializedProperty Lost_CountProp;
		SerializedProperty Face_Offest_AngleProp;
        SerializedProperty Dead_AngleProp;
        SerializedProperty Patrol_Speed_RateProp;
        SerializedProperty Combat_Speed_RateProp;
        SerializedProperty AI_State_TextProp;
		SerializedProperty Tank_NameProp;

		string[] patrolTypeNames = { "Order", "Random" };


		void OnEnable ()
		{
			WayPoint_PackProp = serializedObject.FindProperty ("WayPoint_Pack");
			Patrol_TypeProp = serializedObject.FindProperty ("Patrol_Type");
			Follow_TargetProp = serializedObject.FindProperty ("Follow_Target");
			No_AttackProp = serializedObject.FindProperty ("No_Attack");
			BreakthroughProp = serializedObject.FindProperty ("Breakthrough");
            CommanderProp = serializedObject.FindProperty ("Commander");
			Visibility_RadiusProp = serializedObject.FindProperty ("Visibility_Radius");
			Approach_DistanceProp = serializedObject.FindProperty ("Approach_Distance");
			OpenFire_DistanceProp = serializedObject.FindProperty ("OpenFire_Distance");
			Lost_CountProp = serializedObject.FindProperty ("Lost_Count");
			Face_Offest_AngleProp = serializedObject.FindProperty ("Face_Offest_Angle");
            Dead_AngleProp = serializedObject.FindProperty("Dead_Angle");
            Patrol_Speed_RateProp = serializedObject.FindProperty("Patrol_Speed_Rate");
            Combat_Speed_RateProp = serializedObject.FindProperty("Combat_Speed_Rate");
            AI_State_TextProp = serializedObject.FindProperty ("AI_State_Text");
			Tank_NameProp = serializedObject.FindProperty ("Tank_Name");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			if (EditorApplication.isPlaying == false) {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("AI Patrol settings", MessageType.None, true);
				WayPoint_PackProp.objectReferenceValue = EditorGUILayout.ObjectField("WayPoint Pack", WayPoint_PackProp.objectReferenceValue, typeof(GameObject), true);
				Patrol_TypeProp.intValue = EditorGUILayout.Popup("Patrol Type", Patrol_TypeProp.intValue, patrolTypeNames);
				Follow_TargetProp.objectReferenceValue = EditorGUILayout.ObjectField("Follow Target", Follow_TargetProp.objectReferenceValue, typeof(Transform), true);
				EditorGUILayout.Space();
			}

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("AI Combat settings", MessageType.None, true);
			No_AttackProp.boolValue = EditorGUILayout.Toggle ("No Attack", No_AttackProp.boolValue);
			BreakthroughProp.boolValue = EditorGUILayout.Toggle ("Breakthrough", BreakthroughProp.boolValue);
            CommanderProp.objectReferenceValue = EditorGUILayout.ObjectField("Commander", CommanderProp.objectReferenceValue, typeof(Transform), true);
			EditorGUILayout.Slider (Visibility_RadiusProp, 1.0f, 10000.0f, "Visibility Radius");
			EditorGUILayout.Slider (Approach_DistanceProp, 1.0f, 10000.0f, "Approach Distance");
			if (Approach_DistanceProp.floatValue == 10000.0f) {
				Approach_DistanceProp.floatValue = Mathf.Infinity;
			}
			EditorGUILayout.Slider (OpenFire_DistanceProp, 1.0f, 10000.0f, "Open Fire Distance");
			if (OpenFire_DistanceProp.floatValue == 10000.0f) {
				OpenFire_DistanceProp.floatValue = Mathf.Infinity;
			}
			EditorGUILayout.Slider(Lost_CountProp, 0.0f, 120.0f, "Lost Count");
			EditorGUILayout.Slider(Face_Offest_AngleProp, 0.0f, 90.0f, "Face Offest Angle");
            EditorGUILayout.Slider(Dead_AngleProp, 0.0f, 180.0f, "Dead Angle");

            if (EditorApplication.isPlaying == false) {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("AI Speed settings", MessageType.None, true);
                EditorGUILayout.Slider(Patrol_Speed_RateProp, 0.1f, 1.0f, "Patrol Speed Rate");
                EditorGUILayout.Slider(Combat_Speed_RateProp, 0.1f, 1.0f, "Combat Speed Rate");

                EditorGUILayout.Space();
				EditorGUILayout.HelpBox("AI State Text settings", MessageType.None, true);
				AI_State_TextProp.objectReferenceValue = EditorGUILayout.ObjectField("Text for AI's state", AI_State_TextProp.objectReferenceValue, typeof(Text), true);
				Tank_NameProp.stringValue = EditorGUILayout.TextField("Tank Name", Tank_NameProp.stringValue);
			}


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}