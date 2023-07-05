using UnityEngine;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(AudioListener))]

    public class RC_Camera_CS : MonoBehaviour
    {
        /*
		 * This script controls the RC camera in the scene.
		*/

        // User options >>
        public Camera RC_Camera;
        public float Horizontal_Speed = 50.0f;
        public float Vertical_Speed = 50.0f;
        public float Min_FOV = 1.0f;
        public float Max_FOV = 50.0f;
        public Transform Position_Pack;
        public bool Is_Enabled = false;
        // << User options

       
        // Set by "RC_Camera_Input_##_###_CS" scripts.
        public bool Use_Analog_Stick;
        public float Zoom_Input;
        public float Horizontal_Input;
        public float Vertical_Input;
        public bool Is_Turning;

        Transform thisTransform;
        AudioListener thisAudioListener;
        Transform targetTransform;
        ID_Settings_CS targetIDScript;
        Camera targetMainCamera;
        AudioListener targetMainAudioListener;
        Camera targetGunCamera;
        Transform[] cameraPositions;
        float targetFOV;
        float currentFOV;
        Vector3 currentAngles;
        Vector3 lookAtUp = Vector3.up;
        int currentPosIndex = -1;
        int inputType = 0;

        protected RC_Camera_Input_00_Base_CS inputScript;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisTransform = transform;
            if (RC_Camera == null)
            {
                RC_Camera = GetComponent<Camera>();
            }
            thisAudioListener = GetComponent<AudioListener>();
            currentFOV = RC_Camera.fieldOfView;
            targetFOV = currentFOV;

            this.tag = "Untagged";
            RC_Camera.enabled = false;
            thisAudioListener.enabled = false;

            if (Position_Pack)
            {
                Set_Positions();
            }

            // Get the input type.
            inputType = General_Settings_CS.Input_Type;

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
                    inputScript = gameObject.AddComponent<RC_Camera_Input_01_Mouse_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<RC_Camera_Input_02_For_Single_Stick_Drive_CS>();
                    break;

                case 3: // Gamepad (Twin sticks)
                    inputScript = gameObject.AddComponent<RC_Camera_Input_03_For_Twin_Sticks_Drive_CS>();
                    break;
            }
        }


        void Set_Positions()
        {
            if (Position_Pack.childCount != 0)
            { // The "Position_Pack" is not empty.
                cameraPositions = new Transform[Position_Pack.childCount];
                for (int i = 0; i < Position_Pack.childCount; i++)
                {
                    cameraPositions[i] = Position_Pack.GetChild(i);
                }
            }
            else
            { // The "Position_Pack" is empty.
                // Set the initial position.
                cameraPositions = new Transform[1];
                GameObject newPositionObject = new GameObject("RC_Camera_Position (1)");
                newPositionObject.transform.position = thisTransform.position;
                cameraPositions[0] = newPositionObject.transform;
            }
        }


        void LateUpdate()
        {
            if (targetIDScript == null || targetIDScript.Is_Selected == false)
            { // The target is removed from the scene, or has not been selected.
                if (Find_Target() == false)
                { // The target cannot be found.
                    return;
                }
            }

            // Get the input values.
            inputScript.Get_Input();

            // Turn on / off the three cameras. (RC camera, tatget's main camera, tatget's gun camera)
            if (Control_Cameras_Enabling() == false)
            { // The RC camera is disabled.
                return;
            }

            // Control the camera position.
            if (Position_Pack && Is_Turning == false)
            {
                Control_Position();
            }

            // Zoom.
            Zoom();

            // Rotate.
            if (Use_Analog_Stick)
            {
                Rotate_With_Stick();
            }
            else
            {
                if (Is_Turning)
                {
                    Rotate_With_Mouse();
                }
                else
                {
                    thisTransform.LookAt(targetTransform.position, lookAtUp);
                }
            }
        }


        public void Switch_RC_Camera()
        { // Called from "RC_Camera_Input_##_###_CS".
            Is_Enabled = !Is_Enabled;
        }


        bool Control_Cameras_Enabling()
        {
            if (Is_Enabled == true)
            { // RC camera is enabled now.
              // Disable the target's main camera.
                targetMainCamera.enabled = false;
                targetMainAudioListener.enabled = false;

                // Check the target's gun camera.
                if (targetGunCamera && targetGunCamera.enabled == true)
                { // The target is using the gun camera.
                  // Disable RC camera.
                    this.tag = "Untagged";
                    RC_Camera.enabled = false;
                    thisAudioListener.enabled = false;
                    return false;
                }
                else
                { // The target is not using the gun camera.
                  // Enable RC camera.
                    this.tag = "MainCamera";
                    RC_Camera.enabled = true;
                    thisAudioListener.enabled = true;
                    return true;
                }
            }
            else
            { // RC camera is not selected now.
              // Disable RC camera.
                this.tag = "Untagged";
                RC_Camera.enabled = false;
                thisAudioListener.enabled = false;

                // Check the target's gun camera.
                if (targetGunCamera && targetGunCamera.enabled == true)
                { // The target is using the gun camera.
                  // Disable the target's main camera.
                    targetMainCamera.enabled = false;
                    targetMainAudioListener.enabled = false;
                }
                else
                { // The target is not using the gun camera.
                  // Enable the target's main camera.
                    targetMainCamera.enabled = true;
                    targetMainAudioListener.enabled = true;
                }
                return false;
            }
        }


        bool Find_Target()
        {
            // Find "Camera_Points_Manager_CS" in the current selected tank.
            if (ID_Manager_CS.Instance == null)
            {
                Debug.LogWarning("RC Camera cannot find the target. There is no 'ID_Manager_CS' in this scene.");
                return false;
            }

            for (int i = 0; i < ID_Manager_CS.Instance.ID_Scripts_List.Count; i++)
            {
                if (ID_Manager_CS.Instance.ID_Scripts_List[i].Is_Selected)
                {
                    targetIDScript = ID_Manager_CS.Instance.ID_Scripts_List[i];
                    var cameraScript = targetIDScript.GetComponentInChildren<Camera_Points_Manager_CS>();
                    if (cameraScript == null)
                    {
                        Debug.LogWarning("RC Camera cannot find the target. The current selected tank has no 'Camera_Points_Manager_CS'.");
                        targetIDScript = null;
                        return false;
                    }

                    // Get the components of the target.
                    targetMainCamera = cameraScript.Main_Camera;
                    targetMainAudioListener = cameraScript.Main_AudioListener;
                    targetTransform = targetIDScript.GetComponentInChildren<Rigidbody>().transform;
                    break;
                }
            }
            
            // Get the gun camera in the target.
            var targetGunCameraScript = targetTransform.GetComponentInChildren<Gun_Camera_CS>();
            if (targetGunCameraScript)
            {
                targetGunCamera = targetGunCameraScript.Gun_Camera;
            }
           
            return true;
        }


        void Control_Position()
        {
            float shortestDistance = Mathf.Infinity;
            int tempIndex = 0;
            for (int i = 0; i < cameraPositions.Length; i++)
            {
                float tempDist = (targetTransform.position - cameraPositions[i].position).sqrMagnitude;
                if (tempDist < shortestDistance)
                {
                    shortestDistance = tempDist;
                    tempIndex = i;
                }
            }

            if (tempIndex != currentPosIndex)
            {
                currentPosIndex = tempIndex;
                lookAtUp = cameraPositions[tempIndex].up;
                thisTransform.position = cameraPositions[tempIndex].position;
                thisTransform.rotation = cameraPositions[tempIndex].rotation;
            }
        }


        float currentZoomVelocity;
        void Zoom()
        {
            // Set the target FOV.
            targetFOV *= 1.0f + Zoom_Input;
            targetFOV = Mathf.Clamp(targetFOV, Min_FOV, Max_FOV);

            // Set the RC camera's FOV.
            if (currentFOV != targetFOV)
            {
                currentFOV = Mathf.SmoothDamp(currentFOV, targetFOV, ref currentZoomVelocity, 2.0f * Time.deltaTime);
                currentFOV = Mathf.Clamp(currentFOV, Min_FOV, Max_FOV);
                RC_Camera.fieldOfView = currentFOV;
            }
        }


        float currentVerticalInput;
        float currentHorizontalInput;
        Vector2 currentRotationVelocity;
        void Rotate_With_Mouse()
        {
            // Rotate smoothly.
            currentVerticalInput = Mathf.SmoothDamp(currentVerticalInput, Vertical_Input, ref currentRotationVelocity.x, 2.0f * Time.deltaTime);
            currentHorizontalInput = Mathf.SmoothDamp(currentHorizontalInput, Horizontal_Input, ref currentRotationVelocity.y, 2.0f * Time.deltaTime);

            // Set the angles.
            currentAngles = thisTransform.localEulerAngles;
            currentAngles.x -= currentVerticalInput;
            currentAngles.y += currentHorizontalInput;
            thisTransform.eulerAngles = currentAngles;
        }


        
        void Rotate_With_Stick()
        {
            // Rotate smoothly.
            currentVerticalInput = Mathf.SmoothDamp(currentVerticalInput, Vertical_Input, ref currentRotationVelocity.x, 2.0f * Time.deltaTime);
            currentHorizontalInput = Mathf.SmoothDamp(currentHorizontalInput, Horizontal_Input, ref currentRotationVelocity.y, 2.0f * Time.deltaTime);

            // Set the angles.
            thisTransform.LookAt(targetTransform.position);
            currentAngles = thisTransform.eulerAngles;
            currentAngles.x -= currentVerticalInput * 30.0f;
            currentAngles.y += currentHorizontalInput * 60.0f;
            currentAngles.z = 0.0f;
            thisTransform.eulerAngles = currentAngles;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }
    }

}