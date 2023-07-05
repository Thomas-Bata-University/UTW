namespace ChobiAssets.PTM
{

    public class Event_Event_04_Remove_Tank_CS : Event_Event_00_Base_CS
    {

        public override void Execute_Event()
        {
            for (int i = 0; i < eventControllerScript.Target_Tanks.Length; i++)
            {
                if (eventControllerScript.Target_Tanks[i] == null)
                {
                    continue;
                }

                // In case that the tank has not been spawned yet.
                var targetEventScript = eventControllerScript.Target_Tanks[i].GetComponentInChildren<Event_Controller_CS>();
                if (targetEventScript)
                { // The tank should not have been spawned yet.
                    Destroy(eventControllerScript.Target_Tanks[i].gameObject);
                }

                // In case that the tank has already been spawned.
                var respawnScript = eventControllerScript.Target_Tanks[i].GetComponentInChildren<Respawn_Controller_CS>();
                if (respawnScript)
                {
                    respawnScript.StartCoroutine("Remove_Tank", 0.0f);
                }
            }

            // End the event.
            Destroy(this.gameObject);
        }

    }

}
