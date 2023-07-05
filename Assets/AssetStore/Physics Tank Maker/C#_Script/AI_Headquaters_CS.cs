using System.Collections.Generic;
using UnityEngine;

namespace ChobiAssets.PTM
{

	public class AI_Headquaters_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "Game_Controller" in the scene.
		 * This script works in combination with "AI_Headquaters_Helper_CS" and "AI_CS" in the tank.
		 * This script gives an oder that assign the target tank to "AI_CS" in the tank.
		*/

		// User options >>
		public float Order_Interval = 3.0f;
        // << User options


        // Referred to from "Aiming_Control_CS".
        public List <AI_Headquaters_Helper_CS> Friendly_Tanks_List = new List <AI_Headquaters_Helper_CS>();
		public List <AI_Headquaters_Helper_CS> Hostile_Tanks_List = new List <AI_Headquaters_Helper_CS >();

        float orderCount;


        public static AI_Headquaters_CS Instance;


        void Awake()
        {
            Instance = this;
        }


        public void Receive_Helpers(AI_Headquaters_Helper_CS helperScript)
        { // Called from "AI_Headquaters_Helper_CS", when the tank is spawend.
            // Add the script into the lists according to the relationship.
            switch (helperScript.ID_Script.Relationship)
            {
                case 0:
                    Friendly_Tanks_List.Add(helperScript);
                    break;

                case 1:
                    Hostile_Tanks_List.Add(helperScript);
                    break;
            }
        }


        void Update()
        {
            Count_Order_Interval();
        }


        void Count_Order_Interval()
        {
            orderCount += Time.deltaTime;
            if (orderCount > Order_Interval)
            {
                orderCount = 0.0f;
                Give_Order();
            }
        }


        public void Give_Order()
        { // This function is called also from "AI_CS" in the tanks, when its target has been destroyed.
            // Assign the target to all the AI tanks in the scene.
            Assign_Target(Friendly_Tanks_List, Hostile_Tanks_List);
            Assign_Target(Hostile_Tanks_List, Friendly_Tanks_List);
        }


        void Assign_Target(List<AI_Headquaters_Helper_CS> teamA, List<AI_Headquaters_Helper_CS> teamB)
        {
            // Assign the target to all the AI tanks in the scene.
            for (int i = 0; i < teamA.Count; i++)
            {
                var shortestDistance = Mathf.Infinity;
                var targetIsFound = false;
                var tempTargetIndex = 0;
                var aiScript = teamA[i].AI_Script;
                if (aiScript == null || aiScript.Is_Sharing_Target || aiScript.Settings_Script.No_Attack || aiScript.Detect_Flag)
                { // The tank is not AI, or is sharing the target, or is set not to attack, or is detecting the current target.
                    continue;
                }
                for (int j = 0; j < teamB.Count; j++)
                {
                    if (teamB[j].Body_Transform.root.tag == "Finish")
                    { // The target is alredy destroyed.
                        continue;
                    } // The target is living.

                    // Find the closest enemy tank.
                    var distance = Vector3.Distance(teamA[i].Body_Transform.position, teamB[j].Body_Transform.position);
                    if ((aiScript.Wakeful_Flag == false && distance > aiScript.Settings_Script.Visibility_Radius) || distance > shortestDistance)
                    { // The target is out of the "Visibility_Radius", or is not the closest enemy.
                        continue;
                    } // The target is within the "Visibility_Radius", and is the closest enemy.

                    // Check the AI can detect the target.
                    if (aiScript.Check_for_Assigning(teamB[j]) == true)
                    { // The target can be detected by the AI.
                        // Store the distance and the index.
                        shortestDistance = distance;
                        targetIsFound = true;
                        tempTargetIndex = j;
                    } // The target can not be detected by the AI.
                }

                if (targetIsFound)
                {
                    // Send the new target to "AI_CS".
                    aiScript.Set_Target(teamB[tempTargetIndex]);
                }
            }
        }


        public void Remove_From_List(AI_Headquaters_Helper_CS helperScript)
        { // Called from "AI_Headquaters_Helper_CS", just before the tank is removed.
            switch (helperScript.ID_Script.Relationship)
            {
                case 0:
                    Friendly_Tanks_List.Remove(helperScript);
                    break;

                case 1:
                    Hostile_Tanks_List.Remove(helperScript);
                    break;
            }
        }

    }

}
