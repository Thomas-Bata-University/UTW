using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Trigger_03_03_TriggerCollider_Any_CS : Event_Trigger_03_00_TriggerCollider_Base_CS
    {

        public override void Detect_Collider(Transform detectedTransform)
        { // Called from "Trigger_Collider_CS".
            // Find "ID_Settings_CS" script in the "detectedTransform".
            ID_Settings_CS idScript = detectedTransform.GetComponentInChildren<ID_Settings_CS>();
            if (idScript == null)
            {
                return;
            }

            // Check the trigger.
            switch (eventControllerScript.Trigger_Setting_Type)
            {
                case 1: // "Any hostile tank"
                    if (idScript.Relationship == 1)
                    { // Hostile
                        break;
                    }
                    else
                    {
                        return;
                    }

                case 2: // "Any friendly tank"
                    if (idScript.Relationship == 0)
                    { // Friendly
                        break;
                    }
                    else
                    {
                        return;
                    }

                case 3: // "Any tank"
                    break;

                default:
                    return;
            }

            // The "detectedTransform" is the trigger.
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
                          // Start the event waiting other triggers
                            eventControllerScript.Start_Event();
                            return; // Keep checking the triggers.

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
        }

    }

}