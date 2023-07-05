using UnityEngine;

namespace ChobiAssets.PTM
{

    public class RC_Camera_Input_02_For_Single_Stick_Drive_CS : RC_Camera_Input_00_Base_CS
    {

        protected bool dPadPressed;


        public override void Prepare(RC_Camera_CS rcCameraScript)
        {
            this.rcCameraScript = rcCameraScript;

            rcCameraScript.Use_Analog_Stick = true;
        }


        public override void Get_Input()
        {
            // Switch
            if (dPadPressed == false && Input.GetAxis(General_Settings_CS.RC_Camera_Switch_Pad_Axis) == General_Settings_CS.RC_Camera_Switch_Pad_Axis_Direction)
            {
                dPadPressed = true;
                rcCameraScript.Switch_RC_Camera();
            }
            else if (dPadPressed == true && Input.GetAxis(General_Settings_CS.RC_Camera_Switch_Pad_Axis) == 0.0f)
            {
                dPadPressed = false;
            }

            // Check the RC camera is enabled.
            if (rcCameraScript.RC_Camera.enabled == false)
            {
                return;
            }

            // Zoom
            var inputValue = 0.0f;
            if (Input.GetKey(General_Settings_CS.Camera_Zoom_In_Pad_Button))
            {
                inputValue = -1.0f;
            }
            else if (Input.GetKey(General_Settings_CS.Camera_Zoom_Out_Pad_Button))
            {
                inputValue = 1.0f;
            }
            rcCameraScript.Zoom_Input = inputValue * 0.05f;

            // Turn
            var multiplier = Mathf.Lerp(0.01f, 1.0f, rcCameraScript.RC_Camera.fieldOfView / 50.0f); // Change the rotation speed according to the FOV of the RC camera.
            rcCameraScript.Horizontal_Input = Mathf.Pow(Input.GetAxis("Horizontal2"), 2.0f) * Mathf.Sign(Input.GetAxis("Horizontal2")) * multiplier;
            rcCameraScript.Vertical_Input = Mathf.Pow(Input.GetAxis("Vertical2"), 2.0f) * Mathf.Sign(Input.GetAxis("Vertical2")) * multiplier;
            rcCameraScript.Is_Turning = (rcCameraScript.Horizontal_Input != 0.0f || rcCameraScript.Vertical_Input != 0.0f);
        }

    }

}
