using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{
	[ CustomEditor (typeof(Emission_Control_CS))]
	public class Emission_Control_CSEditor : Editor
	{
		
		SerializedProperty Reference_RigidbodyProp;
		SerializedProperty Reference_NameProp;
		SerializedProperty Reference_Parent_NameProp;
		SerializedProperty Max_VelocityProp;
		SerializedProperty Emission_TypeProp;
		SerializedProperty CurveProp;
		SerializedProperty Max_RateProp;
		SerializedProperty Adjsut_AlphaProp;
		SerializedProperty Standard_Light_IntensityProp;


		string[] typeNames = { "Time", "Distance" };


		void OnEnable ()
		{
			Reference_RigidbodyProp = serializedObject.FindProperty ("Reference_Rigidbody");
			Reference_NameProp = serializedObject.FindProperty ("Reference_Name");
			Reference_Parent_NameProp = serializedObject.FindProperty ("Reference_Parent_Name");
			Max_VelocityProp = serializedObject.FindProperty ("Max_Velocity");
			Emission_TypeProp = serializedObject.FindProperty ("Emission_Type");
			CurveProp = serializedObject.FindProperty ("Curve");
			Max_RateProp = serializedObject.FindProperty ("Max_Rate");
			Adjsut_AlphaProp = serializedObject.FindProperty ("Adjsut_Alpha");
			Standard_Light_IntensityProp = serializedObject.FindProperty ("Standard_Light_Intensity");
		}


		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Reference settings", MessageType.None, true);

			EditorGUILayout.Space ();
			if (GUILayout.Button("Set Automatically", GUILayout.Width(200))) {
				Transform thisTransform = Selection.activeGameObject.transform;
				Reference_RigidbodyProp.objectReferenceValue = Find_Reference_RoadWheel(thisTransform.localPosition.x < 0.0f);
				Get_Max_Velocity();
			}
			EditorGUILayout.Space ();

			Reference_RigidbodyProp.objectReferenceValue = EditorGUILayout.ObjectField ("Reference Wheel", Reference_RigidbodyProp.objectReferenceValue, typeof(Rigidbody), true);
			if (Reference_RigidbodyProp.objectReferenceValue != null) {
				Reference_NameProp.stringValue = Reference_RigidbodyProp.objectReferenceValue.name;
				Rigidbody tempRigidbody = Reference_RigidbodyProp.objectReferenceValue as Rigidbody;
				Transform tempParentTransform = tempRigidbody.transform.parent;
				if (tempParentTransform) {
					Reference_Parent_NameProp.stringValue = tempParentTransform.name;
				}
			}
			else {
				string tempName = Reference_NameProp.stringValue;
				string tempParentName = Reference_Parent_NameProp.stringValue;
				if (string.IsNullOrEmpty (tempName) == false && string.IsNullOrEmpty (tempParentName) == false) {
					Transform bodyTransform = Selection.activeGameObject.GetComponentInParent <Rigidbody>().transform;
                    var referenceTransform = bodyTransform.Find(tempParentName + "/" + tempName);
                    if (referenceTransform)
                    {
                        Reference_RigidbodyProp.objectReferenceValue = referenceTransform.GetComponent<Rigidbody>() as Rigidbody;
                    }
                }
			}

			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
			Reference_Parent_NameProp.stringValue = EditorGUILayout.TextField ("Reference Parent Name", Reference_Parent_NameProp.stringValue);
			Reference_NameProp.stringValue = EditorGUILayout.TextField ("Reference Name", Reference_NameProp.stringValue);
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			EditorGUILayout.Space ();

			EditorGUILayout.Slider (Max_VelocityProp, 0.0f, 64.0f, "Max Velocity");
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();


			EditorGUILayout.HelpBox ("Emission settings", MessageType.None, true);
			Emission_TypeProp.intValue = EditorGUILayout.Popup ("Type", Emission_TypeProp.intValue, typeNames);
			CurveProp.animationCurveValue = EditorGUILayout.CurveField ("Curve", CurveProp.animationCurveValue, Color.red, new Rect (Vector2.zero, new Vector2 (1.0f, 1.0f)));
			EditorGUILayout.Slider (Max_RateProp, 0.0f, 100.0f, "Max Rate");
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			if (EditorApplication.isPlaying == false) {
				Adjsut_AlphaProp.boolValue = EditorGUILayout.Toggle ("Adjust Alpha", Adjsut_AlphaProp.boolValue);
				if (Adjsut_AlphaProp.boolValue) {
					EditorGUILayout.Slider (Standard_Light_IntensityProp, 0.0f, 10.0f, "Standard Light Intensity");
				}
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}


		Rigidbody Find_Reference_RoadWheel (bool isLeft)
		{
			Transform bodyTransform = null;
			Rigidbody bodyRigidbody = Selection.activeGameObject.GetComponentInParent <Rigidbody>();
			if (bodyRigidbody) {
				bodyTransform = bodyRigidbody.transform;
			}
			if (bodyRigidbody == null) {
				Debug.LogWarning("The script cannot find the 'Reference_Rigidbody'.");
				return null;
			}

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


		void Get_Max_Velocity()
		{
			Transform thisTransform = Selection.activeGameObject.transform;
			Drive_Control_CS driveControlScript = thisTransform.GetComponentInParent <Drive_Control_CS>();
			if (driveControlScript) {
				Max_VelocityProp.floatValue = driveControlScript.Max_Speed;
			}
		}

	}

}