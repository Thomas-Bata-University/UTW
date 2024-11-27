using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(ID_Settings_CS))]
	public class ID_Settings_CSEditor : Editor
	{
		
		SerializedProperty Tank_IDProp;
		SerializedProperty RelationshipProp;

		string[] relationshipNames = { "Friendly", "Hostile", "Landmark" };


		void OnEnable ()
		{
			Tank_IDProp = serializedObject.FindProperty ("Tank_ID");
			RelationshipProp = serializedObject.FindProperty ("Relationship");
		}


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("ID settings", MessageType.None, true);
			EditorGUILayout.IntSlider (Tank_IDProp, 0, 100, "Tank ID");
			if (Tank_IDProp.intValue == 0) {
				EditorGUILayout.HelpBox ("This tank is not selectable.", MessageType.None, false);
			}

			EditorGUILayout.Space ();
			RelationshipProp.intValue = EditorGUILayout.Popup ("Relationship", RelationshipProp.intValue, relationshipNames);
			if (RelationshipProp.intValue == 2) { // Landmark.
				Tank_IDProp.intValue = 0;
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}