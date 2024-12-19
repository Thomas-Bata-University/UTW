using UnityEngine;
using UnityEditor;


namespace ChobiAssets.PTM
{
	
	[ CustomEditor (typeof(Respawn_Controller_CS))]
	public class Respawn_Controller_CSEditor : Editor
	{

		SerializedProperty This_PrefabProp;
		SerializedProperty Respawn_TimesProp;
		SerializedProperty Auto_Respawn_IntervalProp;
		SerializedProperty Remove_After_DeathProp;
		SerializedProperty Remove_IntervalProp;
        SerializedProperty Respawn_Point_PackProp;
        SerializedProperty Respawn_TargetProp;


        Transform thisTransform;


        void OnEnable ()
		{
			This_PrefabProp = serializedObject.FindProperty("This_Prefab");
			Respawn_TimesProp = serializedObject.FindProperty("Respawn_Times");
			Auto_Respawn_IntervalProp = serializedObject.FindProperty("Auto_Respawn_Interval");
			Remove_After_DeathProp = serializedObject.FindProperty("Remove_After_Death");
			Remove_IntervalProp = serializedObject.FindProperty("Remove_Interval");
            Respawn_Point_PackProp = serializedObject.FindProperty("Respawn_Point_Pack");
            Respawn_TargetProp = serializedObject.FindProperty("Respawn_Target");

            if (Selection.activeGameObject)
            {
                thisTransform = Selection.activeGameObject.transform;
            }
        }


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

            GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// Set this Prefab.
			Get_This_Prefab ();

            // Check the Prefab Mode.
            if (PrefabUtility.IsPartOfPrefabInstance(thisTransform))
            {
                EditorGUILayout.HelpBox("To use the respawning function,\nSpawn this tank via \"Event_Controller\", or Unpack the prefab.", MessageType.Warning, true);
                if (GUILayout.Button("Unpack Prefab", GUILayout.Width(200)))
                {
                    PrefabUtility.UnpackPrefabInstance(thisTransform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    return;
                }
            }

            // Input settings.
            EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Respawn settings", MessageType.None, true);

			This_PrefabProp.objectReferenceValue = EditorGUILayout.ObjectField ("This Prefab", This_PrefabProp.objectReferenceValue, typeof(GameObject), false);

			EditorGUILayout.IntSlider (Respawn_TimesProp, 0, 100, "Respawn Times");
			if (Respawn_TimesProp.intValue != 0){
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider (Auto_Respawn_IntervalProp, 1.0f, 120.0f, "Auto Respawn Interval");
                Respawn_Point_PackProp.objectReferenceValue = EditorGUILayout.ObjectField("Respawn Point Pack", Respawn_Point_PackProp.objectReferenceValue, typeof(Transform), true);
                Respawn_TargetProp.objectReferenceValue = EditorGUILayout.ObjectField("Respawn Target", Respawn_TargetProp.objectReferenceValue, typeof(Transform), true);
                EditorGUI.indentLevel--;
            }

			Remove_After_DeathProp.boolValue = EditorGUILayout.Toggle ("Remove After Death", Remove_After_DeathProp.boolValue);
			if (Remove_After_DeathProp.boolValue) {
				EditorGUILayout.Slider(Remove_IntervalProp, 1.0f, 120.0f, "Remove Interval");
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}


		void Get_This_Prefab ()
		{
			Object thisPrefab = PrefabUtility.GetCorrespondingObjectFromSource (Selection.activeGameObject);
            if (thisPrefab) {
				This_PrefabProp.objectReferenceValue = thisPrefab as GameObject;
			}
        }

    }
}
