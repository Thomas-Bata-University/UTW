using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
    public class ReticleWheel_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Gun_Camera" under the "Barrel_Base" in the tank.
		 * This script controls the reticle wheel image displayed in the gun camera.
		 * This script works in combination with "Aiming_Control_CS" in the MainBody.
		*/

        // User options >>
        public string ReticleWheel_Name = "Reticle_Wheel";
        public Camera Gun_Camera;
        public float Speed = 1000.0f;
        public float Max_Distance = 4000.0f;
        public float Multiplier = 2.0f;
        // << User options

        Image reticleWheelImage;
        Transform reticleWheelTransform;
        Aiming_Control_CS aimingScript;
        float currentDistance;
        Transform thisTransform;

        bool isSelected;


        void Start()
        {
            thisTransform = transform;

            // Get the reticle wheel image in the scene.
            if (string.IsNullOrEmpty(ReticleWheel_Name))
            {
                return;
            }
            GameObject reticleWheelObject = GameObject.Find(ReticleWheel_Name);
            if (reticleWheelObject)
            {
                reticleWheelImage = reticleWheelObject.GetComponent<Image>();
            }
            if (reticleWheelImage == null)
            {
                Debug.LogWarning(ReticleWheel_Name + " cannot be found in the scene.");
                Destroy(this);
                return;
            }
            reticleWheelTransform = reticleWheelImage.transform;

            // Get the gun camera in the tank.
            if (Gun_Camera == null)
            {
                Gun_Camera = GetComponent<Camera>();
            }

            // Get "Aiming_Control_CS" in the tank.
            aimingScript = GetComponentInParent<Aiming_Control_CS>();
        }


        void Update()
        {
            if (isSelected == false)
            {
                return;
            }

            // Set the appearance.
            reticleWheelImage.enabled = Gun_Camera.enabled;

            // Set the rotation.
            if (reticleWheelImage.enabled)
            {
                Rotation_Controll();
            }
        }


        void Rotation_Controll()
        {
            // Get the distance to the target.
            var targetDistance = Vector3.Distance(thisTransform.position, aimingScript.Target_Position) * Multiplier;
            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Speed * Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(targetDistance - currentDistance) / 500.0f) * Time.deltaTime);

            // Set the angles.
            var currentLocalAngles = reticleWheelTransform.localEulerAngles;
            currentLocalAngles.z = (currentDistance / Max_Distance) * 180.0f;
            reticleWheelTransform.localEulerAngles = currentLocalAngles;
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
                    reticleWheelImage.enabled = false;
                }
            }
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            // Disable the image.
            if (isSelected)
            {
                reticleWheelImage.enabled = false;
            }

            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}
