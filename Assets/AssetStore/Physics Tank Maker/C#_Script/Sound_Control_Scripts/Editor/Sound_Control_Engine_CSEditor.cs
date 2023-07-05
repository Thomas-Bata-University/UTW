using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Sound_Control_Engine_CS))]
	public class Sound_Control_Engine_CSEditor : Editor
	{

		SerializedProperty Min_Engine_PitchProp;
		SerializedProperty Max_Engine_PitchProp;
		SerializedProperty Min_Engine_VolumeProp;
		SerializedProperty Max_Engine_VolumeProp;

		SerializedProperty Left_Reference_RigidbodyProp;
		SerializedProperty Right_Reference_RigidbodyProp;
		SerializedProperty Reference_Name_LProp;
		SerializedProperty Reference_Name_RProp;
		SerializedProperty Reference_Parent_Name_LProp;
		SerializedProperty Reference_Parent_Name_RProp;

		SerializedProperty Left_VelocityProp;
		SerializedProperty Right_VelocityProp;

        SerializedProperty Has_ChangedProp;


        void OnEnable ()
		{
			Min_Engine_PitchProp = serializedObject.FindProperty ("Min_Engine_Pitch");
			Max_Engine_PitchProp = serializedObject.FindProperty ("Max_Engine_Pitch");
			Min_Engine_VolumeProp = serializedObject.FindProperty ("Min_Engine_Volume");
			Max_Engine_VolumeProp = serializedObject.FindProperty ("Max_Engine_Volume");

			Left_Reference_RigidbodyProp = serializedObject.FindProperty ("Left_Reference_Rigidbody");
			Right_Reference_RigidbodyProp = serializedObject.FindProperty ("Right_Reference_Rigidbody");
			Reference_Name_LProp = serializedObject.FindProperty ("Reference_Name_L");
			Reference_Name_RProp = serializedObject.FindProperty ("Reference_Name_R");
			Reference_Parent_Name_LProp = serializedObject.FindProperty ("Reference_Parent_Name_L");
			Reference_Parent_Name_RProp = serializedObject.FindProperty ("Reference_Parent_Name_R");

			Left_VelocityProp = serializedObject.FindProperty ("Left_Velocity");
			Right_VelocityProp = serializedObject.FindProperty ("Right_Velocity");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");
        }


        public override void  OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
		
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Engine sound settings.", MessageType.None, true);

			if (GUILayout.Button ("Set Automatically", GUILayout.Width (200))) {
				Left_Reference_RigidbodyProp.objectReferenceValue = Find_Reference_RoadWheel (true);
				Right_Reference_RigidbodyProp.objectReferenceValue = Find_Reference_RoadWheel (false);
			}

			EditorGUILayout.Space ();
			if (Left_Reference_RigidbodyProp.objectReferenceValue != null) {
				Reference_Name_LProp.stringValue = Left_Reference_RigidbodyProp.objectReferenceValue.name;
				Rigidbody leftRigidbody = Left_Reference_RigidbodyProp.objectReferenceValue as Rigidbody;
				Transform leftParentTransform = leftRigidbody.transform.parent;
				if (leftParentTransform) {
					Reference_Parent_Name_LProp.stringValue = leftParentTransform.name;
				}
			}
			else {
				string leftName = Reference_Name_LProp.stringValue;
				string leftParentParentName = Reference_Parent_Name_LProp.stringValue;
				if (string.IsNullOrEmpty (leftName) == false && string.IsNullOrEmpty (leftParentParentName) == false) {
					Transform bodyTransform = Selection.activeGameObject.transform.parent;
					Transform leftTransform = bodyTransform.Find(leftParentParentName + "/" + leftName);
					if (leftTransform) {
						Left_Reference_RigidbodyProp.objectReferenceValue = leftTransform.GetComponent <Rigidbody>() as Rigidbody;
					}
				}
			}

			if (Right_Reference_RigidbodyProp.objectReferenceValue != null) {
				Reference_Name_RProp.stringValue = Right_Reference_RigidbodyProp.objectReferenceValue.name;
				Rigidbody rightRigidbody = Right_Reference_RigidbodyProp.objectReferenceValue as Rigidbody;
				Transform rightParentTransform = rightRigidbody.transform.parent;
				if (rightParentTransform) {
					Reference_Parent_Name_RProp.stringValue = rightParentTransform.name;
				}
			}
			else {
				string rightName = Reference_Name_RProp.stringValue;
				string rightParentParentName = Reference_Parent_Name_RProp.stringValue;
				if (string.IsNullOrEmpty (rightName) == false && string.IsNullOrEmpty (rightParentParentName) == false) {
					Transform bodyTransform = Selection.activeGameObject.transform.parent;
					Transform rightTransform = bodyTransform.Find(rightParentParentName + "/" + rightName);
					if (rightTransform) {
						Right_Reference_RigidbodyProp.objectReferenceValue = rightTransform.GetComponent <Rigidbody>() as Rigidbody;
					}
				}
			}

			Left_Reference_RigidbodyProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Reference Wheel", Left_Reference_RigidbodyProp.objectReferenceValue, typeof(Rigidbody), true);
			Right_Reference_RigidbodyProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Reference Wheel", Right_Reference_RigidbodyProp.objectReferenceValue, typeof(Rigidbody), true);
			EditorGUILayout.Space ();

			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
			Reference_Parent_Name_LProp.stringValue = EditorGUILayout.TextField ("Left Reference Parent Name", Reference_Parent_Name_LProp.stringValue);
			Reference_Name_LProp.stringValue = EditorGUILayout.TextField ("Left Reference Name", Reference_Name_LProp.stringValue);
			Reference_Parent_Name_RProp.stringValue = EditorGUILayout.TextField ("Right Reference Parent Name", Reference_Parent_Name_RProp.stringValue);
			Reference_Name_RProp.stringValue = EditorGUILayout.TextField ("Right Reference Name", Reference_Name_RProp.stringValue);
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			EditorGUILayout.Space ();


			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Min_Engine_PitchProp, 0.1f, 10.0f, "Idling Pitch");
			EditorGUILayout.Slider (Max_Engine_PitchProp, 0.1f, 10.0f, "Max Pitch");
			EditorGUILayout.Slider (Min_Engine_VolumeProp, 0.0f, 1.0f, "Idling Volume");
			EditorGUILayout.Slider (Max_Engine_VolumeProp, 0.0f, 1.0f, "Max Volume");

			EditorGUILayout.Space ();
			if (EditorApplication.isPlaying) {
				float currentVelocity = (Left_VelocityProp.floatValue + Right_VelocityProp.floatValue) / 2.0f;
				EditorGUILayout.HelpBox("Current Velocity = " + currentVelocity, MessageType.None, false);
			}
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

            if (GUI.changed)
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
            }

            serializedObject.ApplyModifiedProperties ();
		}


		Rigidbody Find_Reference_RoadWheel (bool isLeft)
		{
			Transform bodyTransform = Selection.activeGameObject.transform.parent;
			Drive_Wheel_CS [] driveScripts = bodyTransform.GetComponentsInChildren <Drive_Wheel_CS> ();
			float minDist = Mathf.Infinity;
			Transform closestWheel = null;
			foreach (Drive_Wheel_CS driveScript in driveScripts) {
				Transform connectedTransform = driveScript.GetComponent <HingeJoint> ().connectedBody.transform;
				MeshFilter meshFilter = driveScript.GetComponent <MeshFilter> ();
				if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // connected to Suspension && not invisible.
					float tempDist = Vector3.Distance (bodyTransform.position, driveScript.transform.position); // Distance to the MainBody.
					if (isLeft) { // Left.
						if (driveScript.transform.localEulerAngles.z == 0.0f) { // Left.
							if (tempDist < minDist) {
								closestWheel = driveScript.transform;
								minDist = tempDist;
							}
						}
					} else { // Right.
						if (driveScript.transform.localEulerAngles.z != 0.0f) { // Right.
							if (tempDist < minDist) {
								closestWheel = driveScript.transform;
								minDist = tempDist;
							}
						}
					}
				}
			}
			if (closestWheel) {
				Rigidbody closestRigidbody = closestWheel.GetComponent <Rigidbody>();
				return closestRigidbody;
			}
			return null;
		}
		
	}

}