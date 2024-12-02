namespace ChobiAssets.PTM
{

    public class Event_Trigger_02_01_Destroy_AND_All_CS : Event_Trigger_02_00_Destroy_Base_CS
    {

        public override void Check_Trigger()
        {
            if (allTriggersAreReady == false)
            { // All the trigger tanks have not been spawned yet.
                Observe_Trigger_Tanks();
            }

            if (allTriggersAreReady)
            { // All the trigger tanks have been spawned.
                if (Check_Destroy_AND_All(eventControllerScript))
                { // All the trigger tanks have been destroyed.

                    // Remove this reference in the "Event_Controller_CS".
                    eventControllerScript.Trigger_Script = null; // (Note.) The "Trigger_Script" must be set to null before starting the event.
                    Destroy(this);

                    // Start the event.
                    eventControllerScript.Start_Event();
                }
            }
        }


        bool Check_Destroy_AND_All(Event_Controller_CS eventControllerScript)
        { // Check all the trigger tanks are destroyed.
            for (int i = 0; i < triggerTanksList.Count; i++)
            {
                if (triggerTanksList[i].Root_Transform && triggerTanksList[i].Root_Transform.tag != "Finish")
                { // At least one of the trigger tanks is alive.
                    return false;
                }
                else if (triggerTanksList[i].Respawn_Script && triggerTanksList[i].Respawn_Script.Respawn_Times > 0)
                { // The "Respawn_Times" remains.
                    return false;
                }
            }
            return true; // All the trigger tanks should have been destroyed or removed.
        }

    }

}