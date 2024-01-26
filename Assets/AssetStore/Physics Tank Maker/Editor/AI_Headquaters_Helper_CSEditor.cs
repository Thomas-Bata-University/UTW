using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(AI_Headquaters_Helper_CS))]
	public class AI_Headquaters_Helper_CSEditor : Editor
	{
		
		SerializedProperty Visibility_Upper_OffsetProp;


		void OnEnable ()
		{
			Visibility_Upper_OffsetProp = serializedObject.FindProperty ("Visibility_Upper_Offset");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Tank information for AI", MessageType.None, true);
			EditorGUILayout.Slider (Visibility_Upper_OffsetProp, 0.0f, 10.0f, "Visibility Upper Offset");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}