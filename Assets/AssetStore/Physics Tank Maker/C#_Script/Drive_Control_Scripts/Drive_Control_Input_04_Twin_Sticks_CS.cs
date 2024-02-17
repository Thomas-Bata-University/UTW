using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Drive_Control_Input_04_Twin_Sticks_CS : Drive_Control_Input_03_Single_Stick_CS
    {

        public override void Drive_Input()
        {
            

            if (Input.GetButton("Bumper L") == false)
            {
                vertical = Input.GetAxis("Vertical");
                horizontal = Input.GetAxis("Horizontal2");
            }
            else
            {
                vertical = 0.0f;
                horizontal = 0.0f;
            }

            Set_Values();
        }

    }

}
