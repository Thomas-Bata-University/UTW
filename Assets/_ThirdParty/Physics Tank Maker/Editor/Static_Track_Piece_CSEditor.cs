using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Static_Track_Piece_CS))]
	public class Static_Track_Piece_CSEditor : Editor
	{

		SerializedProperty TypeProp;
		SerializedProperty Front_TransformProp;
		SerializedProperty Rear_TransformProp;
		SerializedProperty Front_ScriptProp;
		SerializedProperty Rear_ScriptProp;
		SerializedProperty Anchor_NameProp;
		SerializedProperty Anchor_Parent_NameProp;
		SerializedProperty Anchor_TransformProp;
		SerializedProperty Simple_FlagProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;

        string[] typeNames = { "Static", "Anchor", "Dynamic"};


		void OnEnable ()
		{
			TypeProp = serializedObject.FindProperty ("Type");
			Front_TransformProp = serializedObject.FindProperty ("Front_Transform");
			Rear_TransformProp = serializedObject.FindProperty ("Rear_Transform");
			Front_ScriptProp = serializedObject.FindProperty ("Front_Script");
			Rear_ScriptProp = serializedObject.FindProperty ("Rear_Script");
			Anchor_NameProp = serializedObject.FindProperty ("Anchor_Name");
			Anchor_Parent_NameProp = serializedObject.FindProperty ("Anchor_Parent_Name");
			Anchor_TransformProp = serializedObject.FindProperty ("Anchor_Transform");
			Simple_FlagProp = serializedObject.FindProperty ("Simple_Flag");

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


		void Set_Inspector ()
		{
            serializedObject.Update();

            GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Static Track settings", MessageType.None, true);
			EditorGUILayout.Space ();

			TypeProp.intValue = EditorGUILayout.Popup ("Type", TypeProp.intValue, typeNames);
			EditorGUILayout.Space ();
			switch (TypeProp.intValue) {
				case 0: // Static
					break;
				case 1: // Anchor
					if (GUILayout.Button("Set Closest DrivingWheel", GUILayout.Width(200))) {
						if (Set_Closest_DrivingWheel(thisTransform.GetComponent <Static_Track_Piece_CS>(), Mathf.Infinity) == false) {
							Debug.LogWarning("DrivingWheel cannot be found in this tank.");
						}
					}
					if (GUILayout.Button("Set Closest SwingBall", GUILayout.Width(200))) {
						if (Set_Closest_SwingBall(thisTransform.GetComponent <Static_Track_Piece_CS>(), Mathf.Infinity) == false) {
							Debug.LogWarning("SwingBall cannot be found in this tank.");
						}
					}
					Anchor_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField("Anchor Transform", Anchor_TransformProp.objectReferenceValue, typeof(Transform), true);
					if (Anchor_TransformProp.objectReferenceValue != null) {
						Anchor_NameProp.stringValue = Anchor_TransformProp.objectReferenceValue.name;
						Transform tempTransform = Anchor_TransformProp.objectReferenceValue as Transform;
						if (tempTransform.parent) {
							Anchor_Parent_NameProp.stringValue = tempTransform.parent.name;
						}
					}
					else {
						// Find Anchor with reference to the name.
						string tempName = Anchor_NameProp.stringValue;
						string tempParentName = Anchor_Parent_NameProp.stringValue;
						if (string.IsNullOrEmpty(tempName) == false && string.IsNullOrEmpty(tempParentName) == false) {
							Anchor_TransformProp.objectReferenceValue = thisTransform.parent.parent.Find(tempParentName + "/" + tempName);
						}
					}
					EditorGUILayout.Space();
					GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
					Anchor_NameProp.stringValue = EditorGUILayout.TextField("Anchor Name", Anchor_NameProp.stringValue);
					Anchor_Parent_NameProp.stringValue = EditorGUILayout.TextField("Anchor Parent Name", Anchor_Parent_NameProp.stringValue);
					GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
					break;
				case 2: // Dynamic
					break;
			}

			EditorGUILayout.Space ();
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
			Front_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Front Piece", Front_TransformProp.objectReferenceValue, typeof(Transform), true);
			Rear_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Rear Piece", Rear_TransformProp.objectReferenceValue, typeof(Transform), true);
			Front_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Front Script", Front_ScriptProp.objectReferenceValue, typeof(Static_Track_Piece_CS), true);
			Rear_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Rear Script", Rear_ScriptProp.objectReferenceValue, typeof(Static_Track_Piece_CS), true);
			EditorGUILayout.HelpBox("Simple Flag = " + Simple_FlagProp.boolValue, MessageType.None, false);
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

            // Update values.
            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
                EditorApplication.delayCall += () =>
                {
                    Copy_To_Opposite(thisTransform.GetComponent<Static_Track_Piece_CS>());
                    Update_Scripts_Values();
                };
            }

            serializedObject.ApplyModifiedProperties ();
		}


		bool Set_Closest_DrivingWheel (Static_Track_Piece_CS pieceScript, float radius)
		{
			Transform bodyTransform = thisTransform.parent.parent;
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
                EditorApplication.delayCall += () =>
                {
                    pieceScript.Type = 1; // Anchor.
                    pieceScript.Anchor_Transform = closestWheel;
                    pieceScript.Anchor_Parent_Name = closestWheel.parent.name;
                    pieceScript.Anchor_Name = closestWheel.name;
                    Switch_Both_Sides_Types(pieceScript);
                    Update_Scripts_Values();
                };
				return true;
			} else {
				return false;
			}
		}


		void Switch_Both_Sides_Types (Static_Track_Piece_CS pieceScript)
		{ // If the front and rear pieces are 'Static' type, change them to 'Dynamic' type.
			serializedObject.ApplyModifiedProperties ();
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
			serializedObject.ApplyModifiedProperties ();
			Transform parentTransform = thisTransform.parent;

			// Find the opposite piece. 
			Transform oppositeTransform;
			if (pieceScript.Is_Left) { //Left
				oppositeTransform = parentTransform.Find (pieceScript.name.Replace ("_L", "_R"));
			}
			else { //Right
				oppositeTransform = parentTransform.Find (pieceScript.name.Replace ("_R", "_L"));
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
					Transform oppositeAnchorTransform = parentTransform.parent.Find (oppositeAnchorParentName + "/" + oppositeAnchorName);
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


		bool Set_Closest_SwingBall (Static_Track_Piece_CS pieceScript, float radius)
		{
			Transform bodyTransform = thisTransform.parent.parent;
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
                EditorApplication.delayCall += () =>
                {
                    pieceScript.Type = 1; // Anchor
                    pieceScript.Anchor_Transform = closestBall;
                    pieceScript.Anchor_Parent_Name = closestBall.parent.name;
                    pieceScript.Anchor_Name = closestBall.name;
                    Switch_Both_Sides_Types(pieceScript);
                    Update_Scripts_Values();
                };
				return true;
			} else {
				return false;
			}
		}


		void Update_Scripts_Values()
		{
			serializedObject.ApplyModifiedProperties ();

			Transform parentTransform = thisTransform.parent;

			// Set values of the pieces scripts.
			Static_Track_Piece_CS[] pieceScripts = parentTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < pieceScripts.Length; i++) {
				// Set direction.
				pieceScripts[i].Is_Left = (pieceScripts[i].transform.localPosition.y > 0.0f);

				// Set invertValue.
				float localAngleY = pieceScripts[i].transform.localEulerAngles.y;
				if (localAngleY > 90.0f && localAngleY < 270.0f) { // Upper piece.
					pieceScripts[i].Invert_Angle = 180.0f;
				} else { // Lower piece
					pieceScripts[i].Invert_Angle = 0.0f;
				}

				// Set the reference scripts.
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
						pieceScripts[i].Anchor_Transform = parentTransform.parent.Find(pieceScripts[i].Anchor_Parent_Name + "/" + pieceScripts[i].Anchor_Name);
					}
				}

				// Set the "Half_Length".
				pieceScripts[i].Half_Length = pieceScripts[i].Parent_Script.Length * 0.5f;

				// Set the "Pieces_Count".
				pieceScripts[i].Pieces_Count = pieceScripts.Length / 2;
			}
		}

	}

}