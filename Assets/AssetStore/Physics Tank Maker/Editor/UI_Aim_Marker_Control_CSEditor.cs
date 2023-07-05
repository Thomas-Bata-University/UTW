using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_Aim_Marker_Control_CS))]
	public class UI_Aim_Marker_Control_CS_CSEditor : Editor
	{
	
		SerializedProperty Aim_Marker_NameProp;


		void OnEnable ()
		{
			Aim_Marker_NameProp = serializedObject.FindProperty ("Aim_Marker_Name");
		}


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Aim Marker settings", MessageType.None, true);

			Aim_Marker_NameProp.stringValue = EditorGUILayout.TextField ("Aim Marker Name", Aim_Marker_NameProp.stringValue);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}