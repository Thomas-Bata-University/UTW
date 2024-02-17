using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Points_Manager_Input_01_Mouse_CS : Camera_Points_Manager_Input_00_Base_CS
    {

        public override void Get_Input()
        {
            if (Input.GetKeyDown(General_Settings_CS.Camera_Switch_Key))
            {
                managerScript.Switch_Camera_Point();
            }
		}

	}

}
