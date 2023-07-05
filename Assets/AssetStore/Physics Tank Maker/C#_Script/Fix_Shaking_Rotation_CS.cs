using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Fix_Shaking_Rotation_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the drive wheels only when the tank has Physics_Tracks.
		 * This script fixes the shaking rotation issue of the wheels.
		*/

        // User options >>
        public bool Is_Left;
        public Transform This_Transform;
        // << User options


        Quaternion storedRot;
        bool isFixing = false;
        Drive_Control_CS controlScript;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            controlScript = GetComponentInParent<Drive_Control_CS>();
        }


        void Update()
        {
            // Fix the useless rotation produced by physics engine.
            if (controlScript.Parking_Brake == true)
            { // The parking brake is working.

                if (isFixing == false)
                { // The wheel is not fixed.

                    // Store the rotaion.
                    storedRot = This_Transform.localRotation;
                    isFixing = true;
                }
            }
            else
            { // The parking brake is not working.
                isFixing = false;
                return;
            }

            // Fix the rotation.
            if (isFixing)
            {
                This_Transform.localRotation = storedRot;
            }
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            // Remove this script.
            Destroy(this);
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}