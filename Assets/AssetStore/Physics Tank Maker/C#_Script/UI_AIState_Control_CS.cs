using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

    public class UI_AIState_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to a Text in the scene, and displays the state of the specified AI tank.
		 * This script works in combination with "AI_CS" in the AI tank in the scene.
		*/


        // User options >>
        public Color Patrol_Color = Color.white;
        public Color Attack_Color = Color.red;
        public Color Lost_Color = Color.magenta;
        public Color Dead_Color = Color.black;
        public Color Respawn_Color = Color.black;
        public string Patrol_Text = "Patrol";
        public string Attack_Text = "Attack";
        public string Lost_Text = "Lost";
        public string Dead_Text = "Dead";
        public string Respawn_Text = "Respawn";
        // << User options


        Text thisText;
        AI_CS aiScript;
        Respawn_Controller_CS respawnScript;
        Transform aiRootTransform;
        string textFormat;


        void Start()
        {
            thisText = GetComponent<Text>();
            thisText.text = "";
        }


        void LateUpdate()
        {
            if (aiRootTransform)
            {
                Control_Appearance();
            }
        }


        void Control_Appearance()
        {
            if (aiRootTransform.tag == "Finish")
            { // The AI tank has been destroyed.
                if (respawnScript && respawnScript.Respawn_Times > 0)
                {
                    thisText.text = string.Format(textFormat, Respawn_Text, "");
                    thisText.color = Respawn_Color;
                }
                else
                {
                    thisText.text = string.Format(textFormat, Dead_Text, "");
                    thisText.color = Dead_Color;
                }
                return;
            }

            switch (aiScript.Action_Type)
            {
                case 0: // Defensive.
                    thisText.text = string.Format(textFormat, Patrol_Text, "");
                    thisText.color = Patrol_Color;
                    break;

                case 1: // Offensive.
                    if (aiScript.Detect_Flag)
                    { // The AI tank has detected the target.
                        thisText.text = string.Format(textFormat, Attack_Text, "");
                        thisText.color = Attack_Color;
                    }
                    else
                    { // The AI tank has lost the target.
                        thisText.text = string.Format(textFormat, Lost_Text, Mathf.CeilToInt(aiScript.Losing_Count));
                        thisText.color = Lost_Color;
                    }
                    break;
            }
        }


        public void Get_AI_Script(AI_CS aiScript)
        { // Called form "AI_CS" in the AI tanks in the scene.
            this.aiScript = aiScript;
            aiRootTransform = aiScript.transform.root;
            if (string.IsNullOrEmpty(aiScript.Settings_Script.Tank_Name))
            {
                aiScript.Settings_Script.Tank_Name = aiRootTransform.name;
            }
            respawnScript = aiScript.GetComponentInParent<Respawn_Controller_CS>();
            textFormat = aiScript.Settings_Script.Tank_Name + " = " + "{0}" + " {1}";
        }

    }

}