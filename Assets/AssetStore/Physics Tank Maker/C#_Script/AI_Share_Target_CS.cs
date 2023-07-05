using UnityEngine;

namespace ChobiAssets.PTM
{
	
	public class AI_Share_Target_CS : MonoBehaviour
	{
        /*
		 * This script is automatically attached to "AI_Core" object by "AI_CS" script, only when the "Commander" is set in the "AI_Settings_CS" script.
		 * This script works in combination with "AI_CS" in this tank, and "Aiming_Control_CS" in the commander.
		 * When the commander locks on the target, this tank will track and attack the target.
         */


        public AI_CS AI_Script; // Set by "AI_CS".

        int thisRelationship;
        Aiming_Control_CS commanderAimingScript;


        void Start()
		{
			Initialize();
		}


        void Initialize()
        {
            // Get this relationship.
            var idScript = GetComponentInParent<ID_Settings_CS>();
            thisRelationship = idScript.Relationship;

            // Get "Aiming_Control_CS" in the commander.
            commanderAimingScript = AI_Script.Settings_Script.Commander.GetComponentInChildren<Aiming_Control_CS>();
            if (commanderAimingScript == null)
            {
                AI_Script.Is_Sharing_Target = false;
                Destroy(this);
            }
        }


        void Update()
        {
            // Check the commander exists.
            if (AI_Script.Settings_Script.Commander == null)
            { // The commander might have been removed from the scene.
                // Stop sharing the target.
                AI_Script.Is_Sharing_Target = false;
                Destroy(this);
                return;
            }

            // Check the commander is living.
            if (commanderAimingScript == null)
            { // The "Aiming_Control_CS" in the commander has been lost.
                if (AI_Script.Settings_Script.Commander.root.tag == "Finish")
                { // The commander has been destroyed.
                    // Stop sharing the target.
                    AI_Script.Is_Sharing_Target = false;
                    return;
                }
                else
                { // The commander has been respawned.
                    // Get "Aiming_Control_CS" again.
                    commanderAimingScript = AI_Script.Settings_Script.Commander.GetComponentInChildren<Aiming_Control_CS>();
                }
            }

            // Sharing Process
            if (AI_Script.Is_Sharing_Target)
            { // Sharing the target now.

                // Check the aiming mode of the commander.
                if (commanderAimingScript.Mode == 0)
                { // The aiming mode is "Keep the initial positon".
                    // Stop sharing the target.
                    AI_Script.Is_Sharing_Target = false;
                    AI_Script.Lost_Target();
                    return;
                }

                // Check the both tanks have the same target.
                if (commanderAimingScript.Target_Transform != null && commanderAimingScript.Target_Transform != AI_Script.Target_Transform)
                { // The commander is locking on different target.
                    // Check the target's relationship.
                    if (Check_Relationship() == true)
                    {
                        // Start sharing the new target.
                        Share_Target();
                    }
                    return;
                }

                // Check the target is living.
                if (AI_Script.Target_Transform == null || AI_Script.Target_Transform.root.tag == "Finish")
                { // The target has been removed or destroyed.
                    // Stop sharing the target.
                    AI_Script.Is_Sharing_Target = false;
                }
            }
            else
            { // Not sharing the target now.
                // Check the commander's target.
                if (commanderAimingScript.Target_Transform)
                { // The commander is locking on any target.
                    // Check the target's relationship.
                    if (Check_Relationship() == true)
                    {
                        // Start sharing the target.
                        Share_Target();
                    }
                }
            }
        }


        bool Check_Relationship()
        {
            var targetIDScript = commanderAimingScript.Target_Transform.GetComponentInParent<ID_Settings_CS>();
            if (targetIDScript)
            {
                return (targetIDScript.Relationship != thisRelationship);
            }
            else
            {
                return false;
            }
        }


        void Share_Target()
        {
            // Get "AI_Headquaters_Helper_CS" of the target.
            AI_Headquaters_Helper_CS targetAIHelperScript = commanderAimingScript.Target_Transform.GetComponentInParent<AI_Headquaters_Helper_CS>();
            if (targetAIHelperScript == null)
            { // The target does not have "AI_Headquaters_Helper_CS".
                return;
            }
            
            // Start sharing the target.
            AI_Script.Is_Sharing_Target = true;

            // Send the target information to the "AI_CS".
            AI_Script.Set_Target(targetAIHelperScript);
        }

	}

}