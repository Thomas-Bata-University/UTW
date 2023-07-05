using UnityEngine;
using UnityEngine.UI;


namespace ChobiAssets.PTM
{
	
	public class UI_Speed_Indicator_Control_CS : MonoBehaviour
	{
        /*
		 * This script is attached to the "Canvas_Speed_Indicator" in the scene.
		 * This script controls the texts for displaying the current selected tank speed.
		 * This script works in combination with "Drive_Control_CS" in the tank.
		*/


        // User options >>
        [SerializeField] GameObject Manual_Parent = default;
        [SerializeField] GameObject Automatic_Parent = default;
        [SerializeField] Image[] Icon_Images = default;
        [SerializeField] Color Enabled_Color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        [SerializeField] Color Disabled_Color = new Color(0.0f, 0.0f, 1.0f, 0.3f);
        [SerializeField] Text Speed_Text = default;
        [SerializeField] AudioSource This_AudioSource = default;
        [SerializeField] AudioClip Sound_Clip = default;
        // << User options


        const string textFormat = "{0} km/h";
        const int reverseStepCount = 2;

        Drive_Control_CS driveScript;
        bool isWorking;


        public static UI_Speed_Indicator_Control_CS Instance;


        void Awake()
        {
            Instance = this;
        }


        void LateUpdate()
        {
            if (isWorking == false)
            {
                return;
            }

            if (driveScript == null)
            { // The tank should be destroyed.
                isWorking = false;

                // Shift into neutral.
                Get_Current_Step(-reverseStepCount);

                // Change the speed text to zero.
                Update_Speed_Text(0);
                return;
            }

            // Update the speed text.
            Update_Speed_Text((int)(driveScript.Current_Velocity * 3.6f));
        }


        void Update_Speed_Text(int currentSpeed)
        {
            Speed_Text.text = string.Format(textFormat, currentSpeed);
        }


        public void Get_Current_Step(int currentStep)
        { // Called from "Drive_Control_CS".
            // Turn on / off the icons.
            for (int i = 0; i < Icon_Images.Length; i++)
            {
                if (i == currentStep + reverseStepCount)
                {
                    Icon_Images[i].color = Enabled_Color;
                }
                else
                {
                    Icon_Images[i].color = Disabled_Color;
                }
            }

            // Play the sound clip. 
            if (This_AudioSource && Sound_Clip)
            {
                This_AudioSource.PlayOneShot(Sound_Clip);
            }
        }


        public void Get_Drive_Script(Drive_Control_CS driveScript, bool isManual, int currentStep)
        { // Called from "Drive_Control_CS".
            isWorking = true;
            this.driveScript = driveScript;

            // Set the activation of the parents.
            Manual_Parent.SetActive(isManual);
            Automatic_Parent.SetActive(!isManual);

            // Set the current step.
            if (isManual)
            {
                Get_Current_Step(currentStep);
            }
        }


        
    }

}
