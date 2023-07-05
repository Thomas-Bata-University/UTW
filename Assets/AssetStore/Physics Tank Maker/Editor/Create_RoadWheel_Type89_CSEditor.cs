using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_RoadWheel_Type89_CS))]
	public class Create_RoadWheel_Type89_CSEditor : Editor
	{

		SerializedProperty Fit_ST_FlagProp;

		SerializedProperty DistanceProp;
		SerializedProperty ParentArm_NumProp;
		SerializedProperty ParentArm_SpacingProp;
		SerializedProperty ParentArm_Offset_YProp;
		SerializedProperty ParentArm_MassProp;
        SerializedProperty ParentArm_LinearLimitProp;
        SerializedProperty ParentArm_AngleLimitProp;
		SerializedProperty ParentArm_SpringProp;
		SerializedProperty ParentArm_DamperProp;
		SerializedProperty ParentArm_L_MeshProp;
		SerializedProperty ParentArm_R_MeshProp;
		SerializedProperty ParentArm_L_MaterialProp;
		SerializedProperty ParentArm_R_MaterialProp;
	
		SerializedProperty ChildArm_NumProp;
		SerializedProperty ChildArm_SpacingProp;
		SerializedProperty ChildArm_Offset_YProp;
		SerializedProperty ChildArm_MassProp;
		SerializedProperty ChildArm_AngleLimitProp;
		SerializedProperty ChildArm_DamperProp;
		SerializedProperty ChildArm_L_MeshProp;
		SerializedProperty ChildArm_R_MeshProp;
		SerializedProperty ChildArm_L_MaterialProp;
		SerializedProperty ChildArm_R_MaterialProp;
	
		SerializedProperty Wheel_NumProp;
		SerializedProperty Wheel_SpacingProp;
		SerializedProperty Wheel_Offset_YProp;
		SerializedProperty Wheel_MassProp;
		SerializedProperty Wheel_RadiusProp;
		SerializedProperty Wheel_Collider_MaterialProp;
		SerializedProperty Wheel_MeshProp;
		SerializedProperty Wheel_MaterialProp;

		SerializedProperty Drive_WheelProp;
		SerializedProperty Wheel_ResizeProp;
		SerializedProperty ScaleDown_SizeProp;
		SerializedProperty Return_SpeedProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;


		void OnEnable ()
		{
			Fit_ST_FlagProp = serializedObject.FindProperty ("Fit_ST_Flag");

			DistanceProp = serializedObject.FindProperty ("Distance");
			ParentArm_NumProp = serializedObject.FindProperty ("ParentArm_Num");
			ParentArm_SpacingProp = serializedObject.FindProperty ("ParentArm_Spacing");
			ParentArm_Offset_YProp = serializedObject.FindProperty ("ParentArm_Offset_Y");
			ParentArm_MassProp = serializedObject.FindProperty ("ParentArm_Mass");
            ParentArm_LinearLimitProp = serializedObject.FindProperty("ParentArm_LinearLimit");
            ParentArm_AngleLimitProp = serializedObject.FindProperty ("ParentArm_AngleLimit");
			ParentArm_SpringProp = serializedObject.FindProperty ("ParentArm_Spring");
			ParentArm_DamperProp = serializedObject.FindProperty ("ParentArm_Damper");
			ParentArm_L_MeshProp = serializedObject.FindProperty ("ParentArm_L_Mesh");
			ParentArm_R_MeshProp = serializedObject.FindProperty ("ParentArm_R_Mesh");
			ParentArm_L_MaterialProp = serializedObject.FindProperty ("ParentArm_L_Material");
			ParentArm_R_MaterialProp = serializedObject.FindProperty ("ParentArm_R_Material");
		
			ChildArm_NumProp = serializedObject.FindProperty ("ChildArm_Num");
			ChildArm_SpacingProp = serializedObject.FindProperty ("ChildArm_Spacing");
			ChildArm_Offset_YProp = serializedObject.FindProperty ("ChildArm_Offset_Y");
			ChildArm_MassProp = serializedObject.FindProperty ("ChildArm_Mass");
			ChildArm_AngleLimitProp = serializedObject.FindProperty ("ChildArm_AngleLimit");
			ChildArm_DamperProp = serializedObject.FindProperty ("ChildArm_Damper");
			ChildArm_L_MeshProp = serializedObject.FindProperty ("ChildArm_L_Mesh");
			ChildArm_R_MeshProp = serializedObject.FindProperty ("ChildArm_R_Mesh");
			ChildArm_L_MaterialProp = serializedObject.FindProperty ("ChildArm_L_Material");
			ChildArm_R_MaterialProp = serializedObject.FindProperty ("ChildArm_R_Material");
		
			Wheel_NumProp = serializedObject.FindProperty ("Wheel_Num");
			Wheel_SpacingProp = serializedObject.FindProperty ("Wheel_Spacing");
			Wheel_Offset_YProp = serializedObject.FindProperty ("Wheel_Offset_Y");
			Wheel_MassProp = serializedObject.FindProperty ("Wheel_Mass");
			Wheel_RadiusProp = serializedObject.FindProperty ("Wheel_Radius");
			Wheel_Collider_MaterialProp = serializedObject.FindProperty ("Wheel_Collider_Material");
			Wheel_MeshProp = serializedObject.FindProperty ("Wheel_Mesh");
			Wheel_MaterialProp = serializedObject.FindProperty ("Wheel_Material");

			Drive_WheelProp = serializedObject.FindProperty ("Drive_Wheel");
			Wheel_ResizeProp = serializedObject.FindProperty ("Wheel_Resize");
			ScaleDown_SizeProp = serializedObject.FindProperty ("ScaleDown_Size");
			Return_SpeedProp = serializedObject.FindProperty ("Return_Speed");

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

            serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// for Static Wheel
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheel Type", MessageType.None, true);
			Fit_ST_FlagProp.boolValue = EditorGUILayout.Toggle ("Fit for Static Tracks", Fit_ST_FlagProp.boolValue);

			// 'Parent Arm' settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("'Parent Arm' settings", MessageType.None, true);
			EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.IntSlider (ParentArm_NumProp, 0, 3, "Number");
			EditorGUILayout.Slider (ParentArm_SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (ParentArm_Offset_YProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (ParentArm_MassProp, 0.1f, 300.0f, "Mass");
            EditorGUILayout.Slider(ParentArm_LinearLimitProp, 0.0f, 1.0f, "Limit Distance");
            EditorGUILayout.Slider (ParentArm_AngleLimitProp, 0.0f, 90.0f, "Limit Angle");
			EditorGUILayout.Slider (ParentArm_SpringProp, 0.0f, 1000000.0f, "Spring Force");
			if (ParentArm_SpringProp.floatValue == 1000000.0f) {
				ParentArm_SpringProp.floatValue = Mathf.Infinity;
			}
			EditorGUILayout.Slider (ParentArm_DamperProp, 0.0f, 1000000.0f, "Damper Force");
			ParentArm_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", ParentArm_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ParentArm_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", ParentArm_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ParentArm_L_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Left", ParentArm_L_MaterialProp.objectReferenceValue, typeof(Material), false);
			ParentArm_R_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Right", ParentArm_R_MaterialProp.objectReferenceValue, typeof(Material), false);

			// 'Child Arm' settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("'Child Arm' settings", MessageType.None, true);
			EditorGUILayout.IntSlider (ChildArm_NumProp, 0, 3, "Number");
			EditorGUILayout.Slider (ChildArm_SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (ChildArm_Offset_YProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (ChildArm_MassProp, 0.1f, 300.0f, "Mass");
			EditorGUILayout.Slider (ChildArm_AngleLimitProp, 0.0f, 90.0f, "Limit Angle");
			EditorGUILayout.Slider (ChildArm_DamperProp, 0.0f, 1000.0f, "Damper Force");
			ChildArm_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", ChildArm_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ChildArm_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", ChildArm_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ChildArm_L_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Left", ChildArm_L_MaterialProp.objectReferenceValue, typeof(Material), false);
			ChildArm_R_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Right", ChildArm_R_MaterialProp.objectReferenceValue, typeof(Material), false);

			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.IntSlider (Wheel_NumProp, 0, 3, "Number");
			EditorGUILayout.Slider (Wheel_SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (Wheel_Offset_YProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (Wheel_MassProp, 0.1f, 300.0f, "Mass");
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "Wheel Collider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Wheel_Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Wheel_Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);
			Wheel_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material", Wheel_MaterialProp.objectReferenceValue, typeof(Material), false);
			// Scripts settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Scripts settings", MessageType.None, true);
			// Drive Wheel
			Drive_WheelProp.boolValue = EditorGUILayout.Toggle ("Drive Wheel", Drive_WheelProp.boolValue);
			EditorGUILayout.Space ();
			// Wheel Resize
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				Wheel_ResizeProp.boolValue = EditorGUILayout.Toggle ("Wheel Resize Script", Wheel_ResizeProp.boolValue);
				if (Wheel_ResizeProp.boolValue) {
					EditorGUILayout.Slider (ScaleDown_SizeProp, 0.1f, 3.0f, "Scale Size");
					EditorGUILayout.Slider (Return_SpeedProp, 0.01f, 0.1f, "Return Speed");
				}
				EditorGUILayout.Space ();
			}

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
            // Delete Objects
            int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}

			// Set "Drive_Wheel_Parent_CS" script in this object.
			Set_Drive_Wheel_Parent_Script();

			// Create Parent Arm
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				Vector3 pos;
				pos.x = 0.0f;	
				pos.y = DistanceProp.floatValue / 2.0f;
				pos.z = -ParentArm_SpacingProp.floatValue * i;
				int num = i + 1;
				Set_ParentArm ("L", pos, num);
			}
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				Vector3 pos;
				pos.x = 0.0f;	
				pos.y = -DistanceProp.floatValue / 2.0f;
				pos.z = -ParentArm_SpacingProp.floatValue * i;
				int num = i + 1;
				Set_ParentArm ("R", pos, num);
			}

			// Create Child Arm
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					Vector3 pos;
					pos.x = ChildArm_Offset_YProp.floatValue;
					pos.y = DistanceProp.floatValue / 2.0f;
					pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
					pos.z += (-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j);
					int num = (j + 1) + (ChildArm_NumProp.intValue * i);
					int parentNum = i + 1;
					Set_ChildArm ("L", pos, num, parentNum);
				}
			}
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					Vector3 pos;
					pos.x = ChildArm_Offset_YProp.floatValue;
					pos.y = -DistanceProp.floatValue / 2.0f;
					pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
					pos.z += (-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j);
					int num = (j + 1) + (ChildArm_NumProp.intValue * i);
					int parentNum = i + 1;
					Set_ChildArm ("R", pos, num, parentNum);
				}
			}

			// Create Wheel
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					for (int k = 0; k < Wheel_NumProp.intValue; k++) {
						Vector3 pos;
						pos.x = ChildArm_Offset_YProp.floatValue + Wheel_Offset_YProp.floatValue;
						pos.y = DistanceProp.floatValue / 2.0f;
						pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
						pos.z += (Wheel_NumProp.intValue - 1) * Wheel_SpacingProp.floatValue / 2.0f;
						pos.z += ((-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j)) + (-Wheel_SpacingProp.floatValue * k);
						int num = (k + 1) + (Wheel_NumProp.intValue * j) + (Wheel_NumProp.intValue * ChildArm_NumProp.intValue * i);
						int parentNum = (j + 1) + (ChildArm_NumProp.intValue * i);
						Set_Wheel ("L", pos, num, parentNum);
					}
				}
			}
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					for (int k = 0; k < Wheel_NumProp.intValue; k++) {
						Vector3 pos;
						pos.x = ChildArm_Offset_YProp.floatValue + Wheel_Offset_YProp.floatValue;
						pos.y = -DistanceProp.floatValue / 2.0f;
						pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
						pos.z += (Wheel_NumProp.intValue - 1) * Wheel_SpacingProp.floatValue / 2.0f;
						pos.z += ((-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j)) + (-Wheel_SpacingProp.floatValue * k);
						int num = (k + 1) + (Wheel_NumProp.intValue * j) + (Wheel_NumProp.intValue * ChildArm_NumProp.intValue * i);
						int parentNum = (j + 1) + (ChildArm_NumProp.intValue * i);
						Set_Wheel ("R", pos, num, parentNum);
					}
				}
			}
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
			driveWheelParentScript.Use_BrakeTurn = true;
		}


		void Set_ParentArm (string direction, Vector3 position, int number)
		{
			// Create ParentArm GameObject
			GameObject armObject = new GameObject ("ParentArm_" + direction + "_" + number);
			armObject.transform.parent = thisTransform;
			armObject.transform.localPosition = position;
			armObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
			// Mesh
			MeshFilter meshFilter = armObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = armObject.AddComponent < MeshRenderer > ();
			if (direction == "L") {
				meshFilter.mesh = ParentArm_L_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ParentArm_L_MaterialProp.objectReferenceValue as Material;
			} else {
				meshFilter.mesh = ParentArm_R_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ParentArm_R_MaterialProp.objectReferenceValue as Material;
			}
			// Rigidbody
			Rigidbody rigidbody = armObject.AddComponent < Rigidbody > ();
			rigidbody.mass = ParentArm_MassProp.floatValue;
			// Reinforce SphereCollider
			SphereCollider sphereCollider = armObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = 0.45f;
			// ConfigurableJoint
			ConfigurableJoint configJoint = armObject.AddComponent < ConfigurableJoint > ();
			configJoint.connectedBody = thisTransform.parent.gameObject.GetComponent < Rigidbody > ();
			configJoint.anchor = new Vector3 (0.0f, ParentArm_Offset_YProp.floatValue, 0.0f);
			configJoint.axis = Vector3.zero;
			configJoint.secondaryAxis = Vector3.zero;
			configJoint.xMotion = ConfigurableJointMotion.Locked;
			configJoint.yMotion = ConfigurableJointMotion.Limited;
			configJoint.zMotion = ConfigurableJointMotion.Locked;
			configJoint.angularXMotion = ConfigurableJointMotion.Limited;
			configJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configJoint.angularZMotion = ConfigurableJointMotion.Locked;
			SoftJointLimit softJointLimit = configJoint.linearLimit; // Set Linear Limit
            softJointLimit.limit = ParentArm_LinearLimitProp.floatValue;
			configJoint.linearLimit = softJointLimit;
			softJointLimit = configJoint.lowAngularXLimit; // Set Low Angular XLimit
			softJointLimit.limit = -ParentArm_AngleLimitProp.floatValue;
			configJoint.lowAngularXLimit = softJointLimit;
			softJointLimit = configJoint.highAngularXLimit; // Set High Angular XLimit
			softJointLimit.limit = ParentArm_AngleLimitProp.floatValue;
			configJoint.highAngularXLimit = softJointLimit;
			JointDrive jointDrive = configJoint.yDrive; // Set Vertical Spring.
			//jointDrive.mode = JointDriveMode.Position ;
			jointDrive.positionSpring = ParentArm_SpringProp.floatValue;
			jointDrive.positionDamper = ParentArm_DamperProp.floatValue;
			configJoint.yDrive = jointDrive;
			// Set Layer
			armObject.layer = Layer_Settings_CS.Reinforce_Layer;
        }


		void Set_ChildArm (string direction, Vector3 position, int number, int parentNumber)
		{
			// Create ChildArm GameObject
			GameObject armObject = new GameObject ("ChildArm_" + direction + "_" + number);
			armObject.transform.parent = thisTransform;
			armObject.transform.localPosition = position;
			armObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
			// Add components
			MeshFilter meshFilter = armObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = armObject.AddComponent < MeshRenderer > ();
			if (direction == "L") {
				meshFilter.mesh = ChildArm_L_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ChildArm_L_MaterialProp.objectReferenceValue as Material;
			} else {
				meshFilter.mesh = ChildArm_R_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ChildArm_R_MaterialProp.objectReferenceValue as Material;
			}
			// Rigidbody
			Rigidbody rigidbody = armObject.AddComponent < Rigidbody > ();
			rigidbody.mass = ChildArm_MassProp.floatValue;
			// Reinforce SphereCollider
			SphereCollider sphereCollider = armObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = 0.3f;
			// HingeJoint
			HingeJoint hingeJoint = armObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = thisTransform.Find ("ParentArm_" + direction + "_" + parentNumber).gameObject.GetComponent < Rigidbody > ();
			hingeJoint.anchor = new Vector3 (0.0f, ChildArm_Offset_YProp.floatValue, 0.0f);
			hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
			hingeJoint.useLimits = true;
			JointLimits jointLimits = hingeJoint.limits;
			jointLimits.max = ChildArm_AngleLimitProp.floatValue;
			jointLimits.min = -ChildArm_AngleLimitProp.floatValue;
			hingeJoint.limits = jointLimits;
			hingeJoint.useSpring = true;
			JointSpring jointSpring = hingeJoint.spring;
			jointSpring.damper = ChildArm_DamperProp.floatValue;
			hingeJoint.spring = jointSpring;
			// Set Layer
			armObject.layer = Layer_Settings_CS.Reinforce_Layer;
        }


		void Set_Wheel (string direction, Vector3 position, int number, int parentNumber)
		{
			// Create RoadWheel GameObject
			GameObject wheelObject = new GameObject ("RoadWheel_" + direction + "_" + number);
			wheelObject.transform.parent = thisTransform;
			wheelObject.transform.localPosition = position;
			if (direction == "L") {
				wheelObject.transform.localRotation = Quaternion.identity;
			} else {
				wheelObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 180.0f);
			}
			// Mesh
			MeshFilter meshFilter = wheelObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = wheelObject.AddComponent < MeshRenderer > ();
			meshRenderer.material = Wheel_MaterialProp.objectReferenceValue as Material;
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
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
			hingeJoint.connectedBody = thisTransform.Find ("ChildArm_" + direction + "_" + parentNumber).gameObject.GetComponent < Rigidbody > ();
			// Drive_Wheel_CS
			Drive_Wheel_CS driveScript = wheelObject.AddComponent <Drive_Wheel_CS> ();
			driveScript.This_Rigidbody = rigidbody;
			driveScript.Is_Left = (direction == "L");
			driveScript.Parent_Script = thisTransform.GetComponent <Drive_Wheel_Parent_CS>();
			// Fix_Shaking_Rotation_CS
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				Fix_Shaking_Rotation_CS fixScript = wheelObject.AddComponent <Fix_Shaking_Rotation_CS>();
				fixScript.Is_Left = (direction == "L");
				fixScript.This_Transform = wheelObject.transform;
			}
			// Wheel_Resize_CS
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				if (Wheel_ResizeProp.boolValue) {
					Wheel_Resize_CS resizeScript;
					resizeScript = wheelObject.AddComponent < Wheel_Resize_CS > ();
					resizeScript.ScaleDown_Size = ScaleDown_SizeProp.floatValue;
					resizeScript.Return_Speed = Return_SpeedProp.floatValue;
				}
			}
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