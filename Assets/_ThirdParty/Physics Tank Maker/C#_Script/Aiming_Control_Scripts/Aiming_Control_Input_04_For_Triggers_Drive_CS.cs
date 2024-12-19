using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Aiming_Control_Input_04_For_Triggers_Drive_CS : Aiming_Control_Input_03_For_Twin_Sticks_Drive_CS
    {

        public override void Get_Input()
        {
            // Rotate the turret and the cannon manually.
            aimingScript.Turret_Turn_Rate = Input.GetAxis("Horizontal");
            aimingScript.Cannon_Turn_Rate = -Input.GetAxis("Vertical");
        }

    }

}
