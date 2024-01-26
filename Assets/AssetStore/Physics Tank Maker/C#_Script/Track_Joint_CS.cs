using UnityEngine;

namespace ChobiAssets.PTM
{
    public class Track_Joint_CS : MonoBehaviour
    {
        /*
		 * This script controls the position and rotation of the joint piece in the track.
		*/

        // User options >>
        public Transform Base_Transform;
        public Transform Front_Transform;
        public float Joint_Offset;
        public bool Is_Left;
        // << User options


        Transform thisTransform;
        MainBody_Setting_CS bodyScript;
        bool isRepairing;


        void Start()
        {
            thisTransform = transform;
            bodyScript = GetComponentInParent<MainBody_Setting_CS>();
        }


        void LateUpdate()
        {
            if (bodyScript.Visible_Flag)
            { // MainBody is visible by any camera.
                var basePos = Base_Transform.position + (Base_Transform.forward * Joint_Offset);
                var frontPos = Front_Transform.position - (Front_Transform.forward * Joint_Offset);
                thisTransform.SetPositionAndRotation(Vector3.Slerp(basePos, frontPos, 0.5f), Quaternion.Slerp(Base_Transform.rotation, Front_Transform.rotation, 0.5f));
            }
        }

        
        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Make this joint invisible.
            GetComponent<Renderer>().enabled = false;

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

            // Make this piece visible.
            GetComponent<Renderer>().enabled = true;

            // Enable this script.
            this.enabled = true;

            // Switch the flag.
            isRepairing = false;
        }


        void Duplicated()
        { // Called from "Static_Track_Piece_CS" (this).

            // Make this joint visible.
            GetComponent<Renderer>().enabled = true;

            // Destroy this script.
            Destroy(this);
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