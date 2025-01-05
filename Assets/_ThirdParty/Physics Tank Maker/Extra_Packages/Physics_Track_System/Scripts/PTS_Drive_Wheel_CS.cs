using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Drive_Wheel_CS : MonoBehaviour
	{
	
		public bool Drive_Flag = true;
		public float Radius = 0.3f;

		Rigidbody thisRigidbody;
		Transform thisTransform;
		bool isLeft;
		float maxAngVelocity;

		Quaternion storedRot;
		bool isFixed = false;

		PTS_Drive_Control_CS driveControlScript;

		void Awake ()
		{
			thisRigidbody = GetComponent < Rigidbody > ();
			thisTransform = transform;
			// Set isLeft
			isLeft = (thisTransform.localEulerAngles.z == 0.0f);
			// Get reference
			driveControlScript = GetComponentInParent <PTS_Drive_Control_CS> ();
			maxAngVelocity = Mathf.Deg2Rad * ((driveControlScript.Max_Speed / (2.0f * Radius * Mathf.PI)) * 360.0f);
			maxAngVelocity = Mathf.Clamp (maxAngVelocity, 0.0f, driveControlScript.MaxAngVelocity_Limit); // To solve physics issues in the default physics quality.
		}

		void Update ()
		{ // only for wheels with Physics_Track.
			if (driveControlScript.Fix_Useless_Rotaion) {
				Fix_Rotaion ();
			}
		}

		void Fix_Rotaion ()
		{ // only for wheels with Physics_Track.
			if (driveControlScript.Parking_Brake) {
				if (isFixed == false) {
					isFixed = true;
					storedRot = thisTransform.localRotation;
				}
			} else {
				isFixed = false;
				return;
			}
			if (isFixed) {
				thisTransform.localRotation = storedRot;
			}
		}

		void FixedUpdate ()
		{
			if (isLeft) { // Left
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * driveControlScript.L_Input_Rate);
				// Add Torque.
				if (Drive_Flag) {
					thisRigidbody.AddRelativeTorque (0.0f, driveControlScript.Torque * Mathf.Sign (driveControlScript.L_Input_Rate), 0.0f);
				}
			} else { // Right
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * driveControlScript.R_Input_Rate);
				// Add Torque.
				if (Drive_Flag) {
					thisRigidbody.AddRelativeTorque (0.0f, driveControlScript.Torque * Mathf.Sign (driveControlScript.R_Input_Rate), 0.0f);
				}
			}
		}

	}

}