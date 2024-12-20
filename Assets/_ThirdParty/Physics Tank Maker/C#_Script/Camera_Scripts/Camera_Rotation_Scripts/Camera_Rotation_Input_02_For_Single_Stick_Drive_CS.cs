using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Camera_Rotation_Input_02_For_Single_Stick_Drive_CS : Camera_Rotation_Input_00_Base_CS
    {

        protected Transform bodyTransform;


        public override void Prepare(Camera_Rotation_CS rotationScript)
        {
            this.rotationScript = rotationScript;
            bodyTransform = transform.root.GetComponentInChildren<Rigidbody>().transform;
        }


        public override void Get_Input()
        {
            // Check the main camera is enabled.
            if (rotationScript.Main_Camera.enabled == false)
            {
                // Do not rotate.
                rotationScript.Horizontal_Input = 0.0f;
                rotationScript.Vertical_Input = 0.0f;
                return;
            }

            // Look forward.
            if (Input.GetKeyDown(General_Settings_CS.Camera_Look_Forward_Pad_Button))
            {
                rotationScript.Look_At_Target(bodyTransform.position + bodyTransform.forward * 64.0f);
            }

            // Rotation.
            multiplier = Mathf.Lerp(0.1f, 2.0f, rotationScript.Main_Camera.fieldOfView / 15.0f); // Change the rotation speed according to the FOV of the main camera.
            rotationScript.Horizontal_Input = Input.GetAxis("Horizontal2") * multiplier;
            rotationScript.Vertical_Input = Input.GetAxis("Vertical2") * multiplier * 0.5f;
        }

    }

}
