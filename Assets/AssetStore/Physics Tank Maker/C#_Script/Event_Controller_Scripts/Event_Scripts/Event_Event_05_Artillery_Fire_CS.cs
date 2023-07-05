using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Event_05_Artillery_Fire_CS : Event_Event_00_Base_CS
    {

        public override void Prepare_Event(Event_Controller_CS eventControllerScript)
        {
            // Store the reference to "Event_Controller_CS".
            this.eventControllerScript = eventControllerScript;

            if (eventControllerScript.Artillery_Script == null || eventControllerScript.Artillery_Target == null)
            {
                Debug.LogWarning("The event (" + this.name + ")  cannot be executed, because the 'Artillery_Script' and 'Artillery_Target' are not assigned.");
                Destroy(eventControllerScript);
                Destroy(this);
            }
        }


        public override void Execute_Event()
        {
            if (eventControllerScript.Artillery_Script)
            {
                eventControllerScript.Artillery_Script.Fire(eventControllerScript.Artillery_Target, eventControllerScript.Artillery_Num);
            }

            // End the event.
            Destroy(this.gameObject);
        }

    }

}
