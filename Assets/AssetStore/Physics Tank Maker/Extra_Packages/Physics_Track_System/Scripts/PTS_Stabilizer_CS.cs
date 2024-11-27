using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Stabilizer_CS : MonoBehaviour
	{
	
		Transform thisTransform;
		float initialPosY;
		Vector3 angles;

		void Awake ()
		{
			thisTransform = transform;
			initialPosY = thisTransform.localPosition.y;
			angles = thisTransform.localEulerAngles;
		}

		void Update ()
		{
			// Stabilize position.
			Vector3 currentPos = thisTransform.localPosition;
			currentPos.y = initialPosY;
			// Stabilize angle.
			angles.y = thisTransform.localEulerAngles.y;
			// Set Position and Rotation.
			thisTransform.localPosition = currentPos;
			thisTransform.localEulerAngles = angles;
		}

		void OnJointBreak ()
		{
			if (thisTransform.parent) {
				thisTransform.parent.BroadcastMessage("Break_Joint_Linkage", SendMessageOptions.DontRequireReceiver);
			}
		}

		void Break_Joint_Linkage ()
		{
			thisTransform.parent = null;
			Destroy (this);
		}

	}

}