using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_SwingBall_CS))]
	public class Create_SwingBall_CSEditor : Editor
	{
	
		SerializedProperty DistanceProp;
		SerializedProperty NumProp;
		SerializedProperty SpacingProp;
		SerializedProperty Set_IndividuallyProp;
		SerializedProperty Pos_ArrayProp;
		SerializedProperty MassProp;
		SerializedProperty GravityProp;
		SerializedProperty RadiusProp;
		SerializedProperty RangeProp;
		SerializedProperty SpringProp;
		SerializedProperty DamperProp;
		SerializedProperty LayerProp;
		SerializedProperty Collider_MaterialProp;

        SerializedProperty Has_ChangedProp;

        string[] layerNames = { "Reinforce", "Ignore Raycast (2)"};
	
		Transform thisTransform;


		void OnEnable ()
		{
			DistanceProp = serializedObject.FindProperty ("Distance");
			NumProp = serializedObject.FindProperty ("Num");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			Set_IndividuallyProp = serializedObject.FindProperty ("Set_Individually");
			Pos_ArrayProp = serializedObject.FindProperty ("Pos_Array");
			MassProp = serializedObject.FindProperty ("Mass");
			GravityProp = serializedObject.FindProperty ("Gravity");
			RadiusProp = serializedObject.FindProperty ("Radius");
			RangeProp = serializedObject.FindProperty ("Range");
			SpringProp = serializedObject.FindProperty ("Spring");
			DamperProp = serializedObject.FindProperty ("Damper");
			LayerProp = serializedObject.FindProperty ("Layer");
			Collider_MaterialProp = serializedObject.FindProperty ("Collider_Material");

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
                EditorGUILayout.HelpBox("\n'SwingBalls' can be modified only in the Prefab Mode.\nPlease go to the Prefab Mode, or Unpack the prefab.\n", MessageType.Warning, true);
                return;
            }

            serializedObject.Update();

            GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			// Balls settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Balls settings", MessageType.None, true);
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Align with RoadWheels", GUILayout.Width (200))) {
				Find_RoadWheels ();
            }
			if (GUILayout.Button ("Align with SupportWheels", GUILayout.Width (200))) {
				Find_SupportWheels ();
			}
			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (NumProp, 1, 30, "Number");
			EditorGUILayout.Space ();
			if (NumProp.intValue > 1) {
				Set_IndividuallyProp.boolValue = EditorGUILayout.Toggle ("Set Position Individually", Set_IndividuallyProp.boolValue);
				if (Set_IndividuallyProp.boolValue) {
					Pos_ArrayProp.arraySize = NumProp.intValue;
					for (int i = 0; i < Pos_ArrayProp.arraySize; i++) {
						Pos_ArrayProp.GetArrayElementAtIndex (i).vector3Value = EditorGUILayout.Vector3Field ("   Position ", Pos_ArrayProp.GetArrayElementAtIndex (i).vector3Value);
					}
				} else {
					EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
					EditorGUILayout.Slider (SpacingProp, 0.1f, 10.0f, "Spacing");
				}
			} else {
				EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
				EditorGUILayout.Slider (SpacingProp, 0.1f, 10.0f, "Spacing");
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (MassProp, 0.1f, 300.0f, "Mass");
			GravityProp.boolValue = EditorGUILayout.Toggle ("Use Gravity", GravityProp.boolValue);
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (RadiusProp, 0.03f, 0.1f, "Radius");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (RangeProp, 0.0f, 1.0f, "Movable Range");
			EditorGUILayout.Slider (SpringProp, 0.0f, 10000.0f, "Spring Force");
			EditorGUILayout.Slider (DamperProp, 0.0f, 10000.0f, "Damper Force");
			EditorGUILayout.Space ();
			LayerProp.intValue = EditorGUILayout.Popup ("Layer", LayerProp.intValue, layerNames);
			Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);

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

            serializedObject.ApplyModifiedProperties();
        }

	
		void Create ()
		{
            // Delete Objects
            int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}
		
			// Create Ball	
			for (int i = 0; i < NumProp.intValue; i++) {
				Vector3 pos;
				if (Set_IndividuallyProp.boolValue) {
					pos = Pos_ArrayProp.GetArrayElementAtIndex (i).vector3Value;
				} else {
					pos.x = 0.0f;
					pos.y = DistanceProp.floatValue / 2.0f;
					pos.z = -SpacingProp.floatValue * i;
				}
				Create_Ball ("L", pos, i + 1);
			}
			for (int i = 0; i < NumProp.intValue; i++) {
				Vector3 pos;
				if (Set_IndividuallyProp.boolValue) {
					pos = Pos_ArrayProp.GetArrayElementAtIndex (i).vector3Value;
					pos.y *= -1.0f;
				} else {
					pos.x = 0.0f;
					pos.y = -DistanceProp.floatValue / 2.0f;
					pos.z = -SpacingProp.floatValue * i;
				}
				Create_Ball ("R", pos, i + 1);
			}
		}


		void Create_Ball (string direction, Vector3 position, int number)
		{
			//Create gameobject & Set transform
			GameObject gameObject = new GameObject ("SwingBall_" + direction + "_" + number);
			gameObject.transform.parent = thisTransform;
			gameObject.transform.localPosition = position;
			// SphereCollider
			SphereCollider sphereCollider;
			sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = RadiusProp.floatValue;
			sphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = MassProp.floatValue;
			rigidbody.useGravity = GravityProp.boolValue;
			// ConfigurableJoint
			ConfigurableJoint configJoint = gameObject.AddComponent < ConfigurableJoint > ();
			configJoint.connectedBody = thisTransform.parent.gameObject.GetComponent<Rigidbody> ();
			configJoint.anchor = Vector3.zero;
			configJoint.axis = Vector3.zero;
			configJoint.secondaryAxis = Vector3.zero;
			configJoint.xMotion = ConfigurableJointMotion.Locked;
			configJoint.yMotion = ConfigurableJointMotion.Limited;
			configJoint.zMotion = ConfigurableJointMotion.Locked;
			configJoint.angularXMotion = ConfigurableJointMotion.Locked;
			configJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configJoint.angularZMotion = ConfigurableJointMotion.Locked;
			SoftJointLimit softJointLimit = configJoint.linearLimit; // Set Vertical Range
			softJointLimit.limit = RangeProp.floatValue;
			configJoint.linearLimit = softJointLimit;
			JointDrive jointDrive = configJoint.yDrive; // Set Vertical Spring.
			//jointDrive.mode = JointDriveMode.Position ;
			jointDrive.positionSpring = SpringProp.floatValue;
			jointDrive.positionDamper = DamperProp.floatValue;
			configJoint.yDrive = jointDrive;
            // Set Layer
            switch (LayerProp.intValue)
            {
                case 0:
                    gameObject.layer = Layer_Settings_CS.Reinforce_Layer;  // Reinforce (ignore all collisions)
                    configJoint.enableCollision = false;
                    break;
                case 1:
                    gameObject.layer = 2; // Ignore Raycast (2)
                    configJoint.enableCollision = true;
                    break;
            }
        }


        void Find_RoadWheels()
        {
            // Find RoadWheels, and store positions.
            List<Vector3> posList = new List<Vector3>();
            Drive_Wheel_CS[] driveScripts = thisTransform.parent.GetComponentsInChildren<Drive_Wheel_CS>();
            foreach (Drive_Wheel_CS driveScript in driveScripts)
            {
                Transform wheelTransform = driveScript.transform;
                if (wheelTransform.parent.GetComponent<Create_RoadWheel_CS>() && wheelTransform.GetComponent<MeshFilter>())
                { // RoadWheel && visible
                    if (wheelTransform.localPosition.y > 0.0f)
                    { // Left
                        Vector3 wheelPos = wheelTransform.position;
                        wheelPos.y += wheelTransform.GetComponent<SphereCollider>().radius + RadiusProp.floatValue;
                        posList.Add(wheelPos);
                    }
                }
            }
            if (posList.Count == 0)
            {
                Debug.LogWarning("RoadWheel cannot be found in this tank.");
                return;
            }

            Finish_Alignment(posList);
        }


        void Find_SupportWheels()
        {
            // Find SupportWheels, and store positions.
            List<Vector3> posList = new List<Vector3>();
            Static_Wheel_CS[] wheelScripts = thisTransform.parent.GetComponentsInChildren<Static_Wheel_CS>();
            foreach (Static_Wheel_CS wheelScript in wheelScripts)
            {
                Transform wheelTransform = wheelScript.transform;
                if (wheelTransform.parent.GetComponent<Create_SupportWheel_CS>())
                { // SupportWheel
                    if (wheelTransform.localPosition.y > 0.0f)
                    { // Left
                        Vector3 wheelPos = wheelTransform.position;
                        wheelPos.y += wheelTransform.GetComponent<SphereCollider>().radius + RadiusProp.floatValue;
                        posList.Add(wheelPos);
                    }
                }
            }
            if (posList.Count == 0)
            {
                Debug.LogWarning("SupportWheel cannot be found in this tank.");
                return;
            }

            Finish_Alignment(posList);
        }
        

        void Finish_Alignment(List<Vector3> posList)
        {
            // Move this position to the 1st ball.
            Vector3 currentPos;
            currentPos.x = 0.0f;
            currentPos.y = posList[0].y;
            currentPos.z = posList[0].z;
            thisTransform.position = currentPos;

            // Change the world position to the local position.
            for (int i = 0; i < posList.Count; i++)
            {
                Vector3 tempPos;
                tempPos.x = posList[i].y - thisTransform.localPosition.y;
                tempPos.y = -posList[i].x;
                tempPos.z = posList[i].z - thisTransform.localPosition.z;
                posList[i] = tempPos;
            }

            // Set Array.
            NumProp.intValue = posList.Count;
            Set_IndividuallyProp.boolValue = true;
            Pos_ArrayProp.arraySize = NumProp.intValue;
            for (int i = 0; i < Pos_ArrayProp.arraySize; i++)
            {
                Pos_ArrayProp.GetArrayElementAtIndex(i).vector3Value = posList[i];
            }

            // Set Layer.
            LayerProp.intValue = 1; // Ignore Raycast (2)
        }

    }

}