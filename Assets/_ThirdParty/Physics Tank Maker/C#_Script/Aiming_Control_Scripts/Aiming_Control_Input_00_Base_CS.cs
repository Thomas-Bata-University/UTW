using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Aiming_Control_Input_00_Base_CS : MonoBehaviour
	{

		protected Aiming_Control_CS aimingScript;


		public virtual void Prepare(Aiming_Control_CS aimingScript)
		{
			this.aimingScript = aimingScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
