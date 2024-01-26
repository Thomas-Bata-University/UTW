using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Aiming_Control_Input_01_Mouse_Keyboard_CS : Aiming_Control_Input_00_Base_CS
	{

        protected Gun_Camera_CS gunCameraScript;
        protected int thisRelationship;
		protected Vector3 screenCenter = Vector3.zero;


        public override void Prepare(Aiming_Control_CS aimingScript)
        {
            this.aimingScript = aimingScript;

            // Set the "Use_Auto_Turn".
            aimingScript.Use_Auto_Turn = true;

            // Get the "Gun_Camera_CS".
            gunCameraScript = GetComponentInChildren<Gun_Camera_CS>();

            // Set the relationship.
            ID_Settings_CS idScript = GetComponentInParent<ID_Settings_CS>();
            if (idScript)
            {
                thisRelationship = idScript.Relationship;
            }

            // Set the initial aiming mode.
            aimingScript.Mode = 1; // Free aiming.
            aimingScript.Switch_Mode();

            // Set the initial target position.
            aimingScript.Target_Position = transform.position + (transform.forward * 128.0f);
        }


        public override void Get_Input()
		{
            // Switch the aiming mode.
            if (Input.GetKeyDown(General_Settings_CS.Aim_Mode_Switch_Key))
            {
                if (aimingScript.Mode == 0 || aimingScript.Mode == 2)
                {
                    aimingScript.Mode = 1; // Free aiming.
                }
                else
                {
                    aimingScript.Mode = 0; // Keep the initial positon.
                }
                aimingScript.Switch_Mode();
            }


            // Adjust aiming.
            if (gunCameraScript && gunCameraScript.Gun_Camera.enabled)
            { // The gun camera is enabled now.

                // Set the adjust angle.
                aimingScript.Adjust_Angle.x += Input.GetAxis("Mouse X") * General_Settings_CS.Aiming_Sensibility;
                aimingScript.Adjust_Angle.y += Input.GetAxis("Mouse Y") * General_Settings_CS.Aiming_Sensibility;

                // Check it is locking-on now.
                if (aimingScript.Target_Transform)
                { // Now locking-on the target.
                    // Cancel the lock-on.
                    if (Input.GetKeyDown(General_Settings_CS.Turret_Cancel_Key))
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
                    if (Input.GetKey(General_Settings_CS.Turret_Cancel_Key) == false)
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
            { // The gun camera is disabled now.

                // Reset the adjust angle.
                aimingScript.Adjust_Angle = Vector3.zero;

                // Stop the turret and cannon rotation while pressing the cancel button. >> Only the camera rotates.
                if (Input.GetKey(General_Settings_CS.Turret_Cancel_Key))
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
            // Left lock on.
            if (Input.GetKeyDown(General_Settings_CS.Aim_Lock_On_Left_Key))
            {
                aimingScript.Auto_Lock(0, thisRelationship);
                return;
            }
            // Right lock on.
            if (Input.GetKeyDown(General_Settings_CS.Aim_Lock_On_Right_Key))
            {
                aimingScript.Auto_Lock(1, thisRelationship);
                return;
            }
            // Front lock on.
            if (Input.GetKeyDown(General_Settings_CS.Aim_Lock_On_Front_Key))
            {
                aimingScript.Auto_Lock(2, thisRelationship);
                return;
			}
            */
            
        }

	}

}
