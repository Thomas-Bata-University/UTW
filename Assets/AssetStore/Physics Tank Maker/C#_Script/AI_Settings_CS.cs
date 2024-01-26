using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

    public class AI_Settings_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the top object of the AI tank.
		 * This script works in combination with "AI_CS" in the tank.
		 * You can change the highly-used variables for AI from this script.
		*/

        // User options >>
        // AI patrol settings.
        public GameObject WayPoint_Pack;
        public int Patrol_Type = 0; // 0 = Order, 1 = Random.
        public Transform Follow_Target;
        // AI combat settings.
        public bool No_Attack = false;
        public bool Breakthrough = false;
        public Transform Commander;
        public float Visibility_Radius = 512.0f;
        public float Approach_Distance = 256.0f;
        public float OpenFire_Distance = 512.0f;
        public float Lost_Count = 20.0f;
        public float Face_Offest_Angle = 0.0f;
        public float Dead_Angle = 30.0f;
        // AI speed settings.
        public float Patrol_Speed_Rate = 1.0f;
        public float Combat_Speed_Rate = 1.0f;
        // AI text settings.
        public Text AI_State_Text;
        public string Tank_Name;
        // << User options


        public void Changed_By_Event()
        { // Called from "Event_Event_03_Change_AI_Settings_CS" script, when the event occurs.
            
            // Call "AI_CS" in the tank to reset the settings.
            AI_CS aiScript = GetComponentInChildren<AI_CS>();
            if (aiScript)
            {
                aiScript.Reset_Settings();
            }
        }

    }

}