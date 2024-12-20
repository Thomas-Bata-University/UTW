using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Drive_Control_Input_99_AI_CS : Drive_Control_Input_00_Base_CS
    {

        float vertical;
        float horizontal;
        AI_CS aiScript;


        public override void Prepare(Drive_Control_CS controlScript)
        {
            // Store the reference to "Drive_Control_CS".
            this.controlScript = controlScript;

            // Store the reference to "AI_CS".
            aiScript = GetComponentInChildren<AI_CS>();
        }


        public override void Drive_Input(bool isOwner)
        {
            vertical = aiScript.Speed_Order;
            horizontal = aiScript.Turn_Order;

            Set_Values();
        }


        void Set_Values()
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
            if (vertical == 0.0f)
            { // The tank should be doing pivot-turn.
                horizontal *= controlScript.Pivot_Turn_Rate;
                controlScript.L_Input_Rate = -horizontal;
                controlScript.R_Input_Rate = -horizontal;
                controlScript.Turn_Brake_Rate = 0.0f;
                controlScript.Pivot_Turn_Flag = true;
                return;
            }

            // In case of brake-turn.
            controlScript.L_Input_Rate = Mathf.Clamp(-vertical - horizontal, -1.0f, 1.0f);
            controlScript.R_Input_Rate = Mathf.Clamp(vertical - horizontal, -1.0f, 1.0f);
            controlScript.Turn_Brake_Rate = horizontal;
            controlScript.Pivot_Turn_Flag = false;
        }

    }

}
