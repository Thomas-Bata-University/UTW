using UnityEngine;

namespace ChobiAssets.PTM
{

    [DefaultExecutionOrder(+1)] // (Note.) This script must be executed after the "Static_Track_Piece_CS" executed the initializing.
    public class Track_LOD_Control_CS : MonoBehaviour
    {
        /*
		 * This script controls the texture scrolling of Scroll_Track.
		*/


        // User options >>
        public GameObject Static_Track;
        public GameObject Scroll_Track_L;
        public GameObject Scroll_Track_R;
        public float Threshold = 15.0f;
        // << User options


        Transform thisTransform;
        MainBody_Setting_CS bodyScript;
        float frustumHeight;
        bool tankIsNear;
        bool isRepairing;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisTransform = transform;
            bodyScript = GetComponent<MainBody_Setting_CS>();

            // Check the tracks.
            if (Static_Track == null || Scroll_Track_L == null || Scroll_Track_R == null)
            {
                Debug.LogWarning("Track LOD system cannot work, because the tracks are not assigned.");
                Destroy(this);
                return;
            }

            // Set the tracks activations at the first time.
            frustumHeight = 2.0f * Vector3.Distance(thisTransform.position, Camera.main.transform.position) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            tankIsNear = (frustumHeight < Threshold);
            if (tankIsNear)
            {
                Static_Track.SetActive(true);
                Scroll_Track_L.SetActive(false);
                Scroll_Track_R.SetActive(false);
            }
            else
            {
                Static_Track.SetActive(false);
                Scroll_Track_L.SetActive(true);
                Scroll_Track_R.SetActive(true);
            }
        }


        void Update()
        {
            // Check the tank is visible by any camera.
            if (bodyScript.Visible_Flag)
            {
                Tracks_LOD();
            }
        }


        void Tracks_LOD()
        {
            var mainCamera = Camera.main;
            frustumHeight = 2.0f * Vector3.Distance(thisTransform.position, mainCamera.transform.position) * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            if (tankIsNear)
            {
                if (frustumHeight > Threshold)
                { // The tank has been far from the camera.
                    tankIsNear = false;
                    Static_Track.SetActive(false);
                    Scroll_Track_L.SetActive(true);
                    Scroll_Track_R.SetActive(true);
                }
            }
            else
            {
                if (frustumHeight < Threshold)
                { // The tank has been near the camera.
                    tankIsNear = true;
                    Static_Track.SetActive(true);
                    Scroll_Track_L.SetActive(false);
                    Scroll_Track_R.SetActive(false);
                }
            }
        }


        void Track_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Switch to Static_Track.
            Static_Track.SetActive(true);
            Scroll_Track_L.SetActive(false);
            Scroll_Track_R.SetActive(false);

            // Disable this script.
            this.enabled = false;

            // Switch the flag.
            isRepairing = true;
        }


        void Track_Repaired_Linkage()
        { // Called from "Damage_Control_Center_CS".

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
