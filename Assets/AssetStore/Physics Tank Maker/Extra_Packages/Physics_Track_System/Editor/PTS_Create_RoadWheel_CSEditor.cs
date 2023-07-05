using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTS
{

	[ CustomEditor (typeof(PTS_Create_RoadWheel_CS))]
	public class PTS_Create_RoadWheel_CSEditor : Editor
	{

		SerializedProperty Fit_ST_FlagProp;

		SerializedProperty Sus_DistanceProp;
		SerializedProperty NumProp;
		SerializedProperty SpacingProp;
		SerializedProperty Sus_LengthProp;
		SerializedProperty Set_IndividuallyProp;
		SerializedProperty Sus_AngleProp;
		SerializedProperty Sus_AnglesProp;
		SerializedProperty Sus_AnchorProp;
		SerializedProperty Sus_MassProp;
		SerializedProperty Sus_SpringProp;
		SerializedProperty Sus_DamperProp;
		SerializedProperty Sus_TargetProp;
		SerializedProperty Sus_Forward_LimitProp;
		SerializedProperty Sus_Backward_LimitProp;
		SerializedProperty Sus_L_MeshProp;
		SerializedProperty Sus_R_MeshProp;
		SerializedProperty Sus_Materials_NumProp;
		SerializedProperty Sus_MaterialsProp;
		SerializedProperty Sus_L_MaterialProp;
		SerializedProperty Sus_R_MaterialProp;
		SerializedProperty Reinforce_RadiusProp;

		SerializedProperty Wheel_DistanceProp;
		SerializedProperty Wheel_MassProp;
		SerializedProperty Wheel_RadiusProp;
		SerializedProperty Collider_MaterialProp;
		SerializedProperty Wheel_MeshProp;
		SerializedProperty Wheel_Materials_NumProp;
		SerializedProperty Wheel_MaterialsProp;
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

			Sus_DistanceProp = serializedObject.FindProperty ("Sus_Distance");
			NumProp = serializedObject.FindProperty ("Num");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			Sus_LengthProp = serializedObject.FindProperty ("Sus_Length");
			Set_IndividuallyProp = serializedObject.FindProperty ("Set_Individually");
			Sus_AngleProp = serializedObject.FindProperty ("Sus_Angle");
			Sus_AnglesProp = serializedObject.FindProperty ("Sus_Angles");
			Sus_AnchorProp = serializedObject.FindProperty ("Sus_Anchor");
			Sus_MassProp = serializedObject.FindProperty ("Sus_Mass");
			Sus_SpringProp = serializedObject.FindProperty ("Sus_Spring");
			Sus_DamperProp = serializedObject.FindProperty ("Sus_Damper");
			Sus_TargetProp = serializedObject.FindProperty ("Sus_Target");
			Sus_Forward_LimitProp = serializedObject.FindProperty ("Sus_Forward_Limit");
			Sus_Backward_LimitProp = serializedObject.FindProperty ("Sus_Backward_Limit");
			Sus_L_MeshProp = serializedObject.FindProperty ("Sus_L_Mesh");
			Sus_R_MeshProp = serializedObject.FindProperty ("Sus_R_Mesh");
			Sus_Materials_NumProp = serializedObject.FindProperty ("Sus_Materials_Num");
			Sus_MaterialsProp = serializedObject.FindProperty ("Sus_Materials");
			Sus_L_MaterialProp = serializedObject.FindProperty ("Sus_L_Material");
			Sus_R_MaterialProp = serializedObject.FindProperty ("Sus_R_Material");
			Reinforce_RadiusProp = serializedObject.FindProperty ("Reinforce_Radius");

			Wheel_DistanceProp = serializedObject.FindProperty ("Wheel_Distance");
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

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject) {
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

            serializedObject.Update();

            GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// for Static Wheel
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheel Type", MessageType.None, true);
			Fit_ST_FlagProp.boolValue = EditorGUILayout.Toggle ("Fit for Static Tracks", Fit_ST_FlagProp.boolValue);
			if (Fit_ST_FlagProp.boolValue) {
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Update using stored values", GUILayout.Width (200))) {
					Use_StoredValues ();
				}
			}

			// Suspension settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Suspension settings", MessageType.None, true);
			EditorGUILayout.Slider (Sus_DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.IntSlider (NumProp, 0, 30, "Number");
			EditorGUILayout.Slider (SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (Sus_LengthProp, -1.0f, 1.0f, "Length");
			EditorGUILayout.Space ();
			Set_IndividuallyProp.boolValue = EditorGUILayout.Toggle ("Set Angles Individually", Set_IndividuallyProp.boolValue);
			if (Set_IndividuallyProp.boolValue) {
                EditorGUI.indentLevel++;
                Sus_AnglesProp.arraySize = NumProp.intValue;
				for (int i = 0; i < Sus_AnglesProp.arraySize; i++) {
					EditorGUILayout.Slider (Sus_AnglesProp.GetArrayElementAtIndex (i), -180.0f, 180.0f, "Angle " + i);
				}
                EditorGUI.indentLevel--;
            } else {
				EditorGUILayout.Slider (Sus_AngleProp, -180.0f, 180.0f, "Angle");
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Sus_AnchorProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (Sus_MassProp, 0.1f, 300.0f, "Mass");
			EditorGUILayout.Slider (Sus_SpringProp, 0.0f, 100000.0f, "Sus Spring Force");
			if (Sus_SpringProp.floatValue == 100000.0f) {
				Sus_SpringProp.floatValue = Mathf.Infinity;
			}
			EditorGUILayout.Slider (Sus_DamperProp, 0.0f, 10000.0f, "Sus Damper Force");
			EditorGUILayout.Slider (Sus_TargetProp, -90.0f, 90.0f, "Sus Spring Target Angle");
			EditorGUILayout.Slider (Sus_Forward_LimitProp, -90.0f, 90.0f, "Forward Limit Angle");
			EditorGUILayout.Slider (Sus_Backward_LimitProp, -90.0f, 90.0f, "Backward Limit Angle");
			EditorGUILayout.Space ();
			Sus_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", Sus_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
			Sus_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", Sus_R_MeshProp.objectReferenceValue, typeof(Mesh), false);

			EditorGUILayout.IntSlider (Sus_Materials_NumProp, 1, 10, "Number of Materials");
			Sus_MaterialsProp.arraySize = Sus_Materials_NumProp.intValue;
			if (Sus_Materials_NumProp.intValue == 1 && Sus_L_MaterialProp.objectReferenceValue != null) {
				if (Sus_MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue == null) {
					Sus_MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue = Sus_L_MaterialProp.objectReferenceValue;
				}
				Sus_L_MaterialProp.objectReferenceValue = null;
				Sus_R_MaterialProp.objectReferenceValue = null;
			}
            EditorGUI.indentLevel++;
            for (int i = 0; i < Sus_Materials_NumProp.intValue; i++) {
				Sus_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material", Sus_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
            EditorGUI.indentLevel--;

            EditorGUILayout.Space ();
			EditorGUILayout.Slider (Reinforce_RadiusProp, 0.1f, 1.0f, "SphereCollider Radius");

			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.Slider (Wheel_DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.Slider (Wheel_MassProp, 0.1f, 300.0f, "Mass");
			EditorGUILayout.Space ();
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "SphereCollider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			EditorGUILayout.Space ();
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);

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
				Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material", Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}
            EditorGUI.indentLevel--;

            // Scripts settings
            EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Scripts settings", MessageType.None, true);
			// Drive Wheel
			Drive_WheelProp.boolValue = EditorGUILayout.Toggle ("Drive Wheel", Drive_WheelProp.boolValue);
			EditorGUILayout.Space ();
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
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

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			//
			serializedObject.ApplyModifiedProperties ();
		}


		void Create ()
		{
			// Delete Objects
			int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}
			// Create Suspension
			for (int i = 0; i < NumProp.intValue; i++) {
				Create_Suspension ("L", i + 1);
			}
			for (int i = 0; i < NumProp.intValue; i++) {
				Create_Suspension ("R", i + 1);
			}
			// Create Wheel
			for (int i = 0; i < NumProp.intValue; i++) {
				Create_Wheel ("L", i + 1);
			}
			for (int i = 0; i < NumProp.intValue; i++) {
				Create_Wheel ("R", i + 1);
			}
		}


		void Create_Suspension (string direction, int number)
		{
			// Create gameobject & Set parent
			GameObject gameObject = new GameObject ("Suspension_" + direction + "_" + number);
			gameObject.transform.parent = thisTransform;
			// Set position.
			Vector3 pos;
			pos.x = 0.0f;
			pos.z = -SpacingProp.floatValue * (number - 1);
			pos.y = Sus_DistanceProp.floatValue / 2.0f;
			if (direction == "R") {
				pos.y *= -1.0f;
			}
			gameObject.transform.localPosition = pos;
			// Set rotation.
			if (Set_IndividuallyProp.boolValue) {
				gameObject.transform.localRotation = Quaternion.Euler (0.0f, Sus_AnglesProp.GetArrayElementAtIndex (number - 1).floatValue, -90.0f);
			} else {
				gameObject.transform.localRotation = Quaternion.Euler (0.0f, Sus_AngleProp.floatValue, -90.0f);
			}
			// Mesh
			if (direction == "L") { // Left
				if (Sus_L_MeshProp.objectReferenceValue) {
					MeshFilter meshFilter;
					meshFilter = gameObject.AddComponent < MeshFilter > ();
					meshFilter.mesh = Sus_L_MeshProp.objectReferenceValue as Mesh;
				}
			} else { // Right
				if (Sus_R_MeshProp.objectReferenceValue) {
					MeshFilter meshFilter;
					meshFilter = gameObject.AddComponent < MeshFilter > ();
					meshFilter.mesh = Sus_R_MeshProp.objectReferenceValue as Mesh;
				}
			}
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			Material[] materials = new Material [ Sus_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = Sus_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Sus_MassProp.floatValue;
			// HingeJoint
			HingeJoint hingeJoint = gameObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = thisTransform.parent.gameObject.GetComponent< Rigidbody > (); //MainBody's Rigidbody.
			hingeJoint.anchor = new Vector3 (0.0f, 0.0f, Sus_AnchorProp.floatValue);
			hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
			hingeJoint.useSpring = true;
			JointSpring jointSpring = hingeJoint.spring;
			jointSpring.spring = Sus_SpringProp.floatValue;
			jointSpring.damper = Sus_DamperProp.floatValue;
			if (Set_IndividuallyProp.boolValue) {
				jointSpring.targetPosition = Sus_TargetProp.floatValue + Sus_AnglesProp.GetArrayElementAtIndex (number - 1).floatValue;
			} else {
				jointSpring.targetPosition = Sus_TargetProp.floatValue + Sus_AngleProp.floatValue;
			}
			hingeJoint.spring = jointSpring;
			hingeJoint.useLimits = true;
			JointLimits jointLimits = hingeJoint.limits;
			if (Set_IndividuallyProp.boolValue) {
				jointLimits.max = Sus_Forward_LimitProp.floatValue + Sus_AnglesProp.GetArrayElementAtIndex (number - 1).floatValue;
				jointLimits.min = -(Sus_Backward_LimitProp.floatValue - Sus_AnglesProp.GetArrayElementAtIndex (number - 1).floatValue);
			} else {
				jointLimits.max = Sus_Forward_LimitProp.floatValue + Sus_AngleProp.floatValue;
				jointLimits.min = -(Sus_Backward_LimitProp.floatValue - Sus_AngleProp.floatValue);
			}
			hingeJoint.limits = jointLimits;
			// Reinforce SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Reinforce_RadiusProp.floatValue;
			// Set Layer
			gameObject.layer = PTS_Layer_Settings_CS.Reinforce_Layer; // Ignore all collision.
		}


		void Create_Wheel (string direction, int number)
		{
			// Create gameobject & Set parent.
			GameObject gameObject = new GameObject ("RoadWheel_" + direction + "_" + number);
			gameObject.transform.parent = thisTransform;
			// Set position.
			Vector3 pos;
			if (Set_IndividuallyProp.boolValue) {
				pos.x = Mathf.Sin (Mathf.Deg2Rad * (180.0f + Sus_AnglesProp.GetArrayElementAtIndex (number - 1).floatValue)) * Sus_LengthProp.floatValue;
				pos.z = Mathf.Cos (Mathf.Deg2Rad * (180.0f + Sus_AnglesProp.GetArrayElementAtIndex (number - 1).floatValue)) * Sus_LengthProp.floatValue;
			} else {
				pos.x = Mathf.Sin (Mathf.Deg2Rad * (180.0f + Sus_AngleProp.floatValue)) * Sus_LengthProp.floatValue;
				pos.z = Mathf.Cos (Mathf.Deg2Rad * (180.0f + Sus_AngleProp.floatValue)) * Sus_LengthProp.floatValue;
			}
			pos.z -= SpacingProp.floatValue * (number -1);
			pos.y = Wheel_DistanceProp.floatValue / 2.0f;
			if (direction == "R") {
				pos.y *= -1.0f;
			}
			gameObject.transform.localPosition = pos;
			// Set rotation.
			if (direction == "L") { // Left
				gameObject.transform.localRotation = Quaternion.Euler (Vector3.zero);
			} else { // Right
				gameObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 180);
			}
			// Mesh
			if (Wheel_MeshProp.objectReferenceValue) {
				MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
				meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
				MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
				Material[] materials = new Material [ Wheel_Materials_NumProp.intValue ];
				for (int i = 0; i < materials.Length; i++) {
					materials [i] = Wheel_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
				}
				meshRenderer.materials = materials;
			}
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Wheel_MassProp.floatValue;
			// HingeJoint
			HingeJoint hingeJoint = gameObject.AddComponent < HingeJoint > ();
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
			hingeJoint.connectedBody = thisTransform.Find ("Suspension_" + direction + "_" + number).gameObject.GetComponent < Rigidbody > ();
			// SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Wheel_RadiusProp.floatValue;
			sphereCollider.center = Vector3.zero;
			sphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// Drive_Wheel_CS
			PTS_Drive_Wheel_CS driveScript = gameObject.AddComponent < PTS_Drive_Wheel_CS > ();
			driveScript.Radius = Wheel_RadiusProp.floatValue;
			driveScript.Drive_Flag = Drive_WheelProp.boolValue;
			// Wheel_Resize_CS
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				if (Wheel_ResizeProp.boolValue) {
					PTS_Wheel_Resize_CS resizeScript = gameObject.AddComponent < PTS_Wheel_Resize_CS > ();
					resizeScript.ScaleDown_Size = ScaleDown_SizeProp.floatValue;
					resizeScript.Return_Speed = Return_SpeedProp.floatValue;
				}
			}
			// Stabilizer_CS
			gameObject.AddComponent < PTS_Stabilizer_CS > ();
			// Set Layer
			gameObject.layer = PTS_Layer_Settings_CS.Wheels_Layer; // Wheel
        }

		void Use_StoredValues ()
		{
			PTS_Static_Track_CS staticTrackScript = thisTransform.parent.GetComponentInChildren <PTS_Static_Track_CS> ();
			if (staticTrackScript) {
				if (staticTrackScript.RoadWheelsProp_Array.Length == 0) {
					Debug.LogWarning ("The values for Static_Track are not stored. Please set 'Angle' and 'SphereCollider Radius' manually.");
					return;
				}
				for (int i = 0; i < staticTrackScript.RoadWheelsProp_Array.Length; i++) {
					if (staticTrackScript.RoadWheelsProp_Array [i].parentName == thisTransform.name) {
						// Set Radius.
						CapsuleCollider [] capsuleColliders = staticTrackScript.GetComponentsInChildren <CapsuleCollider> ();
						for (int j = 0; j < capsuleColliders.Length; j++) {
							if (capsuleColliders [j].enabled == false) {
								Wheel_RadiusProp.floatValue = staticTrackScript.RoadWheelsProp_Array [i].baseRadius + capsuleColliders [j].radius * 2.0f;
								break;
							}
						}
						// Set Angles.
						Set_IndividuallyProp.boolValue = true;
						Sus_AnglesProp.arraySize = NumProp.intValue;
						for (int j = 0; j < NumProp.intValue; j++) {
							Sus_AnglesProp.GetArrayElementAtIndex (j).floatValue = staticTrackScript.RoadWheelsProp_Array [i].angles [j];
						}
						break;
					}
				}
			} else {
				Debug.LogWarning ("Static_Track cannot be found in this tank.");
				return;
			}
		}

	}

}