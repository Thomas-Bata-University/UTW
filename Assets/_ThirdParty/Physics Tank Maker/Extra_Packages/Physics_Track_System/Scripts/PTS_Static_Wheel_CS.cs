using UnityEngine;

namespace ChobiAssets.PTS
{
	
	public class PTS_Static_Wheel_CS : MonoBehaviour
	{

		public float Radius_Offset;

		Transform thisTransform;
		bool isLeft;
		float staticTrackRate;
		float scrollTrackRate;
		PTS_MainBody_Setting_CS bodyScript;
		PTS_Static_Track_CS staticTrackScript;
		PTS_Track_Scroll_CS scrollTrackScript;

		void Awake ()
		{
			thisTransform = transform;
			isLeft = (thisTransform.localEulerAngles.z == 0.0f);
		}
		
		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				if (staticTrackScript && staticTrackScript.isActiveAndEnabled) {
					Work_With_Static_Track ();
				} else if (scrollTrackScript) {
					Work_With_Scroll_Track ();
				}
			}
		}

		void Work_With_Static_Track ()
		{
			Vector3 currentAng = thisTransform.localEulerAngles;
			if (isLeft) {
				currentAng.y -= staticTrackScript.Delta_Ang_L * staticTrackRate;
			} else {
				currentAng.y -= staticTrackScript.Delta_Ang_R * staticTrackRate;
			}
			thisTransform.localEulerAngles = currentAng;
		}

		void Work_With_Scroll_Track ()
		{
			Vector3 currentAng = thisTransform.localEulerAngles;
			currentAng.y -= scrollTrackScript.Delta_Ang* scrollTrackRate;
			thisTransform.localEulerAngles = currentAng;
		}

		void Prepare_With_Static_Track (PTS_Static_Track_CS tempScript)
		{ // Called from "PTS_Static_Track_CS".
			staticTrackScript = tempScript;
			if (staticTrackScript.Reference_L && staticTrackScript.Reference_R) {
				// Set rate.
				float radius = GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset;
				if (isLeft) { // Left
					staticTrackRate = staticTrackScript.Reference_Radius_L / radius;
				} else { // Right
					staticTrackRate = staticTrackScript.Reference_Radius_R / radius;
				}
			} else {
				Debug.LogWarning ("Static_Wheel can not find the reference wheel in the Static_Tracks.");
				Destroy (this);
			}
		}

		void Prepare_With_Scroll_Track (PTS_Track_Scroll_CS tempScript)
		{ // Called from "PTS_Track_Scroll_CS".
			if (tempScript.Reference_Wheel) {
				if ((isLeft && tempScript.Direction == 0) || (isLeft == false && tempScript.Direction == 1)) {
					scrollTrackScript = tempScript;
					float radius = GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset;
					float referenceRadius = scrollTrackScript.Reference_Wheel.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
					scrollTrackRate = referenceRadius / radius;
					return;
				}
			} else {
				Debug.LogWarning ("Static_Wheel can not find the reference wheel in the Scroll_Tracks.");
				Destroy (this);
			}
		}

		void Get_Body_Script (PTS_MainBody_Setting_CS tempScript)
		{ // Called from "PTS_MainBody_Setting_CS".
			bodyScript = tempScript;
		}

	}
		
}