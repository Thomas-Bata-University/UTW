using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Steer_Wheel_Input_01_Mouse_CS : Steer_Wheel_Input_00_Base_CS
    {

        public override void Get_Input()
        {
            if (Input.GetKey(General_Settings_CS.Drive_Left_Key))
            {
                steerScript.Horizontal_Input = -1.0f;
            }
            else if (Input.GetKey(General_Settings_CS.Drive_Right_Key))
            {
                steerScript.Horizontal_Input = 1.0f;
            }
            else
            {
                steerScript.Horizontal_Input = 0.0f;
            }
        }

    }

}
