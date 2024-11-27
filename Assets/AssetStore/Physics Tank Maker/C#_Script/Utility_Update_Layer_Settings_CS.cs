using UnityEngine;


namespace ChobiAssets.PTM
{

    public class Utility_Update_Layer_Settings_CS : MonoBehaviour
    {

        [ContextMenu("Update_Layer_Settings")]


        void Update_Layer_Settings()
        {

            // MainBody.
            var bodyObject = GetComponentInChildren<Rigidbody>().gameObject;
            bodyObject.layer = Layer_Settings_CS.Body_Layer;


            // Road Wheels.
            var createRoadWheelScripts = GetComponentsInChildren<Create_RoadWheel_CS>();
            foreach (var createRoadWheelScript in createRoadWheelScripts)
            {
                var children = createRoadWheelScript.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child.GetComponent<Drive_Wheel_CS>())
                    { // The child is a wheel.
                        child.gameObject.layer = Layer_Settings_CS.Wheels_Layer;
                    }
                    else
                    { // The child is a suspension.
                        child.gameObject.layer = Layer_Settings_CS.Reinforce_Layer;
                    }
                }

            }


            // Road Wheels Type89.
            var createRoadWheelType89Scripts = GetComponentsInChildren<Create_RoadWheel_Type89_CS>();
            foreach (var createRoadWheelType89Script in createRoadWheelType89Scripts)
            {
                var children = createRoadWheelType89Script.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child.GetComponent<Drive_Wheel_CS>())
                    { // The child is a wheel.
                        child.gameObject.layer = Layer_Settings_CS.Wheels_Layer;
                    }
                    else
                    { // The child is a suspension.
                        child.gameObject.layer = Layer_Settings_CS.Reinforce_Layer;
                    }
                }

            }


            // Sprocket Wheels.
            var createSprocketWheelScripts = GetComponentsInChildren<Create_SprocketWheel_CS>();
            foreach (var createSprocketWheelScript in createSprocketWheelScripts)
            {
                var children = createSprocketWheelScript.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child.GetComponent<Drive_Wheel_CS>() || child.GetComponent<Static_Wheel_CS>())
                    { // The child is a wheel.
                        child.gameObject.layer = Layer_Settings_CS.Wheels_Layer;
                    } // The child is a tensioner arm. >> Default layer.
                }
            }


            // Idler Wheels.
            var createIdlerWheelScripts = GetComponentsInChildren<Create_IdlerWheel_CS>();
            foreach (var createIdlerWheelScript in createIdlerWheelScripts)
            {
                var children = createIdlerWheelScript.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child.GetComponent<Drive_Wheel_CS>() || child.GetComponent<Static_Wheel_CS>())
                    { // The child is a wheel.
                        child.gameObject.layer = Layer_Settings_CS.Wheels_Layer;
                    } // The child is a tensioner arm. >> Default layer.
                }
            }


            // Support Wheels.
            var createSupportWheelScripts = GetComponentsInChildren<Create_SupportWheel_CS>();
            foreach (var createSupportWheelScript in createSupportWheelScripts)
            {
                var children = createSupportWheelScript.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    // All the children are wheels.
                    child.gameObject.layer = Layer_Settings_CS.Wheels_Layer;
                }

            }


            // Swing Balls.
            var createSwingBallScripts = GetComponentsInChildren<Create_SwingBall_CS>();
            foreach (var createSwingBallScript in createSwingBallScripts)
            {
                var children = createSwingBallScript.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child.gameObject.layer == 2)
                    { // The child's layer is set to 2 (Ignore Raycast).
                        // No need to change the layer.
                        continue;
                    } // The child's layer should be set to the old Reinforce layer.
                    child.gameObject.layer = Layer_Settings_CS.Reinforce_Layer;
                }
            }


            // Reinforce objects in Physics Track Pieces.
            var createTrackBeltScripts = GetComponentsInChildren<Create_TrackBelt_CS>();
            foreach (var createTrackBeltScript in createTrackBeltScripts)
            {
                // (Note.)The reinforce object must have a SphereCollider.
                var sphereColliders = createTrackBeltScript.GetComponentsInChildren<SphereCollider>();
                foreach (var sphereCollider in sphereColliders)
                {
                    sphereCollider.gameObject.layer = Layer_Settings_CS.Reinforce_Layer;
                }
            }


            /* (Note.)
             *  The layers of "Armor_Collider", "Extra_Collider" and Bullets are set automatically at the start by attached scripts.
            */

        }
    }

}