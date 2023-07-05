using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Rotation_Input_00_Base_CS : MonoBehaviour
	{

		protected Camera_Rotation_CS rotationScript;
		protected float multiplier;


		public virtual void Prepare(Camera_Rotation_CS rotationScript)
		{
			this.rotationScript = rotationScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
