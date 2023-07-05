using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Trigger_03_02_TriggerCollider_OR_CS : Event_Trigger_03_00_TriggerCollider_Base_CS
    {

        public override void Detect_Collider(Transform detectedTransform)
        { // Called from "Trigger_Collider_CS".
            // Check the triggers.
            for (int i = 0; i < eventControllerScript.Trigger_Tanks.Length; i++)
            {
                if (eventControllerScript.Trigger_Tanks[i] != detectedTransform)
                { // The "detectedTransform" is not the trigger.
                    continue;
                }

                // The "detectedTransform" is one of the trigger.
                if (eventControllerScript.Trigger_Itself_Flag == true)
                { // The trigger should be the target of the events.
                    switch (eventControllerScript.Event_Type)
                    {
                        case 2: // "Change AI Settings"
                        case 3: // "Remove Tank"
                        case 5: // "Destroy Tank"
                                // Make the trigger the target of the event.
                            eventControllerScript.Target_Tanks[0] = detectedTransform;

                            if (eventControllerScript.Wait_For_All_Triggers == true)
                            { // Keep this event until all the triggers touch the collider.
                              // Remove it from the triggers.
                                eventControllerScript.Trigger_Tanks[i] = null;
                                // Check the remaining triggers.
                                for (int j = 0; j < eventControllerScript.Trigger_Tanks.Length; j++)
                                {
                                    if (eventControllerScript.Trigger_Tanks[j])
                                    { // At least one of the triggers remains.
                                      // Start the event waiting other triggers
                                        eventControllerScript.Start_Event();
                                        return; // Keep checking the triggers.

                                    }
                                } // All the triggers are detected or removed.
                            } // No need to wait for other triggers.
                              // Start the event in the following lines.
                            break;
                    }
                }

                // Remove this reference in the "Event_Controller_CS".
                eventControllerScript.Trigger_Script = null; // (Note.) The "Trigger_Script" must be set to null before starting the event.
                Destroy(this);

                // Start the event.
                eventControllerScript.Start_Event();
                return;
            }
        }

    }

}