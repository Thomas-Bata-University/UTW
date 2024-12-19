using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Drive_Control_CS : MonoBehaviour
	{
	
		public float Torque = 2000.0f;
		public float Max_Speed = 8.0f;
		public float MaxAngVelocity_Limit = 45.0f;

		public float ParkingBrake_Velocity = 0.5f;
		public float ParkingBrake_Lag = 0.5f;
		public bool Fix_Useless_Rotaion = false;

		public bool Use_AntiSlip = false;
		public float Ray_Distance = 1.5f;
		public float Support_Velocity = 0.05f;

		// Referred to from Drive_Wheel.
		public bool Parking_Brake = false;
		public float L_Input_Rate;
		public float R_Input_Rate;


		bool stopFlag;
		float lagCount;
		Rigidbody bodyRigidbody;
		Transform thisTransform;
		float vertical;
		float horizontal;

		void Awake ()
		{
			thisTransform = transform;
			bodyRigidbody = GetComponent <Rigidbody> ();
		}

		void Update ()
		{
			vertical = Input.GetAxis ("Vertical");
			horizontal = Input.GetAxis ("Horizontal");
			if (vertical == 0.0f && horizontal == 0.0f) {
				stopFlag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
			} else {
				stopFlag = false;
				if (vertical == 0.0f) { // Pivot Turn
					L_Input_Rate = -horizontal;
					R_Input_Rate = -horizontal;
				}
				else { // Brake Turn
					float clamp = Mathf.Abs(vertical);
					horizontal = Mathf.Clamp(horizontal, -clamp, clamp);
					L_Input_Rate = Mathf.Clamp(-vertical - horizontal, -1.0f, 1.0f);
					R_Input_Rate = Mathf.Clamp(vertical - horizontal, -1.0f, 1.0f);
				}
			}
		}


		void Sticks_Input_Exam ()
		{
			L_Input_Rate = -Input.GetAxis ("Vertical");
			R_Input_Rate = Input.GetAxis ("Vertical2");
			stopFlag = (L_Input_Rate == 0.0f && R_Input_Rate == 0.0f);
			// Set vertical value for Anti_Slip function.
			if (stopFlag || Mathf.Sign (L_Input_Rate) == Mathf.Sign (R_Input_Rate)) { // Stopping or Pivot Turning
				vertical = 0.0f;
			}
			else if (L_Input_Rate < 0.0f && R_Input_Rate > 0.0f) { // Going forward
				vertical = 1.0f;
			}
			else if (L_Input_Rate > 0.0f && R_Input_Rate < 0.0f) { // Going Backward
				vertical = -1.0f;
			}
		}

		void FixedUpdate ()
		{
			// Parking Brake Control.
			if (stopFlag) { // No input for driving.
				float currentVelocity = bodyRigidbody.velocity.magnitude;
				float currentAngularVelocity = bodyRigidbody.angularVelocity.magnitude;
				if (Parking_Brake == false) { // Parking Brake does not work yet.
					if (currentVelocity < ParkingBrake_Velocity && currentAngularVelocity < ParkingBrake_Velocity) { // The MainBody almost stops.
						lagCount += Time.fixedDeltaTime;
						if (lagCount > ParkingBrake_Lag) {
							Parking_Brake = true;
							bodyRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
							return;
						}
					}
				} else { // Parking Brake is working now.
					if (currentVelocity < ParkingBrake_Velocity && currentAngularVelocity < ParkingBrake_Velocity) { //The MainBody almost stops.
						return;
					} else {
						Parking_Brake = false;
						bodyRigidbody.constraints = RigidbodyConstraints.None;
						lagCount = 0.0f;
					}
				}
			} else if (Parking_Brake) { // There is any input for driving && Parking Brake is still working.
				Parking_Brake = false;
				bodyRigidbody.constraints = RigidbodyConstraints.None;
				lagCount = 0.0f;
			}

			// Anti Spin function.
			if (L_Input_Rate == -R_Input_Rate) { // not turnning.
				Vector3 currentAngularVelocity = bodyRigidbody.angularVelocity;
				currentAngularVelocity.y *= 0.9f; // Reduce the velocity on Y-axis.
				bodyRigidbody.angularVelocity = currentAngularVelocity;
			}
			// Anti Slip function.
			if (Use_AntiSlip) {
				Anti_Slip ();
			}
		}

		Ray ray = new Ray ();
		void Anti_Slip ()
		{
			ray.origin = thisTransform.position;
			ray.direction = -thisTransform.up;
			if (Physics.Raycast(ray, Ray_Distance, PTS_Layer_Settings_CS.Anti_Slipping_Layer_Mask)) { // On the ground.
				Vector3 currentVelocity = bodyRigidbody.velocity;
				if (stopFlag) {
					currentVelocity.x *= 0.99f;
					currentVelocity.z *= 0.99f;
				} else {
					currentVelocity = Vector3.MoveTowards (currentVelocity, thisTransform.forward * (Mathf.Sign (vertical) * currentVelocity.magnitude), 32.0f * Time.fixedDeltaTime);
				}
				bodyRigidbody.velocity = currentVelocity;
			}
		}

	}

}