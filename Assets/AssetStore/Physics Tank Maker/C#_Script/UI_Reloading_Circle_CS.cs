using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

    public class UI_Reloading_Circle_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Cannon_Base" in the tank.
		 * This script controls the loading circle displyaed while the bullet is reloaded.
		 * This script works in combination with "Cannon_Fire_CS" in the tank.
		*/


        // User options >>
        public string Reloading_Circle_Name = "Relocad_Circle";
        // << User options


        Cannon_Fire_CS cannonFireScript;
        Image reloadingCircle;
        Image[] reloadingCircleImages;
        bool imagesEnabled = true;

        bool isSelected;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            // Get the "Cannon_Fire_CS" script in the Cannon_Base.
            cannonFireScript = GetComponent<Cannon_Fire_CS>();
            if (cannonFireScript == null)
            {
                Debug.LogWarning("'Cannon_Fire_CS' script cannot be found.");
                Destroy(this);
                return;
            }

            // Get the bar Images.
            reloadingCircle = Find_Image(Reloading_Circle_Name);
            if (reloadingCircle == null)
            {
                Destroy(this);
                return;
            }

            // Get all the child images.
            reloadingCircleImages = reloadingCircle.GetComponentsInChildren<Image>();
        }


        Image Find_Image(string name)
        {
            // Check the name.
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            // Find the gameobject.
            GameObject barObject = GameObject.Find(name);
            if (barObject == null)
            {
                Debug.LogWarning("'" + name + "' is not found in the scene.");
                return null;
            }

            // Get the Image.
            Image tempImage = barObject.GetComponent<Image>();
            if (tempImage == null)
            {
                Debug.LogWarning("'" + name + "' does not have an Image in it.");
                return null;
            }

            return tempImage;
        }


        void LateUpdate()
        {
            if (isSelected == false)
            {
                return;
            }

            if (cannonFireScript.Is_Loaded)
            {
                Enable_Images(false);
                return;
            }
            else
            {
                Enable_Images(true);
                Control_Circle();
            }
        }


        void Enable_Images(bool enabled)
        {
            if (imagesEnabled != enabled)
            {
                imagesEnabled = enabled;

                // Enable or disable all the images.
                for (int i = 0; i < reloadingCircleImages.Length; i++)
                {
                    reloadingCircleImages[i].enabled = enabled;
                }
            }
        }


        void Control_Circle()
        {
            reloadingCircle.fillAmount = cannonFireScript.Loading_Count / cannonFireScript.Reload_Time;
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
                    Enable_Images(false);
                }
            }
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Turn off the image.
            if (isSelected)
            {
                Enable_Images(false);
            }

            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}