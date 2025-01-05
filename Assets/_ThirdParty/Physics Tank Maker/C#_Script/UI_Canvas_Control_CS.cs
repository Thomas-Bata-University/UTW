using UnityEngine;

namespace ChobiAssets.PTM
{

    public class UI_Canvas_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to some of the Canvases in the scene, and turns on / off it by pressing the specified key.
		*/

        Canvas thisCanvas;
        bool isEnabled;


        void Start()
        {
            thisCanvas = GetComponent<Canvas>();
            isEnabled = thisCanvas.enabled;
        }


        void Update()
        {
            if (Input.GetKeyDown(General_Settings_CS.Switch_Canvas_Key))
            {
                isEnabled = !isEnabled;
                thisCanvas.enabled = isEnabled;
            }
        }


        public void Button_Push()
        { // Called from the button.
            isEnabled = !isEnabled;
            thisCanvas.enabled = isEnabled;
        }

    }

}