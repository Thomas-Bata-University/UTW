using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_Lead_Marker_Control_CS))]
	public class UI_Lead_Marker_Control_CS_CSEditor : Editor
	{
	
		SerializedProperty Lead_Marker_NameProp;
        SerializedProperty Right_SpriteProp;
        SerializedProperty Wrong_SpriteProp;
        SerializedProperty Calculation_TimeProp;
        SerializedProperty Bullet_Generator_ScriptProp;


        void OnEnable ()
		{
			Lead_Marker_NameProp = serializedObject.FindProperty ("Lead_Marker_Name");
            Right_SpriteProp = serializedObject.FindProperty("Right_Sprite");
            Wrong_SpriteProp = serializedObject.FindProperty("Wrong_Sprite");
            Calculation_TimeProp = serializedObject.FindProperty("Calculation_Time");
            Bullet_Generator_ScriptProp = serializedObject.FindProperty("Bullet_Generator_Script");
        }


        public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Lead Marker settings", MessageType.None, true);

			Lead_Marker_NameProp.stringValue = EditorGUILayout.TextField ("Lead Marker Name", Lead_Marker_NameProp.stringValue);
            Right_SpriteProp.objectReferenceValue = EditorGUILayout.ObjectField("Right Sprite", Right_SpriteProp.objectReferenceValue, typeof(Sprite), false);
            Wrong_SpriteProp.objectReferenceValue = EditorGUILayout.ObjectField("Wrong Sprite", Wrong_SpriteProp.objectReferenceValue, typeof(Sprite), false);
            EditorGUILayout.Slider(Calculation_TimeProp, 1.0f, 30.0f, "Calculation Time");
            Bullet_Generator_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Generator Script", Bullet_Generator_ScriptProp.objectReferenceValue, typeof(Bullet_Generator_CS), true);

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}