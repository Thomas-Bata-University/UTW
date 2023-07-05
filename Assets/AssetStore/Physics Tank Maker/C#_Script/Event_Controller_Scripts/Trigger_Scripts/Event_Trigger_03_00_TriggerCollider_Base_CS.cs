using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Trigger_03_00_TriggerCollider_Base_CS : Event_Trigger_00_Base_CS
    {

        public override void Prepare_Trigger(Event_Controller_CS eventControllerScript)
        {
            // Store the reference to "Event_Controller_CS".
            this.eventControllerScript = eventControllerScript;

            // Send this reference to all the "Trigger_Collider_CS" in the "Trigger_Collider_Scripts".
            for (int i = 0; i < eventControllerScript.Trigger_Collider_Scripts.Length; i++)
            {
                if (eventControllerScript.Trigger_Collider_Scripts[i])
                {
                    eventControllerScript.Trigger_Collider_Scripts[i].Get_Trigger_Script(this);
                }
            }
        }


        public virtual void Detect_Collider(Transform detectedTransform)
        { // Called from "Trigger_Collider_CS".
        }

    }

}