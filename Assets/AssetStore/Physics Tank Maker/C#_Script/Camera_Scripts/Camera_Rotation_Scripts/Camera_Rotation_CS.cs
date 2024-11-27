using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Rotation_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "Camera_Pivot" in the tank.
		 * This script rotate the camera pivot in two ways, first person view and third person view.
		*/


		// User options >>
		public Camera Main_Camera;
        public bool Use_Demo_Camera = false;
		// << User options


		// Set by "Camera_Rotation_Input_##_##_CS" scripts.
		public float Horizontal_Input;
		public float Vertical_Input;

		Transform thisTransform;
        Vector3 targetAngles;
        Vector3 currentAngles;
        Vector3 baseAngles = new Vector3(0.0f, 90.0f, 0.0f);
        Transform bodyTransform;
        CameraPoint currentCameraPoint;
        int inputType;

        protected Camera_Rotation_Input_00_Base_CS inputScript;


        void Awake()
		{ // (Note.) The "thisTransform" must be prepared before "Start()", because the "Change_Camera_Settings()" function is called at Start().
			thisTransform = transform;
		}


        void Start()
		{
			Initialize();
		}


        void Initialize()
		{
            targetAngles = thisTransform.eulerAngles;

            // Get the input type.
            if (Use_Demo_Camera)
            {
                inputType = 99;
            }
            else
            {
                if (inputType != 10)
                { // This tank is not an AI tank.
                    inputType = General_Settings_CS.Input_Type;
                }
            }

            // Get the main camera.
            if (Main_Camera == null)
            {
                Main_Camera = GetComponentInChildren<Camera>();
            }
            if (Main_Camera == null)
            {
                Debug.LogError("'Camera_Rotation_CS' cannot find the main camera.");
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
                    inputScript = gameObject.AddComponent<Camera_Rotation_Input_01_Mouse_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<Camera_Rotation_Input_02_For_Single_Stick_Drive_CS>();
                    break;

                case 3: // Gamepad (Twin sticks)
                    inputScript = gameObject.AddComponent<Camera_Rotation_Input_03_For_Twin_Sticks_Drive_CS>();
                    break;

                case 99: // Demo.
                    inputScript = gameObject.AddComponent<Camera_Rotation_Input_99_Demo_CS>();
                    break;
            }
        }


        void Update()
        {
            // Check the camera.
            if (Main_Camera.enabled == false)
            {
                return;
            }

            // Get the input.
            inputScript.Get_Input();
            if (General_Settings_CS.Camera_Invert_Flag)
            {
                Vertical_Input = -Vertical_Input;
            }

            // Adjust the input rate accoding to the current fps.
            if (Time.deltaTime != 0.0f)
            { // (Note.) Sometimes the delta time will be zero while scene transition. 
                var currentDelta = Time.fixedDeltaTime / Time.deltaTime;
                Horizontal_Input *= currentDelta;
                Vertical_Input *= currentDelta;
            }
        }


        void FixedUpdate()
        {
            // Check the camera.
            if (Main_Camera.enabled == false)
            {
                return;
            }

            // Rotate the camera pivot.
            switch (currentCameraPoint.Rotation_Type)
            {
                case 0: // No rotation.
                    break;

                case 1: // Third person view.
                    Rotate_TPV();
                    break;

                case 2: // First person view.
                    Rotate_FPV();
                    break;
            }
        }


        Vector3 currentRotationVelocity;
        void Rotate_TPV()
        {
            // Set the target angles.
            targetAngles.y += Horizontal_Input * General_Settings_CS.Camera_Horizontal_Speed;
            targetAngles.z -= Vertical_Input * General_Settings_CS.Camera_Vertical_Speed;

            // Clamp the angles.
            if (General_Settings_CS.Camera_Use_Clamp)
            {
                targetAngles.z = Mathf.Clamp(targetAngles.z, -10.0f, 90.0f);
            }

            // Rotate smoothly.
            currentAngles.y = Mathf.SmoothDampAngle(currentAngles.y, targetAngles.y, ref currentRotationVelocity.y, 2.0f * Time.fixedDeltaTime);
            currentAngles.z = Mathf.SmoothDampAngle(currentAngles.z, targetAngles.z, ref currentRotationVelocity.z, 2.0f * Time.fixedDeltaTime);
            thisTransform.eulerAngles = currentAngles;

        }


        void Rotate_FPV()
        {
            // Horizontal rotation.
            targetAngles.y += Horizontal_Input * General_Settings_CS.Camera_Horizontal_Speed;

            // Vertical rotation.
            if (General_Settings_CS.Camera_Simulate_Head_Physics)
            {
                Simulate_Head_Vertical_Rotation();
            }
            else
            {
                Simple_Vertical_Rotation();
            }

            // Rotate smoothly.
            currentAngles.x = Mathf.SmoothDampAngle(currentAngles.x, targetAngles.x, ref currentRotationVelocity.x, 2.0f * Time.fixedDeltaTime);
            currentAngles.y = Mathf.SmoothDampAngle(currentAngles.y, targetAngles.y, ref currentRotationVelocity.y, 2.0f * Time.fixedDeltaTime);
            currentAngles.z = Mathf.SmoothDampAngle(currentAngles.z, targetAngles.z, ref currentRotationVelocity.z, 2.0f * Time.fixedDeltaTime);
            thisTransform.rotation = currentCameraPoint.Camera_Point.rotation * Quaternion.Euler(currentAngles);
        }


        void Simple_Vertical_Rotation()
        {
            // Z aixs (Vertical rotation)
            targetAngles.z -= Vertical_Input * General_Settings_CS.Camera_Vertical_Speed;
            targetAngles.z = Mathf.Clamp(targetAngles.z, -60.0f, 60.0f);
        }


        float noInputCount;
        void Simulate_Head_Vertical_Rotation()
        {
            // Check the vertical input.
            if (Vertical_Input == 0.0f)
            {
                noInputCount += Time.fixedDeltaTime;
                noInputCount = Mathf.Clamp(noInputCount, 0.0f, 1.0f);
            }
            else
            {
                noInputCount = 0.0f;
            }

            // Control the vertical angle.
            if (noInputCount != 0.0f && Mathf.Abs(Mathf.DeltaAngle(0.0f, currentAngles.z)) <= 15.0f)
            { // There is no vertical input, and the camera is almost horizontal.
                // X axis (Rolling rotation).
                var zenithPos = thisTransform.InverseTransformPoint(thisTransform.position + Vector3.up);
                targetAngles.x = Mathf.Atan(zenithPos.z / zenithPos.y) * Mathf.Rad2Deg;

                // Z aixs (Vertical rotation)
                var lookAtPos = thisTransform.position + thisTransform.right;
                var tempRot = Quaternion.LookRotation(lookAtPos - thisTransform.position);
                targetAngles.z = Mathf.Lerp(targetAngles.z, Mathf.DeltaAngle(0.0f, tempRot.eulerAngles.x), noInputCount);
            }
            else
            { // There is any vertical input.
                // Z aixs (Vertical rotation)
                targetAngles.z -= Vertical_Input * General_Settings_CS.Camera_Vertical_Speed;
                targetAngles.z = Mathf.Clamp(targetAngles.z, -60.0f, 60.0f);
            }
        }


        public void Look_At_Target(Vector3 targetPos)
        { // Called from "Aiming_Control_CS".
            if (currentCameraPoint.Enable_Auto_Look == false)
            {
                return;
            }

            switch (currentCameraPoint.Rotation_Type)
            {
                case 0: // No rotation.
                    break;

                case 1: // Third person view.
                    var targetDir = (targetPos - thisTransform.position).normalized;
                    targetAngles.y = Vector2.Angle(Vector2.up, new Vector2 (targetDir.x, targetDir.z)) * Mathf.Sign(targetDir.x) + 90.0f;
                    targetAngles.z = Vector2.Angle(Vector2.up, new Vector2(targetDir.y, Vector2.Distance(Vector2.zero, new Vector2(targetDir.x, targetDir.z)))) * -Mathf.Sign(targetDir.y);
                    targetAngles.z += Main_Camera.fieldOfView * General_Settings_CS.Aiming_Offset;
                    break;

                case 2: // First person view.
                    var localPos = currentCameraPoint.Camera_Point.InverseTransformPoint(targetPos);
                    var local2DPos = new Vector2(localPos.x, localPos.z);
                    targetAngles.y = Vector2.Angle(Vector2.up, new Vector2(localPos.x, localPos.z)) * Mathf.Sign(localPos.x) + 90.0f;
                    targetAngles.z = Vector2.Angle(Vector2.up, new Vector2(localPos.y, Vector2.Distance(Vector2.zero, new Vector2(localPos.x, localPos.z)))) * -Mathf.Sign(localPos.y);
                    targetAngles.z += Main_Camera.fieldOfView * General_Settings_CS.Aiming_Offset;
                    break;
            }
        }


        void Change_Camera_Settings(CameraPoint cameraPointClass)
        { // Called from "Camera_Points_Manager_CS".
            currentCameraPoint = cameraPointClass;

            // Set the initial angles.
            switch (currentCameraPoint.Rotation_Type)
            {
                case 0: // Not allowed.
                    thisTransform.localEulerAngles = baseAngles;
                    break;

                case 1: // Third Person.
                    targetAngles.x = 0.0f;
                    targetAngles.y = thisTransform.eulerAngles.y;
                    currentAngles = targetAngles;
                    break;

                case 2: // First Person.
                    targetAngles.x = 0.0f;
                    targetAngles.y = thisTransform.eulerAngles.y - currentCameraPoint.Camera_Point.eulerAngles.y;
                    currentAngles = targetAngles;
                    break;
            }
        }


        void Enable_Camera(Vector3 targetPos)
        { // Called from "Gun_Camera_CS".
            Look_At_Target(targetPos);
            currentAngles = targetAngles;
            FixedUpdate();
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