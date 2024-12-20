using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Static_Track_CS : MonoBehaviour
	{

		public int Type; // 0=Static, 1=Anchor, 2=Dynamic, 9=Parent.
		public Transform Front_Transform;
		public Transform Rear_Transform;
		public PTS_Static_Track_CS Front_Script;
		public PTS_Static_Track_CS Rear_Script;
		public string Anchor_Name;
		public string Anchor_Parent_Name;
		public Transform Anchor_Transform;
		public Transform Reference_L; // Referred to from "PTS_Static_Wheel_CS".
		public Transform Reference_R; // Referred to from "PTS_Static_Wheel_CS".
		public string Reference_Name_L;
		public string Reference_Name_R;
		public string Reference_Parent_Name_L;
		public string Reference_Parent_Name_R;
		public float Length;
		public float Radius_Offset;
		public float Mass = 30.0f;
		public Mesh Track_L_Shadow_Mesh;
		public Mesh Track_R_Shadow_Mesh;
		public float RoadWheel_Effective_Range = 0.4f;
		public float SwingBall_Effective_Range = 0.15f;
		public float Anti_Stroboscopic_Min = 0.125f;
		public float Anti_Stroboscopic_Max = 0.375f;

        // For editor script.
        public bool Has_Changed;

        // Set by "PTS_Create_TrackBelt_CSEditor".
        public RoadWheelsProp [] RoadWheelsProp_Array;
		public float Stored_Body_Mass;
		public float Stored_Torque;

		// Referred to from "PTS_Static_Wheel_CS".
		public float Reference_Radius_L;
		public float Reference_Radius_R;
		public float Delta_Ang_L;
		public float Delta_Ang_R;

		Transform thisTransform;
		bool isLeft; // Left = true.
		float invertValue; // Lower piece = 0.0f, Upper pieces = 180.0f.
		public bool simpleFlag = false;
		public float Rate_L;
		public float Rate_R;
		Vector3 invisiblePos;
		float invisibleAngY;
		PTS_Static_Track_CS parentScript;
		float halfLength;
		PTS_MainBody_Setting_CS bodyScript;
		// only for Anchor.
		float initialPosX;
		float anchorInitialPosX;
		// only for Parent.
		float leftPreviousAng;
		float rightPreviousAng;
		float leftAngRate;
		float rightAngRate;

		void Awake ()
		{
			thisTransform = transform;
			// Start initial settings.
			switch (Type) {
			case 0: // Static.
				Initial_Settings ();
				break;
			case 1: // Anchor.
				Find_Anchor ();
				Initial_Settings ();
				break;
			case 2: // Dynamic.
				Initial_Settings ();
				break;
			case 9: // Parent.
				Parent_Settings ();
				break;
			}
		}

		void Parent_Settings ()
		{
			Transform bodyTransform = thisTransform.parent;
			// Find Reference Wheels.
			if (Reference_L == null) { // Left Reference Wheel is lost.
				if (string.IsNullOrEmpty (Reference_Name_L) == false && string.IsNullOrEmpty (Reference_Parent_Name_L) == false) {
					Reference_L = bodyTransform.Find (Reference_Parent_Name_L + "/" + Reference_Name_L);
				}
			}
			if (Reference_R == null) { // Right Reference Wheel is lost.
				if (string.IsNullOrEmpty (Reference_Name_R) == false && string.IsNullOrEmpty (Reference_Parent_Name_R) == false) {
					Reference_R = bodyTransform.Find (Reference_Parent_Name_R + "/" + Reference_Name_R);
				}
			}
			// Set "Reference_Radius" and "Rate_Ang".
			if (Reference_L && Reference_R) {
				Reference_Radius_L = Reference_L.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
				leftAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_L) / Length);
				Reference_Radius_R = Reference_R.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
				rightAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_R) / Length);
			} else {
				Debug.LogError ("'Reference Wheels' for Static_Track cannot be found. " + thisTransform.root.name);
				this.enabled = false;
			}
			// Boradcast this reference.
			bodyTransform.BroadcastMessage ("Prepare_With_Static_Track", this, SendMessageOptions.DontRequireReceiver);
		}

		void Initial_Settings ()
		{
			// Set initial position and angle.
			invisiblePos = thisTransform.localPosition;
			invisibleAngY = thisTransform.localEulerAngles.y ;
			// Set direction.
			isLeft = (invisiblePos.y > 0.0f);
			// Set invertValue.
			if (invisibleAngY > 90.0f && invisibleAngY < 270.0f) {  // Upper piece
				invertValue = 180.0f;
			} else { // Lower piece
				invertValue = 0.0f;
			}
			// Set the reference scripts.
			if (Front_Transform && Front_Script == null) {
				Front_Script = Front_Transform.GetComponent < PTS_Static_Track_CS > ();
			}
			if (Rear_Transform && Rear_Script == null) {
				Rear_Script = Rear_Transform.GetComponent < PTS_Static_Track_CS > ();
			}
			// Set simpleFlag.
			switch (Type) {
			case 1: // Anchor
				if (Front_Script.Type == 1 && Rear_Script.Type == 1) {
					simpleFlag = false;
				} else if (Front_Script.Type == 1 && (Front_Script.Anchor_Parent_Name + Front_Script.Anchor_Name) != (Anchor_Parent_Name + Anchor_Name)) {
					simpleFlag = false;
				} else if (Rear_Script.Type == 1 && (Rear_Script.Anchor_Parent_Name + Rear_Script.Anchor_Name) != (Anchor_Parent_Name + Anchor_Name)) {
					simpleFlag = false;
				} else {
					simpleFlag = true;
				}
				break;
			case 2: // Dynamic
				if (Front_Script.Type == 2 && Rear_Script.Type == 2) {
					simpleFlag = true;
				}
				break;
			}
		}
			
		void Find_Anchor ()
		{
			if (Anchor_Transform == null) { // Anchor_Transform is lost.
				// Find Anchor with reference to the name.
				if (string.IsNullOrEmpty (Anchor_Name) == false && string.IsNullOrEmpty (Anchor_Parent_Name) == false) {
					Anchor_Transform = thisTransform.parent.parent.Find (Anchor_Parent_Name + "/" + Anchor_Name);
				}
			}
			// Set initial hight.
			if (Anchor_Transform) {
				initialPosX = thisTransform.localPosition.x; // Axis X = hight.
				anchorInitialPosX = Anchor_Transform.localPosition.x; // Axis X = hight.
			} else {
				Type = 2; // change the Type to 'Dynamic'.
			}
		}

		void Update ()
		{
			if (Type == 9 && bodyScript.Visible_Flag) { // Parent && MainBody is visible by any camera.
				Speed_Control ();
			}
		}

		void LateUpdate ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				switch (Type) {
				case 0: // Static.
					Slide_Control ();
					break;
				case 1: // Anchor.
					Anchor_Control ();
					Slide_Control ();
					break;
				case 2: // Dynamic.
					Dynamic_Control ();
					Slide_Control ();
					break;
				}
			}
		}

		void Anchor_Control ()
		{
			// Set position.
			invisiblePos.x = initialPosX + (Anchor_Transform.localPosition.x - anchorInitialPosX);  // Axis X = hight.
			// Set angle.
			if (simpleFlag == false) {
				// Calculate end positions.
				float tempRad = Rear_Script.invisibleAngY * Mathf.Deg2Rad;
				Vector3 rearEndPos = Rear_Script.invisiblePos + new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				tempRad = Front_Script.invisibleAngY * Mathf.Deg2Rad;
				Vector3 frontEndPos = Front_Script.invisiblePos - new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				// Set angle.
				invisibleAngY = Mathf.Rad2Deg * Mathf.Atan ((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + invertValue;
			}
		}

		void Dynamic_Control ()
		{
			if (simpleFlag) {
				// Set position.
				invisiblePos = Vector3.Lerp (Rear_Script.invisiblePos, Front_Script.invisiblePos, 0.5f);
				// Set angle.
				invisibleAngY = Mathf.LerpAngle (Rear_Script.invisibleAngY, Front_Script.invisibleAngY, 0.5f);
			} else {
				// Calculate end positions.
				float tempRad = Rear_Script.invisibleAngY * Mathf.Deg2Rad;
				Vector3 rearEndPos = Rear_Script.invisiblePos + new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				tempRad = Front_Script.invisibleAngY * Mathf.Deg2Rad;
				Vector3 frontEndPos = Front_Script.invisiblePos - new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				// Set position.
				invisiblePos = Vector3.Lerp (rearEndPos, frontEndPos, 0.5f);
				// Set angle.
				invisibleAngY = Mathf.Rad2Deg * Mathf.Atan ((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + invertValue;
			}
		}

		void Slide_Control ()
		{
			if (isLeft) { // Left
				thisTransform.localPosition = Vector3.Lerp (invisiblePos, Rear_Script.invisiblePos, parentScript.Rate_L);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, Mathf.LerpAngle (invisibleAngY, Rear_Script.invisibleAngY, parentScript.Rate_L), 270.0f));
			} else { // Right
				thisTransform.localPosition = Vector3.Lerp (invisiblePos, Rear_Script.invisiblePos, parentScript.Rate_R);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, Mathf.LerpAngle (invisibleAngY, Rear_Script.invisibleAngY, parentScript.Rate_R), 270.0f));
			}
		}

		public bool Switch_Mesh_L;
		public bool Switch_Mesh_R;
		void Speed_Control ()
		{
			// Left
			float currentAng = Reference_L.localEulerAngles.y;
			Delta_Ang_L = Mathf.DeltaAngle (currentAng, leftPreviousAng); // Also referred to from Static_Wheels.
			float tempClampRate = Random.Range (Anti_Stroboscopic_Min, Anti_Stroboscopic_Max);
			Delta_Ang_L = Mathf.Clamp (Delta_Ang_L, -leftAngRate * tempClampRate, leftAngRate * tempClampRate); // Anti Stroboscopic Effect.
			Rate_L += Delta_Ang_L / leftAngRate;
			if (Rate_L > 1.0f) {
				Rate_L %= 1.0f;
				Switch_Mesh_L = !Switch_Mesh_L;
			} else if (Rate_L < 0.0f) {
				Rate_L = 1.0f + (Rate_L % 1.0f);
				Switch_Mesh_L = !Switch_Mesh_L;
			}
			leftPreviousAng = currentAng;
			// Right
			currentAng = Reference_R.localEulerAngles.y;
			Delta_Ang_R = Mathf.DeltaAngle (currentAng, rightPreviousAng); // Also referred to from Static_Wheels.
			Delta_Ang_R = Mathf.Clamp (Delta_Ang_R, -rightAngRate * tempClampRate, rightAngRate * tempClampRate); // Anti Stroboscopic Effect.
			Rate_R += Delta_Ang_R / rightAngRate;
			if (Rate_R > 1.0f) {
				Rate_R %= 1.0f;
				Switch_Mesh_R = !Switch_Mesh_R;
			} else if (Rate_R < 0.0f) {
				Rate_R = 1.0f + (Rate_R % 1.0f);
				Switch_Mesh_R = !Switch_Mesh_R;
			}
			rightPreviousAng = currentAng;
		}

		void Prepare_With_Static_Track (PTS_Static_Track_CS tempScript)
		{ // Called from parent's "PTS_Static_Track_CS".
			parentScript = tempScript;
			halfLength = parentScript.Length * 0.5f;
		}

		void Get_Body_Script (PTS_MainBody_Setting_CS tempScript)
		{ // Called from "PTS_MainBody_Setting_CS".
			bodyScript = tempScript;
		}

	}

}