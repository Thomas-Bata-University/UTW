using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Zoom_Input_01_Mouse_CS : Camera_Zoom_Input_00_Base_CS
	{

		public override void Get_Input()
		{
			zoomScript.Zoom_Input = -Input.GetAxis("Mouse ScrollWheel") * 2.0f;
		}

	}

}
