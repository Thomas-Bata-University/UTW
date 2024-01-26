using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Drive_Control_Input_03_Single_Stick_CS : Drive_Control_Input_02_Keyboard_Pressing_CS
    {
		
		public override void Drive_Input()
		{
            // Set "vertical".
            vertical = Input.GetAxis("Vertical");
            vertical = Mathf.Clamp(vertical, -0.5f, 1.0f);

            // Set "horizontal".
            horizontal = Input.GetAxis("Horizontal");

            // Set the "Stop_Flag", "L_Input_Rate", "R_Input_Rate" and "Turn_Brake_Rate".
            Set_Values();
		}


        protected override void Brake_Turn()
        {
            if (horizontal < 0.0f)
            { // Left turn.
                controlScript.L_Input_Rate = 0.0f;
                controlScript.R_Input_Rate = vertical;
            }
            else
            { // Right turn.
                controlScript.L_Input_Rate = -vertical;
                controlScript.R_Input_Rate = 0.0f;
            }

            // Set the "Turn_Brake_Rate".
            controlScript.Turn_Brake_Rate = horizontal;
		}

	}

}
