using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Camera_Rotation_Input_99_Demo_CS : Camera_Rotation_Input_00_Base_CS
    {

        protected float targetVerticalAngle;


        public override void Get_Input()
        {
            // Set the horizontal input.
            rotationScript.Horizontal_Input = Mathf.Sin(Time.frameCount * 0.002f) * 0.1f;

            // Set the vertical input.
            float deltaAngle = Mathf.DeltaAngle(rotationScript.transform.eulerAngles.z, targetVerticalAngle);
            if (Mathf.Abs(deltaAngle) < 0.1f)
            { // The current angle is almost the target angle.
                // Update the target angle.
                targetVerticalAngle = Random.Range(0.0f, 10.0f);
                rotationScript.Vertical_Input = 0.0f;
            }
            else
            { // The current angle is different from the target angle.
                rotationScript.Vertical_Input = Mathf.Lerp(0.0f, -1.0f, Mathf.Abs(deltaAngle) / 5.0f) * Mathf.Sign(deltaAngle) * Time.deltaTime;
            }
        }

    }

}
