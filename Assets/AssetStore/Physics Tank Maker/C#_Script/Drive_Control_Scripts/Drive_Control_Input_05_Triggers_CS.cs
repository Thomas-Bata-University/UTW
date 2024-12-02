using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Drive_Control_Input_05_Triggers_CS : Drive_Control_Input_00_Base_CS
    {

        public override void Drive_Input(bool isOwner)
        {
            var leftTrigger = -Input.GetAxis("Trigger L");
            var rightTrigger = Input.GetAxis("Trigger R");
            var leftButton = Input.GetAxis("Bumper L");
            var rightButton = -Input.GetAxis("Bumper R");

            controlScript.L_Input_Rate = leftTrigger + leftButton;
            controlScript.R_Input_Rate = rightTrigger + rightButton;

            // Set the "Stop_Flag".
            controlScript.Stop_Flag = (controlScript.L_Input_Rate == 0.0f && controlScript.R_Input_Rate == 0.0f);
            if (controlScript.Stop_Flag)
            {
                return;
            }

            // Pivot turn.
            if (controlScript.L_Input_Rate == controlScript.R_Input_Rate)
            {
                controlScript.L_Input_Rate *= controlScript.Pivot_Turn_Rate;
                controlScript.R_Input_Rate *= controlScript.Pivot_Turn_Rate;
                controlScript.Turn_Brake_Rate = 0.0f;
                return;
            }

            // Going straight.
            if (controlScript.L_Input_Rate == -controlScript.R_Input_Rate)
            {
                controlScript.Turn_Brake_Rate = 0.0f;
                return;
            }

            // Brake turn.
            var tempHorizontal = Mathf.Clamp(-controlScript.L_Input_Rate - controlScript.R_Input_Rate, -1.0f, 1.0f);
            if ((controlScript.L_Input_Rate == 0.0f || controlScript.R_Input_Rate == 0.0f) && (controlScript.L_Input_Rate > 0.0f || controlScript.R_Input_Rate < 0.0f))
            { // Backward turn.
                tempHorizontal = -tempHorizontal;
            }
            controlScript.Turn_Brake_Rate = tempHorizontal;
            return;
        }

    }

}
