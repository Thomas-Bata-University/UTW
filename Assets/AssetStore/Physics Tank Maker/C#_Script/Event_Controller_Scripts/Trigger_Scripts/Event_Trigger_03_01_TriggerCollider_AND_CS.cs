using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Trigger_03_01_TriggerCollider_AND_CS : Event_Trigger_03_00_TriggerCollider_Base_CS
    {

        int detectedCount;


        public override void Detect_Collider(Transform detectedTransform)
        { // Called from "Trigger_Collider_CS".
            // Check the triggers.
            for (int i = 0; i < eventControllerScript.Trigger_Tanks.Length; i++)
            {
                if (eventControllerScript.Trigger_Tanks[i] == null)
                {
                    continue;
                }

                if (eventControllerScript.Trigger_Tanks[i] != detectedTransform)
                { // The "detectedTransform" is not the trigger.
                    continue;
                }

                // The "detectedTransform" is one of the trigger.
                eventControllerScript.Trigger_Tanks[i] = null; // Remove it from the triggers.
                detectedCount += 1;

                // Check the remaining trigger.
                if (eventControllerScript.All_Trigger_Flag)
                { // Requires that all the triggers are detected.
                    for (int j = 0; j < eventControllerScript.Trigger_Tanks.Length; j++)
                    {
                        if (eventControllerScript.Trigger_Tanks[j])
                        { // At least one of the trigger remains.
                            return;
                        }
                    } // All the triggers are detected or removed.

                    // Remove this reference in the "Event_Controller_CS".
                    eventControllerScript.Trigger_Script = null; // (Note.) The "Trigger_Script" must be set to null before starting the event.
                    Destroy(this);

                    // Start the event.
                    eventControllerScript.Start_Event();
                }
                else
                { // Requires that the specified number of triggers are detected.
                    if (detectedCount >= eventControllerScript.Necessary_Num)
                    { // The necessary number of triggers are detected.

                        // Remove this reference in the "Event_Controller_CS".
                        eventControllerScript.Trigger_Script = null; // (Note.) The "Trigger_Script" must be set to null before starting the event.
                        Destroy(this);

                        // Start the event.
                        eventControllerScript.Start_Event();
                    }
                }
            }
        }

    }

}