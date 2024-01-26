using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Wheel_Resize_CS : MonoBehaviour
	{
	
		public float ScaleDown_Size = 0.5f;
		public float Return_Speed = 0.05f;
	
		bool isSmall;
		Transform thisTransform;
		Vector3 currentScale;

		void Awake ()
		{
			if (ScaleDown_Size <= 1.0f) {
				isSmall = true;
			} else {
				isSmall = false;
			}
			thisTransform = transform;
			currentScale = new Vector3 (ScaleDown_Size, ScaleDown_Size, ScaleDown_Size);
			thisTransform.localScale = currentScale;
		}

		void Update ()
		{
			if (isSmall) {
				ScaleDown_Size += Return_Speed;
				if (ScaleDown_Size >= 1.0f) {
					thisTransform.localScale = Vector3.one;
					Destroy (this);
				}
			} else {
				ScaleDown_Size -= Return_Speed;
				if (ScaleDown_Size <= 1.0f) {
					thisTransform.localScale = Vector3.one;
					Destroy (this);
				}
			}
			currentScale.x = ScaleDown_Size;
			currentScale.y = ScaleDown_Size;
			currentScale.z = ScaleDown_Size;
			thisTransform.localScale = currentScale;
		}

	}

}