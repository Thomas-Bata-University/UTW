using UnityEngine;

namespace ChobiAssets.PTS
{
	public class PTS_Simple_Camera_CS : MonoBehaviour
	{

		Transform thisTransform;
		Transform childTransform;
		Vector3 currentAngles;
		Vector3 currentChildPos;

		void Awake ()
		{
			Cursor.lockState = CursorLockMode.Locked;
			thisTransform = transform;
			childTransform = thisTransform.GetChild (0);
			currentAngles = thisTransform.eulerAngles;
			currentChildPos = childTransform.localPosition;
		}

		void Update ()
		{
			// Switch Cursor mode.
			if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)) {
				if (Cursor.lockState == CursorLockMode.Locked) {
					Cursor.lockState = CursorLockMode.None;
				} else {
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
			//
			if (Cursor.lockState == CursorLockMode.Locked) {
				// Pivot Rotation
				currentAngles.y += Input.GetAxis ("Mouse X") * 360.0f * Time.deltaTime;
				currentAngles.z -= Input.GetAxis ("Mouse Y") * 360.0f * Time.deltaTime;
				currentAngles.z = Mathf.Clamp (currentAngles.z, -45.0f, 90.0f);
				// Camera Position
				if (Input.GetAxis("Mouse ScrollWheel") != 0.0f) {
					currentChildPos.x -= Input.GetAxis("Mouse ScrollWheel") * 1000.0f * Time.deltaTime;
					currentChildPos.x = Mathf.Clamp(currentChildPos.x, 5.0f, 50.0f);
					childTransform.localPosition = currentChildPos;
				}
			}
			// Set rotation for every frame.
			thisTransform.eulerAngles = currentAngles;
		}

	}

}
