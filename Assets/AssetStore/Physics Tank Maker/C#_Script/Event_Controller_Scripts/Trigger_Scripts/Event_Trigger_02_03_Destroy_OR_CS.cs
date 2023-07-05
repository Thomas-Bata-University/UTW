namespace ChobiAssets.PTM
{

    public class Event_Trigger_02_03_Destroy_OR_CS : Event_Trigger_02_00_Destroy_Base_CS
    {

        public override void Check_Trigger()
        {
            if (allTriggersAreReady == false)
            { // All the trigger tanks have not been spawned yet.
                Observe_Trigger_Tanks();
            }

            if (spawnedTankCount > 0)
            { // At least one of the trigger tanks has been spawned. 
                if (Check_Destroy_OR(eventControllerScript))
                { // At least one of the trigger tanks has been destroyed.

                    // Remove this reference in the "Event_Controller_CS".
                    eventControllerScript.Trigger_Script = null; // (Note.) The "Trigger_Script" must be set to null before starting the event.
                    Destroy(this);

                    // Start the event.
                    eventControllerScript.Start_Event();
                }
            }
        }


        bool Check_Destroy_OR(Event_Controller_CS eventControllerScript)
        {
            for (int i = 0; i < triggerTanksList.Count; i++)
            {
                if (triggerTanksList[i].Root_Transform == null)
                { // The tank has already been removed from the scene.
                    return true;
                }
                if (triggerTanksList[i].Root_Transform.tag == "Finish")
                { // The tank has been destroyed.
                    if (triggerTanksList[i].Respawn_Script && triggerTanksList[i].Respawn_Script.Respawn_Times <= 0)
                    { // The "Respawn_Times" does not remain.
                        return true;
                    }
                }
            }
            return false; // There is no destroyed tank in the trigger tanks.
        }

    }

}