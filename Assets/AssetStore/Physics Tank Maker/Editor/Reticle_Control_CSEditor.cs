using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Reticle_Control_CS))]
	public class Reticle_Control_CSEditor : Editor
	{

		SerializedProperty Reticle_NameProp;
		SerializedProperty Gun_Camera_ScriptProp;
	

		void OnEnable ()
		{
			Reticle_NameProp = serializedObject.FindProperty ("Reticle_Name");
            Gun_Camera_ScriptProp = serializedObject.FindProperty ("Gun_Camera_Script");
		}


		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Reticle settings", MessageType.None, true);
			Reticle_NameProp.stringValue = EditorGUILayout.TextField ("Reticle Name", Reticle_NameProp.stringValue);
            Gun_Camera_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("'Gun_Camera_CS' Script", Gun_Camera_ScriptProp.objectReferenceValue, typeof(Gun_Camera_CS), true);


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}