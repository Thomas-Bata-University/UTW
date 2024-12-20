using UnityEngine;

namespace ChobiAssets.PTM
{
	
	public class Event_Event_07_Show_Result_Canvas_CS : Event_Event_00_Base_CS
	{

        public override void Prepare_Event(Event_Controller_CS eventControllerScript)
        {
            // Store the reference to "Event_Controller_CS".
            this.eventControllerScript = eventControllerScript;

            // Check the "Result_Canvas".
            if (eventControllerScript.Result_Canvas == null)
            {
                Debug.LogWarning("The event (" + this.name + ") cannot be executed, because the 'Result Canvas' is not assigned.");
                Destroy(eventControllerScript);
                Destroy(this);
                return;
            }

            // Disable the result canvas.
            eventControllerScript.Result_Canvas.enabled = false;
        }


        public override void Execute_Event()
		{
            // Call the "Game_Controller_CS" in the scene to disallow the pause.
            if (Game_Controller_CS.Instance)
            {
                Game_Controller_CS.Instance.Allow_Pause = false;
            }

            // Enable the result canvas.
            eventControllerScript.Result_Canvas.enabled = true;

            // Show cursor.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // End the event.
            Destroy(this.gameObject);
        }

	}

}
