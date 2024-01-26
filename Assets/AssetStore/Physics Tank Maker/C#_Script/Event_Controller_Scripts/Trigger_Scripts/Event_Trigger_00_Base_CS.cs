using UnityEngine;

namespace ChobiAssets.PTM
{
	
	public class Event_Trigger_00_Base_CS : MonoBehaviour
	{

		protected Event_Controller_CS eventControllerScript;


		public virtual void Prepare_Trigger(Event_Controller_CS eventControllerScript)
		{
			// Store the reference to "Event_Controller_CS".
			this.eventControllerScript = eventControllerScript;
		}


		public virtual void Check_Trigger()
		{
		}

	}

}
