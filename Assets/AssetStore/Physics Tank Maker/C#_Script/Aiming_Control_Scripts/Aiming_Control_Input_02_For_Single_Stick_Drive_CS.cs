using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Aiming_Control_Input_02_For_Single_Stick_Drive_CS : Aiming_Control_Input_01_Mouse_Keyboard_CS
    {

        bool dPadPressed;


        public override void Get_Input()
        {
            // Switch the aiming mode.
            if (Input.GetKeyDown(General_Settings_CS.Aim_Mode_Switch_Pad_Button))
            {
                if (aimingScript.Mode == 1)
                { // Free aiming.
                    aimingScript.Mode = 0; // Lock off.
                }
                else
                {
                    aimingScript.Mode = 1; // Free aiming.
                }
                aimingScript.Switch_Mode();
            }


            // Adjust aiming.
            if (gunCameraScript && gunCameraScript.Gun_Camera.enabled)
            { // The gun camera is enabled now.

                // Set the adjust angle.
                var multiplier = Mathf.Lerp(0.05f, 1.0f, Camera.main.fieldOfView / 10.0f); // Set the multiplier according to the FOV.
                aimingScript.Adjust_Angle.x += Input.GetAxis("Horizontal2") * General_Settings_CS.Aiming_Sensibility * multiplier;
                aimingScript.Adjust_Angle.y += Input.GetAxis("Vertical2") * General_Settings_CS.Aiming_Sensibility * 0.5f * multiplier;

                // Check it is locking-on now.
                if (aimingScript.Target_Transform)
                { // Now locking-on the target.
                    // Cancel the lock-on.
                    if (Input.GetKeyDown(General_Settings_CS.Turret_Cancel_Pad_Button))
                    {
                        aimingScript.Target_Transform = null;
                        aimingScript.Target_Rigidbody = null;
                    }

                    // Control "reticleAimingFlag" in "Aiming_Control_CS".
                    aimingScript.reticleAimingFlag = false;
                }
                else
                { // Now not locking-on.
                    // Try to find a new target.
                    if (Input.GetKey(General_Settings_CS.Turret_Cancel_Pad_Button) == false)
                    {
                        screenCenter.x = Screen.width * 0.5f;
                        screenCenter.y = Screen.height * 0.5f;
                        aimingScript.Reticle_Aiming(screenCenter, thisRelationship);
                    }

                    // Control "reticleAimingFlag" in "Aiming_Control_CS".
                    aimingScript.reticleAimingFlag = true;
                }

                // Reset the "Turret_Speed_Multiplier".
                aimingScript.Turret_Speed_Multiplier = 1.0f;
            }
            else
            { // The gun camera is not enabled now.

                // Reset the adjust angle.
                aimingScript.Adjust_Angle = Vector3.zero;

                // Stop the turret and cannon rotation while pressing the cancel button. >> Only the camera rotates.
                if (Input.GetKey(General_Settings_CS.Turret_Cancel_Pad_Button))
                {
                    aimingScript.Turret_Speed_Multiplier -= 2.0f * Time.deltaTime;
                }
                else
                {
                    aimingScript.Turret_Speed_Multiplier += 2.0f * Time.deltaTime;
                }
                aimingScript.Turret_Speed_Multiplier = Mathf.Clamp01(aimingScript.Turret_Speed_Multiplier);

                // Free aiming.
                if (aimingScript.Mode == 1)
                { // Free aiming.

                    // Find the target.
                    screenCenter.x = Screen.width * 0.5f;
                    screenCenter.y = Screen.height * (0.5f + General_Settings_CS.Aiming_Offset);
                    aimingScript.Cast_Ray_Free(screenCenter);
                }

                // Control "reticleAimingFlag" in "Aiming_Control_CS".
                aimingScript.reticleAimingFlag = false;
            }

            
            /*
            // Front lock on.
            if (dPadPressed == false && Input.GetAxis(General_Settings_CS.Aim_Lock_On_Front_Pad_Axis) == General_Settings_CS.Aim_Lock_On_Front_Pad_Axis_Direction)
            {
                dPadPressed = true;
                aimingScript.Auto_Lock(2, thisRelationship);
            }
            else if (dPadPressed == true && Input.GetAxis(General_Settings_CS.Aim_Lock_On_Front_Pad_Axis) == 0.0f)
            {
                dPadPressed = false;
            }
            */

        }

    }

}
