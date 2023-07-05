using System.Collections;
using UnityEngine;

namespace ChobiAssets.PTM
{
	
	public class Event_Event_01_Spawn_Tank_CS : Event_Event_00_Base_CS
	{

        bool isAnyTankRemoved;


        public override void Prepare_Event(Event_Controller_CS eventControllerScript)
        {
            // Store the reference to "Event_Controller_CS".
            this.eventControllerScript = eventControllerScript;

            // Check the "Prefab_Object".
            if (eventControllerScript.Prefab_Object == null)
            {
                Debug.LogWarning("The event (" + this.name + ") cannot be executed, because the 'Tank Prefab' is not assigned.");
                Destroy(eventControllerScript);
                Destroy(this);
            }
        }


        public override void Execute_Event()
        {
            // Check the tank around the spawning point.
            StartCoroutine("Check_Spawning_Point");
        }


        IEnumerator Check_Spawning_Point()
        {
            while (Detect_Tank() == true)
            { // There is any tank in the spawning point.
                // Wait a second.
                yield return new WaitForSeconds(1.0f);
                yield return null;
            }
            // There is no tank in the spawning point.

            if (isAnyTankRemoved)
            { // Any destroyed tank has been removed.
                // Wait for the destroyed tank to be completely removed.
                yield return null;
            }

            // Spawn the tank.
            Spawn_Tank();
        }


        bool Detect_Tank()
        {
            // Check the tank around the spawning point.
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f, Layer_Settings_CS.Detect_Body_Layer_Mask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform.root.tag != "Finish")
                { // There is a living tank in the area.
                    return true;
                }
            }
            // There is no living tank in the area.

            // Remove destroyed tanks in the area. 
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform.root.tag == "Finish")
                { // The tank is already dead.
                    var respawnScript = colliders[i].GetComponentInParent<Respawn_Controller_CS>();
                    if (respawnScript)
                    {
                        if (respawnScript.Respawn_Times > 0)
                        { // The "ReSpawn_Times" of the tank remains.
                            return true;
                        }
                        else
                        { // The "ReSpawn_Times" of the tank does not remain.
                            // Remove the tank.
                            respawnScript.StartCoroutine("Remove_Tank", 0.0f);
                            isAnyTankRemoved = true;
                        }
                    }
                }
            }
            return false;
        }


        void Spawn_Tank()
        {
            // Spawn the tank, and make it a child of the event GameObject.
            // (Note.) In this event system, the spawned tank must be placed under the event GameObject as its child in the hierarchy.
            GameObject newTank = Instantiate(eventControllerScript.Prefab_Object, transform.position, transform.rotation, eventControllerScript.transform) as GameObject;

            // Overwrite "ID_Settings_CS".
            var idScript = newTank.GetComponent<ID_Settings_CS>();
            if (idScript)
            {
                idScript.Tank_ID = eventControllerScript.Tank_ID;
                idScript.Relationship = eventControllerScript.Relationship;
            }

            // Overwrite "Respawn_Controller_CS".
            var respawnScript = newTank.GetComponent<Respawn_Controller_CS>();
            if (respawnScript)
            {
                respawnScript.This_Prefab = eventControllerScript.Prefab_Object; // (Note.) The current prefab reference is linked to the new tank spawned in the scene. So it must be overwritten.
                respawnScript.Respawn_Times = eventControllerScript.Respawn_Times;
                respawnScript.Auto_Respawn_Interval = eventControllerScript.Auto_Respawn_Interval;
                respawnScript.Remove_After_Death = eventControllerScript.Remove_After_Death;
                if (respawnScript.Remove_After_Death)
                {
                    respawnScript.Remove_Interval = eventControllerScript.Remove_Interval;
                }
                respawnScript.Respawn_Point_Pack = eventControllerScript.Respawn_Point_Pack;
                respawnScript.Respawn_Target = eventControllerScript.Respawn_Target;
            }

            // Overwrite "Special_Settings_CS".
            var specialSettingsScript = newTank.GetComponent<Special_Settings_CS>();
            if (specialSettingsScript)
            {
                specialSettingsScript.Attack_Multiplier = eventControllerScript.Attack_Multiplier;
                specialSettingsScript.Defence_Multiplier = eventControllerScript.Defence_Multiplier;
            }
            
            // Overwrite "AI_Settings_CS".
            var aiSettingsScript = newTank.GetComponent<AI_Settings_CS>();
            if (aiSettingsScript)
            {
                aiSettingsScript.WayPoint_Pack = eventControllerScript.WayPoint_Pack;
                aiSettingsScript.Patrol_Type = eventControllerScript.Patrol_Type;
                aiSettingsScript.Follow_Target = eventControllerScript.Follow_Target;
                aiSettingsScript.No_Attack = eventControllerScript.No_Attack;
                aiSettingsScript.Breakthrough = eventControllerScript.Breakthrough;
                aiSettingsScript.Commander = eventControllerScript.Commander;
                aiSettingsScript.Visibility_Radius = eventControllerScript.Visibility_Radius;
                aiSettingsScript.Approach_Distance = eventControllerScript.Approach_Distance;
                aiSettingsScript.OpenFire_Distance = eventControllerScript.OpenFire_Distance;
                aiSettingsScript.Lost_Count = eventControllerScript.Lost_Count;
                aiSettingsScript.Face_Offest_Angle = eventControllerScript.Face_Offest_Angle;
                aiSettingsScript.Dead_Angle = eventControllerScript.Dead_Angle;
                aiSettingsScript.Patrol_Speed_Rate = eventControllerScript.Patrol_Speed_Rate;
                aiSettingsScript.Combat_Speed_Rate = eventControllerScript.Combat_Speed_Rate;
                aiSettingsScript.AI_State_Text = eventControllerScript.AI_State_Text;
                aiSettingsScript.Tank_Name = eventControllerScript.Tank_Name;
            }

            // End the event.
            Destroy(eventControllerScript); // (Note.) Do not destroy the event GameObject. Because the spawned tank is placed under this object in the hierarchy.
            Destroy(this);
        }

    }

}
