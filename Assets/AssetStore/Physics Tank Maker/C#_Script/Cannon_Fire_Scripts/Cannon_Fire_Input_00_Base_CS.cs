using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_Input_00_Base_CS : MonoBehaviour
	{

		protected Cannon_Fire_CS cannonFireScript;


		public virtual void Prepare(Cannon_Fire_CS cannonFireScript)
		{
			this.cannonFireScript = cannonFireScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
