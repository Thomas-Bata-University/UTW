using UnityEngine;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]
	[ RequireComponent (typeof(AudioListener))]

	public class Gun_Camera_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "Gun_Camera" under the "Barrel_Base" in the tank.
		 * This script controls the gun camera used for aiming the target.
		 * The main camera and the gun camera are switched by this script.
		*/


		// User options >>
		public Camera Gun_Camera;
		public Camera_Points_Manager_CS Camera_Manager_Script;
		public float Minimum_FOV = 2.0f;
		public float Maximum_FOV = 10.0f;
		// << User options


		// Set by "inputType_Settings_CS".
		public int inputType = 0;

		//Set by "Gun_Camera_Input_##_##_CS" scripts.
		public float Zoom_Input;

        Transform thisTransform;
        float targetFOV;
        float currentFOV;
        AudioListener thisListener;

        protected Gun_Camera_Input_00_Base_CS inputScript;

        bool isSelected;


        void Start()
		{
			Initialize();
		}


        void Initialize()
        {
            thisTransform = transform;

            // Get the input type.
            if (inputType != 10)
            { // This tank is not an AI tank.
                inputType = General_Settings_CS.Input_Type;
            }

            // Get the gun camera.
            if (Gun_Camera == null)
            {
                Gun_Camera = GetComponent<Camera>();
            }
            Gun_Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            Gun_Camera.enabled = false;
            Gun_Camera.fieldOfView = Maximum_FOV;
            currentFOV = Gun_Camera.fieldOfView;
            targetFOV = currentFOV;

            // Get the AudioListener.
            thisListener = GetComponent<AudioListener>();
            if (thisListener == null)
            {
                thisListener = gameObject.AddComponent<AudioListener>();
            }
            thisListener.enabled = false;
            this.tag = "Untagged";

            // Get the "Camera_Points_Manager_CS" script in the tank.
            if (Camera_Manager_Script == null)
            {
                Camera_Manager_Script = transform.root.GetComponentInChildren<Camera_Points_Manager_CS>();
            }
            if (Camera_Manager_Script == null)
            {
                Debug.LogWarning("'Gun_Camera_CS' cannot find 'Camera_Points_Manager_CS' in the tank.");
                Destroy(this.gameObject);
                return;
            }

            // Set the input script.
            Set_Input_Script(inputType);

            // Prepare the input script.
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
                    inputScript = gameObject.AddComponent<Gun_Camera_Input_01_Mouse_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                    inputScript = gameObject.AddComponent<Gun_Camera_Input_02_For_Single_Stick_Drive_CS>();
                    break;

                case 3: // Gamepad (Twin sticks)
                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<Gun_Camera_Input_03_For_Twin_Stick_Drive_CS>();
                    break;
            }
        }


        void Update()
        {
            if (isSelected == false)
            {
                return;
            }

            inputScript.Get_Input();

            if (Gun_Camera.enabled)
            {
                Zoom();
            }
        }


        public void Switch_Mode(int mode)
        { // Called from "Gun_Camera_Input_##_###".
            switch (mode)
            {
                case 0: // Not selected.
                    // Disable the gun camera.
                    Gun_Camera.enabled = false;
                    thisListener.enabled = false;
                    this.tag = "Untagged";
                    break;

                case 1: // Off
                    // Call "Camera_Points_Manager_CS" to enable the main camera.
                    // Call "Camera_Rotation_CS" to point the camera in the same direction. 
                    Vector3 lookAtPos;
                    var ray = new Ray(thisTransform.position, thisTransform.forward);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit, 2048.0f, Layer_Settings_CS.Aiming_Layer_Mask))
                    {
                        lookAtPos = raycastHit.point;
                    }
                    else
                    {
                        lookAtPos = thisTransform.position + (thisTransform.forward * 2048.0f);
                    }
                    Camera_Manager_Script.SendMessage("Enable_Camera", lookAtPos, SendMessageOptions.DontRequireReceiver);

                    // Disable the gun camera.
                    Gun_Camera.enabled = false;
                    thisListener.enabled = false;
                    this.tag = "Untagged";
                    break;

                case 2: // On
                    // Call "Camera_Points_Manager_CS" to disable the main camera.
                    Camera_Manager_Script.SendMessage("Disable_Camera", SendMessageOptions.DontRequireReceiver);
                    
                    // Enable the gun camera.
                    Gun_Camera.enabled = true;
                    thisListener.enabled = true;
                    this.tag = "MainCamera";
                    break;
            }
        }


        float currentZoomVelocity;
        void Zoom()
        {
            targetFOV *= 1.0f + Zoom_Input;
            targetFOV = Mathf.Clamp(targetFOV, Minimum_FOV, Maximum_FOV);

            if (currentFOV != targetFOV)
            {
                currentFOV = Mathf.SmoothDamp(currentFOV, targetFOV, ref currentZoomVelocity, 2.0f * Time.deltaTime);
                Gun_Camera.fieldOfView = currentFOV;
            }
        }


        void Get_AI_CS()
        { // Called from "AI_CS".
            inputType = 10;
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            if (isSelected)
            {
                this.isSelected = true;
            }
            else
            {
                if (this.isSelected)
                { // This tank is selected until now.
                    this.isSelected = false;
                    Switch_Mode(0); // Not selected.
                }
            }
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Turn off the gun camera. >> Switch to the main camera.
            if (isSelected)
            {
                Switch_Mode(1); // Off
            }

            Destroy(this.gameObject);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}