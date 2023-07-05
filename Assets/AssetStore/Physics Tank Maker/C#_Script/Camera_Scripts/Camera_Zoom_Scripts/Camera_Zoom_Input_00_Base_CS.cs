using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Zoom_Input_00_Base_CS : MonoBehaviour
	{

		protected Camera_Zoom_CS zoomScript;


		public virtual void Prepare(Camera_Zoom_CS zoomScript)
		{
			this.zoomScript = zoomScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
