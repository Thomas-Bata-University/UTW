using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Static_Wheel_Parent_CS : MonoBehaviour
    {
        /*
		 * This script controls the rotation speed of Static_Wheel.
		 * Static_Wheel can rotate synchronizing with the Static_Track or Scroll_Track in the tank by this script.
		 * This script works in combination with "Static_Track_Piece_CS" or "Track_Scroll_CS" in the tank, and "Static_Wheel_Parent_CS" in the child wheels.
		*/


        // User options >>
        public float Wheel_Radius;
        // << User optionsss

        // Referred to from "Static_Wheel_CS".
        public float Left_Angle_Y;
        public float Right_Angle_Y;
        public bool Is_Visible;

        float leftStaticTrackRate;
        float rightStaticTrackRate;
        float scrollTrackRateL;
        float scrollTrackRateR;
        Static_Track_Parent_CS staticTrackScript;
        Track_Scroll_CS leftScrollTrackScript;
        Track_Scroll_CS rightScrollTrackScript;
        MainBody_Setting_CS bodyScript;

        public bool isOwner = false;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            bodyScript = GetComponentInParent<MainBody_Setting_CS>();
        }


        public void Prepare_With_Static_Track(Static_Track_Parent_CS tempStaticTrackScript)
        { // Called from "Static_Track_Parent_CS".
            staticTrackScript = tempStaticTrackScript;

            // Set the rotation rate.
            leftStaticTrackRate = staticTrackScript.Reference_Radius_L / Wheel_Radius;
            rightStaticTrackRate = staticTrackScript.Reference_Radius_R / Wheel_Radius;
        }


        public void Prepare_With_Scroll_Track(Track_Scroll_CS tempScrollTrackScript)
        { // Called from "Track_Scroll_CS".
            if (tempScrollTrackScript.Is_Left)
            { // Left
                leftScrollTrackScript = tempScrollTrackScript;

                // Set the rotation rate.
                float referenceRadius = leftScrollTrackScript.Reference_Wheel.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
                scrollTrackRateL = referenceRadius / Wheel_Radius;
            }
            else
            { // Right
                rightScrollTrackScript = tempScrollTrackScript;

                // Set the rotation rate.
                float referenceRadius = rightScrollTrackScript.Reference_Wheel.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
                scrollTrackRateR = referenceRadius / Wheel_Radius;
            }
        }


        void Update()
        {
            // Check the tank is visible by any camera.
            Is_Visible = bodyScript.Visible_Flag;

            if (Is_Visible && isOwner)
            {
                if (staticTrackScript && staticTrackScript.isActiveAndEnabled)
                { // (Note.) Static_Track is enabled and disbaled by "Track_LOD_Control_CS" in the runtime.
                    Work_With_Static_Track();
                }
                else
                {
                    if (leftScrollTrackScript)
                    {
                        Work_With_Scroll_Track_Left();
                    }
                    if (rightScrollTrackScript)
                    {
                        Work_With_Scroll_Track_Right();
                    }
                }
            }
        }


        void Work_With_Static_Track()
        {
            Left_Angle_Y -= staticTrackScript.Delta_Ang_L * leftStaticTrackRate;
            Right_Angle_Y -= staticTrackScript.Delta_Ang_R * rightStaticTrackRate;
        }


        void Work_With_Scroll_Track_Left()
        {
            Left_Angle_Y -= leftScrollTrackScript.Delta_Ang * scrollTrackRateL;
        }


        void Work_With_Scroll_Track_Right()
        {
            Right_Angle_Y -= rightScrollTrackScript.Delta_Ang * scrollTrackRateR;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}