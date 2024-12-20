using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Wheel_Resize_CS : MonoBehaviour
    {
        /*
		 * This script is attached to specified wheels in the tank, and controls the wheel scale at the opening.
		 * When the tank is spawned, this script changes the wheel scale to the specified size, and returns it gently.
		 * This script is used for avoiding breaking of Physics_Track.
		*/


        // User options >>
        public float ScaleDown_Size = 0.5f;
        public float Return_Speed = 0.05f;
        // << User options


        Transform thisTransform;
        float currentScale;


        void Start()
        {
            thisTransform = transform;
            currentScale = ScaleDown_Size;

            // Change the scale.
            thisTransform.localScale = Vector3.one * currentScale;
        }


        void Update()
        {
            currentScale = Mathf.MoveTowards(currentScale, 1.0f, Return_Speed);
            thisTransform.localScale = Vector3.one * currentScale;
            if (currentScale == 1.0f)
            {
                Destroy(this);
            }
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}