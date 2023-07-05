using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Track_Scroll_CS : MonoBehaviour
	{

		public Transform Reference_Wheel; // Referred to from "PTS_Static_Wheel_CS".
		public string Reference_Name;
		public string Reference_Parent_Name;
		public float Scroll_Rate = 0.005f;
		public string Tex_Name = "_MainTex";

        // For editor script.
        public bool Has_Changed;

        Material thisMaterial;
		public int Direction; // Referred to from "PTS_Static_Wheel_CS"
		public float Delta_Ang; // Referred to from "PTS_Static_Wheel_CS".
		float previousAng;
		Vector2 offset;
		PTS_MainBody_Setting_CS bodyScript;

		void Awake ()
		{
			bodyScript = GetComponentInParent <PTS_MainBody_Setting_CS> ();
			// Set Reference Wheel.
			if (Reference_Wheel == null) {
				if (string.IsNullOrEmpty (Reference_Name) == false && string.IsNullOrEmpty (Reference_Parent_Name) == false) {
					Reference_Wheel = transform.parent.Find (Reference_Parent_Name + "/" + Reference_Name);
				}
			}
			if (Reference_Wheel) {
				thisMaterial = GetComponent < Renderer > ().material;
				// Set Direction.
				if (Reference_Wheel.localEulerAngles.z == 0.0f) { // Left
					Direction = 0;
				} else { // Right
					Direction = 1;
				}
			} else {
				Debug.LogWarning ("Reference Wheel is not assigned in " + this.name);
				Destroy (this);
			}
			// Boradcast this reference.
			transform.parent.BroadcastMessage ("Prepare_With_Scroll_Track", this, SendMessageOptions.DontRequireReceiver);
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				float currentAng = Reference_Wheel.localEulerAngles.y;
				Delta_Ang = Mathf.DeltaAngle (currentAng, previousAng);
				offset.x += Scroll_Rate * Delta_Ang;
				thisMaterial.SetTextureOffset (Tex_Name, offset);
				previousAng = currentAng;
			}
		}

		void Get_Body_Script (PTS_MainBody_Setting_CS tempScript)
		{ // Called from "PTS_MainBody_Setting_CS".
			bodyScript = tempScript;
		}

	}

}