using UnityEngine;

namespace ChobiAssets.PTM
{

    [DefaultExecutionOrder(+2)] // (Note.) This script is executed after "Camera_Points_Manager_CS", in order to add the height.
    public class Camera_PopUp_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Camera_Pivot" in the tank.
		 * The camera pivot is popped up according to the main camera's FOV by this script.
		*/

        // User options >>
        public Camera Main_Camera;
        public float PopUp_Length = 1.0f;
        public float PopUp_Start_FOV = 40.0f;
        public float PopUp_Goal_FOV = 10.0f;
        public AnimationCurve Motion_Curve;
        // << User options


        Transform thisTransform;
        bool isPoppingUp;
        float currentHeight;
        float targetHeight;

        bool enableThisFunction;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisTransform = transform;

            // Get the main camera.
            if (Main_Camera == null)
            {
                Main_Camera = GetComponentInChildren<Camera>();
            }
            if (Main_Camera == null)
            {
                Debug.LogError("'Camera_PopUp_CS' cannot find the main camera.");
                Destroy(this.gameObject);
                return;
            }
        }


        void LateUpdate()
        {
            if (Main_Camera.enabled && enableThisFunction)
            {
                PopUp();
            }
        }


        void PopUp()
        {
            float rate = Mathf.Clamp01((PopUp_Start_FOV - Main_Camera.fieldOfView) / (PopUp_Start_FOV - PopUp_Goal_FOV));
            targetHeight = Motion_Curve.Evaluate(rate) * PopUp_Length;

            if (currentHeight != targetHeight)
            {
                currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, 10.0f * Time.deltaTime);
            }

            // Add the height to the current position.
            Vector3 currentPosition = thisTransform.position;
            currentPosition.y += currentHeight;
            thisTransform.position = currentPosition;
        }


        void Change_Camera_Settings(CameraPoint cameraPointClass)
        { // Called from "Camera_Points_Manager_CS".
            if (cameraPointClass.Rotation_Type == 1)
            { // Third person
                enableThisFunction = cameraPointClass.Enable_PopUp;
                if (enableThisFunction)
                {
                    currentHeight = 0.0f;
                    targetHeight = currentHeight;
                }
                return;
            }
            // (Note.) This function is only for Third Person View.
            enableThisFunction = false;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}
