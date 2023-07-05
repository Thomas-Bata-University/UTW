using UnityEngine;
using UnityEngine.UI;


namespace ChobiAssets.PTM
{
	
	[DefaultExecutionOrder (+2)] // (Note.) This script is executed after the "Aiming_Control_CS", in order to move the marker smoothly.
    public class UI_Aim_Marker_Control_CS : MonoBehaviour
	{
        /*
		 * This script is attached to the "MainBody" of the tank.
		 * This script controls the 'Aim_Marker' in the scene.
         * This script works in combination with "Aiming_Control_CS" in the tank.
		*/

        // User options >>
        public string Aim_Marker_Name = "Aim_Marker";
		// << User options


		Aiming_Control_CS aimingScript;
		Image markerImage;
		Transform markerTransform;

		bool isSelected;


		void Start()
        {
            // Get the marker image in the scene.
            if (string.IsNullOrEmpty(Aim_Marker_Name))
            {
                return;
            }
            GameObject markerObject = GameObject.Find(Aim_Marker_Name);
            if (markerObject)
            {
                markerImage = markerObject.GetComponent<Image>();
            }
            else
            {
                // The marker cannot be found in the scene.
                Debug.LogWarning(Aim_Marker_Name + " cannot be found in the scene.");
                Destroy(this);
                return;
            }
            markerTransform = markerImage.transform;

            // Get the "Aiming_Control_CS" in the tank.
            aimingScript = GetComponent<Aiming_Control_CS>();
            if (aimingScript == null)
            {
                Debug.LogWarning("'Aiming_Control_CS' cannot be found in the MainBody.");
                Destroy(this);
            }
        }


        void Update()
        {
            if (isSelected == false)
            {
                return;
            }

            Marker_Control();
        }


        void Marker_Control()
        {
            // Set the appearance.
            switch (aimingScript.Mode)
            {
                case 0: // Keep the initial positon.
                    markerImage.enabled = false;
                    return;

                case 1: // Free aiming.
                case 2: // Locking on.
                    markerImage.enabled = true;
                    if (aimingScript.Target_Transform)
                    {
                        markerImage.color = Color.red;
                    }
                    else
                    {
                        markerImage.color = Color.white;
                    }
                    break;
            }

            // Set the position.
            // Check the player is finding a target using the gun camera now.
            if (aimingScript.reticleAimingFlag)
            {
                // Set the marker at the center of the screen.
                markerTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 128.0f);
                return;
            }
            // Set the marker on the target position.
            Vector3 currentPosition = Camera.main.WorldToScreenPoint(aimingScript.Target_Position);
            if (currentPosition.z < 0.0f)
            { // Behind of the camera.
                markerImage.enabled = false;
            }
            else
            {
                currentPosition.z = 128.0f;
            }
            markerTransform.position = currentPosition;
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            if (isSelected)
            {
                this.isSelected = true;
            }
            else
            {
                if (this.isSelected)
                { // This tank is selected until now.
                    this.isSelected = false;
                    markerImage.enabled = false;
                }
            }
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Turn off the marker.
            if (isSelected)
            {
                markerImage.enabled = false;
            }

            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}
