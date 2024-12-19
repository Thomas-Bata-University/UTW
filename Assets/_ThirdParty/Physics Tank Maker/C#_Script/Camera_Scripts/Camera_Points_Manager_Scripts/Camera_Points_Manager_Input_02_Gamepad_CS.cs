using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Points_Manager_Input_02_Gamepad_CS : Camera_Points_Manager_Input_00_Base_CS
    {

        protected bool dPadPressed;


        public override void Get_Input()
        {
            // Switch
            if (dPadPressed == false && Input.GetAxis(General_Settings_CS.Camera_Switch_Pad_Axis) == General_Settings_CS.Camera_Switch_Pad_Axis_Direction)
            {
                dPadPressed = true;
                managerScript.Switch_Camera_Point();
            }
            else if (dPadPressed == true && Input.GetAxis(General_Settings_CS.Camera_Switch_Pad_Axis) == 0.0f)
            {
                dPadPressed = false;
            }
        }

	}

}
