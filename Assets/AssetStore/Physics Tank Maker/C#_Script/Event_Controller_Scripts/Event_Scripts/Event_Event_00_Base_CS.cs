using UnityEngine;

namespace ChobiAssets.PTM
{
	
	public class Event_Event_00_Base_CS : MonoBehaviour
	{

		protected Event_Controller_CS eventControllerScript;


		public virtual void Prepare_Event(Event_Controller_CS eventControllerScript)
		{
			// Store the reference to "Event_Controller_CS".
			this.eventControllerScript = eventControllerScript;
		}


		public virtual void Execute_Event()
		{
		}

	}

}
