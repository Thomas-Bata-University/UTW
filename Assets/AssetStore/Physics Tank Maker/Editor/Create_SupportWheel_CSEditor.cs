using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_SupportWheel_CS))]
	public class Create_SupportWheel_CSEditor : Editor
	{

		SerializedProperty Wheel_DistanceProp;
		SerializedProperty NumProp;
		SerializedProperty SpacingProp;
		SerializedProperty Wheel_MassProp;
		SerializedProperty Wheel_RadiusProp;
		SerializedProperty Collider_MaterialProp;
		SerializedProperty Wheel_MeshProp;
		SerializedProperty Wheel_Materials_NumProp;
		SerializedProperty Wheel_MaterialsProp;
		SerializedProperty Wheel_MaterialProp; // for old versions.
		SerializedProperty Drive_WheelProp;
		SerializedProperty Wheel_ResizeProp;
		SerializedProperty ScaleDown_SizeProp;
		SerializedProperty Return_SpeedProp;
		SerializedProperty Static_FlagProp;
		SerializedProperty Radius_OffsetProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;


		void OnEnable ()
		{
			Wheel_DistanceProp = serializedObject.FindProperty ("Wheel_Distance");
			NumProp = serializedObject.FindProperty ("Num");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			Wheel_MassProp = serializedObject.FindProperty ("Wheel_Mass");
			Wheel_RadiusProp = serializedObject.FindProperty ("Wheel_Radius");
			Collider_MaterialProp = serializedObject.FindProperty ("Collider_Material");
			Wheel_MeshProp = serializedObject.FindProperty ("Wheel_Mesh");
			Wheel_Materials_NumProp = serializedObject.FindProperty ("Wheel_Materials_Num");
			Wheel_MaterialsProp = serializedObject.FindProperty ("Wheel_Materials");
			Wheel_MaterialProp = serializedObject.FindProperty ("Wheel_Material");
			Drive_WheelProp = serializedObject.FindProperty ("Drive_Wheel");
			Wheel_ResizeProp = serializedObject.FindProperty ("Wheel_Resize");
			ScaleDown_SizeProp = serializedObject.FindProperty ("ScaleDown_Size");
			Return_SpeedProp = serializedObject.FindProperty ("Return_Speed");
			Static_FlagProp = serializedObject.FindProperty ("Static_Flag");
			Radius_OffsetProp = serializedObject.FindProperty ("Radius_Offset");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject)
            {
				thisTransform = Selection.activeGameObject.transform;
			}
		}


		public override void OnInspectorGUI ()
		{
            bool isPrepared;
            if (Application.isPlaying || thisTransform.parent == null || thisTransform.parent.gameObject.GetComponent<Rigidbody>() == null)
            {
                isPrepared = false;
            }
            else
            {
                isPrepared = true;
            }

            if (isPrepared)
            {
                // Keep rotation.
                Vector3 localAngles = thisTransform.localEulerAngles;
                localAngles.z = 90.0f;
                thisTransform.localEulerAngles = localAngles;

                // Set Inspector window.
                Set_Inspector();
            }
        }


		void Set_Inspector ()
		{
            // Check the Prefab Mode.
            if (PrefabUtility.IsPartOfPrefabInstance(thisTransform))
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("\n'The wheels can be modified only in the Prefab Mode.\nPlease go to the Prefab Mode, or Unpack the prefab.\n", MessageType.Warning, true);
                return;
            }

            serializedObject.Update();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// for Static Wheel
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheel Type", MessageType.None, true);
			Static_FlagProp.boolValue = EditorGUILayout.Toggle ("Static Wheel", Static_FlagProp.boolValue);
			if (Static_FlagProp.boolValue) {
				EditorGUILayout.Slider (Radius_OffsetProp, -0.5f, 0.5f, "Radius Offset");
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Update Values", GUILayout.Width (200))) {
					Create ();
				}
			}
			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.Slider (Wheel_DistanceProp, 0.1f, 10.0f, "Wheel Distance");
			EditorGUILayout.IntSlider (NumProp, 0, 30, "Number");
			EditorGUILayout.Slider (SpacingProp, 0.1f, 10.0f, "Spacing");
			if (!Static_FlagProp.boolValue) { // Physics Wheel.
				EditorGUILayout.Slider (Wheel_MassProp, 0.1f, 300.0f, "Mass");
			}
			EditorGUILayout.Space ();
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "Wheel Collider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			EditorGUILayout.Space ();
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Wheel Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.IntSlider (Wheel_Materials_NumProp, 1, 10, "Number of Materials");
			Wheel_MaterialsProp.arraySize = Wheel_Materials_NumProp.intValue;
			if (Wheel_Materials_NumProp.intValue == 1 && Wheel_MaterialProp.objectReferenceValue != null) {
				if (Wheel_MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue == null) {
					Wheel_MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue = Wheel_MaterialProp.objectReferenceValue;
				}
				Wheel_MaterialProp.objectReferenceValue = null;
			}
			EditorGUI.indentLevel++;
			for (int i = 0; i < Wheel_Materials_NumProp.intValue; i++) {
				Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
			EditorGUI.indentLevel--;

			if (!Static_FlagProp.boolValue) {
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				// Scripts settings
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Scripts settings", MessageType.None, true);
				// Drive Wheel
				Drive_WheelProp.boolValue = EditorGUILayout.Toggle ("Drive Wheel", Drive_WheelProp.boolValue);
				EditorGUILayout.Space ();
				// Wheel Resize
				Wheel_ResizeProp.boolValue = EditorGUILayout.Toggle ("Wheel Resize Script", Wheel_ResizeProp.boolValue);
				if (Wheel_ResizeProp.boolValue) {
					EditorGUILayout.Slider (ScaleDown_SizeProp, 0.1f, 3.0f, "Scale Size");
					EditorGUILayout.Slider (Return_SpeedProp, 0.01f, 0.1f, "Return Speed");
				}
				EditorGUILayout.Space ();
			}

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Update Value
            if (GUI.changed || GUILayout.Button("Update Values") || Event.current.commandName == "UndoRedoPerformed")
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
                Create();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties ();
		}


		void Create ()
		{
            // Delete the old wheels.
            int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}

			// For the parent script.
			if (Static_FlagProp.boolValue) {
				Set_Static_Wheel_Parent_Script();
			}
			else {
				Set_Drive_Wheel_Parent_Script();
			}

			// Create the new wheels.
			Vector3 pos;
			for (int i = 0; i < NumProp.intValue; i++) {
				pos.x = 0.0f;
				pos.y = Wheel_DistanceProp.floatValue / 2.0f;
				pos.z = -SpacingProp.floatValue * i;
				Create_Wheel ("L", pos, i + 1);
			}
			for (int i = 0; i < NumProp.intValue; i++) {
				pos.x = 0.0f;
				pos.y = -Wheel_DistanceProp.floatValue / 2.0f;
				pos.z = -SpacingProp.floatValue * i;
				Create_Wheel ("R", pos, i + 1);
			}
		}


		void Set_Drive_Wheel_Parent_Script()
		{
			// Remove "Static_Wheel_Parent_CS" in this object.
			Static_Wheel_Parent_CS staticWheelParentScript = thisTransform.GetComponent <Static_Wheel_Parent_CS>();
			if (staticWheelParentScript) {
                EditorApplication.delayCall += () => DestroyImmediate(staticWheelParentScript);
            }

            // Set "Drive_Wheel_Parent_CS" in this object.
            Drive_Wheel_Parent_CS driveWheelParentScript = thisTransform.GetComponent <Drive_Wheel_Parent_CS>();
			if (driveWheelParentScript == null) {
				driveWheelParentScript = thisTransform.gameObject.AddComponent <Drive_Wheel_Parent_CS>();
			}
			driveWheelParentScript.Drive_Flag = Drive_WheelProp.boolValue;
			driveWheelParentScript.Radius = Wheel_RadiusProp.floatValue;
			driveWheelParentScript.Use_BrakeTurn = true;
		}


		void Set_Static_Wheel_Parent_Script()
		{
            // Remove "Drive_Wheel_Parent_CS" in this object.
			Drive_Wheel_Parent_CS driveWheelParentScript = thisTransform.GetComponent <Drive_Wheel_Parent_CS>();
			if (driveWheelParentScript) {
                EditorApplication.delayCall += () => DestroyImmediate(driveWheelParentScript);
            }

            // Set "Static_Wheel_Parent_CS" in this object.
            Static_Wheel_Parent_CS staticWheelParentScript = thisTransform.GetComponent <Static_Wheel_Parent_CS>();
			if (staticWheelParentScript == null) {
				staticWheelParentScript = thisTransform.gameObject.AddComponent <Static_Wheel_Parent_CS>();
			}
			Mesh wheelMesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			float wheelRadius = wheelMesh.bounds.extents.x;
			staticWheelParentScript.Wheel_Radius = wheelRadius + Radius_OffsetProp.floatValue;
		}


		void Create_Wheel(string direction, Vector3 position, int number)
		{
			// Create a new gameobject.
			GameObject wheelObject = new GameObject("SupportWheel_" + direction + "_" + number);
			wheelObject.transform.parent = thisTransform;
			wheelObject.transform.localPosition = position;
			if (direction == "L") {
				wheelObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
			}
			else {
				wheelObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
			}
			wheelObject.layer = Layer_Settings_CS.Wheels_Layer;

            // Mesh
            MeshFilter meshFilter = wheelObject.AddComponent < MeshFilter >();
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = wheelObject.AddComponent < MeshRenderer >();
			Material[] materials = new Material [ Wheel_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials[i] = Wheel_MaterialsProp.GetArrayElementAtIndex(i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;

			// SphereCollider
			SphereCollider sphereCollider = wheelObject.AddComponent < SphereCollider >();
			sphereCollider.radius = Wheel_RadiusProp.floatValue;
			sphereCollider.center = Vector3.zero;
			sphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;

			// Other components.
			if (Static_FlagProp.boolValue) { // Static_Wheel
				Set_Static_Wheel(wheelObject, direction);
			}
			else { // Driving_Wheel
				Set_Physics_Wheel(wheelObject, direction);
			}
		}


		void Set_Physics_Wheel(GameObject wheelObject, string direction)
		{
			// Rigidbody
			Rigidbody rigidbody = wheelObject.AddComponent < Rigidbody >();
			rigidbody.mass = Wheel_MassProp.floatValue;
			// HingeJoint
			HingeJoint hingeJoint = wheelObject.AddComponent < HingeJoint >();
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.axis = new Vector3(0.0f, 1.0f, 0.0f);
			hingeJoint.connectedBody = thisTransform.parent.gameObject.GetComponent < Rigidbody >();
			// Drive_Wheel_CS
			Drive_Wheel_CS driveScript = wheelObject.AddComponent <Drive_Wheel_CS>();
			driveScript.This_Rigidbody = rigidbody;
			driveScript.Is_Left = (direction == "L");
			driveScript.Parent_Script = thisTransform.GetComponent <Drive_Wheel_Parent_CS>();
			// Wheel_Resize_CS
			if (Wheel_ResizeProp.boolValue) {
				Wheel_Resize_CS resizeScript = wheelObject.AddComponent < Wheel_Resize_CS >();
				resizeScript.ScaleDown_Size = ScaleDown_SizeProp.floatValue;
				resizeScript.Return_Speed = Return_SpeedProp.floatValue;
			}
			// Fix_Shaking_Rotation_CS
			Fix_Shaking_Rotation_CS fixScript = wheelObject.AddComponent <Fix_Shaking_Rotation_CS>();
			fixScript.Is_Left = (direction == "L");
			fixScript.This_Transform = wheelObject.transform;
			// Stabilizer_CS
			Stabilizer_CS stabilizerScript = wheelObject.AddComponent <Stabilizer_CS>();
			stabilizerScript.This_Transform = wheelObject.transform;
			stabilizerScript.Is_Left = (direction == "L");
			stabilizerScript.Initial_Pos_Y = wheelObject.transform.localPosition.y;
			stabilizerScript.Initial_Angles = wheelObject.transform.localEulerAngles;
		}


		void Set_Static_Wheel (GameObject wheelObject, string direction)
		{
			Static_Wheel_CS staticWheelScript = wheelObject.AddComponent <Static_Wheel_CS>();
			staticWheelScript.Is_Left = (direction == "L");
			staticWheelScript.Parent_Script = thisTransform.GetComponent <Static_Wheel_Parent_CS>();
		}

	}

}