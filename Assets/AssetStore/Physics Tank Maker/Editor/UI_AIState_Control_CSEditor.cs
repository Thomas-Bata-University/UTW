using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_AIState_Control_CS))]
	public class UI_AIState_Control_CSEditor : Editor
	{
		
		SerializedProperty Patrol_ColorProp;
		SerializedProperty Attack_ColorProp;
		SerializedProperty Lost_ColorProp;
		SerializedProperty Dead_ColorProp;
        SerializedProperty Respawn_ColorProp;

        SerializedProperty Patrol_TextProp;
		SerializedProperty Attack_TextProp;
		SerializedProperty Lost_TextProp;
		SerializedProperty Dead_TextProp;
        SerializedProperty Respawn_TextProp;


        void OnEnable ()
		{
			Patrol_ColorProp = serializedObject.FindProperty ("Patrol_Color");
			Attack_ColorProp = serializedObject.FindProperty ("Attack_Color");
			Lost_ColorProp = serializedObject.FindProperty ("Lost_Color");
			Dead_ColorProp = serializedObject.FindProperty ("Dead_Color");
            Respawn_ColorProp = serializedObject.FindProperty("Respawn_Color");

            Patrol_TextProp = serializedObject.FindProperty ("Patrol_Text");
			Attack_TextProp = serializedObject.FindProperty ("Attack_Text");
			Lost_TextProp = serializedObject.FindProperty ("Lost_Text");
			Dead_TextProp = serializedObject.FindProperty ("Dead_Text");
            Respawn_TextProp = serializedObject.FindProperty("Respawn_Text");
        }


        public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("AI State Display settings", MessageType.None, true);
			Patrol_ColorProp.colorValue = EditorGUILayout.ColorField("Patrol Color", Patrol_ColorProp.colorValue);
			Attack_ColorProp.colorValue = EditorGUILayout.ColorField("Attack Color", Attack_ColorProp.colorValue);
			Lost_ColorProp.colorValue = EditorGUILayout.ColorField("Lost Color", Lost_ColorProp.colorValue);
			Dead_ColorProp.colorValue = EditorGUILayout.ColorField("Dead Color", Dead_ColorProp.colorValue);
            Respawn_ColorProp.colorValue = EditorGUILayout.ColorField("Respawn Color", Respawn_ColorProp.colorValue);

            Patrol_TextProp.stringValue = EditorGUILayout.TextField ("Patrol Text", Patrol_TextProp.stringValue);
			Attack_TextProp.stringValue = EditorGUILayout.TextField ("Attack Text", Attack_TextProp.stringValue);
			Lost_TextProp.stringValue = EditorGUILayout.TextField ("Lost Text", Lost_TextProp.stringValue);
			Dead_TextProp.stringValue = EditorGUILayout.TextField ("Dead Text", Dead_TextProp.stringValue);
            Respawn_TextProp.stringValue = EditorGUILayout.TextField("Respawn Text", Respawn_TextProp.stringValue);

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}