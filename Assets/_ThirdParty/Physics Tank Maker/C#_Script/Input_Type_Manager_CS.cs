using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Input_Type_Manager_CS : MonoBehaviour
	{
        /*
		 * This script is attached to the "Game_Controller" in the scene.
		 * User can change the input type from this script.
		 * 
         * This script overwrites the values in "General_Settings_CS".
		 * However, when "Option_Manager_CS" has come from other scene, this script never overwrite the values.
		 * 
		 * [Input_Type]
		 * 0 => Mouse + Keyboard (Stepwise)
		 * 1 => Mouse + Keyboard (Pressing)
		 * 2 => GamePad (Single stick)
		 * 3 => GamePad (Twin sticks)
		 * 4 => GamePad (Triggers)
		 * 10 => AI
		*/

        // User options >>
        public int Input_Type = 1;
        public bool Show_Cursor_Forcibly = false;
        // << User options


        void Awake()
        {
            // Check "Option_Manager_CS" exists in the scene.
            if (Option_Manager_CS.Instance)
            { // "Option_Manager_CS" has come from other scene.
                // Do not overwrite the values.
                return;
            } // There is no "Option_Manager_CS" in the scene.

            // Overwrite the values in "General_Settings_CS".
            Overwrite_General_Settings();
        }


        protected virtual void Overwrite_General_Settings()
        {
            // Overwrite the values in "General_Settings_CS".
            General_Settings_CS.Input_Type = Input_Type;
        }


        void Start()
		{
			Set_Cursor_State();
		}


        protected virtual void Set_Cursor_State()
        {
            if (Show_Cursor_Forcibly == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            switch (General_Settings_CS.Input_Type)
            {
                case 0: // Mouse + Keyboard (Stepwise)
                case 1: // Mouse + Keyboard (Pressing)
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;

                case 2: // Mouse + Keyboard (Legacy)
                case 3: // GamePad (Single stick)
                case 4: // GamePad (Twin sticks)
                case 5: // GamePad (Triggers)
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }

    }

}