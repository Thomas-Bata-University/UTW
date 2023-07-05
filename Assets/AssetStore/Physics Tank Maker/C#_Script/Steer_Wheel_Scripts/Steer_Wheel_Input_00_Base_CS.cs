using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Steer_Wheel_Input_00_Base_CS : MonoBehaviour
	{

		protected Steer_Wheel_CS steerScript;


		public virtual void Prepare(Steer_Wheel_CS steerScript)
		{
			this.steerScript = steerScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
