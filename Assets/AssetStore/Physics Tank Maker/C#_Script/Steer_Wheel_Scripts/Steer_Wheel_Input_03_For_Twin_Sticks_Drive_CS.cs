using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Steer_Wheel_Input_03_For_Twin_Sticks_Drive_CS : Steer_Wheel_Input_00_Base_CS
	{
		
		public override void Get_Input()
		{
			steerScript.Horizontal_Input = Input.GetAxis ("Horizontal2");
		}

	}

}
