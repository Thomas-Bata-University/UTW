using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Event_02_Show_Message_CS : Event_Event_00_Base_CS
    {

        public override void Prepare_Event(Event_Controller_CS eventControllerScript)
        {
            // Store the reference to "Event_Controller_CS".
            this.eventControllerScript = eventControllerScript;

            if (eventControllerScript.Event_Text == null)
            {
                Debug.LogWarning("The event (" + this.name + ")  cannot be executed, because the 'Text' is not assigned.");
                Destroy(eventControllerScript);
                Destroy(this);
            }
        }


        public override void Execute_Event()
        {
            // Send message to "UI_Text_Control_CS".
            var textScript = eventControllerScript.Event_Text.GetComponent<UI_Text_Control_CS>();
            if (textScript == null)
            {
                textScript = eventControllerScript.Event_Text.gameObject.AddComponent<UI_Text_Control_CS>();
            }
            textScript.Receive_Text(eventControllerScript.Event_Message, eventControllerScript.Event_Message_Color, eventControllerScript.Event_Message_Time);

            // End the event.
            // (Note.) This event can be repeatedly executed.
            if (eventControllerScript.Trigger_Script == null)
            { // All the triggers are pulled.
                Destroy(this.gameObject);
            }
        }

    }

}
