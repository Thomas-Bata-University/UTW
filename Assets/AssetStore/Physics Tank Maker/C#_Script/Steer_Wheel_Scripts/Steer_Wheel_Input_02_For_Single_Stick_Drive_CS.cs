using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Steer_Wheel_Input_02_For_Single_Stick_Drive_CS : Steer_Wheel_Input_00_Base_CS
	{
		
		public override void Get_Input()
		{
			steerScript.Horizontal_Input = Input.GetAxis ("Horizontal");
		}

	}

}
