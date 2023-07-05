using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	public class Option_Manager_CS : MonoBehaviour
	{
        /*
		 * This script overwrites the values of "General_Settings_CS".
		 * Usually, this script is put in the title scene and menu scenes opened before battle scene.
         * Then, this object transfers to the next scenes by using 'DontDestroyOnLoad()'.
		*/

        // User options >>
        public int Input_Type = 0;
        public Dropdown Input_Dropdown;
        // << User options


        public static Option_Manager_CS Instance;


        protected virtual void Awake()
		{ // This function is called only once at the first time. It is not called after the this object moves to other scenes by using 'DontDestroyOnLoad()'.
			// Find the old one in the scene.
            if (Instance)
            {
                // Copy the values from the old one.
                Input_Type = Instance.Input_Type;

                // Destroy the old one.
                Destroy(Instance.gameObject);
            }

            // Store this instance.
            Instance = this;

            // Keep this object even if the scene has been changed.
            transform.parent = null; // (Note.) 'DontDestroyOnLoad()' can be used only in the top object.
            DontDestroyOnLoad (this.gameObject);

            // Set the dropdown for "Input_Type".
            if (Input_Dropdown)
            {
                Input_Dropdown.value = Input_Type;
            }

            // Overwrite the values in "General_Settings_CS".
            Overwrite_General_Settings();
        }


        public void On_Value_Changed_Input (int value)
		{ // Called from the dropdown for "Input_Type".
			Input_Type = value;
            Overwrite_General_Settings();
        }
	

        protected virtual void Overwrite_General_Settings()
        {
            // Overwrite the values in "General_Settings_CS".
            General_Settings_CS.Input_Type = Input_Type;
        }


    }

}
