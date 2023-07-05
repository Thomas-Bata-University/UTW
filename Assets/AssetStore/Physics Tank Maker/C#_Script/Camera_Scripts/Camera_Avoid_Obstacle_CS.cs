using UnityEngine;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(Camera))]

    public class Camera_Avoid_Obstacle_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the main camera of the tank.
		 * This script moves the camera forward and backward to avoid an obstacle between the camera and the tank.
		*/


        // User options >>
        public Camera Main_Camera;
        // << User options


        Transform thisTransform;
        Transform parentTransform;
        Transform rootTransform;
        Vector3 currentLocalPos;
        float currentDistance;
        float targetDistance;
        bool isAvoidingObstacle;
        float hittingTime;
        float storedDistance;

        bool enableThisFunction = true;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            if (Main_Camera == null)
            {
                Main_Camera = GetComponent<Camera>();
            }
            thisTransform = transform;
            rootTransform = thisTransform.root;
            parentTransform = thisTransform.parent;
            currentLocalPos = thisTransform.localPosition;
            currentDistance = currentLocalPos.x;
            targetDistance = currentDistance;
        }


        void FixedUpdate()
        {
            if (Main_Camera.enabled && enableThisFunction)
            {
                Avoid_Obstacle();
                Move();
            }
        }


        void Avoid_Obstacle()
        {
            // Detect an obstacle by casting a ray from the camera pivot to the camera.
            Ray ray = new Ray();
            ray.origin = parentTransform.position;
            ray.direction = (thisTransform.position - parentTransform.position);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, thisTransform.localPosition.x + 1.0f, Layer_Settings_CS.Layer_Mask);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].transform.root != rootTransform)
                { // The hit object is not myself.
                    if (isAvoidingObstacle == false)
                    { // Not avoiding now.
                        hittingTime += Time.deltaTime;
                        if (hittingTime > General_Settings_CS.Camera_Avoid_Lag)
                        {
                            // Start avoiding the obstacle.
                            hittingTime = 0.0f;
                            isAvoidingObstacle = true;
                            storedDistance = currentDistance;
                            targetDistance = raycastHits[i].distance;
                            targetDistance = Mathf.Clamp(targetDistance, General_Settings_CS.Camera_Avoid_Min_Dist, General_Settings_CS.Camera_Avoid_Max_Dist);
                        }
                    } // Avoiding now.
                    else if (raycastHits[i].distance < storedDistance)
                    { // Find a new obstacle that is closer to the tank.
                        targetDistance = raycastHits[i].distance;
                        targetDistance = Mathf.Clamp(targetDistance, General_Settings_CS.Camera_Avoid_Min_Dist, General_Settings_CS.Camera_Avoid_Max_Dist);
                    }
                    return;
                } // The hit object is myself.
            } // The ray does not hit anything.

            // Return the camera to the stored position.
            if (isAvoidingObstacle)
            {
                isAvoidingObstacle = false;
                targetDistance = storedDistance;
            }
        }


        void Move()
        {
            // Move the camera forward and backward.
            if (currentDistance != targetDistance)
            {
                currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, General_Settings_CS.Camera_Avoid_Move_Speed * Time.fixedDeltaTime);
                currentLocalPos.x = currentDistance;
                thisTransform.localPosition = currentLocalPos;
            }
        }


        void Change_Camera_Settings(CameraPoint cameraPointClass)
        { // Called from "Camera_Points_Manager_CS".
            if (cameraPointClass.Rotation_Type == 1)
            { // Third person
                enableThisFunction = cameraPointClass.Enable_Avoid_Obstacle;
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
