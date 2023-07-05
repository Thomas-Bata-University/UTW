using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTS
{
	
	[ CustomEditor (typeof(PTS_Track_Scroll_CS))]
	public class PTS_Track_Scroll_CSEditor : Editor
	{
		SerializedProperty Reference_WheelProp;
		SerializedProperty Reference_NameProp;
		SerializedProperty Reference_Parent_NameProp;
		SerializedProperty Scroll_RateProp;
		SerializedProperty Tex_NameProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;

        void OnEnable ()
		{
			Reference_WheelProp = serializedObject.FindProperty ("Reference_Wheel");
			Reference_NameProp = serializedObject.FindProperty ("Reference_Name");
			Reference_Parent_NameProp = serializedObject.FindProperty ("Reference_Parent_Name");
			Scroll_RateProp = serializedObject.FindProperty ("Scroll_Rate");
			Tex_NameProp = serializedObject.FindProperty ("Tex_Name");

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
                // Set Inspector window.
                Set_Inspector();
            }
        }

		void  Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox ("Track Scroll settings", MessageType.None, true);

			if (GUILayout.Button ("Find RoadWheel [ Left ]", GUILayout.Width (200))) {
				Find_RoadWheel (true);
			}
			if (GUILayout.Button ("Find RoadWheel [ Right ]", GUILayout.Width (200))) {
				Find_RoadWheel (false);
			}
			EditorGUILayout.Space ();

			Reference_WheelProp.objectReferenceValue = EditorGUILayout.ObjectField ("Reference Wheel", Reference_WheelProp.objectReferenceValue, typeof(Transform), true);
			if (Reference_WheelProp.objectReferenceValue != null) {
				Transform tempTransform = Reference_WheelProp.objectReferenceValue as Transform;
				Reference_NameProp.stringValue = tempTransform.name;
				if (tempTransform.parent) {
					Reference_Parent_NameProp.stringValue = tempTransform.parent.name;
				}
			} else {
				// Find Reference Wheel with reference to the name.
				string tempName = Reference_NameProp.stringValue;
				string tempParentName = Reference_Parent_NameProp.stringValue;
				if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
					Reference_WheelProp.objectReferenceValue = Selection.activeGameObject.transform.parent.Find (tempParentName + "/" + tempName);
				}
			}
			EditorGUILayout.Space ();
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
			Reference_NameProp.stringValue = EditorGUILayout.TextField ("Reference Name", Reference_NameProp.stringValue);
			Reference_Parent_NameProp.stringValue = EditorGUILayout.TextField ("Reference Parent Name", Reference_Parent_NameProp.stringValue);
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Slider (Scroll_RateProp, -0.01f, 0.01f, "Scroll Rate");
			Tex_NameProp.stringValue = EditorGUILayout.TextField ("Texture Name in Shader", Tex_NameProp.stringValue);

            // Update Value
            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
            }

            EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}

		void Find_RoadWheel (bool isLeft)
		{
            Transform bodyTransform = thisTransform.parent;
            PTS_Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren <PTS_Drive_Wheel_CS> ();
			float minDist = Mathf.Infinity;
			Transform closestWheel = null;
			foreach (PTS_Drive_Wheel_CS driveScript in driveScripts) {
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
			Reference_WheelProp.objectReferenceValue = closestWheel;
		}

	}
}
