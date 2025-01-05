using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(Slider))]
    [DefaultExecutionOrder(+1)] // (Note.) This script must be executed after the old "Menu_Dictionary_CS" was destroyed in "Awake()" by the new "Menu_Dictionary_CS".
    public class Menu_Slider_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the slider for adjusting the 'Attack Multiplier' and 'Defence Multiplier' in the menu scene.
		 * This script is used for creating the 'Attack Multiplier Dictionary' and 'Defence Multiplier Dictionary' in the "Menu_Dictionary_CS".
		*/

        // User options >>
        public Slider This_Slider;
        public Text Value_Text;
        public string Key_Name = "";
        public int Type; // 0 = Attack_Multiplier, 1 = Defence_Multiplier.
        public float Initial_Value;
        // << User options


        bool isPrepared;


        void Awake()
        { // This function is called after the old "Menu_Dictionary_CS" was destroyed in "Awake()" by the new "Menu_Dictionary_CS".
            // These values must be setup before "OnSceneLoaded()", because the "Menu_Dictionary_CS" uses these values in "OnSceneLoaded()".
            if (This_Slider == null)
            {
                This_Slider = GetComponent<Slider>();
            }

            // Set the initial value.
            This_Slider.value = Initial_Value;

            // Set the "Key_Name".
            if (string.IsNullOrEmpty(Key_Name))
            {
                Key_Name = this.name;
            }

            // Send this reference to "Menu_Dictionary_CS".
            Send_Reference();

            // Prepared.
            isPrepared = true;
        }


        protected virtual void Send_Reference()
        {
            // Send this reference to "Menu_Dictionary_CS".
            if (Menu_Dictionary_CS.Instance)
            {
                switch (Type)
                {
                    case 0: // for Attack Multiplier.
                        Menu_Dictionary_CS.Instance.Get_Attack_Slider_Script(this);
                        break;

                    case 1: // for Defence Multiplier.
                        Menu_Dictionary_CS.Instance.Get_Defence_Slider_Script(this);
                        break;
                }
            }
        }


        public void On_Value_Changed(float value)
        { // Called from the slider, when the value has been changed.

            // Round the value.
            This_Slider.value = Mathf.Round(value * 10.0f) / 10.0f;

            // Update the value text.
            if (Value_Text)
            {
                Value_Text.text = This_Slider.value.ToString();
            }

            // Check it is called after "Awake()".
            if (isPrepared == false)
            { // Called in "Awake()" to set the initial selection. >> The stored values in "Menu_Dictionary_CS" will be over written.
                return;
            }

            // Send the new value to "Menu_Dictionary_CS".
            if (Menu_Dictionary_CS.Instance)
            {
                Menu_Dictionary_CS.Instance.Update_Slider_Disctionary(Type, Key_Name, This_Slider.value);
            }
        }
    }

}
