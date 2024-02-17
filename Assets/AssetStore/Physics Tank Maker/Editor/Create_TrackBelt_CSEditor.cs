using UnityEngine;
using System;
using UnityEditor;
using System.IO;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_TrackBelt_CS))]
	public class Create_TrackBelt_CSEditor : Editor
	{

		SerializedProperty SelectedAngleProp;
		SerializedProperty Angle_RearProp;
		SerializedProperty NumberProp;
		SerializedProperty SpacingProp;
		SerializedProperty DistanceProp;
		SerializedProperty Track_MassProp;
		SerializedProperty Angular_DragProp;
		SerializedProperty Collider_InfoProp;
		SerializedProperty Collider_MaterialProp;
		SerializedProperty Use_BoxColliderProp;

		SerializedProperty Track_R_MeshProp;
		SerializedProperty Track_L_MeshProp;
		SerializedProperty Track_Materials_NumProp;
		SerializedProperty Track_MaterialsProp;
		SerializedProperty Use_2ndPieceProp;
		SerializedProperty Track_R_2nd_MeshProp;
		SerializedProperty Track_L_2nd_MeshProp;
		SerializedProperty Track_2nd_Materials_NumProp;
		SerializedProperty Track_2nd_MaterialsProp;
		SerializedProperty Use_ShadowMeshProp;
		SerializedProperty Track_R_Shadow_MeshProp;
		SerializedProperty Track_L_Shadow_MeshProp;

		SerializedProperty SubJoint_TypeProp;
		SerializedProperty Reinforce_RadiusProp;

		SerializedProperty Use_JointProp;
		SerializedProperty Joint_OffsetProp;
		SerializedProperty Joint_R_MeshProp;
		SerializedProperty Joint_L_MeshProp;
		SerializedProperty Joint_Materials_NumProp;
		SerializedProperty Joint_MaterialsProp;
		SerializedProperty Joint_ShadowProp;

		SerializedProperty BreakForceProp;

		SerializedProperty Static_FlagProp;

        SerializedProperty Has_ChangedProp;


        string[] angleNames = { "10", "11.25", "12", "15", "18", "20", "22.5", "25.71", "30", "36", "45", "60", "90" };
		int[] angleValues = { 1000, 1125, 1200, 1500, 1800, 2000, 2250, 2571, 3000, 3600, 4500, 6000, 9000 };
		string[] subJointNames = { "All", "Every Two pieces", "None" };

		Transform thisTransform;


		void OnEnable ()
		{
			SelectedAngleProp = serializedObject.FindProperty ("SelectedAngle");
			Angle_RearProp = serializedObject.FindProperty ("Angle_Rear");
			NumberProp = serializedObject.FindProperty ("Number_Straight");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			DistanceProp = serializedObject.FindProperty ("Distance");
			Track_MassProp = serializedObject.FindProperty ("Track_Mass");
			Angular_DragProp = serializedObject.FindProperty ("Angular_Drag");
			Collider_InfoProp = serializedObject.FindProperty ("Collider_Info");
			Collider_MaterialProp = serializedObject.FindProperty ("Collider_Material");
			Use_BoxColliderProp = serializedObject.FindProperty ("Use_BoxCollider");

			Track_R_MeshProp = serializedObject.FindProperty ("Track_R_Mesh");
			Track_L_MeshProp = serializedObject.FindProperty ("Track_L_Mesh");
			Track_Materials_NumProp = serializedObject.FindProperty ("Track_Materials_Num");
			Track_MaterialsProp = serializedObject.FindProperty ("Track_Materials");
			Use_2ndPieceProp = serializedObject.FindProperty ("Use_2ndPiece");
			Track_R_2nd_MeshProp = serializedObject.FindProperty ("Track_R_2nd_Mesh");
			Track_L_2nd_MeshProp = serializedObject.FindProperty ("Track_L_2nd_Mesh");
			Track_2nd_Materials_NumProp = serializedObject.FindProperty ("Track_2nd_Materials_Num");
			Track_2nd_MaterialsProp = serializedObject.FindProperty ("Track_2nd_Materials");
			Use_ShadowMeshProp = serializedObject.FindProperty ("Use_ShadowMesh");
			Track_R_Shadow_MeshProp = serializedObject.FindProperty ("Track_R_Shadow_Mesh");
			Track_L_Shadow_MeshProp = serializedObject.FindProperty ("Track_L_Shadow_Mesh");

			SubJoint_TypeProp = serializedObject.FindProperty ("SubJoint_Type");
			Reinforce_RadiusProp = serializedObject.FindProperty ("Reinforce_Radius");

			Use_JointProp = serializedObject.FindProperty ("Use_Joint");
			Joint_OffsetProp = serializedObject.FindProperty ("Joint_Offset");
			Joint_R_MeshProp = serializedObject.FindProperty ("Joint_R_Mesh");
			Joint_L_MeshProp = serializedObject.FindProperty ("Joint_L_Mesh");
			Joint_Materials_NumProp = serializedObject.FindProperty ("Joint_Materials_Num");
			Joint_MaterialsProp = serializedObject.FindProperty ("Joint_Materials");
			Joint_ShadowProp = serializedObject.FindProperty ("Joint_Shadow");

			BreakForceProp = serializedObject.FindProperty ("BreakForce");

			Static_FlagProp = serializedObject.FindProperty ("Static_Flag");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject)
            {
				thisTransform = Selection.activeGameObject.transform;
			}
		}


		public override void OnInspectorGUI ()
		{
            bool isPrepared;
            if ((Application.isPlaying && Static_FlagProp.boolValue == false) || thisTransform.parent == null || thisTransform.parent.gameObject.GetComponent<Rigidbody>() == null)
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
                localAngles.y = 0.0f;
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
                EditorGUILayout.HelpBox("\n'The tracks can be modified only in the Prefab Mode.\nPlease go to the Prefab Mode, or Unpack the prefab.\n\nUnpack the prefab to convert the tracks into 'Static_Track'.\n", MessageType.Warning, true);
                if (GUILayout.Button("Unpack Prefab", GUILayout.Width(200)))
                {
                    PrefabUtility.UnpackPrefabInstance(thisTransform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                return;
            }

            serializedObject.Update ();

            if (Static_FlagProp.boolValue)
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            }
            else
            {
                GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
            }

            if (EditorApplication.isPlaying == false) {
				// Basic settings
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Basic settings", MessageType.None, true);
				EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
				EditorGUILayout.Slider (SpacingProp, 0.05f, 1.0f, "Spacing");
				EditorGUILayout.Slider (Track_MassProp, 0.1f, 100.0f, "Mass");
				EditorGUILayout.Slider (Angular_DragProp, 0.01f, 100.0f, "Angular Drag");

				// Shape settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Shape settings", MessageType.None, true);
				SelectedAngleProp.intValue = EditorGUILayout.IntPopup ("Front Arc Angle", SelectedAngleProp.intValue, angleNames, angleValues);
				Angle_RearProp.intValue = EditorGUILayout.IntPopup ("Rear Arc Angle", Angle_RearProp.intValue, angleNames, angleValues);
				EditorGUILayout.IntSlider (NumberProp, 0, 80, "Number of Straight");

				// Collider settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Collider settings", MessageType.None, true);
				Bounds tempBounds = new Bounds();
				tempBounds.center = EditorGUILayout.Vector3Field ("Center", Collider_InfoProp.boundsValue.center);
				tempBounds.size = EditorGUILayout.Vector3Field ("Size", Collider_InfoProp.boundsValue.size);
				Collider_InfoProp.boundsValue = tempBounds;
				Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
				Use_BoxColliderProp.boolValue = EditorGUILayout.Toggle ("Use BoxCollider", Use_BoxColliderProp.boolValue);

				// Mesh settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Mesh settings", MessageType.None, true);
				Track_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Piece Mesh", Track_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Track_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Piece Mesh", Track_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
				EditorGUILayout.IntSlider (Track_Materials_NumProp, 1, 10, "Number of Materials");
				Track_MaterialsProp.arraySize = Track_Materials_NumProp.intValue;
				EditorGUI.indentLevel++;
				for (int i = 0; i < Track_Materials_NumProp.intValue; i++) {
					Track_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Track_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
				}
				EditorGUI.indentLevel--;

				EditorGUILayout.Space ();
				Use_2ndPieceProp.boolValue = EditorGUILayout.Toggle ("Use Secondary Piece", Use_2ndPieceProp.boolValue);
				if (Use_2ndPieceProp.boolValue) {
					Track_L_2nd_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("2nd Left Piece Mesh", Track_L_2nd_MeshProp.objectReferenceValue, typeof(Mesh), false);
					Track_R_2nd_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("2nd Right Piece Mesh", Track_R_2nd_MeshProp.objectReferenceValue, typeof(Mesh), false);
					EditorGUILayout.IntSlider (Track_2nd_Materials_NumProp, 1, 10, "Number of Materials");
					Track_2nd_MaterialsProp.arraySize = Track_2nd_Materials_NumProp.intValue;
					EditorGUI.indentLevel++;
					for (int i = 0; i < Track_2nd_Materials_NumProp.intValue; i++) {
						Track_2nd_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Track_2nd_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
					}
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Space ();
				Use_ShadowMeshProp.boolValue = EditorGUILayout.Toggle ("Use Shadow Mesh", Use_ShadowMeshProp.boolValue);
				if (Use_ShadowMeshProp.boolValue) {
					Track_L_Shadow_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Shadow Mesh", Track_L_Shadow_MeshProp.objectReferenceValue, typeof(Mesh), false);
					Track_R_Shadow_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Shadow Mesh", Track_R_Shadow_MeshProp.objectReferenceValue, typeof(Mesh), false);
				}

				// Reinforce settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Reinforce settings", MessageType.None, true);
				SubJoint_TypeProp.intValue = EditorGUILayout.Popup ("Reinforce Type", SubJoint_TypeProp.intValue, subJointNames);
				if (SubJoint_TypeProp.intValue != 2) {
					EditorGUILayout.Slider (Reinforce_RadiusProp, 0.1f, 1.0f, "Radius of SphereCollider");
				}

				// Joint settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Additional Joint settings", MessageType.None, true);
				Use_JointProp.boolValue = EditorGUILayout.Toggle ("Use Additional Joint", Use_JointProp.boolValue);
				if (Use_JointProp.boolValue) {
					EditorGUILayout.Slider (Joint_OffsetProp, 0.0f, 1.0f, "Joint Offset");
					Joint_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", Joint_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
					Joint_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", Joint_R_MeshProp.objectReferenceValue, typeof(Mesh), false);

					EditorGUILayout.IntSlider (Joint_Materials_NumProp, 1, 10, "Number of Materials");
					Joint_MaterialsProp.arraySize = Joint_Materials_NumProp.intValue;
					EditorGUI.indentLevel++;
					for (int i = 0; i < Joint_Materials_NumProp.intValue; i++) {
						Joint_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material " + "(" + i + ")", Joint_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
					}
					EditorGUI.indentLevel--;
					Joint_ShadowProp.boolValue = EditorGUILayout.Toggle ("Shadow", Joint_ShadowProp.boolValue);
				}

				// Durability settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Durability settings", MessageType.None, true);
				EditorGUILayout.Slider (BreakForceProp, 10000.0f, 1000000.0f, "HingeJoint BreakForce");
				if (BreakForceProp.floatValue >= 1000000) {
					BreakForceProp.floatValue = Mathf.Infinity;
				}

                // for converting intp "Static_Track".
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("Convert into 'Static_Track'", MessageType.None, true);
                Static_FlagProp.boolValue = EditorGUILayout.Toggle("For 'Static_Track'", Static_FlagProp.boolValue);

                if (Static_FlagProp.boolValue)
                {
                    GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.HelpBox("Enter the Play Mode to convert the tracks into 'Static_Track'.", MessageType.Warning, true);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
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
			}

			else
            { // In PlayMode.
                // for converting into 'Static_Track'.
                EditorGUILayout.Space ();
				EditorGUILayout.Space ();
                EditorGUILayout.HelpBox("Convert into 'Static_Track'", MessageType.None, true);
                if (Static_FlagProp.boolValue)
                { 
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
                    EditorGUILayout.HelpBox("Press the button to create the new 'Static_Track' as a prefab.", MessageType.Warning, true);
                    if (GUILayout.Button ("Create Prefab in 'User' folder", GUILayout.Width(200)))
                    {
						Create_Prefab ();
					}
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
				}
			}

			serializedObject.ApplyModifiedProperties ();
		}

	
		void Create ()
		{
            // Delete Objects
            int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}
			// Create Track Pieces	(Preparation)
			float frontAng = SelectedAngleProp.intValue / 100.0f;
			float rearAng = Angle_RearProp.intValue / 100.0f;
			float frontRad = SpacingProp.floatValue / (2.0f * Mathf.Tan (Mathf.PI / (360.0f / frontAng)));
			float rearRad = SpacingProp.floatValue / (2.0f * Mathf.Tan (Mathf.PI / (360.0f / rearAng)));
			float height = frontRad - rearRad;
			float bottom;
			float slopeAngle;
			if (Mathf.Abs (height) > SpacingProp.floatValue * NumberProp.intValue || NumberProp.intValue == 0) {
				bottom = 0.0f;
				slopeAngle = 0.0f;
			} else {
				slopeAngle = Mathf.Asin (height / (SpacingProp.floatValue * NumberProp.intValue));
				if (slopeAngle != 0.0f) {
					bottom = height / Mathf.Tan (slopeAngle);
				} else {
					bottom = SpacingProp.floatValue * NumberProp.intValue;
				}
				slopeAngle *= Mathf.Rad2Deg;
			}
			// Create Front Arc
			int num = 0;
			Vector3 centerPos;
			centerPos.x = frontRad;
			centerPos.y = DistanceProp.floatValue / 2.0f;
			centerPos.z = 0.0f;
			Vector3 pos;
			pos.y = DistanceProp.floatValue / 2.0f;
			for (int i = 0; i <= 180 / frontAng; i++) {
				num++;
				pos.x = frontRad * Mathf.Sin (Mathf.Deg2Rad * (270.0f + (frontAng * i)));
				pos.x += centerPos.x;
				pos.z = frontRad * Mathf.Cos (Mathf.Deg2Rad * (270.0f + (frontAng * i)));
				Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), i * frontAng, num);
				Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), i * frontAng, num);
			}
			// Create Upper Straight
			if (bottom != 0.0f) {
				centerPos.x = (frontRad * 2.0f) - (height / NumberProp.intValue / 2.0f);
				centerPos.z = -((SpacingProp.floatValue / 2.0f) + (bottom / NumberProp.intValue / 2.0f));
				for (int i = 0; i < NumberProp.intValue; i++) {
					num++;
					pos.x = centerPos.x - (height / NumberProp.intValue * i);
					pos.z = centerPos.z - (bottom / NumberProp.intValue * i);
					Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), 180.0f + slopeAngle, num);
					Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), 180.0f + slopeAngle, num);
				}
			}
			// Create Rear Arc
			centerPos.x = frontRad;
			centerPos.z = -(bottom + SpacingProp.floatValue);
			for (int i = 0; i <= 180 / rearAng; i++) {
				num++;
				pos.x = rearRad * Mathf.Sin (Mathf.Deg2Rad * (90.0f + (rearAng * i)));
				pos.x += centerPos.x;
				pos.z = rearRad * Mathf.Cos (Mathf.Deg2Rad * (90.0f + (rearAng * i)));
				pos.z += centerPos.z;
				Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), 180.0f + (i * rearAng), num);
				Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), 180.0f + (i * rearAng), num);
			}
			// Create lower Straight
			if (bottom != 0.0f) {
				centerPos.x = (frontRad - rearRad) - (height / NumberProp.intValue / 2.0f);
				centerPos.z = -(bottom + (SpacingProp.floatValue / 2.0f)) + (bottom / NumberProp.intValue / 2.0f);
				for (int i = 0; i < NumberProp.intValue; i++) {
					num++;
					pos.x = centerPos.x - (height / NumberProp.intValue * i);
					pos.z = centerPos.z + (bottom / NumberProp.intValue * i);
					Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), -slopeAngle, num);
					Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), -slopeAngle, num);
				}
			}
			//Create Shadow Mesh.
			if (Use_ShadowMeshProp.boolValue) {
				for (int i = 0; i < num; i++) {
					Create_ShadowMesh ("L", i + 1);
					Create_ShadowMesh ("R", i + 1);
				}
			}
			// Create Reinforce Collider.
			if (SubJoint_TypeProp.intValue != 2) {
				for (int i = 0; i < num; i++) {
					if (SubJoint_TypeProp.intValue == 0 || (i + 1) % 2 == 0) {
						Create_Reinforce ("L", i + 1);
						Create_Reinforce ("R", i + 1);
					}
				}
			}
			// Create Additional Joint.
			if (Use_JointProp.boolValue) {
				for (int i = 0; i < num; i++) {
					Create_Joint ("L", i + 1);
					Create_Joint ("R", i + 1);
				}
			}
			// Add RigidBody and Joint.
			Finishing ("L");
			Finishing ("R");
		}


		void Create_TrackPiece (string direction, Vector3 position, float angleY, int number)
		{
			//Create gameobject & Set transform
			GameObject pieceObject = new GameObject ("TrackBelt_" + direction + "_" + number);
			pieceObject.transform.parent = thisTransform;
			pieceObject.transform.localPosition = position;
			pieceObject.transform.localRotation = Quaternion.Euler (0.0f, angleY, -90.0f);
			// Mesh
			MeshFilter meshFilter = pieceObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = pieceObject.AddComponent < MeshRenderer > ();
			if (Use_ShadowMeshProp.boolValue) {
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			}
			if (direction == "L") {
				if (Use_2ndPieceProp.boolValue && number % 2 == 0) {
					meshFilter.mesh = Track_L_2nd_MeshProp.objectReferenceValue as Mesh;
				} else {
					meshFilter.mesh = Track_L_MeshProp.objectReferenceValue as Mesh;
				}
			} else {
				if (Use_2ndPieceProp.boolValue && number % 2 == 0) {
					meshFilter.mesh = Track_R_2nd_MeshProp.objectReferenceValue as Mesh;
				} else {
					meshFilter.mesh = Track_R_MeshProp.objectReferenceValue as Mesh;
				}
			}
			if (Use_2ndPieceProp.boolValue && number % 2 == 0) {
				Material[] materials = new Material [ Track_2nd_Materials_NumProp.intValue ];
				for (int i = 0; i < materials.Length; i++) {
					materials [i] = Track_2nd_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
				}
				meshRenderer.materials = materials;
			} else {
				Material[] materials = new Material [ Track_Materials_NumProp.intValue ];
				for (int i = 0; i < materials.Length; i++) {
					materials [i] = Track_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
				}
				meshRenderer.materials = materials;
			}
			// Collider
			if (Use_BoxColliderProp.boolValue) {
				// Box Collider
				BoxCollider boxCollider = pieceObject.AddComponent <BoxCollider>();
				boxCollider.center = Collider_InfoProp.boundsValue.center;
				boxCollider.size = Collider_InfoProp.boundsValue.size;
				boxCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			} else {
				// CapsuleColliders
				CapsuleCollider horizontalCollider = pieceObject.AddComponent <CapsuleCollider>();
				horizontalCollider.center = Collider_InfoProp.boundsValue.center;
				if (direction == "R") {
					horizontalCollider.center = new Vector3(-horizontalCollider.center.x, horizontalCollider.center.y, horizontalCollider.center.z);
				}
				horizontalCollider.direction = 0; // X-Axis
				horizontalCollider.radius = Collider_InfoProp.boundsValue.extents.y;
				horizontalCollider.height = Collider_InfoProp.boundsValue.size.x;
				horizontalCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
				CapsuleCollider verticalCollider = pieceObject.AddComponent <CapsuleCollider>();
				verticalCollider.center = Collider_InfoProp.boundsValue.center;
				if (direction == "R") {
					verticalCollider.center = new Vector3(-verticalCollider.center.x, verticalCollider.center.y, verticalCollider.center.z);
				}
				verticalCollider.direction = 2; // Z-Axis
				verticalCollider.radius = Collider_InfoProp.boundsValue.extents.y;
				verticalCollider.height = Collider_InfoProp.boundsValue.size.z;
				verticalCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			}
			// Stabilizer_CS
			Stabilizer_CS stabilizerScript = pieceObject.AddComponent <Stabilizer_CS>();
			stabilizerScript.This_Transform = pieceObject.transform;
			stabilizerScript.Is_Left = (direction == "L");
			stabilizerScript.Initial_Pos_Y = pieceObject.transform.localPosition.y;
			stabilizerScript.Initial_Angles = pieceObject.transform.localEulerAngles;
			// Damage_Control_02_Physics_Track_Piece_CS
			var damageScript = pieceObject.AddComponent < Damage_Control_03_Physics_Track_Piece_CS > ();
			if (direction == "L") { // Left
				damageScript.Track_Index = 0;
			}
			else { // Right
				damageScript.Track_Index = 1;
			}
			// Static_Track_Setting_CS
			if (Static_FlagProp.boolValue) {
				Static_Track_Setting_CS settingScript = pieceObject.AddComponent < Static_Track_Setting_CS > ();
				settingScript.Use_2ndPiece = Use_2ndPieceProp.boolValue;
			}
			// Set Layer
			pieceObject.layer = 0;
		}


		void Create_ShadowMesh (string direction, int number)
		{
			//Create gameobject & Set transform
			Transform basePiece = thisTransform.Find ("TrackBelt_" + direction + "_" + number);
			GameObject shadowObject = new GameObject ("ShadowMesh_" + direction + "_" + number);
			shadowObject.transform.position = basePiece.position;
			shadowObject.transform.rotation = basePiece.rotation;
			shadowObject.transform.parent = basePiece;
			// Mesh
			MeshFilter meshFilter = shadowObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = shadowObject.AddComponent < MeshRenderer > ();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			meshRenderer.receiveShadows = false;
			if (direction == "R") {
				meshFilter.mesh = Track_R_Shadow_MeshProp.objectReferenceValue as Mesh;
			} else {
				meshFilter.mesh = Track_L_Shadow_MeshProp.objectReferenceValue as Mesh;
			}
			Material[] materials = new Material [ Track_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = Track_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
		}


		void Create_Reinforce (string direction, int number)
		{
			//Create gameobject & Set transform
			Transform basePiece = thisTransform.Find ("TrackBelt_" + direction + "_" + number);
			GameObject reinforceObject = new GameObject ("Reinforce_" + direction + "_" + number);
			reinforceObject.transform.position = basePiece.position;
			reinforceObject.transform.rotation = basePiece.rotation;
			reinforceObject.transform.parent = basePiece;
			// SphereCollider
			SphereCollider sphereCollider = reinforceObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Reinforce_RadiusProp.floatValue;
			// Set Layer
			reinforceObject.layer = Layer_Settings_CS.Reinforce_Layer;
        }


		void Create_Joint (string direction, int number)
		{
			//Create gameobject & Set transform
			Transform baseTransform = thisTransform.Find ("TrackBelt_" + direction + "_" + number);
			Transform frontTransform = thisTransform.Find ("TrackBelt_" + direction + "_" + (number + 1));
			if (frontTransform == null) {
				frontTransform = thisTransform.Find ("TrackBelt_" + direction + "_1");
			}
			GameObject jointObject = new GameObject ("Joint_" + direction + "_" + number);
			jointObject.transform.parent = baseTransform;
			Vector3 basePos = baseTransform.position + (baseTransform.forward * Joint_OffsetProp.floatValue);
			Vector3 frontPos = frontTransform.position - (frontTransform.forward * Joint_OffsetProp.floatValue);
			jointObject.transform.position = Vector3.Lerp (basePos, frontPos, 0.5f);
			jointObject.transform.rotation = Quaternion.Lerp (baseTransform.rotation, frontTransform.rotation, 0.5f);		
			// Mesh
			MeshFilter meshFilter = jointObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = jointObject.AddComponent < MeshRenderer > ();
			if (Joint_ShadowProp.boolValue == false) {
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			}
			if (direction == "R") {
				meshFilter.mesh = Joint_R_MeshProp.objectReferenceValue as Mesh;
			} else {
				meshFilter.mesh = Joint_L_MeshProp.objectReferenceValue as Mesh;
			}
			Material[] materials = new Material [ Joint_Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = Joint_MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
			// Track_Joint_CS
			Track_Joint_CS jointScript = jointObject.AddComponent < Track_Joint_CS > ();
			jointScript.Base_Transform = baseTransform;
			jointScript.Front_Transform = frontTransform;
			jointScript.Joint_Offset = Joint_OffsetProp.floatValue;
			jointScript.Is_Left = (direction == "L");
		}


		void Finishing (string direction)
		{
			// Add RigidBody.
			for (int i = 1; i <= thisTransform.childCount; i++) {
				Transform basePiece = thisTransform.Find ("TrackBelt_" + direction + "_" + i);
				if (basePiece) {
					// Add RigidBody.
					Rigidbody rigidbody = basePiece.gameObject.AddComponent < Rigidbody > ();
					rigidbody.mass = Track_MassProp.floatValue;
					rigidbody.angularDrag = Angular_DragProp.floatValue;
					// for Static_Track creating
					if (Static_FlagProp.boolValue) {
						rigidbody.drag = 10.0f;
					}
				}
			}
			// Add HingeJoint.
			for (int i = 1; i <= thisTransform.childCount; i++) {
				Transform basePiece = thisTransform.Find ("TrackBelt_" + direction + "_" + i);
				if (basePiece) {
					HingeJoint hingeJoint = basePiece.gameObject.AddComponent < HingeJoint > ();
					hingeJoint.anchor = new Vector3 (0.0f, 0.0f, SpacingProp.floatValue / 2.0f);
					hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
					hingeJoint.breakForce = BreakForceProp.floatValue;
					Transform frontPiece = thisTransform.Find ("TrackBelt_" + direction + "_" + (i + 1));
					if (frontPiece) {
						hingeJoint.connectedBody = frontPiece.GetComponent < Rigidbody > ();
					} else {
						frontPiece = thisTransform.Find ("TrackBelt_" + direction + "_1");
						if (frontPiece) {
							hingeJoint.connectedBody = frontPiece.GetComponent < Rigidbody > ();
						}
					}
				}
			}
		}


		void Create_Prefab ()
		{
			EditorApplication.isPaused = true;

			// Create the new parent object.
			GameObject newParentObject = new GameObject ("Static_Track");
			newParentObject.transform.parent = thisTransform.parent;
			newParentObject.transform.localPosition = thisTransform.localPosition;
			newParentObject.transform.localRotation = thisTransform.localRotation;
			Static_Track_Parent_CS parentScript = newParentObject.AddComponent <Static_Track_Parent_CS>();
			Set_Parent_Script (parentScript);

			// Call all the "Static_Track_Setting_CS" in the pieces.
			thisTransform.BroadcastMessage ("Set_Child_Scripts", parentScript, SendMessageOptions.DontRequireReceiver);
            thisTransform.BroadcastMessage("Set_Front_And_Rear_Scripts", parentScript, SendMessageOptions.DontRequireReceiver);
            thisTransform.BroadcastMessage ("Remove_Setting_Script", SendMessageOptions.DontRequireReceiver);

			// Change the hierarchy.
			Static_Track_Piece_CS[] piecesScripts = thisTransform.GetComponentsInChildren <Static_Track_Piece_CS>();
			for (int i = 0; i < piecesScripts.Length; i++) {
				piecesScripts[i].transform.parent = newParentObject.transform;
			}

			// Create Prefab.
			if (Directory.Exists ("Assets/Physics Tank Maker/User/") == false) {
				AssetDatabase.CreateFolder ("Assets/Physics Tank Maker", "User");
			}
            PrefabUtility.SaveAsPrefabAsset(newParentObject, "Assets/Physics Tank Maker/User/" + "Static_Track " + DateTime.Now.ToString("yyMMdd_HHmmss") + ".prefab");

            // Message.
            Debug.Log ("New 'Static_Track' has been created in 'User' folder.");

			// Return to EditMode.
			EditorApplication.isPlaying = false;
		}


		void Set_Parent_Script (Static_Track_Parent_CS parentScript)
		{
            // Set the values.
            parentScript.Length = SpacingProp.floatValue;
			parentScript.Stored_Body_Mass = thisTransform.GetComponentInParent <Rigidbody>().mass + (Track_MassProp.floatValue * thisTransform.childCount); // To increase the MainBody's mass later.

			// Store the roadwheels information.
			Create_RoadWheel_CS[] createRoadWheelScripts = thisTransform.parent.GetComponentsInChildren <Create_RoadWheel_CS>();
			RoadWheelsProp[] roadWheelsPropArray = new RoadWheelsProp [createRoadWheelScripts.Length];
			for (int i = 0; i < createRoadWheelScripts.Length; i++) {
				// Call "Create_RoadWheel_CS" to get the roadwheels information, such as the wheel radius and the arm angle.
				roadWheelsPropArray [i] = createRoadWheelScripts [i].Get_Current_Angles ();
				roadWheelsPropArray[i].baseRadius += Collider_InfoProp.boundsValue.size.y; // Add the hight of the piece.
			}
			parentScript.RoadWheelsProp_Array = roadWheelsPropArray;
		}

	}

}