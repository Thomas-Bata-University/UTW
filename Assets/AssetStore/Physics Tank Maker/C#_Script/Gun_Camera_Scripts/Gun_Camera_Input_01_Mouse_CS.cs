using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Gun_Camera_Input_01_Mouse_CS : Gun_Camera_Input_00_Base_CS
	{

		public override void Get_Input()
		{
            // Turn on / off.
            if (General_Settings_CS.Gun_Camera_While_Pressing)
            { // Gun camera is enabled only while pressing the key.
                if (Input.GetKeyDown(General_Settings_CS.Gun_Camera_Switch_Key))
                {
                    gunCameraScript.Switch_Mode(2); // On
                }
                else if (Input.GetKeyUp(General_Settings_CS.Gun_Camera_Switch_Key))
                {
                    gunCameraScript.Switch_Mode(1); // Off
                }
            }
            else
            { // Gun camera is turned on / off each time the key is pressed.
                if (Input.GetKeyDown(General_Settings_CS.Gun_Camera_Switch_Key))
                {
                    if (gunCameraScript.Gun_Camera.enabled)
                    { // The gun camera is enabled.
                        gunCameraScript.Switch_Mode(1); // Off
                    }
                    else
                    { // The gun camera is disabled.
                        gunCameraScript.Switch_Mode(2); // On
                    }
                }
            }

            // Zoom.
            if (gunCameraScript.Gun_Camera.enabled)
            { // The gun camera is enabled.
                gunCameraScript.Zoom_Input = -Input.GetAxis("Mouse ScrollWheel") * 2.0f;
            }

        }

	}

}
