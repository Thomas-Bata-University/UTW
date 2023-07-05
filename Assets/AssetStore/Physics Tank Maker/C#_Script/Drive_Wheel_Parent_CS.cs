using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Drive_Wheel_Parent_CS : MonoBehaviour
    {
        /*
		 * This script is attached to "Create_###Wheels" objects that has drive wheels under the hierarchy.
		 * This script controls the angular velocity, angular drug and torque of the child drive wheels.
		 * This script works in combination with "Drive_Control_CS" in the MainBody, and "Drive_Wheel_CS" in the child drive wheels.
		*/

        // User options >>
        public bool Drive_Flag = true;
        public float Radius = 0.3f;
        public bool Use_BrakeTurn = true;
        // << User options


        // Referred to from "Drive_Wheel_CS".
        public float Max_Angular_Velocity;
        public float Left_Angular_Drag;
        public float Right_Angular_Drag;
        public float Left_Torque;
        public float Right_Torque;


        float maxAngVelocity;
        Drive_Control_CS controlScript;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            // Get the "Drive_Control_CS".
            controlScript = GetComponentInParent<Drive_Control_CS>();

            // Set the "maxAngVelocity".
            maxAngVelocity = Mathf.Deg2Rad * ((controlScript.Max_Speed / (2.0f * Radius * Mathf.PI)) * 360.0f);
        }


        void Update()
        {
            Control_Velocity_And_Torque();
        }


        void Control_Velocity_And_Torque()
        {
            // Set the "Max_Angular_Velocity".
            Max_Angular_Velocity = controlScript.Speed_Rate * maxAngVelocity;

            // Set the brake drag.
            if (Use_BrakeTurn)
            {
                Left_Angular_Drag = controlScript.L_Brake_Drag;
                Right_Angular_Drag = controlScript.R_Brake_Drag;
            }
            else
            {
                Left_Angular_Drag = 0.0f;
                Right_Angular_Drag = 0.0f;
            }

            // Set the torque.
            if (Drive_Flag == true)
            {
                Left_Torque = controlScript.Left_Torque;
                Right_Torque = controlScript.Right_Torque;
            }
            else
            {
                Left_Torque = 0.0f;
                Right_Torque = 0.0f;
            }
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            if (controlScript)
            {
                this.enabled = !isPaused;
            }
        }

    }

}