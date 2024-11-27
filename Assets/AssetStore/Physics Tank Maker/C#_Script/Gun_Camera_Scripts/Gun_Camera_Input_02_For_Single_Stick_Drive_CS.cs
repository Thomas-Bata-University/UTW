using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Gun_Camera_Input_02_For_Single_Stick_Drive_CS : Gun_Camera_Input_00_Base_CS
    {

        public override void Get_Input()
        {
            // Turn on / off.
            if (General_Settings_CS.Gun_Camera_While_Pressing)
            { // Gun camera is enabled only while pressing the key.
                if (Input.GetKeyDown(General_Settings_CS.Gun_Camera_Switch_Pad_Button))
                {
                    gunCameraScript.Switch_Mode(2); // On
                }
                else if (Input.GetKeyUp(General_Settings_CS.Gun_Camera_Switch_Pad_Button))
                {
                    gunCameraScript.Switch_Mode(1); // Off
                }
            }
            else
            { // Gun camera is turned on / off each time the key is pressed.
                if (Input.GetKeyDown(General_Settings_CS.Gun_Camera_Switch_Pad_Button))
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
                var inputValue = 0.0f;
                if (Input.GetKey(General_Settings_CS.Camera_Zoom_In_Pad_Button))
                {
                    inputValue = -1.0f;
                }
                else if (Input.GetKey(General_Settings_CS.Camera_Zoom_Out_Pad_Button))
                {
                    inputValue = 1.0f;
                }

                gunCameraScript.Zoom_Input = inputValue * 0.05f;
            }

        }

    }

}
