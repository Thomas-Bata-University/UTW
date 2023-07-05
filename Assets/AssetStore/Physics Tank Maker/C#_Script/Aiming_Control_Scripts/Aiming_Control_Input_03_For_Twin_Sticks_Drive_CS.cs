using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Aiming_Control_Input_03_For_Twin_Sticks_Drive_CS : Aiming_Control_Input_00_Base_CS
    {

        public override void Prepare(Aiming_Control_CS aimingScript)
        {
            this.aimingScript = aimingScript;

            aimingScript.Use_Auto_Turn = false;
        }


        public override void Get_Input()
        {
            // Rotate the turret and the cannon manually.
            if (Input.GetButton("Bumper L"))
            {
                aimingScript.Turret_Turn_Rate = Input.GetAxis("Horizontal2");
                aimingScript.Cannon_Turn_Rate = -Input.GetAxis("Vertical2");
            }
            else
            {
                aimingScript.Turret_Turn_Rate = 0.0f;
                aimingScript.Cannon_Turn_Rate = 0.0f;
            }

        }

    }

}
