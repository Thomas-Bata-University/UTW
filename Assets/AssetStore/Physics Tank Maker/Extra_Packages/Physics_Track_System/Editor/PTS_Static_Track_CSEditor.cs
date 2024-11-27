using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTS
{

	[ CustomEditor (typeof(PTS_Static_Track_CS))]
	public class PTS_Static_Track_CSEditor : Editor
	{
	
		SerializedProperty TypeProp;
		SerializedProperty Front_TransformProp;
		SerializedProperty Rear_TransformProp;
		SerializedProperty Front_ScriptProp;
		SerializedProperty Rear_ScriptProp;
		SerializedProperty Anchor_NameProp;
		SerializedProperty Anchor_Parent_NameProp;
		SerializedProperty Anchor_TransformProp;
		SerializedProperty Reference_LProp;
		SerializedProperty Reference_RProp;
		SerializedProperty Reference_Name_LProp;
		SerializedProperty Reference_Parent_Name_LProp;
		SerializedProperty Reference_Name_RProp;
		SerializedProperty Reference_Parent_Name_RProp;
		SerializedProperty Radius_OffsetProp;
		SerializedProperty MassProp;
		SerializedProperty Track_L_Shadow_MeshProp;
		SerializedProperty Track_R_Shadow_MeshProp;
		SerializedProperty RoadWheel_Effective_RangeProp;
		SerializedProperty SwingBall_Effective_RangeProp;
		SerializedProperty Anti_Stroboscopic_MinProp;
		SerializedProperty Anti_Stroboscopic_MaxProp;

		SerializedProperty Stored_Body_MassProp;
		SerializedProperty Stored_TorqueProp;

        SerializedProperty Has_ChangedProp;

        string[] typeNames = { "Static", "Anchor", "Dynamic", "", "", "", "", "", "", "Parent" };

		void  OnEnable ()
		{
			TypeProp = serializedObject.FindProperty ("Type");
			Front_TransformProp = serializedObject.FindProperty ("Front_Transform");
			Rear_TransformProp = serializedObject.FindProperty ("Rear_Transform");
			Front_ScriptProp = serializedObject.FindProperty ("Front_Script");
			Rear_ScriptProp = serializedObject.FindProperty ("Rear_Script");
			Anchor_NameProp = serializedObject.FindProperty ("Anchor_Name");
			Anchor_Parent_NameProp = serializedObject.FindProperty ("Anchor_Parent_Name");
			Anchor_TransformProp = serializedObject.FindProperty ("Anchor_Transform");
			Reference_LProp = serializedObject.FindProperty ("Reference_L");
			Reference_RProp = serializedObject.FindProperty ("Reference_R");
			Reference_Name_LProp = serializedObject.FindProperty ("Reference_Name_L");
			Reference_Parent_Name_LProp = serializedObject.FindProperty ("Reference_Parent_Name_L");
			Reference_Name_RProp = serializedObject.FindProperty ("Reference_Name_R");
			Reference_Parent_Name_RProp = serializedObject.FindProperty ("Reference_Parent_Name_R");
			Radius_OffsetProp = serializedObject.FindProperty ("Radius_Offset");
			MassProp = serializedObject.FindProperty ("Mass");
			Track_L_Shadow_MeshProp = serializedObject.FindProperty ("Track_L_Shadow_Mesh");
			Track_R_Shadow_MeshProp = serializedObject.FindProperty ("Track_R_Shadow_Mesh");
			RoadWheel_Effective_RangeProp = serializedObject.FindProperty ("RoadWheel_Effective_Range");
			SwingBall_Effective_RangeProp = serializedObject.FindProperty ("SwingBall_Effective_Range");
			Anti_Stroboscopic_MinProp = serializedObject.FindProperty ("Anti_Stroboscopic_Min");
			Anti_Stroboscopic_MaxProp = serializedObject.FindProperty ("Anti_Stroboscopic_Max");

			Stored_Body_MassProp = serializedObject.FindProperty ("Stored_Body_Mass");
			Stored_TorqueProp = serializedObject.FindProperty ("Stored_Torque");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");
        }

        public override void OnInspectorGUI ()
		{
			bool isPrepared;
			if (Application.isPlaying) {
				isPrepared = false;
			} else {
				isPrepared = true;
			}

			if (isPrepared) {
                // Set Inspector window.
                Set_Inspector();
			}
		}

		void  Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Static Track settings", MessageType.None, true);
			EditorGUILayout.Space ();

            // Check the Prefab Mode.
            if (PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject))
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("We need to unpack this prefab to equip the tank with Ststic Track.", MessageType.Warning, true);
                if (GUILayout.Button("Unpack Prefab", GUILayout.Width(200)))
                {
                    PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                return;
            }

            TypeProp.intValue = EditorGUILayout.Popup ("Type", TypeProp.intValue, typeNames);
			EditorGUILayout.Space ();
			switch (TypeProp.intValue) {
			case 0:
				break;
			case 1:
				if (GUILayout.Button ("Set Closest DrivingWheel", GUILayout.Width (200))) {
					if (Set_Closest_DrivingWheel (Selection.activeGameObject.transform, Mathf.Infinity) == false) {
						Debug.LogWarning ("DrivingWheel cannot be found in this tank.");
					}
				}
				if (GUILayout.Button ("Set Closest SwingBall", GUILayout.Width (200))) {
					if (Set_Closest_SwingBall (Selection.activeGameObject.transform, Mathf.Infinity) == false) {
						Debug.LogWarning ("SwingBall cannot be found in this tank.");
					}
				}
				Anchor_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Anchor Transform", Anchor_TransformProp.objectReferenceValue, typeof(Transform), true);
				if (Anchor_TransformProp.objectReferenceValue != null) {
					Anchor_NameProp.stringValue = Anchor_TransformProp.objectReferenceValue.name;
					Transform tempTransform = Anchor_TransformProp.objectReferenceValue as Transform;
					if (tempTransform.parent) {
						Anchor_Parent_NameProp.stringValue = tempTransform.parent.name;
					}
				} else {
					// Find Anchor with reference to the name.
					string tempName = Anchor_NameProp.stringValue;
					string tempParentName = Anchor_Parent_NameProp.stringValue;
					if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
						Anchor_TransformProp.objectReferenceValue = Selection.activeGameObject.transform.parent.parent.Find (tempParentName + "/" + tempName);
					}
				}
				EditorGUILayout.Space ();
				GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
				Anchor_NameProp.stringValue = EditorGUILayout.TextField ("Anchor Name", Anchor_NameProp.stringValue);
				Anchor_Parent_NameProp.stringValue = EditorGUILayout.TextField ("Anchor Parent Name", Anchor_Parent_NameProp.stringValue);
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				break;
			case 2:
				break;
			case 9:
                EditorGUILayout.Slider (MassProp, 1.0f, 100.0f, "Mass");
				EditorGUILayout.Slider (Radius_OffsetProp, -0.5f, 0.5f, "Radius Offset");
				EditorGUILayout.Slider (Anti_Stroboscopic_MinProp, 0.0f, 1.0f, "Anti Stroboscopic Min");
				EditorGUILayout.Slider (Anti_Stroboscopic_MaxProp, 0.0f, 1.0f, "Anti Stroboscopic Max");
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Set Automatically", GUILayout.Width (200))) {
					Auto_Set_Reference ();
					Auto_Set_Others ();
					Set_Children_References ();
				}
				Reference_LProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Reference Wheel", Reference_LProp.objectReferenceValue, typeof(Transform), true);
				Reference_RProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Reference Wheel", Reference_RProp.objectReferenceValue, typeof(Transform), true);
				EditorGUILayout.Space ();

				if (Reference_RProp.objectReferenceValue != null) {
					Reference_Name_RProp.stringValue = Reference_RProp.objectReferenceValue.name;
					Transform tempTransform = Reference_RProp.objectReferenceValue as Transform;
					if (tempTransform.parent) {
						Reference_Parent_Name_RProp.stringValue = tempTransform.parent.name;
					}
				} else {
					// Find Reference Wheel with reference to the name.
					string tempName = Reference_Name_RProp.stringValue;
					string tempParentName = Reference_Parent_Name_RProp.stringValue;
					if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
						Reference_RProp.objectReferenceValue = Selection.activeGameObject.transform.parent.Find (tempParentName + "/" + tempName);
					}
				}
				if (Reference_LProp.objectReferenceValue != null) {
					Reference_Name_LProp.stringValue = Reference_LProp.objectReferenceValue.name;
					Transform tempTransform = Reference_LProp.objectReferenceValue as Transform;
					if (tempTransform.parent) {
						Reference_Parent_Name_LProp.stringValue = tempTransform.parent.name;
					}
				} else {
					// Find Reference Wheel with reference to the name.
					string tempName = Reference_Name_LProp.stringValue;
					string tempParentName = Reference_Parent_Name_LProp.stringValue;
					if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
						Reference_LProp.objectReferenceValue = Selection.activeGameObject.transform.parent.Find (tempParentName + "/" + tempName);
					}
				}
				EditorGUILayout.Space ();
				GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
				Reference_Parent_Name_LProp.stringValue = EditorGUILayout.TextField ("Left Parent Name", Reference_Parent_Name_LProp.stringValue);
				Reference_Name_LProp.stringValue = EditorGUILayout.TextField ("Left Wheel Name", Reference_Name_LProp.stringValue);
				Reference_Parent_Name_RProp.stringValue = EditorGUILayout.TextField ("Right Parent Name", Reference_Parent_Name_RProp.stringValue);
				Reference_Name_RProp.stringValue = EditorGUILayout.TextField ("Right Wheel Name", Reference_Name_RProp.stringValue);
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Set Types with RoadWheels", GUILayout.Width (200))) {
					Set_Types_With_RoadWheels (RoadWheel_Effective_RangeProp.floatValue);
				}
				EditorGUILayout.Slider (RoadWheel_Effective_RangeProp, 0.01f, 2.0f, "   RoadWheel Effective Range");
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Set Types with SwingBalls", GUILayout.Width (200))) {
					Set_Types_With_SwingBalls (SwingBall_Effective_RangeProp.floatValue);
				}
				EditorGUILayout.Slider (SwingBall_Effective_RangeProp, 0.01f, 1.0f, "   SwingBall Effective Range");
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Make Upper Pieces 'Static'", GUILayout.Width (200))) {
					Make_Upper_Pieces_Static ();
				}
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Make All Pieces 'Static'", GUILayout.Width (200))) {
					Make_All_Pieces_Static ();
				}
				EditorGUILayout.Space ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Shadow Mesh settings", MessageType.None, true);
				EditorGUILayout.Space ();
				Track_L_Shadow_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Shadow Mesh of Left", Track_L_Shadow_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Track_R_Shadow_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Shadow Mesh of Right", Track_R_Shadow_MeshProp.objectReferenceValue, typeof(Mesh), false);
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Update Shadow Mesh", GUILayout.Width (200))) {
					Update_ShadowMesh ();
				}

				GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Remove Colliders from the pieces", MessageType.None, true);
				if (GUILayout.Button ("Remove Colliders", GUILayout.Width (200))) {
					Remove_Colliders ();
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Set children's references for optimizing.", MessageType.None, true);
				if (GUILayout.Button ("Set Children's References", GUILayout.Width (200))) {
					Set_Children_References ();
				}
				break;
			}
			EditorGUILayout.Space ();
			if (TypeProp.intValue != 9) { // Except for Parent type.
				GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
				Front_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Front Piece", Front_TransformProp.objectReferenceValue, typeof(Transform), true);
				Rear_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Rear Piece", Rear_TransformProp.objectReferenceValue, typeof(Transform), true);
				Front_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Front Script", Front_ScriptProp.objectReferenceValue, typeof(PTS_Static_Track_CS), true);
				Rear_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Rear Script", Rear_ScriptProp.objectReferenceValue, typeof(PTS_Static_Track_CS), true);
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			}

            // Update values.
            if (GUI.changed)
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;

                if (TypeProp.intValue != 9)
                { // Child piece.
                    EditorApplication.delayCall += () =>
                    {
                        Copy_To_Opposite(Selection.activeGameObject.transform, TypeProp.intValue);
                    };
                }
            }

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			serializedObject.ApplyModifiedProperties ();
		}

		void Copy_To_Opposite (Transform	thisTransform, int type)
		{
			Transform oppositeTransform;
			bool isLeft = thisTransform.localPosition.y > 0.0f;
			if (isLeft) { //Left
				oppositeTransform = thisTransform.parent.Find (thisTransform.name.Replace ("_L", "_R"));
			} else { //Right
				oppositeTransform = thisTransform.parent.Find (thisTransform.name.Replace ("_R", "_L"));
			}
			if (oppositeTransform) {
				PTS_Static_Track_CS oppositeScript = oppositeTransform.GetComponent <PTS_Static_Track_CS> ();
				oppositeScript.Type = type; // Set Type.
				// Set Anchor.
				if (type == 1) {
					PTS_Static_Track_CS thisScript = thisTransform.GetComponent <PTS_Static_Track_CS> ();
					if (thisScript.Anchor_Transform) {
						string oppositeAnchorParentName = thisScript.Anchor_Parent_Name;
						string oppositeAnchorName = thisScript.Anchor_Name;
						if (isLeft) { //Left
							oppositeAnchorName = oppositeAnchorName.Replace ("_L", "_R");
						} else { //Right
							oppositeAnchorName = oppositeAnchorName.Replace ("_R", "_L");
						}
						Transform oppositeAnchorTransform = oppositeTransform.parent.parent.Find (oppositeAnchorParentName + "/" + oppositeAnchorName);
						if (oppositeAnchorTransform) {
							oppositeScript.Anchor_Transform = oppositeAnchorTransform;
							oppositeScript.Anchor_Parent_Name = oppositeAnchorParentName;
							oppositeScript.Anchor_Name = oppositeAnchorName;
						} else {
							Debug.LogWarning ("Opposite Anchor cannot be found. " + thisTransform.name);
						}
					}
				}
			}
		}

		void Update_ShadowMesh ()
		{
			Transform	parentTransform = Selection.activeGameObject.transform;
			for (int i = 0; i < parentTransform.childCount; i++) {
				Transform childTransform = parentTransform.GetChild (i);
				if (childTransform.GetComponent <PTS_Static_Track_CS> ()) {
					if (childTransform.localPosition.y > 0.0f) { //Left
						Create_ShadowMesh (childTransform, Track_L_Shadow_MeshProp.objectReferenceValue as Mesh);
					} else { //Right
						Create_ShadowMesh (childTransform, Track_R_Shadow_MeshProp.objectReferenceValue as Mesh);
					}
				}
			}
		}

		void Create_ShadowMesh (Transform baseTransform, Mesh mesh) 
		{
			// Destroy old Shadow Mesh object.
			for (int i = 0; i < baseTransform.childCount; i++) {
				GameObject childObject = baseTransform.GetChild (i).gameObject;
				MeshRenderer childMeshRenderer = childObject.GetComponent <MeshRenderer> ();
				if (childMeshRenderer && childMeshRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly) {
					DestroyImmediate (childObject);
				}
			}
			//Set ShadowCastingMode of the base piece.
			MeshRenderer baseMeshRenderer = baseTransform.GetComponent < MeshRenderer > ();
			if (mesh == null) {
				baseMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
				return;
			}
			baseMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			//Create gameobject & Set transform
			GameObject gameObject = new GameObject ("ShadowMesh");
			gameObject.transform.position = baseTransform.position;
			gameObject.transform.rotation = baseTransform.rotation;
			gameObject.transform.parent = baseTransform;
			// Mesh
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			meshRenderer.receiveShadows = false;
			meshRenderer.material = baseMeshRenderer.sharedMaterial;
		}

		void Auto_Set_Reference ()
		{
			Transform	bodyTransform = Selection.activeGameObject.transform.parent;
			PTS_Drive_Wheel_CS [] driveScripts = bodyTransform.GetComponentsInChildren <PTS_Drive_Wheel_CS> ();
			float minDistL = Mathf.Infinity;
			float minDistR = Mathf.Infinity;
			Transform closestWheelL = null;
			Transform closestWheelR = null;
			foreach (PTS_Drive_Wheel_CS driveScript in driveScripts) {
				Transform connectedTransform = driveScript.GetComponent <HingeJoint> ().connectedBody.transform;
				MeshFilter meshFilter = driveScript.GetComponent <MeshFilter> ();
				if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // connected to Suspension && not invisible.
					float tempDist = Vector3.Distance (bodyTransform.position, driveScript.transform.position); // Distance to the MainBody.
					if (driveScript.transform.localEulerAngles.z == 0.0f) { // Left
						if (tempDist < minDistL) {
							closestWheelL = driveScript.transform;
							minDistL = tempDist;
						}
					} else { // Right
						if (tempDist < minDistR) {
							closestWheelR = driveScript.transform;
							minDistR = tempDist;
						}
					}
				}
			}
			Reference_LProp.objectReferenceValue = closestWheelL;
			Reference_RProp.objectReferenceValue = closestWheelR;
		}

		void Auto_Set_Others ()
		{
			// MainBody (Increase the Mass)
			Transform	bodyTransform = Selection.activeGameObject.transform.parent;
			PTS_MainBody_Setting_CS bodyScript = bodyTransform.GetComponent <PTS_MainBody_Setting_CS> ();
			if (bodyScript.Body_Mass < Stored_Body_MassProp.floatValue) {
				bodyScript.Body_Mass = Stored_Body_MassProp.floatValue;
			}
			bodyScript.SIC = 10;
			// Drive_Control_CS (Increase the Torque and Turn_Brake_Drag)
			PTS_Drive_Control_CS driveControlScript = bodyTransform.GetComponent <PTS_Drive_Control_CS> ();
			if (driveControlScript.Torque < Stored_TorqueProp.floatValue) {
				driveControlScript.Torque = Stored_TorqueProp.floatValue;
			}
			driveControlScript.Fix_Useless_Rotaion = false;
			// IdlerWheel
			PTS_Create_IdlerWheel_CS [] idlerScripts = bodyTransform.GetComponentsInChildren <PTS_Create_IdlerWheel_CS> ();
			for (int i = 0; i < idlerScripts.Length; i++) {
				idlerScripts [i].Static_Flag = true;
			}
			// SprocketWheel
			PTS_Create_SprocketWheel_CS [] sprocketScripts = bodyTransform.GetComponentsInChildren <PTS_Create_SprocketWheel_CS> ();
			for (int i = 0; i < sprocketScripts.Length; i++) {
				sprocketScripts [i].Static_Flag = true;
			}
			// SupportWheel
			PTS_Create_SupportWheel_CS [] supportScripts = bodyTransform.GetComponentsInChildren <PTS_Create_SupportWheel_CS> ();
			for (int i = 0; i < supportScripts.Length; i++) {
				supportScripts [i].Static_Flag = true;
			}
			// RoadWheel
			PTS_Create_RoadWheel_CS [] roadWheeScripts = bodyTransform.GetComponentsInChildren <PTS_Create_RoadWheel_CS> ();
			for (int i = 0; i < roadWheeScripts.Length; i++) {
				roadWheeScripts [i].Fit_ST_Flag = true;
			}
		}

		bool Set_Closest_DrivingWheel (Transform pieceTransform, float radius)
		{
			Transform	bodyTransform = pieceTransform.parent.parent;
			PTS_Drive_Wheel_CS [] driveScripts = bodyTransform.GetComponentsInChildren <PTS_Drive_Wheel_CS> ();
			float minDist = radius;
			Transform closestWheel = null;
			for (int i = 0; i < driveScripts.Length; i++) {
				Transform connectedTransform = driveScripts [i].GetComponent <HingeJoint> ().connectedBody.transform;
				if (connectedTransform != bodyTransform) { // should be connected to Suspension
					float tempDist = Vector3.Distance (pieceTransform.position, driveScripts [i].transform.position); // Distance to the piece.
					if (tempDist < minDist) {
						closestWheel = driveScripts [i].transform;
						minDist = tempDist;
					}
				}
			}
			if (closestWheel) {
                EditorApplication.delayCall += () =>
                {
                    PTS_Static_Track_CS pieceScript = pieceTransform.GetComponent<PTS_Static_Track_CS>();
                    pieceScript.Type = 1;
                    pieceScript.Anchor_Transform = closestWheel;
                    pieceScript.Anchor_Parent_Name = closestWheel.parent.name;
                    pieceScript.Anchor_Name = closestWheel.name;
                    Switch_Both_Sides_Types(pieceTransform);
                };
				return true;
			} else {
				return false;
			}
		}

		bool Set_Closest_SwingBall (Transform pieceTransform, float radius)
		{
			Transform	bodyTransform = pieceTransform.parent.parent;
			PTS_Create_SwingBall_CS [] ballScripts = bodyTransform.GetComponentsInChildren <PTS_Create_SwingBall_CS> ();
			float minDist = radius;
			Transform closestBall = null;
			for (int i = 0; i < ballScripts.Length; i++) {
				ConfigurableJoint[] joints = ballScripts [i].GetComponentsInChildren <ConfigurableJoint> ();
				for (int j = 0; j < joints.Length; j++) {
					float tempDist = Vector3.Distance (pieceTransform.position, joints [j].transform.position); // Distance to the piece.
					if (tempDist < minDist) {
						closestBall = joints [j].transform;
						minDist = tempDist;
					}
				}
			}
			if (closestBall) {
                EditorApplication.delayCall += () =>
                {
                    PTS_Static_Track_CS pieceScript = pieceTransform.GetComponent<PTS_Static_Track_CS>();
                    pieceScript.Type = 1;
                    pieceScript.Anchor_Transform = closestBall;
                    pieceScript.Anchor_Parent_Name = closestBall.parent.name;
                    pieceScript.Anchor_Name = closestBall.name;
                    Switch_Both_Sides_Types(pieceTransform);
                };
				return true;
			} else {
				return false;
			}
		}

		void Switch_Both_Sides_Types (Transform pieceTransform)
		{ // If both sides of Anchor piece are Static, change to Dynamic.
			PTS_Static_Track_CS pieceScript = pieceTransform.GetComponent <PTS_Static_Track_CS> ();
			Transform frontPiece = pieceScript.Front_Transform;
			Transform rearPiece = pieceScript.Rear_Transform;
			PTS_Static_Track_CS frontScript = frontPiece.GetComponent <PTS_Static_Track_CS> ();
			PTS_Static_Track_CS rearScript = rearPiece.GetComponent <PTS_Static_Track_CS> ();
			if (frontScript.Type == 0) { // Static >>
				frontScript.Type = 2; // >> Dynamic
				Copy_To_Opposite (frontPiece, 2);
			}
			if (rearScript.Type == 0) { // Static >>
				rearScript.Type = 2; // >> Dynamic
				Copy_To_Opposite (rearPiece, 2);
			}
		}

		void Set_Types_With_RoadWheels (float radius)
		{ // called only in Parent Type.
			Transform	parentTransform = Selection.activeGameObject.transform;
			int count = 0;
			for (int i = 0; i < parentTransform.childCount; i++) {
				Transform childTransform = parentTransform.GetChild (i);
				PTS_Static_Track_CS childScript = childTransform.GetComponent <PTS_Static_Track_CS> ();
				if (childScript) {
					if (Set_Closest_DrivingWheel (childTransform, radius)) {
						count += 1;
					} else if (childScript.Type == 1) {
						childScript.Type = 2;
						childScript.Anchor_Transform = null;
						childScript.Anchor_Name = null;
						childScript.Anchor_Parent_Name = null;
					}
				}
			}
			Debug.Log (count + " pieces could find RoadWheels as their Anchors.");
		}

		void Set_Types_With_SwingBalls (float radius)
		{ // called only in Parent Type.
			Transform	parentTransform = Selection.activeGameObject.transform;
			int count = 0;
			for (int i = 0; i < parentTransform.childCount; i++) {
				Transform childTransform = parentTransform.GetChild (i);
				PTS_Static_Track_CS childScript = childTransform.GetComponent <PTS_Static_Track_CS> ();
				if (childScript) {
					Vector3 childRotaion = childTransform.localEulerAngles;
					if ( childRotaion.y < 0.0f) {
						childRotaion.y = 360.0f + childRotaion.y;
					}
					if (childRotaion.y > 150.0f && childRotaion.y < 210.0f) { // Flat Upper Piece.
						if (Set_Closest_SwingBall (childTransform, radius)) {
							count += 1;
						} else if (childScript.Type == 1) {
							childScript.Type = 2;
							childScript.Anchor_Transform = null;
							childScript.Anchor_Name = null;
							childScript.Anchor_Parent_Name = null;
						}
					}
				}
			}
			Debug.Log (count + " pieces could find SwingBalls as their Anchors.");
		}
			
		void Make_Upper_Pieces_Static ()
		{ // called only in Parent Type.
			Transform	parentTransform = Selection.activeGameObject.transform;
			int count = 0;
			for (int i = 0; i < parentTransform.childCount; i++) {
				Transform childTransform = parentTransform.GetChild (i);
				PTS_Static_Track_CS childScript = childTransform.GetComponent <PTS_Static_Track_CS> ();
				if (childScript) {
					Vector3 childRotaion = childTransform.localEulerAngles;
					if ( childRotaion.y < 0.0f) {
						childRotaion.y = 360.0f + childRotaion.y;
					}
					if (childRotaion.y > 90.0f && childRotaion.y < 270.0f) { // Upper Piece.
						childScript.Type = 0;
						childScript.Anchor_Transform = null;
						childScript.Anchor_Name = null;
						childScript.Anchor_Parent_Name = null;
						count += 1;
					}
				}
			}
			Debug.Log (count + " pieces have been set to Static.");
		}

		void Make_All_Pieces_Static ()
		{ // called only in Parent Type.
			Transform	parentTransform = Selection.activeGameObject.transform;
			int count = 0;
			for (int i = 0; i < parentTransform.childCount; i++) {
				Transform childTransform = parentTransform.GetChild (i);
				PTS_Static_Track_CS childScript = childTransform.GetComponent <PTS_Static_Track_CS> ();
				if (childScript) {
					childScript.Type = 0;
					childScript.Anchor_Transform = null;
					childScript.Anchor_Name = null;
					childScript.Anchor_Parent_Name = null;
					count += 1;
				}
			}
			Debug.Log (count + " pieces have been set to Static.");
		}

		void Remove_Colliders ()
		{
			Transform	parentTransform = Selection.activeGameObject.transform;
			PTS_Static_Track_CS[] childScripts = parentTransform.GetComponentsInChildren <PTS_Static_Track_CS> ();
			for (int i = 0; i < childScripts.Length; i++) {
				CapsuleCollider[] tempCapsuleColliders = childScripts [i].GetComponents <CapsuleCollider> ();
				for (int j = 0; j < tempCapsuleColliders.Length; j++) {
					DestroyImmediate (tempCapsuleColliders [j]);
				}
				BoxCollider tempBoxCollider = childScripts [i].GetComponent <BoxCollider> ();
				if (tempBoxCollider) {
					DestroyImmediate (tempBoxCollider);
				}
			}
		}

		void Set_Children_References ()
		{
			Transform	parentTransform = Selection.activeGameObject.transform;
			PTS_Static_Track_CS[] childScripts = parentTransform.GetComponentsInChildren <PTS_Static_Track_CS> ();
			for (int i = 0; i < childScripts.Length; i++) {
				PTS_Static_Track_CS childScript = childScripts [i];
				if (childScript.Type != 9) { // Not Parent.
					childScript.Front_Script = childScript.Front_Transform.GetComponent <PTS_Static_Track_CS> ();
					childScript.Rear_Script = childScript.Rear_Transform.GetComponent <PTS_Static_Track_CS> ();
					//
					if (childScript.Anchor_Transform == null) { // Anchor_Transform is lost.
						// Find Anchor with reference to the name.
						if (string.IsNullOrEmpty (childScript.Anchor_Name) == false && string.IsNullOrEmpty (childScript.Anchor_Parent_Name) == false) {
							childScript.Anchor_Transform = parentTransform.parent.Find (childScript.Anchor_Parent_Name + "/" + childScript.Anchor_Name);
						}
					}
				}
			}
		}

	}

}