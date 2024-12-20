using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Stabilizer_CS : MonoBehaviour
    {
        /*
		 * This script is attached to all the drive wheels in the tank.
		 * This script fixes the looseness of the wheels.
		*/


        // User options >>
        public Transform This_Transform;
        public bool Is_Left;
        public float Initial_Pos_Y;
        public Vector3 Initial_Angles;
        // << User options


        Vector3 currentPosition;
        bool isRepairing;


        void Update()
        {
            // Stabilize the position.
            currentPosition = This_Transform.localPosition;
            currentPosition.y = Initial_Pos_Y;

            // Stabilize the angle.
            Initial_Angles.y = This_Transform.localEulerAngles.y;

            // Set the position and rotation.
            This_Transform.localPosition = currentPosition;
            This_Transform.localEulerAngles = Initial_Angles;
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(this);
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Disable this script.
            this.enabled = false;

            // Switch the flag.
            isRepairing = true;
        }


        void Track_Repaired_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Enable this script.
            this.enabled = true;

            // Switch the flag.
            isRepairing = false;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".

            // Check the track is being repaired.
            if (isRepairing)
            {
                return;
            }

            this.enabled = !isPaused;
        }

    }

}