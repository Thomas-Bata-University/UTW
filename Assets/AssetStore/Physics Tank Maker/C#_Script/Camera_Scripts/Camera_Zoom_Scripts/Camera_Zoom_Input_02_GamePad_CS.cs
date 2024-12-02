using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Zoom_Input_02_GamePad_CS : Camera_Zoom_Input_00_Base_CS
	{

		public override void Get_Input()
		{
            var inputValue = 0.0f;
            if (Input.GetKey(General_Settings_CS.Camera_Zoom_In_Pad_Button))
            {
                inputValue = -1.0f;
            }
            else if (Input.GetKey(General_Settings_CS.Camera_Zoom_Out_Pad_Button))
            {
                inputValue = 1.0f;
            }

            zoomScript.Zoom_Input = inputValue * 0.05f;
        }

    }

}
