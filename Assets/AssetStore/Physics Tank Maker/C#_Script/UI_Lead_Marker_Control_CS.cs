using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	[DefaultExecutionOrder (+3)] // (Note.) This script is executed after the main camera is moved, in order to move the marker smoothly.
    public class UI_Lead_Marker_Control_CS : MonoBehaviour
	{
        /*
		 * This script is attached to the "MainBody" of the tank with "Aiming_Control_CS".
		 * This script controls the Lead Marker in the scene.
		*/

        // User options >>
        public string Lead_Marker_Name = "Lead_Marker";
        public Sprite Right_Sprite;
        public Sprite Wrong_Sprite;
        public float Calculation_Time = 2.0f;
        public Bullet_Generator_CS Bullet_Generator_Script;
        // << User options


        Aiming_Control_CS aimingScript;
		Image markerImage;
		Transform markerTransform;
        Transform bulletGeneratorTransform;

		bool isSelected;


        void Start()
        {
            // Get the marker image in the scene.
            if (string.IsNullOrEmpty(Lead_Marker_Name))
            {
                return;
            }
            GameObject markerObject = GameObject.Find(Lead_Marker_Name);
            if (markerObject)
            {
                markerImage = markerObject.GetComponent<Image>();
            }
            else
            {
                // The marker cannot be found in the scene.
                Debug.LogWarning(Lead_Marker_Name + " cannot be found in the scene.");
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

            // Get the "Bullet_Generator_CS".
            if (Bullet_Generator_Script == null)
            {
                Bullet_Generator_Script = GetComponentInChildren<Bullet_Generator_CS>();
            }
            if (Bullet_Generator_Script == null)
            {
                Debug.LogWarning("'Bullet_Generator_CS' cannot be found. The cannon cannot get the bullet velocity.");
                Destroy(this);
                return;
            }
            bulletGeneratorTransform = Bullet_Generator_Script.transform;
        }


        void LateUpdate()
        {
            if (isSelected == false)
            {
                return;
            }

            Marker_Control();
        }


        void Marker_Control()
        {
            // Check the aiming mode.
            switch (aimingScript.Mode)
            {
                case 0: // Keep the initial positon.
                    markerImage.enabled = false;
                    return;
            }

            // Check the target is locked on now.
            if (aimingScript.Target_Transform == null)
            { // The target is not locked on.
                markerImage.enabled = false;
                return;
            }

            // Calculate the ballistic.
            var muzzlePos = bulletGeneratorTransform.position;
            var targetDir = aimingScript.Target_Position - muzzlePos;
            var targetBase = Vector2.Distance(Vector2.zero, new Vector2(targetDir.x, targetDir.z));
            var bulletVelocity = bulletGeneratorTransform.forward * Bullet_Generator_Script.Current_Bullet_Velocity;
            if (aimingScript.Target_Rigidbody)
            { // The target has a rigidbody.
                // Reduce the target's velocity to help the lead-shooting.
                bulletVelocity -= aimingScript.Target_Rigidbody.velocity;
            }
            var isHit = false;
            var isTank = false;
            var previousPos = muzzlePos;
            var currentPos = previousPos;
            var count = 0.0f;
            while (count < Calculation_Time)
            {
                // Get the current position.
                var virtualPos = bulletVelocity * count;
                virtualPos.y -= 0.5f * -Physics.gravity.y * Mathf.Pow(count, 2.0f);
                currentPos = virtualPos + muzzlePos;

                // Get the hit point by casting a ray.
                if (Physics.Linecast(previousPos, currentPos, out RaycastHit raycastHit, Layer_Settings_CS.Aiming_Layer_Mask))
                {
                    currentPos = raycastHit.point;
                    isHit = true;
                    if (raycastHit.rigidbody && raycastHit.transform.root.tag != "Finish")
                    { // The target has a rigidbody, and it is living.
                        isTank = true;
                    }
                    break;
                }

                // Check the ray has exceeded the target.
                var currenBase = Vector2.Distance(Vector2.zero, new Vector2(virtualPos.x, virtualPos.z));
                if (currenBase > targetBase)
                {
                    break;
                }

                previousPos = currentPos;
                count += Time.fixedDeltaTime;
            }

            // Convert the hit point to the screen point.
            var screenPos = Camera.main.WorldToScreenPoint(currentPos);
            if (screenPos.z < 0.0f)
            { // The hit point is behind the camera.
                markerImage.enabled = false;
                return;
            }

            // Set the position.
            markerImage.enabled = true;
            screenPos.z = 128.0f;
            markerTransform.position = screenPos;

            // Set the appearance.
            if (isHit)
            { // The bullet will hit something.
                if (isTank)
                { // The hit object has a rigidbody.
                    //markerImage.color = Color.red;
                    markerImage.sprite = Right_Sprite;
                }
                else
                { // The hit object has no rigidbody.
                    //markerImage.color = Color.white;
                    markerImage.sprite = Wrong_Sprite;
                }
            }
            else
            { // The bullet will not hit anything.
                //markerImage.color = Color.gray;
                markerImage.sprite = Wrong_Sprite;
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
