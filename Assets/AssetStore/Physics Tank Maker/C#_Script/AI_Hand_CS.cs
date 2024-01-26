using UnityEngine;

namespace ChobiAssets.PTM
{

    public class AI_Hand_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "AI_Hand" placed under the "AI_Core" in the AI tank.
		 * This script provides auto-brake function to the AI tank.
		 * This script works in combination with "AI_CS" in the parent object.
		*/


        // User options >>
        public float Max_Scale = 10.0f;
        // << User options


        public bool Is_Working; // Referred to from "AI_CS".
        bool isTouching;
        float touchCount;
        Collider touchCollider;
        AI_CS aiScript;

        Drive_Control_CS driveControlScript;
        Transform thisTransform;
        Vector3 currentPos;
        Vector3 currentScale;
        float initialPosZ;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            Is_Working = false;

            // Get the "AI_CS".
            aiScript = GetComponentInParent<AI_CS>();

            // Set the layer.
            gameObject.layer = 2; // "Ignore Raycast" layer.

            // Make this gameobject invisible.
            var renderer = GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = false;
            }

            // Make this collider a trigger.
            var collider = GetComponent<Collider>();
            if (collider)
            {
                collider.isTrigger = true;
            }

            // Get the "Drive_Control_CS".
            driveControlScript = GetComponentInParent<Drive_Control_CS>();

            // Get the initial position and scale.
            thisTransform = transform;
            currentPos = thisTransform.localPosition;
            initialPosZ = currentPos.z;
            currentScale = thisTransform.localScale;
        }


        void Update()
        {
            Control_Scale();
            Auto_Brake();
        }


        void Control_Scale()
        {
            float tempRate = Mathf.Pow(driveControlScript.Current_Velocity / driveControlScript.Max_Speed, 2.0f);
            currentScale.z = Mathf.Lerp(0.0f, Max_Scale, tempRate);
            currentPos.z = Mathf.Lerp(initialPosZ, initialPosZ + (Max_Scale * 0.5f), tempRate);

            // Set the position and scale.
            thisTransform.localPosition = currentPos;
            thisTransform.localScale = currentScale;
        }


        void Auto_Brake()
        {
            if (Is_Working == false)
            {
                return;
            }

            if (isTouching)
            { // The hand is touching an obstacle now.
                if (touchCollider == null)
                { // The collider might have been removed from the scene.
                    isTouching = false;
                    return;
                }

                touchCount += Time.deltaTime;
                if (touchCount > aiScript.Stuck_Count)
                {
                    touchCount = 0.0f;
                    // Call the "AI_CS" to escape from a stuck.
                    aiScript.Escape_From_Stuck();
                }
                return;

            }
            else
            { // The hand has been away form the obstacle.
                touchCount -= Time.deltaTime;
                if (touchCount < 0.0f)
                {
                    touchCount = 0.0f;
                    Is_Working = false;
                }
            }
        }


        void OnTriggerStay(Collider collider)
        { // Called when the hand touches an obstacle.
            if (isTouching == false && collider.transform.root.tag != "Finish" && collider.attachedRigidbody)
            { // The hand is not touching an obstacle until now, and the collider has a rigidbody.
                Is_Working = true;
                isTouching = true;
                touchCollider = collider;
            }
        }


        void OnTriggerExit()
        { // Called when the hand has been away form the obstacle.
            isTouching = false;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}