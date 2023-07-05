namespace ChobiAssets.PTM
{

    public class Event_Event_03_Change_AI_Settings_CS : Event_Event_00_Base_CS
    {

        public override void Execute_Event()
        {
            for (int i = 0; i < eventControllerScript.Target_Tanks.Length; i++)
            {
                if (eventControllerScript.Target_Tanks[i] == null)
                {
                    continue;
                }

                // Overwrite "AI_Settings_CS" in the target.
                var aiSettingsScript = eventControllerScript.Target_Tanks[i].GetComponentInChildren<AI_Settings_CS>();
                if (aiSettingsScript)
                { // The tank has been spawned.
                    aiSettingsScript.WayPoint_Pack = eventControllerScript.New_WayPoint_Pack;
                    aiSettingsScript.Patrol_Type = eventControllerScript.New_Patrol_Type;
                    aiSettingsScript.Follow_Target = eventControllerScript.New_Follow_Target;
                    aiSettingsScript.No_Attack = eventControllerScript.New_No_Attack;
                    aiSettingsScript.Breakthrough = eventControllerScript.New_Breakthrough;
                    aiSettingsScript.Visibility_Radius = eventControllerScript.New_Visibility_Radius;
                    aiSettingsScript.Approach_Distance = eventControllerScript.New_Approach_Distance;
                    aiSettingsScript.OpenFire_Distance = eventControllerScript.New_OpenFire_Distance;
                    aiSettingsScript.Lost_Count = eventControllerScript.New_Lost_Count;
                    aiSettingsScript.Face_Offest_Angle = eventControllerScript.New_Face_Offest_Angle;
                    aiSettingsScript.Dead_Angle = eventControllerScript.New_Dead_Angle;

                    // Call the "AI_Settings_CS" to reset the AI behavior.
                    aiSettingsScript.Changed_By_Event();

                    continue; // "Event_Controller_CS" in the target should have already been deleted.
                }

                // Overwrite "Event_Controller_CS" in the "Target_Tanks".
                var targetEventScript = eventControllerScript.Target_Tanks[i].GetComponentInChildren<Event_Controller_CS>();
                if (targetEventScript)
                { // The tank has not been spawned yet.
                    targetEventScript.Overwrite_Values(eventControllerScript);
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
