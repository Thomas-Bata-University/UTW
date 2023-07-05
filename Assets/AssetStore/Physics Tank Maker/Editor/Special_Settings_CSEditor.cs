using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Special_Settings_CS))]
	public class Special_Settings_CSEditor : Editor
	{
		
		SerializedProperty Attack_MultiplierProp;
		SerializedProperty Defence_MultiplierProp;


		void OnEnable ()
		{
			Attack_MultiplierProp = serializedObject.FindProperty ("Attack_Multiplier");
			Defence_MultiplierProp = serializedObject.FindProperty ("Defence_Multiplier");
		}


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox("Special Option settings", MessageType.None, true);
			EditorGUILayout.Slider (Attack_MultiplierProp, 0.1f, 100.0f, "Attack Multiplier");
			EditorGUILayout.Slider (Defence_MultiplierProp, 0.1f, 100.0f, "Defence Multiplier");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}