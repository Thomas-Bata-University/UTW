using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]

	public class Reticle_Control_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "Gun_Camera" under the "Barrel_Base" in the tank.
		 * This script controls the reticle image displayed in the gun camera.
		*/

		// User options >>
		public string Reticle_Name = "Reticle";
		public Gun_Camera_CS Gun_Camera_Script;
        // << User options

        Image reticleImage;

        bool isSelected;


        void Start()
        {
            // Get the reticle image in the scene.
            if (string.IsNullOrEmpty(Reticle_Name))
            {
                return;
            }
            GameObject reticleObject = GameObject.Find(Reticle_Name);
            if (reticleObject)
            {
                reticleImage = reticleObject.GetComponent<Image>();
            }
            if (reticleImage == null)
            {
                Debug.LogWarning(Reticle_Name + " cannot be found in the scene.");
                Destroy(this);
            }

            // Get the "Gun_Camera_CS" script in the tank.
            if (Gun_Camera_Script == null)
            {
                Gun_Camera_Script = GetComponent<Gun_Camera_CS>();
            }
        }


        void Update()
        {
            if (isSelected == false)
            {
                return;
            }

            // Set the appearance.
            reticleImage.enabled = Gun_Camera_Script.Gun_Camera.enabled;

            // Set the scale.
            if (reticleImage.enabled)
            {
                // Change the scale according to the FOV.
                var currentScale = Gun_Camera_Script.Maximum_FOV / Gun_Camera_Script.Gun_Camera.fieldOfView;
                reticleImage.rectTransform.localScale = Vector3.one * currentScale;
            }
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
                    reticleImage.enabled = false;
                }
            }
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Turn off the image.
            if (isSelected)
            {
                reticleImage.enabled = false;
            }

            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}

