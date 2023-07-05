using UnityEngine;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(AudioListener))]

    public class Camera_Zoom_CS : MonoBehaviour
    {

        /*
		 * This script is attached to the main camera of the tank.
		 * This script controls the FOV (Field Of View) of the main camera.
		*/


        // User options >>
        public Camera Main_Camera;
        public float Min_FOV = 5.0f;
        public float Max_FOV = 60.0f;
        // << User options


        //Set by "Camera_Zoom_Input_##_##_CS" scripts.
        public float Zoom_Input;

        float targetFOV;
        float currentFOV;
        bool enableThisFunction = true;
        int inputType;

        protected Camera_Zoom_Input_00_Base_CS inputScript;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            // Get the input type.
            if (inputType != 10)
            { // This tank is not an AI tank.
                inputType = General_Settings_CS.Input_Type;
            }

            // Get the main camera.
            if (Main_Camera == null)
            {
                Main_Camera = GetComponent<Camera>();
            }
            currentFOV = Main_Camera.fieldOfView;
            targetFOV = currentFOV;

            // Set the "inputScript".
            Set_Input_Script(inputType);

            // Prepare the "inputScript".
            if (inputScript != null)
            {
                inputScript.Prepare(this);
            }
        }


        protected virtual void Set_Input_Script(int type)
        {
            switch (type)
            {
                case 0: // Mouse + Keyboard (Stepwise)
                case 1: // Mouse + Keyboard (Pressing)
                case 10: // AI.
                    inputScript = gameObject.AddComponent<Camera_Zoom_Input_01_Mouse_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                case 3: // Gamepad (Twin sticks)
                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<Camera_Zoom_Input_02_GamePad_CS>();
                    break;
            }
        }


        void Update()
        {
            if (Main_Camera.enabled && enableThisFunction)
            { // This tank is selected, and the main camera is enabled, and this function is enabled.

                inputScript.Get_Input();

                Zoom();
            }
        }


        float currentZoomVelocity;
        void Zoom()
        {
            targetFOV *= 1.0f + Zoom_Input;
            targetFOV = Mathf.Clamp(targetFOV, Min_FOV, Max_FOV);

            if (currentFOV != targetFOV)
            {
                currentFOV = Mathf.SmoothDamp(currentFOV, targetFOV, ref currentZoomVelocity, 2.0f * Time.deltaTime);
                Main_Camera.fieldOfView = currentFOV;
            }
        }


        void Change_Camera_Settings(CameraPoint cameraPointClass)
        { // Called from "Camera_Points_Manager_CS".
            targetFOV = cameraPointClass.FOV;
            currentFOV = targetFOV;
            Main_Camera.fieldOfView = currentFOV;
            Main_Camera.nearClipPlane = cameraPointClass.Clipping_Planes_Near;
            enableThisFunction = cameraPointClass.Enable_Zooming;
        }


        void Get_AI_CS()
        { // Called from "AI_CS".
            inputType = 10;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}