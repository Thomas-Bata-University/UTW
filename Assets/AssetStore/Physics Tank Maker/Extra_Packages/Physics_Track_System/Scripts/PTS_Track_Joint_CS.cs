using UnityEngine;

namespace ChobiAssets.PTS
{
	public class PTS_Track_Joint_CS : MonoBehaviour
	{

		public Transform Base_Transform;
		public Transform Front_Transform;
		public float Joint_Offset;

		PTS_MainBody_Setting_CS bodyScript;
		Transform thisTransform;

		void Awake ()
		{
			thisTransform = transform;
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				Vector3 basePos = Base_Transform.position + (Base_Transform.forward * Joint_Offset);
				Vector3 frontPos = Front_Transform.position - (Front_Transform.forward * Joint_Offset);
				thisTransform.SetPositionAndRotation (Vector3.Slerp (basePos, frontPos, 0.5f), Quaternion.Slerp (Base_Transform.rotation, Front_Transform.rotation, 0.5f));
			}
		}

		public void Set_Value (Transform baseTransform, Transform frontTransform, float offset)
		{
			Base_Transform = baseTransform;
			Front_Transform = frontTransform;
			Joint_Offset = offset;
		}

		void Get_Body_Script (PTS_MainBody_Setting_CS tempScript)
		{ // Called from "PTS_MainBody_Setting_CS".
			bodyScript = tempScript;
		}

	}

}