using UnityEngine;

namespace ChobiAssets.PTM
{

	[System.Serializable]
	public class CameraPoint
	{
		public Transform Camera_Point;
		[Range (1.0f, 179.0f)] public float FOV = 30.0f;
		[Range (0.01f, 1.0f)] public float Clipping_Planes_Near = 0.3f;
		[Tooltip ("0 = Not allowed, 1 = Third person, 2 = First person"), Range (0, 2)] public int Rotation_Type = 1; // 0 = Not allowed, 1 = Third person, 2 = First person.
		public bool Enable_Zooming = true;
		[Tooltip ("Look at the target when locking-on")] public bool Enable_Auto_Look = true;
		public bool Enable_PopUp = true;
		public bool Enable_Avoid_Obstacle = true;
	}


    [DefaultExecutionOrder (+1)] // (Note.) This script is executed after other scripts, in order to move the camera smoothly.
    public class Camera_Points_Manager_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "Camera_Pivot" in the tank.
		 * This script switches the camera points in the tank.
		 * You can attach camera points as much as you need, and can change the settings in each point.
		*/


		// User options >>
		public Camera Main_Camera;
		public AudioListener Main_AudioListener;
		public int Camera_Points_Num = 1;
		public CameraPoint[] Camera_Points;
		// << User options


		int index = -1;
        Transform thisTransform;
        Transform cameraTransform;
        Vector3 cameraTPVLocalPos;
        int inputType;

        protected Camera_Points_Manager_Input_00_Base_CS inputScript;

        public bool Is_Selected; // Referred to from "RC_Camera_CS".


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

            // Get the main camera.
            if (Main_Camera == null)
            {
                Main_Camera = GetComponentInChildren<Camera>();
            }
            Main_Camera.tag = "MainCamera";
            cameraTransform = Main_Camera.transform;
            cameraTPVLocalPos = cameraTransform.localPosition;

            // Get the AudioListener.
            if (Main_AudioListener == null)
            {
                Main_AudioListener = GetComponentInChildren<AudioListener>();
            }

            // Switch the camera point at the first time.
            Switch_Camera_Point();

            // Set the input script.
            Set_Input_Script(inputType);

            // Prepare the input script.
            if (inputScript != null)
            {
                inputScript.Prepare(this);
            }

            // Change the hierarchy.
            thisTransform.parent = thisTransform.parent.parent; // Under the top object of the tank.
        }


        protected virtual void Set_Input_Script(int type)
        {
            switch (type)
            {
                case 0: // Mouse + Keyboard (Stepwise)
                case 1: // Mouse + Keyboard (Pressing)
                case 10: // AI.
                    inputScript = gameObject.AddComponent<Camera_Points_Manager_Input_01_Mouse_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                case 3: // Gamepad (Twin sticks)
                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<Camera_Points_Manager_Input_02_Gamepad_CS>();
                    break;
            }
        }


        void LateUpdate()
        {
            if (Is_Selected == false || Main_Camera.enabled == false)
            {
                return;
            }

            inputScript.Get_Input();

            // Set the position to the current camera point.
            thisTransform.position = Camera_Points[index].Camera_Point.position;
        }


        public void Switch_Camera_Point()
        { // Called from "Camera_Points_Manager_Input_##_###_CS".
            // Update the index.
            index += 1;
            if (index >= Camera_Points.Length)
            {
                index = 0;
            }

            // Set the camera's local position.
            switch (Camera_Points[index].Rotation_Type)
            {
                case 0: // Not allowed.
                case 2: // First Person.
                    // Move to the camera pivot.
                    cameraTransform.localPosition = Vector3.zero;
                    break;

                case 1: // Third person.
                    // Keep a distance from the pivot.
                    cameraTransform.localPosition = cameraTPVLocalPos;
                    break;
            }

            // Call the "Change_Camera_Settings" function in "Camera_###_CS" scripts in this object and the child object.
            BroadcastMessage("Change_Camera_Settings", Camera_Points[index], SendMessageOptions.DontRequireReceiver);
        }


        void Enable_Camera()
        { // Called from "Gun_Camera_CS".
            Main_Camera.enabled = true;
            Main_AudioListener.enabled = true;
        }


        void Disable_Camera()
        { // Called also from "Gun_Camera_CS".
            Main_Camera.enabled = false;
            Main_AudioListener.enabled = false;
        }


        void Get_AI_CS()
        { // Called from "AI_CS".
            inputType = 10;
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            this.Is_Selected = isSelected;

            // Turn on / off the main camera, and the AudioListener.
            if (isSelected)
            {
                Enable_Camera();
            }
            else
            {
                Disable_Camera();
            }
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;

            // Set the position to the current camera point.
            if (Is_Selected && Main_Camera.enabled)
            {
                thisTransform.position = Camera_Points[index].Camera_Point.position;
            }
        }

    }

}
