using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Static_Track_Parent_CS))]
	public class Static_Track_Parent_CSEditor : Editor
	{

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

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;


        void OnEnable ()
		{
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

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject)
            {
                thisTransform = Selection.activeGameObject.transform;
            }
        }


        public override void OnInspectorGUI ()
		{
			bool isPrepared = !Application.isPlaying;

			if (isPrepared)
            {
				// Set Inspector window.
				Set_Inspector();
			}
		}


		void Set_Inspector()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Static Track settings", MessageType.None, true);
			EditorGUILayout.Space ();

			EditorGUILayout.Slider (MassProp, 1.0f, 100.0f, "Mass");
			EditorGUILayout.Slider (Radius_OffsetProp, -0.5f, 0.5f, "Radius Offset");
			EditorGUILayout.Slider (Anti_Stroboscopic_MinProp, 0.0f, 1.0f, "Anti Stroboscopic Min");
			EditorGUILayout.Slider (Anti_Stroboscopic_MaxProp, 0.0f, 1.0f, "Anti Stroboscopic Max");

			EditorGUILayout.Space ();
            // Check the Prefab Mode.
            if (PrefabUtility.IsAnyPrefabInstanceRoot(thisTransform.gameObject))
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("We need to unpack this prefab to equip the tank with Ststic Track.", MessageType.Warning, true);
                if (GUILayout.Button("Unpack Prefab", GUILayout.Width(200)))
                {
                    PrefabUtility.UnpackPrefabInstance(thisTransform.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                return;
            }

            if (GUILayout.Button ("Set Automatically", GUILayout.Width (200)))
            {
                Set_Reference_Wheels_Automatically();
				Set_Other_Components_Automatically();
				Update_Scripts_Values();
			}
			Reference_LProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Reference Wheel", Reference_LProp.objectReferenceValue, typeof(Transform), true);
			Reference_RProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Reference Wheel", Reference_RProp.objectReferenceValue, typeof(Transform), true);
			EditorGUILayout.Space ();

			if (Reference_LProp.objectReferenceValue != null) {
				Reference_Name_LProp.stringValue = Reference_LProp.objectReferenceValue.name;
				Transform tempTransform = Reference_LProp.objectReferenceValue as Transform;
				if (tempTransform.parent) {
					Reference_Parent_Name_LProp.stringValue = tempTransform.parent.name;
				}
			}
			else {
				string tempName = Reference_Name_LProp.stringValue;
				string tempParentName = Reference_Parent_Name_LProp.stringValue;
				if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
					Reference_LProp.objectReferenceValue = thisTransform.parent.Find (tempParentName + "/" + tempName);
				}
			}
			if (Reference_RProp.objectReferenceValue != null) {
				Reference_Name_RProp.stringValue = Reference_RProp.objectReferenceValue.name;
				Transform tempTransform = Reference_RProp.objectReferenceValue as Transform;
				if (tempTransform.parent) {
					Reference_Parent_Name_RProp.stringValue = tempTransform.parent.name;
				}
			}
			else {
				string tempName = Reference_Name_RProp.stringValue;
				string tempParentName = Reference_Parent_Name_RProp.stringValue;
				if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
					Reference_RProp.objectReferenceValue = thisTransform.parent.Find (tempParentName + "/" + tempName);
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
            EditorGUI.indentLevel++;
            EditorGUILayout.Slider (RoadWheel_Effective_RangeProp, 0.01f, 2.0f, "Effective Range");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space ();
		
			if (GUILayout.Button ("Set Types with SwingBalls", GUILayout.Width (200))) {
				Set_Types_With_SwingBalls (SwingBall_Effective_RangeProp.floatValue);
			}
            EditorGUI.indentLevel++;
            EditorGUILayout.Slider (SwingBall_Effective_RangeProp, 0.01f, 1.0f, "Effective Range");
            EditorGUI.indentLevel--;
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

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Replace BoxColliders with CapsuleColliders.", MessageType.None, true);
			if (GUILayout.Button ("Replace Colliders B to C", GUILayout.Width (200))) {
				Replace_Colliders_Box_With_Capsules ();
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Replace CapsuleColliders with BoxColliders.", MessageType.None, true);
			if (GUILayout.Button ("Replace Colliders C to B", GUILayout.Width (200))) {
				Replace_Colliders_Capsules_With_Box ();
			}

            GUI.backgroundColor = new Color(0.5f, 0.5f, 1.0f, 1.0f);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Update the scripts in the pieces.", MessageType.None, true);
            if (GUILayout.Button("Update the scripts", GUILayout.Width(200)))
            {
                Update_Scripts_Values();
            }

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

            if (GUI.changed)
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
            }

            serializedObject.ApplyModifiedProperties ();
		}


		void Set_Reference_Wheels_Automatically ()
		{
			Transform	bodyTransform = thisTransform.parent;
			Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren <Drive_Wheel_CS>();
			float minDistL = Mathf.Infinity;
			float minDistR = Mathf.Infinity;
			Transform closestWheelL = null;
			Transform closestWheelR = null;
			foreach (Drive_Wheel_CS driveScript in driveScripts) {
				Transform connectedTransform = driveScript.GetComponent <HingeJoint> ().connectedBody.transform;
				MeshFilter meshFilter = driveScript.GetComponent <MeshFilter> ();
				if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // The wheel is not directly connected with the MainBody. >> The wheel should be a roadwheel. && The wheel is visible.
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
			//
			Update_Scripts_Values();
		}


		void Set_Other_Components_Automatically ()
		{
			// "MainBody_Setting_CS" (Increase the "Mass", and set the "SIC" to 10.)
			Transform	bodyTransform = thisTransform.parent;
			MainBody_Setting_CS bodyScript = bodyTransform.GetComponent <MainBody_Setting_CS>();
			if (bodyScript.Body_Mass < Stored_Body_MassProp.floatValue) {
				bodyScript.Body_Mass = Stored_Body_MassProp.floatValue;
			}
			bodyScript.SIC = 10;

			// "Create_IdlerWheel_CS" (Enable the "Static_Flag")
			Create_IdlerWheel_CS[] idlerScripts = bodyTransform.GetComponentsInChildren <Create_IdlerWheel_CS>();
			for (int i = 0; i < idlerScripts.Length; i++) {
				idlerScripts [i].Static_Flag = true;
			}
			// "Create_SprocketWheel_CS" (Enable the "Static_Flag")
			Create_SprocketWheel_CS [] sprocketScripts = bodyTransform.GetComponentsInChildren <Create_SprocketWheel_CS> ();
			for (int i = 0; i < sprocketScripts.Length; i++) {
				sprocketScripts [i].Static_Flag = true;
			}
			// "Create_SupportWheel_CS" (Enable the "Static_Flag")
			Create_SupportWheel_CS [] supportScripts = bodyTransform.GetComponentsInChildren <Create_SupportWheel_CS> ();
			for (int i = 0; i < supportScripts.Length; i++) {
				supportScripts [i].Static_Flag = true;
			}
			// "Create_RoadWheel_CS" (Enable the "Fit_ST_Flag")
			Create_RoadWheel_CS [] roadWheeScripts = bodyTransform.GetComponentsInChildren <Create_RoadWheel_CS> ();
			for (int i = 0; i < roadWheeScripts.Length; i++) {
				roadWheeScripts [i].Fit_ST_Flag = true;
			}
		}


		void Set_Types_With_RoadWheels (float radius)
		{
			int count = 0;
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				if (Set_Closest_DrivingWheel (pieceScripts[i], radius)) { // The closest roadwheel has been found.
					count += 1;
				} // The closest roadwheel has not been found.
				else if (pieceScripts[i].Type == 1) { // Anchor.
					pieceScripts[i].Type = 2; // Dynamic.
					pieceScripts[i].Anchor_Transform = null;
					pieceScripts[i].Anchor_Name = null;
					pieceScripts[i].Anchor_Parent_Name = null;
				}
			}
			Debug.Log (count + " pieces could find roadwheels as their anchors.");
			Update_Scripts_Values();
		}


		bool Set_Closest_DrivingWheel (Static_Track_Piece_CS pieceScript, float radius)
		{
			Transform bodyTransform = thisTransform.parent;
			Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren <Drive_Wheel_CS>();
			float minDist = radius;
			Transform closestWheel = null;
			for (int i = 0; i < driveScripts.Length; i++) {
				Transform connectedTransform = driveScripts [i].GetComponent <HingeJoint> ().connectedBody.transform;
				if (connectedTransform != bodyTransform) { // The wheel is not directly connected with the MainBody. >> The wheel should be a roadwheel.
					float tempDist = Vector3.Distance (pieceScript.transform.position, driveScripts [i].transform.position); // Distance to the piece.
					if (tempDist < minDist) {
						closestWheel = driveScripts [i].transform;
						minDist = tempDist;
					}
				}
			}
			if (closestWheel) {
				pieceScript.Type = 1; // Anchor.
				pieceScript.Anchor_Transform = closestWheel;
				pieceScript.Anchor_Parent_Name = closestWheel.parent.name;
				pieceScript.Anchor_Name = closestWheel.name;
				Switch_Both_Sides_Types (pieceScript);
				return true;
			} else {
				return false;
			}
		}


		void Switch_Both_Sides_Types (Static_Track_Piece_CS pieceScript)
		{ // If the front and rear pieces are 'Static' type, change them to 'Dynamic' type.
			if (pieceScript.Front_Script.Type == 0) { // Static >>
				pieceScript.Front_Script.Type = 2; // Dynamic
				Copy_To_Opposite (pieceScript.Front_Script);
			}
			if (pieceScript.Rear_Script.Type == 0) { // Static >>
				pieceScript.Rear_Script.Type = 2; // Dynamic
				Copy_To_Opposite (pieceScript.Rear_Script);
			}
		}


		void Copy_To_Opposite (Static_Track_Piece_CS pieceScript)
		{
			// Find the opposite piece. 
			Transform oppositeTransform;
			if (pieceScript.Is_Left) { //Left
				oppositeTransform = thisTransform.Find (pieceScript.name.Replace ("_L", "_R"));
			}
			else { //Right
				oppositeTransform = thisTransform.Find (pieceScript.name.Replace ("_R", "_L"));
			}
			if (oppositeTransform == null) {
				return;
			}

			// Copy the values.
			Static_Track_Piece_CS oppositeScript = oppositeTransform.GetComponent <Static_Track_Piece_CS> ();
			oppositeScript.Type = pieceScript.Type;
			if (oppositeScript.Type == 1) { // Anchor
				if (pieceScript.Anchor_Transform) {
					string oppositeAnchorParentName = pieceScript.Anchor_Parent_Name;
					string oppositeAnchorName = pieceScript.Anchor_Name;
					if (pieceScript.Is_Left) { //Left
						oppositeAnchorName = oppositeAnchorName.Replace("_L", "_R");
					}
					else { //Right
						oppositeAnchorName = oppositeAnchorName.Replace ("_R", "_L");
					}
					Transform oppositeAnchorTransform = thisTransform.parent.Find (oppositeAnchorParentName + "/" + oppositeAnchorName);
					if (oppositeAnchorTransform) {
						oppositeScript.Anchor_Transform = oppositeAnchorTransform;
						oppositeScript.Anchor_Parent_Name = oppositeAnchorParentName;
						oppositeScript.Anchor_Name = oppositeAnchorName;
					} else {
						Debug.LogWarning ("The opposite anchor cannot be found. ");
					}
				}
			}
		}


		void Set_Types_With_SwingBalls (float radius)
		{ // called only in Parent Type.
			int count = 0;
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				Vector3 localAngles = pieceScripts[i].transform.localEulerAngles;
				if (localAngles.y < 0.0f) {
					localAngles.y = 360.0f + localAngles.y;
				}
				if (localAngles.y > 150.0f && localAngles.y < 210.0f) { // Flat upper piece.
					if (Set_Closest_SwingBall (pieceScripts[i], radius)) {
						count += 1;
					} else if (pieceScripts[i].Type == 1) { // Anchor
						pieceScripts[i].Type = 2; // Dynamic
						pieceScripts[i].Anchor_Transform = null;
						pieceScripts[i].Anchor_Name = null;
						pieceScripts[i].Anchor_Parent_Name = null;
					}
				}
			}
			Debug.Log (count + " pieces could find SwingBalls as their anchors.");
			Update_Scripts_Values();
		}


		bool Set_Closest_SwingBall (Static_Track_Piece_CS pieceScript, float radius)
		{
			Transform bodyTransform = thisTransform.parent;
			Create_SwingBall_CS [] ballScripts = bodyTransform.GetComponentsInChildren <Create_SwingBall_CS> ();
			float minDist = radius;
			Transform closestBall = null;
			for (int i = 0; i < ballScripts.Length; i++) {
				ConfigurableJoint[] joints = ballScripts [i].GetComponentsInChildren <ConfigurableJoint> ();
				for (int j = 0; j < joints.Length; j++) {
					float tempDist = Vector3.Distance (pieceScript.transform.position, joints [j].transform.position); // Distance to the piece.
					if (tempDist < minDist) {
						closestBall = joints [j].transform;
						minDist = tempDist;
					}
				}
			}
			if (closestBall) {
				pieceScript.Type = 1; // Anchor
				pieceScript.Anchor_Transform = closestBall;
				pieceScript.Anchor_Parent_Name = closestBall.parent.name;
				pieceScript.Anchor_Name = closestBall.name;
				Switch_Both_Sides_Types (pieceScript);
				return true;
			} else {
				return false;
			}
		}


		void Make_Upper_Pieces_Static ()
		{
			int count = 0;
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				Vector3 localAngles = pieceScripts[i].transform.localEulerAngles;
				if ( localAngles.y < 0.0f) {
					localAngles.y = 360.0f + localAngles.y;
				}
				if (localAngles.y > 90.0f && localAngles.y < 270.0f) { // Upper Piece.
					pieceScripts[i].Type = 0;
					pieceScripts[i].Anchor_Transform = null;
					pieceScripts[i].Anchor_Name = null;
					pieceScripts[i].Anchor_Parent_Name = null;
					count += 1;
				}
			}
			Debug.Log (count + " pieces have been set to 'Static' type.");
			Update_Scripts_Values();
		}


		void Make_All_Pieces_Static ()
		{ // called only in Parent Type.
			int count = 0;
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				pieceScripts[i].Type = 0;
				pieceScripts[i].Anchor_Transform = null;
				pieceScripts[i].Anchor_Name = null;
				pieceScripts[i].Anchor_Parent_Name = null;
				count += 1;
			}
			Debug.Log(count + " pieces have been set to 'Static' type");
			Update_Scripts_Values();
		}


		void Replace_Colliders_Box_With_Capsules ()
		{
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				BoxCollider boxCollider = pieceScripts [i].GetComponent <BoxCollider> ();
				if (boxCollider == null) {
					continue;
				}
				CapsuleCollider horizontalCollider = pieceScripts [i].gameObject.AddComponent <CapsuleCollider> ();
				horizontalCollider.center = boxCollider.center;
				horizontalCollider.direction = 0; // X-Axis
				horizontalCollider.radius = boxCollider.size.y * 0.5f;
				horizontalCollider.height = boxCollider.size.x;
				horizontalCollider.material = boxCollider.material;
				horizontalCollider.enabled = false;
				CapsuleCollider verticalCollider = pieceScripts [i].gameObject.AddComponent <CapsuleCollider> ();
				verticalCollider.center = boxCollider.center;
				verticalCollider.direction = 2; // Z-Axis
				verticalCollider.radius = boxCollider.size.y * 0.5f;
				verticalCollider.height = boxCollider.size.z;
				verticalCollider.material = boxCollider.material;
				verticalCollider.enabled = false;
				DestroyImmediate (boxCollider);
			}
		}


		void Replace_Colliders_Capsules_With_Box ()
		{
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				CapsuleCollider[] capsuleColliders = pieceScripts[i].GetComponents <CapsuleCollider>();
				if (capsuleColliders.Length < 2) {
					continue;
				}
				Vector3 boxSize = Vector3.one * 0.1f;
				foreach (CapsuleCollider capsuleCollider in capsuleColliders) {
					if (capsuleCollider.direction == 0) { // X-Axis
						boxSize.x = capsuleCollider.height;
					}
					else { // Z-Axis
						boxSize.z = capsuleCollider.height;
					}
					boxSize.y = capsuleCollider.radius * 2.0f;
				}
				BoxCollider boxCollider = pieceScripts [i].gameObject.AddComponent <BoxCollider>();
				boxCollider.center = capsuleColliders[0].center;
				boxCollider.size = boxSize;
				boxCollider.material = capsuleColliders[0].material;
				boxCollider.enabled = false;
				foreach (CapsuleCollider capsuleCollider in capsuleColliders) {
					DestroyImmediate(capsuleCollider);
                }
			}
        }


        void Update_ShadowMesh ()
		{
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				if (pieceScripts[i].Is_Left) { // Left
					Create_ShadowMesh (pieceScripts[i].transform, Track_L_Shadow_MeshProp.objectReferenceValue as Mesh);
				} else { // Right
					Create_ShadowMesh (pieceScripts[i].transform, Track_R_Shadow_MeshProp.objectReferenceValue as Mesh);
				}
			}
		}


		void Create_ShadowMesh (Transform baseTransform, Mesh mesh) 
		{
			// Destroy the old shadow mesh object.
			for (int i = 0; i < baseTransform.childCount; i++) {
				GameObject childObject = baseTransform.GetChild (i).gameObject;
				MeshRenderer childMeshRenderer = childObject.GetComponent <MeshRenderer> ();
				if (childMeshRenderer && childMeshRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly) {
					DestroyImmediate (childObject);
				}
			}

			//Set the ShadowCastingMode of the base object.
			MeshRenderer baseMeshRenderer = baseTransform.GetComponent <MeshRenderer>();
			if (mesh == null) { // The mesh is not assigned.
				baseMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
				return;
			}
			baseMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			//Create the new shadow mesh object.
			GameObject gameObject = new GameObject ("ShadowMesh");
			gameObject.transform.position = baseTransform.position;
			gameObject.transform.rotation = baseTransform.rotation;
			gameObject.transform.parent = baseTransform;
			MeshFilter meshFilter = gameObject.AddComponent <MeshFilter> ();
			meshFilter.mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent <MeshRenderer> ();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			meshRenderer.receiveShadows = false;
			meshRenderer.material = baseMeshRenderer.sharedMaterial;
		}


		void Update_Scripts_Values()
		{ // (Note.) These values must be set after all the scripts are prepared.
			// Set values of the "Static_Track_Piece_CS" scripts in the pieces.
			Static_Track_Piece_CS[] pieceScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				// Set the "Front_Script" and "Rear_Script".
				pieceScripts[i].Front_Script = pieceScripts[i].Front_Transform.GetComponent <Static_Track_Piece_CS> ();
				pieceScripts[i].Rear_Script = pieceScripts[i].Rear_Transform.GetComponent <Static_Track_Piece_CS> ();

				// Set the "Simple_Flag".
				switch (pieceScripts[i].Type) {
					case 1: // Anchor
						if (pieceScripts[i].Front_Script.Type == 1 && pieceScripts[i].Rear_Script.Type == 1) {
							pieceScripts[i].Simple_Flag = false;
						}
						else if (pieceScripts[i].Front_Script.Type == 1 && (pieceScripts[i].Front_Script.Anchor_Parent_Name + pieceScripts[i].Front_Script.Anchor_Name) != (pieceScripts[i].Anchor_Parent_Name + pieceScripts[i].Anchor_Name)) {
							pieceScripts[i].Simple_Flag = false;
						}
						else if (pieceScripts[i].Rear_Script.Type == 1 && (pieceScripts[i].Rear_Script.Anchor_Parent_Name + pieceScripts[i].Rear_Script.Anchor_Name) != (pieceScripts[i].Anchor_Parent_Name + pieceScripts[i].Anchor_Name)) {
							pieceScripts[i].Simple_Flag = false;
						}
						else {
							pieceScripts[i].Simple_Flag = true;
						}
						break;
					case 2: // Dynamic
						if (pieceScripts[i].Front_Script.Type == 2 && pieceScripts[i].Rear_Script.Type == 2) {
							pieceScripts[i].Simple_Flag = true;
						}
						else {
							pieceScripts[i].Simple_Flag = false;
						}
						break;
				}

				// Set "Anchor_Transform" with reference to the name.
				if (pieceScripts[i].Type == 1) { // Anchor
					if (string.IsNullOrEmpty(pieceScripts[i].Anchor_Name) == false && string.IsNullOrEmpty(pieceScripts[i].Anchor_Parent_Name) == false) {
						pieceScripts[i].Anchor_Transform = thisTransform.parent.Find(pieceScripts[i].Anchor_Parent_Name + "/" + pieceScripts[i].Anchor_Name);
					}
				}
			}

            // Done.
            Debug.Log("The scripts have been updated.");
        }

    }

}