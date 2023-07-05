using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Artillery_Fire_CS))]
	public class Artillery_Fire_CSEditor : Editor
	{
		
		SerializedProperty Interval_MinProp;
		SerializedProperty Interval_MaxProp;
		SerializedProperty RadiusProp;
		SerializedProperty HeightProp;
		SerializedProperty MassProp;
		SerializedProperty Life_TimeProp;
		SerializedProperty Attack_PointProp;
		SerializedProperty Explosion_ForceProp;
		SerializedProperty Explosion_RadiusProp;
		SerializedProperty Explosion_ObjectProp;


		void OnEnable ()
		{
			Interval_MinProp = serializedObject.FindProperty ("Interval_Min");
			Interval_MaxProp = serializedObject.FindProperty ("Interval_Max");
			RadiusProp = serializedObject.FindProperty ("Radius");
			HeightProp = serializedObject.FindProperty ("Height");
			MassProp = serializedObject.FindProperty ("Mass");
			Life_TimeProp = serializedObject.FindProperty ("Life_Time");
			Attack_PointProp = serializedObject.FindProperty ("Attack_Point");
			Explosion_ForceProp = serializedObject.FindProperty ("Explosion_Force");
			Explosion_RadiusProp = serializedObject.FindProperty ("Explosion_Radius");
			Explosion_ObjectProp = serializedObject.FindProperty ("Explosion_Object");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Artillery settings", MessageType.None, true);
			EditorGUILayout.Slider (Interval_MinProp, 0.01f, 10.0f, "Min Interval");
			EditorGUILayout.Slider (Interval_MaxProp, 0.01f, 10.0f, "Max Interval");
			EditorGUILayout.Slider (RadiusProp, 16.0f, 1024.0f, "Bombardment Radius");

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Shell settings", MessageType.None, true);
			EditorGUILayout.Slider (HeightProp, 16.0f, 128.0f, "Spawn Height");
			EditorGUILayout.Slider (MassProp, 1.0f, 128.0f, "Mass");
			EditorGUILayout.Slider (Life_TimeProp, 10.0f, 60.0f, "Life Time");
			EditorGUILayout.Slider (Attack_PointProp, 10.0f, 10000.0f, "Attack Point");
			EditorGUILayout.Slider (Explosion_ForceProp, 100.0f, 1000000.0f, "Explosion Force");
			EditorGUILayout.Slider (Explosion_RadiusProp, 1.0f, 100.0f, "Explosion Radius");
			Explosion_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ("Explosion Prefab", Explosion_ObjectProp.objectReferenceValue, typeof(GameObject), false);


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}