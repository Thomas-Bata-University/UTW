using UnityEngine;

namespace ChobiAssets.PTM
{

    [DefaultExecutionOrder(-1)] // (Note.) This script is executed before other scripts, in order to overwrite the values.
    public class Special_Settings_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the top object of the tank.
		 * This script is used for changing the tank settings easily.
		*/

        // User options >>
        // These values might be overwritten by "Event_Event_01_Spawn_Tank_CS" script.
        public float Attack_Multiplier = 1.0f;
        public float Defence_Multiplier = 1.0f;
        // << User options


        void Start()
        {
            Send_Settings();
        }


        protected virtual void Send_Settings()
        {
            // Overwrite the "Attack_Multiplier" values in "Bullet_Generator_CS".
            var bulletGeneratorScripts = GetComponentsInChildren<Bullet_Generator_CS>();
            for (int i = 0; i < bulletGeneratorScripts.Length; i++)
            {
                bulletGeneratorScripts[i].Attack_Multiplier = Attack_Multiplier;
            }

            // Apply the "Defence_Multiplier" to the hit points in the "Damage_Control_Center_CS".
            var damageControlCenterScript = GetComponentInChildren<Damage_Control_Center_CS>();
            if (damageControlCenterScript)
            {
                damageControlCenterScript.MainBody_HP *= Defence_Multiplier;
                for (int i = 0; i < damageControlCenterScript.Turret_Props.Length; i++)
                {
                    damageControlCenterScript.Turret_Props[i].hitPoints *= Defence_Multiplier;
                }
            }
        }


        void Respawned()
        { // Called from "Respawn_Controller_CS", when the tank has been respawned.
            Send_Settings();
        }
    }

}
