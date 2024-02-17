using UnityEngine;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

    public class Trigger_Collider_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Trigger_Collider" object used for the events in the scene.
		 * This script works in combination with "Event_Trigger_03_##_TriggerCollider_##_CS" scripts.
		*/


        // User options >>
        public bool Invisible_Flag = true;
        public int Store_Count = 16;
        // << User options


        List<Event_Trigger_03_00_TriggerCollider_Base_CS> triggerScriptsList = new List<Event_Trigger_03_00_TriggerCollider_Base_CS>();
        List<GameObject> detectedObjectsList = new List<GameObject>();


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            // Set the Layer.
            this.gameObject.layer = 2; // Ignore Raycast.

            // Make the colliders a trigger.
            var colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = true;
            }

            // Make it invisible.
            if (Invisible_Flag)
            {
                var renderer = GetComponent<Renderer>();
                if (renderer)
                {
                    renderer.enabled = false;
                }
            }
        }


        void OnTriggerEnter(Collider collider)
        {
            // Check the tank is living.
            var rootTransform = collider.transform.root;
            if (rootTransform.tag == "Finish")
            { // The tank should have been destroyed.
                return;
            }

            // Check the collider is a MainBody of a tank, and this is the first time to be detected. 
            var detectedObject = collider.gameObject;
            if (detectedObject.layer == Layer_Settings_CS.Body_Layer && Check_DetectedObjects(detectedObject))
            { // MainBody && The first time.

                // Call the "Event_Trigger_03_##_TriggerCollider_##_CS" scripts.
                for (int i = 0; i < triggerScriptsList.Count; i++)
                {
                    if (triggerScriptsList[i] == null)
                    {
                        triggerScriptsList.RemoveAt(i);
                        continue;
                    }
                    triggerScriptsList[i].Detect_Collider(rootTransform);
                }
            }
        }


        bool Check_DetectedObjects(GameObject detectedObject)
        {
            // Check the object had already been detected or not.
            var newObject = detectedObjectsList.Find(delegate (GameObject tempObject)
                {
                    return tempObject == detectedObject;
                });

            if (newObject == null)
            { // The "detectedObject" had not been detected yet.

                // Add the "detectedObject" to the list.
                detectedObjectsList.Add(detectedObject);
                if (detectedObjectsList.Count > Store_Count)
                { // The size of the list has been over.

                    // Remove the first element from the list.
                    detectedObjectsList.RemoveAt(0);
                }
                return true;
            }
            
            // The object had already been detected.
            return false;
        }


        public void Get_Trigger_Script(Event_Trigger_03_00_TriggerCollider_Base_CS triggerScript)
        { // Called from "Event_Trigger_03_##_TriggerCollider_##_CS".
            triggerScriptsList.Add(triggerScript);
        }

    }

}