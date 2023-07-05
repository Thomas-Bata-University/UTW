using UnityEngine;

namespace ChobiAssets.PTM
{

	public class RC_Camera_Input_00_Base_CS : MonoBehaviour
	{

		protected RC_Camera_CS rcCameraScript;


		public virtual void Prepare(RC_Camera_CS rcCameraScript)
		{
			this.rcCameraScript = rcCameraScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
