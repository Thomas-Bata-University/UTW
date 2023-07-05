using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTS
{

	[ CustomEditor (typeof(PTS_MainBody_Setting_CS))]
	public class PTS_MainBody_Setting_CSEditor : Editor
	{
	
		SerializedProperty Body_MassProp;
		SerializedProperty Body_MeshProp;

		SerializedProperty Materials_NumProp;
		SerializedProperty MaterialsProp;
		SerializedProperty Body_MaterialProp;

		SerializedProperty Collider_MeshProp;
		SerializedProperty Sub_Collider_MeshProp;

		SerializedProperty SICProp;
		SerializedProperty Soft_Landing_FlagProp;
		SerializedProperty Landing_DragProp;
		SerializedProperty Landing_TimeProp;
		SerializedProperty Mass_Center_ZeroProp;

        SerializedProperty Has_ChangedProp;


        GameObject mainBodyObject;
		bool isActiveInHierarchy;

		void OnEnable ()
		{
			Body_MassProp = serializedObject.FindProperty ("Body_Mass");
			Body_MeshProp = serializedObject.FindProperty ("Body_Mesh");

			Materials_NumProp = serializedObject.FindProperty ("Materials_Num");
			MaterialsProp = serializedObject.FindProperty ("Materials");
			Body_MaterialProp = serializedObject.FindProperty ("Body_Material");

			Collider_MeshProp = serializedObject.FindProperty ("Collider_Mesh");
			Sub_Collider_MeshProp = serializedObject.FindProperty ("Sub_Collider_Mesh");

			SICProp = serializedObject.FindProperty ("SIC");
			Soft_Landing_FlagProp = serializedObject.FindProperty ("Soft_Landing_Flag");
			Landing_DragProp = serializedObject.FindProperty ("Landing_Drag");
			Landing_TimeProp = serializedObject.FindProperty ("Landing_Time");
			Mass_Center_ZeroProp = serializedObject.FindProperty ("Mass_Center_Zero");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            mainBodyObject = Selection.activeGameObject;
		}

		public override void OnInspectorGUI ()
		{
            if (EditorApplication.isPlaying)
            {
                return;
            }

            Set_Inspector();
        }

		void Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			// Basic Settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Basic Settings", MessageType.None, true);
			EditorGUILayout.Slider (Body_MassProp, 1.0f, 100000.0f, "Mass");
			EditorGUILayout.Space ();
			Body_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Body_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (Materials_NumProp, 1, 10, "Number of Materials");
			MaterialsProp.arraySize = Materials_NumProp.intValue;
			if (Materials_NumProp.intValue == 1 && Body_MaterialProp.objectReferenceValue != null) {
				if (MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue == null) {
					MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue = Body_MaterialProp.objectReferenceValue;
				}
				Body_MaterialProp.objectReferenceValue = null;
			}
			for (int i = 0; i < Materials_NumProp.intValue; i++) {
				MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material", MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}

			// Collider settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Collider settings", MessageType.None, true);
			Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("MeshCollider", Collider_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.Space ();
			Sub_Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Sub MeshCollider", Sub_Collider_MeshProp.objectReferenceValue, typeof(Mesh), false);

			// Physics settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Physics settings", MessageType.None, true);
			EditorGUILayout.IntSlider (SICProp, 1, 100, "Solver Iteration Count");
			EditorGUILayout.Space ();
			Soft_Landing_FlagProp.boolValue = EditorGUILayout.Toggle ("Soft Landing", Soft_Landing_FlagProp.boolValue);
			if (Soft_Landing_FlagProp.boolValue) {
				EditorGUILayout.Slider (Landing_DragProp, 0.1f, 10000.0f, "Landing Drag");
				EditorGUILayout.Slider (Landing_TimeProp, 0.1f, 30.0f, "Landing Time");
			}
			EditorGUILayout.Space ();
			Mass_Center_ZeroProp.boolValue = EditorGUILayout.Toggle ("Set Center of Mass to Zero", Mass_Center_ZeroProp.boolValue);

			// Damage settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Damage settings", MessageType.None, true);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

            // Update Value
            if (GUI.changed || GUILayout.Button("Update Values") || Event.current.commandName == "UndoRedoPerformed")
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
                Create();
            }

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}

		void Create ()
		{
			mainBodyObject.GetComponent <Rigidbody> ().mass = Body_MassProp.floatValue;
			mainBodyObject.GetComponent < MeshFilter > ().mesh = Body_MeshProp.objectReferenceValue as Mesh;

			Material[] materials = new Material [ Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			mainBodyObject.GetComponent < MeshRenderer > ().materials = materials;

			MeshCollider[] meshColliders = mainBodyObject.GetComponents < MeshCollider > ();
			MeshCollider mainCollider;
			MeshCollider subCollider;
			switch (meshColliders.Length) {
			case 0:
				mainCollider = mainBodyObject.AddComponent < MeshCollider > ();
				subCollider = mainBodyObject.AddComponent < MeshCollider > ();
				break;
			case 1:
				mainCollider = meshColliders [0] as MeshCollider;
				subCollider = mainBodyObject.AddComponent < MeshCollider > ();
				break;
			default :
				mainCollider = meshColliders [0] as MeshCollider;
				subCollider = meshColliders [1] as MeshCollider;
				break;
			}
			if (Collider_MeshProp.objectReferenceValue) {
				mainCollider.enabled = true;
				mainCollider.sharedMesh = Collider_MeshProp.objectReferenceValue as Mesh;
				mainCollider.convex = true;
			} else {
				mainCollider.enabled = false;
			}
			if (Sub_Collider_MeshProp.objectReferenceValue) {
				subCollider.enabled = true;
				subCollider.sharedMesh = Sub_Collider_MeshProp.objectReferenceValue as Mesh;
				subCollider.convex = true;
			} else {
				subCollider.enabled = false;
			}
			// Set Layer.
			mainBodyObject.layer = PTS_Layer_Settings_CS.Body_Layer; // Main_Body ( ignore collision with wheels).
		}
	
	}

}