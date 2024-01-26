using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Gun_Camera_Input_00_Base_CS : MonoBehaviour
	{

		protected Gun_Camera_CS gunCameraScript;


		public void Prepare(Gun_Camera_CS gunCameraScript)
		{
			this.gunCameraScript = gunCameraScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
