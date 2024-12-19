using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Bullet_Generator_CS))]
	public class Bullet_Generator_CSEditor : Editor
	{

		SerializedProperty AP_Bullet_PrefabProp;
		SerializedProperty HE_Bullet_PrefabProp;
		SerializedProperty MuzzleFire_ObjectProp;
		SerializedProperty Attack_PointProp;
		SerializedProperty Attack_Point_HEProp;
		SerializedProperty Initial_VelocityProp;
		SerializedProperty Initial_Velocity_HEProp;

		SerializedProperty Life_TimeProp;
		SerializedProperty Initial_Bullet_TypeProp;
		SerializedProperty OffsetProp;
		SerializedProperty Debug_FlagProp;

		string[] typeNames = { "AP", "HE" };
		bool folding;


		void  OnEnable ()
		{
			AP_Bullet_PrefabProp = serializedObject.FindProperty ("AP_Bullet_Prefab");
			HE_Bullet_PrefabProp = serializedObject.FindProperty ("HE_Bullet_Prefab");
			MuzzleFire_ObjectProp = serializedObject.FindProperty ("MuzzleFire_Object");
			Attack_PointProp = serializedObject.FindProperty ("Attack_Point");
			Attack_Point_HEProp = serializedObject.FindProperty ("Attack_Point_HE");
			Initial_VelocityProp = serializedObject.FindProperty ("Initial_Velocity");
			Initial_Velocity_HEProp = serializedObject.FindProperty ("Initial_Velocity_HE");

			Life_TimeProp = serializedObject.FindProperty ("Life_Time");
			Initial_Bullet_TypeProp = serializedObject.FindProperty ("Initial_Bullet_Type");
			OffsetProp = serializedObject.FindProperty ("Offset");
			Debug_FlagProp = serializedObject.FindProperty ("Debug_Flag");
		}


		public override void  OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Bullet Prefab settings", MessageType.None, true);
			AP_Bullet_PrefabProp.objectReferenceValue = EditorGUILayout.ObjectField ("AP Bullet Prefab", AP_Bullet_PrefabProp.objectReferenceValue, typeof(GameObject), true);
			HE_Bullet_PrefabProp.objectReferenceValue = EditorGUILayout.ObjectField ("HE Bullet Prefab", HE_Bullet_PrefabProp.objectReferenceValue, typeof(GameObject), true);
			EditorGUILayout.Space ();
			MuzzleFire_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ("MuzzleFire Prefab", MuzzleFire_ObjectProp.objectReferenceValue, typeof(GameObject), true);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Basic settings", MessageType.None, true);
			EditorGUILayout.Slider (Attack_PointProp, 10.0f, 10000.0f, "AP Attack Point");
			EditorGUILayout.Slider (Attack_Point_HEProp, 10.0f, 10000.0f, "HE Attack Point");
			EditorGUILayout.Slider (Initial_VelocityProp, 10.0f, 1000.0f, "AP Initial Velocity");
			EditorGUILayout.Slider (Initial_Velocity_HEProp, 10.0f, 1000.0f, "HE Initial Velocity");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Life_TimeProp, 1.0f, 180.0f, "Life Time");
			EditorGUILayout.Space ();
			Initial_Bullet_TypeProp.intValue = EditorGUILayout.Popup ("Initial Bullet Type", Initial_Bullet_TypeProp.intValue, typeNames);
			EditorGUILayout.Slider (OffsetProp, 0.0f, 10.0f, "Offset");
			Debug_FlagProp.boolValue = EditorGUILayout.Toggle ("Debug Mode", Debug_FlagProp.boolValue);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}

	}

}