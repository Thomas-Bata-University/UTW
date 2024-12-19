using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Drive_Control_Input_02_Keyboard_Pressing_CS : Drive_Control_Input_01_Keyboard_Stepwise_CS
    {

        public override void Drive_Input(bool isOwner)
        {
            if (isOwner) {
                // Set "vertical".
                if (Input.GetKey(General_Settings_CS.Drive_Up_Key)) {
                    vertical = 1.0f;
                }
                else if (Input.GetKey(General_Settings_CS.Drive_Down_Key)) {
                    vertical = -0.5f;
                }
                else {
                    vertical = 0.0f;
                }

                // Set "horizontal".
                if (Input.GetKey(General_Settings_CS.Drive_Left_Key)) {
                    horizontal = -1.0f;
                }
                else if (Input.GetKey(General_Settings_CS.Drive_Right_Key)) {
                    horizontal = 1.0f;
                }
                else {
                    horizontal = 0.0f;
                }

                // Control the brake.
                controlScript.Apply_Brake = Input.GetKey(General_Settings_CS.Drive_Brake_Key);
            }

            // Set the "Stop_Flag", "L_Input_Rate", "R_Input_Rate" and "Turn_Brake_Rate".
            Set_Values();
        }


        protected override void Set_Values()
        {
            // In case of stopping.
            if (vertical == 0.0f && horizontal == 0.0f)
            { // The tank should stop.
                controlScript.Stop_Flag = true;
                controlScript.L_Input_Rate = 0.0f;
                controlScript.R_Input_Rate = 0.0f;
                controlScript.Turn_Brake_Rate = 0.0f;
                controlScript.Pivot_Turn_Flag = false;
                return;
            }
            else
            { // The tank should be driving.
                controlScript.Stop_Flag = false;
            }

            // In case of going straight.
            if (horizontal == 0.0f)
            { // The tank should be going straight.
                controlScript.L_Input_Rate = -vertical;
                controlScript.R_Input_Rate = vertical;
                controlScript.Turn_Brake_Rate = 0.0f;
                controlScript.Pivot_Turn_Flag = false;
                return;
            }

            // In case of pivot-turn.
            if (controlScript.Allow_Pivot_Turn)
            { // Pivot-turn is allowed.
                if (vertical == 0.0f && controlScript.Speed_Rate <= controlScript.Pivot_Turn_Rate)
                { // The tank should be doing pivot-turn.
                    horizontal *= controlScript.Pivot_Turn_Rate;
                    controlScript.L_Input_Rate = -horizontal;
                    controlScript.R_Input_Rate = -horizontal;
                    controlScript.Turn_Brake_Rate = 0.0f;
                    controlScript.Pivot_Turn_Flag = true;
                    return;
                }
            }
            else
            { // Pivot-turn is not allowed.
                if (vertical == 0.0f)
                {
                    vertical = 1.0f;
                }
            }

            // In case of brake-turn.
            controlScript.Pivot_Turn_Flag = false;
            Brake_Turn();
        }

    }

}
