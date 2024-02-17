using UnityEngine;

namespace ChobiAssets.PTS
{
	public class PTS_Track_LOD_Control_CS : MonoBehaviour
	{

		public GameObject Static_Track;
		public GameObject Scroll_Track_L;
		public GameObject Scroll_Track_R;
		public float Threshold = 15.0f;

		Transform thisTransform;
		PTS_MainBody_Setting_CS bodyScript;
		int lod = 9;

		void Awake ()
		{
			thisTransform = transform;
			if (Static_Track == null || Scroll_Track_L == null || Scroll_Track_R == null) {
				Debug.LogWarning ("Track LOD system cannot work, because the tracks for 'LOD_Control_CS' are not assigned.");
				Destroy (this);
			} else {
				// Set Active all the tracks for their initial settings.
				Static_Track.SetActive (true);
				Scroll_Track_L.SetActive (true);
				Scroll_Track_R.SetActive (true);
			}
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) {
				Camera[] currentCams = Camera.allCameras;
				Camera chosenCam = null;
				float value = Mathf.Infinity;
				for (int i = 0; i < currentCams.Length; i++) {
					float tempValue = 2.0f * Vector3.Distance (thisTransform.position, currentCams [i].transform.position) * Mathf.Tan (currentCams [i].fieldOfView * 0.5f * Mathf.Deg2Rad);
					if (tempValue < value) {
						value = tempValue;
						chosenCam = currentCams [i];
					}
				}
				value *= Screen.width / chosenCam.pixelWidth;
				switch (lod) {
					case 0:
						if (value > Threshold) {
							lod = 1;
							Static_Track.SetActive(false);
							Scroll_Track_L.SetActive(true);
							Scroll_Track_R.SetActive(true);
						}
						break;
					case 1:
						if (value < Threshold) {
							lod = 0;
							Static_Track.SetActive(true);
							Scroll_Track_L.SetActive(false);
							Scroll_Track_R.SetActive(false);
						}
						break;
					case 9: // Called only at the first time.
						if (value > Threshold) {
							lod = 1;
							Static_Track.SetActive(false);
							Scroll_Track_L.SetActive(true);
							Scroll_Track_R.SetActive(true);
						} else {
							lod = 0;
							Static_Track.SetActive(true);
							Scroll_Track_L.SetActive(false);
							Scroll_Track_R.SetActive(false);
						}
						break;
				}
			}
		}

		void Get_Body_Script (PTS_MainBody_Setting_CS tempScript)
		{ // Called from "PTS_MainBody_Setting_CS".
			bodyScript = tempScript;
		}

	}
}
