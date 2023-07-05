using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Trigger_Collider_CS))]
	public class Trigger_Collider_CSEditor : Editor
	{
		
		SerializedProperty Invisible_FlagProp;
		SerializedProperty Store_CountProp;
	

		void OnEnable ()
		{
			Invisible_FlagProp = serializedObject.FindProperty ("Invisible_Flag");
			Store_CountProp = serializedObject.FindProperty ("Store_Count");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Trigger Collider settings", MessageType.None, true);
			Invisible_FlagProp.boolValue = EditorGUILayout.Toggle ("Invisible", Invisible_FlagProp.boolValue);
			EditorGUILayout.IntSlider (Store_CountProp, 1, 64, "Store Count");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}