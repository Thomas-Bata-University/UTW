using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Steer_Wheel_CS : MonoBehaviour
	{
		/*
		 * This script is attached to "Hub_L" and "Hub_R" objects under the "Create_SteeredWheel" object.
		 * This script controls steering of the wheels.
		 * This script works in combination with "Drive_Control_CS" in the MainBody.
		*/

		// User options >>
		public float Reverse = 1.0f;
		public float Max_Angle = 35.0f;
		public float Rotation_Speed = 45.0f;
		// << User options


		// Set by "inputType_Settings_CS".
		public int inputType = 0;

		// Set by "Steer_Wheel_Input_##_##_CS" scripts.
		public float Horizontal_Input;

        float currentAng;
        HingeJoint thisHingeJoint;
        JointSpring jointSpring;
        Drive_Control_CS driveControlScript;

        Steer_Wheel_Input_00_Base_CS inputScript;

        bool isSelected;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisHingeJoint = GetComponent<HingeJoint>();
            jointSpring = thisHingeJoint.spring;
            driveControlScript = GetComponentInParent<Drive_Control_CS>();

            // Get the input type.
            if (inputType != 10)
            { // This tank is not an AI tank.
                inputType = General_Settings_CS.Input_Type;
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
                    inputScript = gameObject.AddComponent<Steer_Wheel_Input_01_Mouse_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                    inputScript = gameObject.AddComponent<Steer_Wheel_Input_02_For_Single_Stick_Drive_CS>();
                    break;

                case 3: // Gamepad (Twin sticks)
                    inputScript = gameObject.AddComponent<Steer_Wheel_Input_03_For_Twin_Sticks_Drive_CS>();
                    break;

                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<Steer_Wheel_Input_04_For_Triggers_Drive_CS>();
                    break;

                case 10: // AI.
                    inputScript = gameObject.AddComponent<Steer_Wheel_Input_99_AI_CS>();
                    break;
            }
        }


        void Update()
        {
            if (isSelected || inputType == 10)
            { // The tank is selected, or AI.

                // Stop steering while the tank is stopping.
                if (driveControlScript.Stop_Flag)
                {
                    return;
                }

                inputScript.Get_Input();

                Steer();
            }
        }


        void Steer()
        {
            float targetAng = Max_Angle * Horizontal_Input;
            currentAng = Mathf.MoveTowardsAngle(currentAng, targetAng, Rotation_Speed * Time.deltaTime);
            jointSpring.targetPosition = currentAng * Reverse;
            thisHingeJoint.spring = jointSpring;
        }


        void Get_AI_CS()
        { // Called from "AI_CS".
            inputType = 10;
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            this.isSelected = isSelected;
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}