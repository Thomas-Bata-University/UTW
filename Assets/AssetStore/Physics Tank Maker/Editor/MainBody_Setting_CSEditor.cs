using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(MainBody_Setting_CS))]
	public class MainBody_Setting_CSEditor : Editor
	{
	
		SerializedProperty Body_MassProp;
		SerializedProperty Body_MeshProp;
		SerializedProperty Materials_NumProp;
		SerializedProperty MaterialsProp;
		SerializedProperty Colliders_NumProp;
		SerializedProperty Colliders_MeshProp;
		SerializedProperty Collider_MeshProp; // for old versions.
		SerializedProperty Sub_Collider_MeshProp; // for old versions.
		SerializedProperty SICProp;
		SerializedProperty Soft_Landing_FlagProp;
		SerializedProperty Landing_DragProp;
		SerializedProperty Landing_TimeProp;
		SerializedProperty Mass_Center_OffsetProp;
		SerializedProperty Use_Damage_ControlProp;

        SerializedProperty Has_ChangedProp;

        GameObject thisGameObject;


		void OnEnable ()
		{
			Body_MassProp = serializedObject.FindProperty ("Body_Mass");
			Body_MeshProp = serializedObject.FindProperty ("Body_Mesh");
			Materials_NumProp = serializedObject.FindProperty ("Materials_Num");
			MaterialsProp = serializedObject.FindProperty ("Materials");
			Colliders_NumProp = serializedObject.FindProperty ("Colliders_Num");
			Colliders_MeshProp = serializedObject.FindProperty ("Colliders_Mesh");
			Collider_MeshProp = serializedObject.FindProperty ("Collider_Mesh"); // for old versions.
			Sub_Collider_MeshProp = serializedObject.FindProperty ("Sub_Collider_Mesh"); // for old versions.
			SICProp = serializedObject.FindProperty ("SIC");
			Soft_Landing_FlagProp = serializedObject.FindProperty ("Soft_Landing_Flag");
			Landing_DragProp = serializedObject.FindProperty ("Landing_Drag");
			Landing_TimeProp = serializedObject.FindProperty ("Landing_Time");
            Mass_Center_OffsetProp = serializedObject.FindProperty("Mass_Center_Offset");
            Use_Damage_ControlProp = serializedObject.FindProperty ("Use_Damage_Control");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            thisGameObject = Selection.activeGameObject;
		}


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			Set_Inspector ();
		}


		void Set_Inspector ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// Basic Settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Basic Settings", MessageType.None, true);
			EditorGUILayout.Slider (Body_MassProp, 1.0f, 100000.0f, "Mass");
			EditorGUILayout.Space ();
			Body_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Body_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (Materials_NumProp, 1, 99, "Number of Materials");
			MaterialsProp.arraySize = Materials_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Materials_NumProp.intValue; i++) {
				MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Space ();

			// Collider settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Collider settings", MessageType.None, true);
			// for old versions.
			if (Collider_MeshProp.objectReferenceValue || Sub_Collider_MeshProp.objectReferenceValue) {
				Colliders_NumProp.intValue = 2;
				Colliders_MeshProp.arraySize = Colliders_NumProp.intValue;
				Colliders_MeshProp.GetArrayElementAtIndex(0).objectReferenceValue = Collider_MeshProp.objectReferenceValue as Mesh;
				Colliders_MeshProp.GetArrayElementAtIndex(1).objectReferenceValue = Sub_Collider_MeshProp.objectReferenceValue as Mesh;
				Collider_MeshProp.objectReferenceValue = null;
				Sub_Collider_MeshProp.objectReferenceValue = null;
			}
			// for new version.
			EditorGUILayout.IntSlider (Colliders_NumProp, 1, 10, "Number of Colliders");
			Colliders_MeshProp.arraySize = Colliders_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Colliders_NumProp.intValue; i++) {
				Colliders_MeshProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("MeshCollider " + "(" + i + ")", Colliders_MeshProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Mesh), false);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Space ();

			// Physics settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Physics settings", MessageType.None, true);
			EditorGUILayout.IntSlider (SICProp, 1, 100, "Solver Iteration Count");
			Soft_Landing_FlagProp.boolValue = EditorGUILayout.Toggle ("Soft Landing", Soft_Landing_FlagProp.boolValue);
			if (Soft_Landing_FlagProp.boolValue) {
				EditorGUI.indentLevel++;
				EditorGUILayout.Slider (Landing_DragProp, 0.1f, 10000.0f, "Landing Drag");
				EditorGUILayout.Slider (Landing_TimeProp, 0.1f, 30.0f, "Landing Time");
				EditorGUILayout.Space ();
				EditorGUI.indentLevel--;
			}
            Mass_Center_OffsetProp.vector3Value = EditorGUILayout.Vector3Field("Center of Mass Offset", Mass_Center_OffsetProp.vector3Value);
            EditorGUILayout.Space ();

			// Damage settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Damage settings", MessageType.None, true);
			Use_Damage_ControlProp.boolValue = EditorGUILayout.Toggle ("Use Damage Control", Use_Damage_ControlProp.boolValue);

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
			// Rigidbody settings.
			Rigidbody thisRigidbody = thisGameObject.GetComponent <Rigidbody>();
			thisRigidbody.mass = Body_MassProp.floatValue;

			// Mesh settings.
			thisGameObject.GetComponent <MeshFilter>().mesh = Body_MeshProp.objectReferenceValue as Mesh;
			Material[] materials = new Material [ Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			thisGameObject.GetComponent <MeshRenderer>().materials = materials;

			// Collider settings.
			MeshCollider[] oldMeshColliders = thisGameObject.GetComponents <MeshCollider>();
			for (int i = 0; i < oldMeshColliders.Length; i++) {
                var oldCollider = oldMeshColliders[i];
                EditorApplication.delayCall += () => DestroyImmediate(oldCollider);
			}
			for (int i = 0; i < Colliders_NumProp.intValue; i++) {
				MeshCollider meshCollider = thisGameObject.AddComponent <MeshCollider>();
				meshCollider.sharedMesh = Colliders_MeshProp.GetArrayElementAtIndex(i).objectReferenceValue as Mesh;
				meshCollider.convex = true;
			}

			// Add "Damage_Control_00_MainBody_CS" script.
			var damageScript = thisGameObject.GetComponent < Damage_Control_01_MainBody_CS >();
			if (Use_Damage_ControlProp.boolValue) {
				if (damageScript == null) {
					damageScript = thisGameObject.AddComponent < Damage_Control_01_MainBody_CS >();
				}
			}
			else {
				if (damageScript) {
                    EditorApplication.delayCall += () => DestroyImmediate(damageScript);
				}
			}

			// Set the Layer.
			thisGameObject.layer = Layer_Settings_CS.Body_Layer;
        }
	
	}

}