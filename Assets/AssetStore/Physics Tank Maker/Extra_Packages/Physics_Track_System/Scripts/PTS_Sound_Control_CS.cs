using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Sound_Control_CS : MonoBehaviour
	{

		//public bool For_PhysicsTrack = false;
		public float Min_Engine_Pitch = 1.0f;
		public float Max_Engine_Pitch = 2.0f;
		public float Min_Engine_Volume = 0.1f;
		public float Max_Engine_Volume = 0.3f;
		public float Max_Velocity = 7.0f;
		public Rigidbody Left_Wheel_Rigidbody;
		public Rigidbody Right_Wheel_Rigidbody;

		float currentRate;
		// Used by Editor script to show the current velocity.
		public float Left_Velocity; 
		public float Right_Velocity;

		AudioSource thisAudioSource;

		void Awake () {
			thisAudioSource = GetComponent < AudioSource > ();
			if (thisAudioSource == null) {
				Debug.LogError ("AudioSource cannot be found in " + transform.name);
				Destroy (this);
			}
			thisAudioSource.playOnAwake = false;
			thisAudioSource.loop = true;
			thisAudioSource.volume = 0.0f;
			thisAudioSource.Play ();
			Set_Closest_DrivingWheel ();
		}

		void Set_Closest_DrivingWheel ()
		{
			if (Left_Wheel_Rigidbody == null || Right_Wheel_Rigidbody == null) { // Reference is not assigned.
				Transform	bodyTransform = transform.parent;
				PTS_Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren <PTS_Drive_Wheel_CS> ();
				float minDistL = Mathf.Infinity;
				float minDistR = Mathf.Infinity;
				Transform closestWheelL = null;
				Transform closestWheelR = null;
				for (int i = 0; i < driveScripts.Length; i++) {
					Transform connectedTransform = driveScripts [i].GetComponent <HingeJoint> ().connectedBody.transform;
					MeshFilter meshFilter = driveScripts [i].GetComponent <MeshFilter> ();
					if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // connected to Suspension && not invisible.
						float tempDist = Vector3.Distance (bodyTransform.position, driveScripts [i].transform.position); // Distance to the MainBody.
						if (driveScripts [i].transform.localEulerAngles.z == 0.0f) { // Left
							if (tempDist < minDistL) {
								closestWheelL = driveScripts [i].transform;
								minDistL = tempDist;
							}
						} else { // Right
							if (tempDist < minDistR) {
								closestWheelR = driveScripts [i].transform;
								minDistR = tempDist;
							}
						}
					}
				}
				if (closestWheelL && closestWheelR) {
					Left_Wheel_Rigidbody = closestWheelL.GetComponent <Rigidbody> ();
					Right_Wheel_Rigidbody = closestWheelR.GetComponent <Rigidbody> ();
				}
			}
			if (Left_Wheel_Rigidbody && Right_Wheel_Rigidbody) {
				return;
			} else {
				Debug.LogError ("Reference Wheels for the engine sound can not be found.");
				Destroy (this);
			}
		}

		void FixedUpdate ()
		{
			Engine_Sound ();
		}

		void Engine_Sound ()
		{
			if (Left_Wheel_Rigidbody) {
				Left_Velocity = Left_Wheel_Rigidbody.velocity.magnitude;
			} else {
				Left_Velocity = 0.0f;
			}
			if (Right_Wheel_Rigidbody) {
				Right_Velocity = Right_Wheel_Rigidbody.velocity.magnitude;
			} else {
				Right_Velocity = 0.0f;
			}
			float targetRate = (Left_Velocity + Right_Velocity) / 2.0f / Max_Velocity;
			float maxDelta;
			if (targetRate > currentRate) {
				maxDelta = 2.0f;
			} else {
				maxDelta = 0.75f;
			}
			currentRate = Mathf.MoveTowards (currentRate, targetRate, maxDelta * Time.fixedDeltaTime);
			thisAudioSource.pitch = Mathf.Lerp (Min_Engine_Pitch, Max_Engine_Pitch, currentRate);
			thisAudioSource.volume = Mathf.Lerp (Min_Engine_Volume, Max_Engine_Volume, currentRate);
		}

	}

}