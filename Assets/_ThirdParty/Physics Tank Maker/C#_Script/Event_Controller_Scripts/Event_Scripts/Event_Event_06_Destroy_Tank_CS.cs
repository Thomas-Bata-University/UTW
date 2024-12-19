using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Event_Event_06_Destroy_Tank_CS : Event_Event_00_Base_CS
    {

        public override void Execute_Event()
        {
            for (int i = 0; i < eventControllerScript.Target_Tanks.Length; i++)
            {
                // Get the "Damage_Control_Center_CS" in the target, and send it infinite damage values.
                var damageScript = eventControllerScript.Target_Tanks[i].GetComponentInChildren<Damage_Control_Center_CS>();
                if (damageScript)
                {
                    damageScript.Receive_Damage(Mathf.Infinity, 0, 0);
                }
            }

            // End the event.
            // (Note.) This event can be repeatedly executed.
            if (eventControllerScript.Trigger_Script == null)
            { // All the triggers are pulled.
                Destroy(this.gameObject);
            }
        }

    }

}
