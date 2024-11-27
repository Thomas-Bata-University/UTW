using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_SteeredWheel_CS))]
	public class Create_SteeredWheel_CSEditor : Editor
	{
	
		SerializedProperty Shaft_MassProp;
		SerializedProperty Shaft_MeshProp;
		SerializedProperty Shaft_Materials_NumProp;
		SerializedProperty Shaft_MaterialsProp;
		SerializedProperty Shaft_MaterialProp;
		SerializedProperty Shaft_Collider_RadiusProp;

		SerializedProperty Sus_Vertical_RangeProp;
		SerializedProperty Sus_Vertical_SpringProp;
		SerializedProperty Sus_Vertical_DamperProp;
		SerializedProperty Sus_Torsion_RangeProp;
		SerializedProperty Sus_Torsion_SpringProp;
		SerializedProperty Sus_Torsion_DamperProp;
		SerializedProperty Sus_Anchor_Offset_YProp;
		SerializedProperty Sus_Anchor_Offset_ZProp;
	
		SerializedProperty Hub_DistanceProp;
		SerializedProperty Hub_Offset_YProp;
		SerializedProperty Hub_Offset_ZProp;
		SerializedProperty Hub_MassProp;
		SerializedProperty Hub_SpringProp;
		SerializedProperty Hub_Mesh_LProp;
		SerializedProperty Hub_Mesh_RProp;
		SerializedProperty Hub_Materials_NumProp;
		SerializedProperty Hub_MaterialsProp;
		SerializedProperty Hub_Material_LProp;
		SerializedProperty Hub_Material_RProp;
		SerializedProperty Hub_Anchor_Offset_XProp;
		SerializedProperty Hub_Anchor_Offset_ZProp;
		SerializedProperty Hub_Collider_RadiusProp;

		SerializedProperty Wheel_DistanceProp;
		SerializedProperty Wheel_RadiusProp;
		SerializedProperty Wheel_Collider_MaterialProp;
		SerializedProperty Wheel_Offset_YProp;
		SerializedProperty Wheel_Offset_ZProp;
		SerializedProperty Wheel_MassProp;
		SerializedProperty Wheel_MeshProp;
		SerializedProperty Wheel_Materials_NumProp;
		SerializedProperty Wheel_MaterialsProp;
		SerializedProperty Wheel_MaterialProp;

		SerializedProperty Steer_FlagProp;
		SerializedProperty Reverse_FlagProp;
		SerializedProperty Max_AngleProp;
		SerializedProperty Rotation_SpeedProp;
	
		SerializedProperty Drive_WheelProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;


		void OnEnable ()
		{
			Shaft_MassProp = serializedObject.FindProperty ("Shaft_Mass");
			Shaft_MeshProp = serializedObject.FindProperty ("Shaft_Mesh");
			Shaft_Materials_NumProp = serializedObject.FindProperty ("Shaft_Materials_Num");
			Shaft_MaterialsProp = serializedObject.FindProperty ("Shaft_Materials");
			Shaft_Collider_RadiusProp = serializedObject.FindProperty ("Shaft_Collider_Radius");

			Sus_Vertical_RangeProp = serializedObject.FindProperty ("Sus_Vertical_Range");
			Sus_Vertical_SpringProp = serializedObject.FindProperty ("Sus_Vertical_Spring");
			Sus_Vertical_DamperProp = serializedObject.FindProperty ("Sus_Vertical_Damper");
			Sus_Torsion_RangeProp = serializedObject.FindProperty ("Sus_Torsion_Range");
			Sus_Torsion_SpringProp = serializedObject.FindProperty ("Sus_Torsion_Spring");
			Sus_Torsion_DamperProp = serializedObject.FindProperty ("Sus_Torsion_Damper");
			Sus_Anchor_Offset_YProp = serializedObject.FindProperty ("Sus_Anchor_Offset_Y");
			Sus_Anchor_Offset_ZProp = serializedObject.FindProperty ("Sus_Anchor_Offset_Z");
		
			Hub_DistanceProp = serializedObject.FindProperty ("Hub_Distance");
			Hub_Offset_YProp = serializedObject.FindProperty ("Hub_Offset_Y");
			Hub_Offset_ZProp = serializedObject.FindProperty ("Hub_Offset_Z");
			Hub_MassProp = serializedObject.FindProperty ("Hub_Mass");
			Hub_SpringProp = serializedObject.FindProperty ("Hub_Spring");
			Hub_Mesh_LProp = serializedObject.FindProperty ("Hub_Mesh_L");
			Hub_Mesh_RProp = serializedObject.FindProperty ("Hub_Mesh_R");
			Hub_Materials_NumProp = serializedObject.FindProperty ("Hub_Materials_Num");
			Hub_MaterialsProp = serializedObject.FindProperty ("Hub_Materials");
			Hub_Anchor_Offset_XProp = serializedObject.FindProperty ("Hub_Anchor_Offset_X");
			Hub_Anchor_Offset_ZProp = serializedObject.FindProperty ("Hub_Anchor_Offset_Z");
			Hub_Collider_RadiusProp = serializedObject.FindProperty ("Hub_Collider_Radius");

			Wheel_DistanceProp = serializedObject.FindProperty ("Wheel_Distance");
			Wheel_RadiusProp = serializedObject.FindProperty ("Wheel_Radius");
			Wheel_Collider_MaterialProp = serializedObject.FindProperty ("Wheel_Collider_Material");
			Wheel_MassProp = serializedObject.FindProperty ("Wheel_Mass");
			Wheel_MeshProp = serializedObject.FindProperty ("Wheel_Mesh");
			Wheel_Materials_NumProp = serializedObject.FindProperty ("Wheel_Materials_Num");
			Wheel_MaterialsProp = serializedObject.FindProperty ("Wheel_Materials");
			Wheel_Offset_YProp = serializedObject.FindProperty ("Wheel_Offset_Y");
			Wheel_Offset_ZProp = serializedObject.FindProperty ("Wheel_Offset_Z");

			Steer_FlagProp = serializedObject.FindProperty ("Steer_Flag");
			Reverse_FlagProp = serializedObject.FindProperty ("Reverse_Flag");
			Max_AngleProp = serializedObject.FindProperty ("Max_Angle");
			Rotation_SpeedProp = serializedObject.FindProperty ("Rotation_Speed");
		
			Drive_WheelProp = serializedObject.FindProperty ("Drive_Wheel");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject)
            {
				thisTransform = Selection.activeGameObject.transform;
			}
		}


        public override void OnInspectorGUI()
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

            serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// Axle Shaft settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Axle Shaft settings", MessageType.None, true);
			EditorGUILayout.Slider (Shaft_MassProp, 1.0f, 3000.0f, "Mass");
			Shaft_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Shaft_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.IntSlider (Shaft_Materials_NumProp, 1, 10, "Number of Materials");
			Shaft_MaterialsProp.arraySize = Shaft_Materials_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Shaft_Materials_NumProp.intValue; i++) {
				Shaft_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Shaft_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Slider (Shaft_Collider_RadiusProp, 0.1f, 3.0f, "Shaft Reinforce Radius");

			// Suspension settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Suspension settings", MessageType.None, true);
			EditorGUILayout.Slider (Sus_Vertical_RangeProp, 0.0f, 1.0f, "Vertical Range");
			EditorGUILayout.Slider (Sus_Vertical_SpringProp, 0.0f, 1000000.0f, "Vertical Spring Force");
			EditorGUILayout.Slider (Sus_Vertical_DamperProp, 0.0f, 1000000.0f, "Vertical Damper Force");
			EditorGUILayout.Slider (Sus_Torsion_RangeProp, 0.0f, 90.0f, "Torsion Range");
			EditorGUILayout.Slider (Sus_Torsion_SpringProp, 0.0f, 1000000.0f, "Torsion Spring Force");
			EditorGUILayout.Slider (Sus_Torsion_DamperProp, 0.0f, 1000000.0f, "Torsion Damper Force");
			EditorGUILayout.Slider (Sus_Anchor_Offset_YProp, -10.0f, 10.0f, "Anchor Offset Y");
			EditorGUILayout.Slider (Sus_Anchor_Offset_ZProp, -10.0f, 10.0f, "Anchor Offset Z");

			// Hubs settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Hubs settings", MessageType.None, true);
			EditorGUILayout.Slider (Hub_DistanceProp, 0.1f, 10.0f, "HUb Distance");
			EditorGUILayout.Slider (Hub_Offset_YProp, -10.0f, 10.0f, "Position Offset Y");
			EditorGUILayout.Slider (Hub_Offset_ZProp, -10.0f, 10.0f, "Position Offset Z");
			EditorGUILayout.Slider (Hub_MassProp, 1.0f, 3000.0f, "Mass");
			EditorGUILayout.Slider (Hub_SpringProp, 0.0f, 100000.0f, "Fixing Force");
			Hub_Mesh_LProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", Hub_Mesh_LProp.objectReferenceValue, typeof(Mesh), false);
			Hub_Mesh_RProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", Hub_Mesh_RProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.IntSlider (Hub_Materials_NumProp, 1, 10, "Number of Materials");
			Hub_MaterialsProp.arraySize = Hub_Materials_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Hub_Materials_NumProp.intValue; i++) {
				Hub_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Hub_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Slider (Hub_Anchor_Offset_XProp, -10.0f, 10.0f, "Anchor Offset X");
			EditorGUILayout.Slider (Hub_Anchor_Offset_ZProp, -10.0f, 10.0f, "Anchor Offset Z");
			EditorGUILayout.Slider (Hub_Collider_RadiusProp, 0.1f, 1.0f, "Hub Reinforce Radius");

			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.Slider (Wheel_DistanceProp, 0.1f, 10.0f, "Wheel Distance");
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "Wheel Collider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Wheel_Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Wheel_Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			EditorGUILayout.Slider (Wheel_Offset_YProp, -10.0f, 10.0f, "Position Offset Y");
			EditorGUILayout.Slider (Wheel_Offset_ZProp, -10.0f, 10.0f, "Position Offset Z");
			EditorGUILayout.Slider (Wheel_MassProp, 1.0f, 3000.0f, "Mass");
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.IntSlider (Wheel_Materials_NumProp, 1, 10, "Number of Materials");
			Wheel_MaterialsProp.arraySize = Wheel_Materials_NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < Wheel_Materials_NumProp.intValue; i++) {
				Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
			EditorGUI.indentLevel--;

			// Scripts settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Scripts settings", MessageType.None, true);
			// Steering
			Steer_FlagProp.boolValue = EditorGUILayout.Toggle ("Steer", Steer_FlagProp.boolValue);
			if (Steer_FlagProp.boolValue) {
				EditorGUI.indentLevel++;
				Reverse_FlagProp.boolValue = EditorGUILayout.Toggle ("Reverse", Reverse_FlagProp.boolValue);
				EditorGUILayout.Slider (Max_AngleProp, 0.0f, 180.0f, "Max Steering Angle");
				EditorGUILayout.Slider (Rotation_SpeedProp, 1.0f, 90.0f, "Steering Speed");
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space ();
			// Drive Wheel
			Drive_WheelProp.boolValue = EditorGUILayout.Toggle ("Drive Wheel", Drive_WheelProp.boolValue);
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
            // Delete Objects
            int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}

			// Set "Drive_Wheel_Parent_CS" script in this object.
			Set_Drive_Wheel_Parent_Script();

			// Create Axle Shaft
			Create_Shaft ();

			// Create Hubs
			Vector3 pos;
			pos.x = -Hub_DistanceProp.floatValue / 2.0f;
			pos.y = Hub_Offset_YProp.floatValue;
			pos.z = Hub_Offset_ZProp.floatValue;
			Create_Hub ("L", new Vector3 (pos.x, pos.y, pos.z));
			Create_Hub ("R", new Vector3 (-pos.x, pos.y, pos.z));

			// Create Wheels
			pos.x = Hub_DistanceProp.floatValue / 2.0f;
			pos.y = Wheel_Offset_YProp.floatValue;
			pos.z = Wheel_Offset_ZProp.floatValue;
			Create_Wheel ("L", new Vector3 (pos.x, pos.y, pos.z));
			Create_Wheel ("R", new Vector3 (-pos.x, pos.y, pos.z));
		}


		void Set_Drive_Wheel_Parent_Script()
		{
			// Set "Drive_Wheel_Parent_CS" in this object.
			Drive_Wheel_Parent_CS driveWheelParentScript = thisTransform.GetComponent <Drive_Wheel_Parent_CS>();
			if (driveWheelParentScript == null) {
				driveWheelParentScript = thisTransform.gameObject.AddComponent <Drive_Wheel_Parent_CS>();
			}
			driveWheelParentScript.Drive_Flag = Drive_WheelProp.boolValue;
			driveWheelParentScript.Radius = Wheel_RadiusProp.floatValue;
			driveWheelParentScript.Use_BrakeTurn = false;
		}


		void Create_Shaft ()
		{
			//Create gameobject & Set transform
			GameObject shaftObject = new GameObject ("Axle_Shaft");
			shaftObject.transform.parent = thisTransform;
			shaftObject.transform.localPosition = Vector3.zero;
			shaftObject.transform.localRotation = Quaternion.identity;
			// Mesh
			MeshFilter meshFilter = shaftObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Shaft_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = shaftObject.AddComponent < MeshRenderer > ();
			Material[] materials = new Material [ Shaft_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = Shaft_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
			// Rigidbody
			Rigidbody rigidbody = shaftObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Shaft_MassProp.floatValue;
			// SphereCollider
			SphereCollider sphereCollider = shaftObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Shaft_Collider_RadiusProp.floatValue;
			// ConfigurableJoint
			ConfigurableJoint configJoint = shaftObject.AddComponent < ConfigurableJoint > ();
			configJoint.connectedBody = thisTransform.parent.GetComponent < Rigidbody > ();
			configJoint.anchor = new Vector3 (0.0f, Sus_Anchor_Offset_YProp.floatValue, Sus_Anchor_Offset_ZProp.floatValue);
			configJoint.axis = Vector3.zero;
			configJoint.secondaryAxis = Vector3.zero;
			configJoint.xMotion = ConfigurableJointMotion.Locked;
			configJoint.yMotion = ConfigurableJointMotion.Limited;
			configJoint.zMotion = ConfigurableJointMotion.Locked;
			configJoint.angularXMotion = ConfigurableJointMotion.Locked;
			configJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configJoint.angularZMotion = ConfigurableJointMotion.Limited;
			//
			SoftJointLimit softJointLimit = configJoint.linearLimit; // Set Vertical Range
			softJointLimit.limit = Sus_Vertical_RangeProp.floatValue;
			configJoint.linearLimit = softJointLimit;
			//
			JointDrive jointDrive = configJoint.yDrive; // Set Vertical Spring.
			//jointDrive.mode = JointDriveMode.Position ;
			jointDrive.positionSpring = Sus_Vertical_SpringProp.floatValue;
			jointDrive.positionDamper = Sus_Vertical_DamperProp.floatValue;
			configJoint.yDrive = jointDrive;
			//
			softJointLimit = configJoint.angularZLimit; // Set Torsion Range
			softJointLimit.limit = Sus_Torsion_RangeProp.floatValue;
			configJoint.angularZLimit = softJointLimit;
			//
			jointDrive = configJoint.angularYZDrive; // Set Torsion Spring.
			//jointDrive.mode = JointDriveMode.Position ;
			jointDrive.positionSpring = Sus_Torsion_SpringProp.floatValue;
			jointDrive.positionDamper = Sus_Torsion_DamperProp.floatValue;
			configJoint.angularYZDrive = jointDrive;
			// Set Layer
			shaftObject.layer = Layer_Settings_CS.Reinforce_Layer;
        }


		void Create_Hub (string direction, Vector3 position)
		{
			//Create gameobject & Set transform
			GameObject hubObject = new GameObject ("Hub_" + direction);
			hubObject.transform.parent = thisTransform;
			hubObject.transform.localPosition = position;
			hubObject.transform.localRotation = Quaternion.identity;
			// Mesh
			MeshFilter meshFilter = hubObject.AddComponent < MeshFilter > ();
			if (direction == "L") {
				meshFilter.mesh = Hub_Mesh_LProp.objectReferenceValue as Mesh;
			} else {
				meshFilter.mesh = Hub_Mesh_RProp.objectReferenceValue as Mesh;
			}
			MeshRenderer meshRenderer = hubObject.AddComponent < MeshRenderer > ();
			Material[] materials = new Material [ Hub_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = Hub_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
			// Rigidbody
			Rigidbody rigidbody = hubObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Hub_MassProp.floatValue;
			// HingeJoint
			HingeJoint hingeJoint;
			hingeJoint = hubObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = thisTransform.Find ("Axle_Shaft").GetComponent < Rigidbody > ();
			if (direction == "L") {
				hingeJoint.anchor = new Vector3 (-Hub_Anchor_Offset_XProp.floatValue, 0.0f, Hub_Anchor_Offset_ZProp.floatValue);
			} else {
				hingeJoint.anchor = new Vector3 (Hub_Anchor_Offset_XProp.floatValue, 0.0f, Hub_Anchor_Offset_ZProp.floatValue);
			}
			hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
			hingeJoint.useSpring = true;
			JointSpring jointSpring = hingeJoint.spring;
			jointSpring.spring = Hub_SpringProp.floatValue;
			hingeJoint.spring = jointSpring;
			// SphereCollider
			SphereCollider sphereCollider;
			sphereCollider = hubObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Hub_Collider_RadiusProp.floatValue;
			// Steer_Wheel_CS
			if (Steer_FlagProp.boolValue) {
				Steer_Wheel_CS steerScript = hubObject.AddComponent < Steer_Wheel_CS > ();
				if (Reverse_FlagProp.boolValue) {
					steerScript.Reverse = -1.0f;
				} else {
					steerScript.Reverse = 1.0f;
				}
				steerScript.Max_Angle = Max_AngleProp.floatValue;
				steerScript.Rotation_Speed = Rotation_SpeedProp.floatValue;
			}
			// Set Layer
			hubObject.layer = Layer_Settings_CS.Reinforce_Layer;
        }


		void Create_Wheel (string direction, Vector3 position)
		{
			// Create 'Parent_of_Wheel'
			GameObject parentObject = new GameObject ("Parent_of_Wheel_" + direction);
			parentObject.transform.parent = thisTransform.Find ("Hub_" + direction);
			parentObject.transform.localPosition = position;
			parentObject.transform.localRotation = Quaternion.Euler (0, 0, 90);
			// Create Wheel
			GameObject wheelObject = new GameObject ("SteeredWheel_" + direction);
			wheelObject.transform.parent = parentObject.transform;
			if (direction == "L") {
				wheelObject.transform.localPosition = new Vector3 (0.0f, Wheel_DistanceProp.floatValue / 2.0f, 0.0f);
				wheelObject.transform.localRotation = Quaternion.Euler (0, 0, 0);
			} else {
				wheelObject.transform.localPosition = new Vector3 (0.0f, -Wheel_DistanceProp.floatValue / 2.0f, 0.0f);
				wheelObject.transform.localRotation = Quaternion.Euler (0, 0, 180);
			}
			// Mesh
			MeshFilter meshFilter = wheelObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = wheelObject.AddComponent < MeshRenderer > ();
			Material[] materials = new Material [ Wheel_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
			// Rigidbody
			Rigidbody rigidbody = wheelObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Wheel_MassProp.floatValue;
			// SphereCollider
			SphereCollider sphereCollider = wheelObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Wheel_RadiusProp.floatValue;
			sphereCollider.center = Vector3.zero;
			sphereCollider.material = Wheel_Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// HingeJoint
			HingeJoint hingeJoint;
			hingeJoint = wheelObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = parentObject.transform.parent.GetComponent < Rigidbody > ();
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
			// Drive_Wheel_CS
			Drive_Wheel_CS driveScript = wheelObject.AddComponent <Drive_Wheel_CS> ();
			driveScript.This_Rigidbody = rigidbody;
			driveScript.Is_Left = (direction == "L");
			driveScript.Parent_Script = thisTransform.GetComponent <Drive_Wheel_Parent_CS>();
			// Stabilizer_CS
			Stabilizer_CS stabilizerScript = wheelObject.AddComponent <Stabilizer_CS>();
			stabilizerScript.This_Transform = wheelObject.transform;
			stabilizerScript.Is_Left = (direction == "L");
			stabilizerScript.Initial_Pos_Y = wheelObject.transform.localPosition.y;
			stabilizerScript.Initial_Angles = wheelObject.transform.localEulerAngles;
			// Set Layer
			wheelObject.layer = Layer_Settings_CS.Wheels_Layer;
        }
	
	}

}