using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_Position_Marker_Control_CS))]
	public class UI_Position_Marker_Control_CSEditor : Editor
	{

        SerializedProperty Marker_PrefabProp;
        SerializedProperty Canvas_NameProp;
        SerializedProperty Friend_ColorProp;
		SerializedProperty Hostile_ColorProp;
		SerializedProperty Landmark_ColorProp;
		SerializedProperty Defensive_AlphaProp;
		SerializedProperty Offensive_AlphaProp;
		SerializedProperty Upper_OffsetProp;
		SerializedProperty Side_OffsetProp;
        SerializedProperty Bottom_OffsetProp;
        SerializedProperty Show_AlwaysProp;


		void OnEnable ()
		{
            Marker_PrefabProp = serializedObject.FindProperty("Marker_Prefab");
            Canvas_NameProp = serializedObject.FindProperty("Canvas_Name");
            Friend_ColorProp = serializedObject.FindProperty ("Friend_Color");
			Hostile_ColorProp = serializedObject.FindProperty ("Hostile_Color");
			Landmark_ColorProp = serializedObject.FindProperty ("Landmark_Color");
			Defensive_AlphaProp = serializedObject.FindProperty ("Defensive_Alpha");
			Offensive_AlphaProp = serializedObject.FindProperty ("Offensive_Alpha");
			Upper_OffsetProp = serializedObject.FindProperty ("Upper_Offset");
			Side_OffsetProp = serializedObject.FindProperty ("Side_Offset");
            Bottom_OffsetProp = serializedObject.FindProperty("Bottom_Offset");
            Show_AlwaysProp = serializedObject.FindProperty ("Show_Always");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Position Marker settings", MessageType.None, true);
            Marker_PrefabProp.objectReferenceValue = EditorGUILayout.ObjectField("Marker Prefab", Marker_PrefabProp.objectReferenceValue, typeof(GameObject), false);
            Canvas_NameProp.stringValue = EditorGUILayout.TextField("Canvas Name", Canvas_NameProp.stringValue);
            Friend_ColorProp.colorValue = EditorGUILayout.ColorField("Friend Color", Friend_ColorProp.colorValue);
			Hostile_ColorProp.colorValue = EditorGUILayout.ColorField("Hostile Color", Hostile_ColorProp.colorValue);
			Landmark_ColorProp.colorValue = EditorGUILayout.ColorField("Landmark Color", Landmark_ColorProp.colorValue);
			EditorGUILayout.Slider (Defensive_AlphaProp, 0.01f, 1.0f, "Defensive Alpha");
			EditorGUILayout.Slider (Offensive_AlphaProp, 0.01f, 1.0f, "Offensive Alpha");
			EditorGUILayout.Slider (Upper_OffsetProp, -128.0f, 128.0f, "Upper Offset");
			EditorGUILayout.Slider (Side_OffsetProp, -128.0f, 128.0f, "Side Offset");
            EditorGUILayout.Slider(Bottom_OffsetProp, -128.0f, 128.0f, "Bottom Offset");
            Show_AlwaysProp.boolValue = EditorGUILayout.Toggle ("Show Always", Show_AlwaysProp.boolValue);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}