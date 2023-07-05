using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	public class Scene_Open_CS : MonoBehaviour
	{
		/*
		 * This script is attached to buttons in the scene for opening the specified scene.
		*/


		// User options >>
        public int Scene_Type = 0;
		public string Scene_Name;
        // << User options


        public void Button_Push()
        { // Called from the button.
            // Disable all the button in the scene.
            Button thisButton = GetComponent<Button>();
            thisButton.enabled = false;
            Button[] buttons = FindObjectsOfType<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];
                if (button == thisButton)
                { // The button is itself.
                    continue;
                }

                // Disable the button.
                if (button.targetGraphic)
                {
                    button.targetGraphic.enabled = false;
                }
                Text tempText = button.GetComponentInChildren<Text>();
                if (tempText)
                {
                    tempText.enabled = false;
                }
                button.enabled = false;
            }

            // Open the scene.
            StartCoroutine("Open_Scene");
		}


        IEnumerator Open_Scene()
		{
            // Stop the time.
            Time.timeScale = 0.0f;

            // Disallow the pause.
            if (Game_Controller_CS.Instance)
            {
                Game_Controller_CS.Instance.Allow_Pause = false;
            }

            // Call "Fade_Control_CS".
            if (Fade_Control_CS.Instance)
            {
                Fade_Control_CS.Instance.StartCoroutine("Fade_Out");

                // Wait.
                var count = 0.0f;
                while (count < Fade_Control_CS.Instance.Fade_Time)
                {
                    count += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
           
            // Set the scene name.
            string newName = SceneManager.GetActiveScene().name;
            switch (Scene_Type)
            {
                case 0: // Input Manually.
                    newName = Scene_Name;
                    break;

                case 1: // Current Scene.
                    break;

                case 2: // Menu Scene.
                    newName += "_Menu";
                    newName = newName.Replace("_01_", "_00_");
                    break;

                case 3: // Battle Scene.
                    newName = newName.Replace("_Menu", "");
                    newName = newName.Replace("_00_", "_01_");
                    break;
            }
            
            // Load the scene.
            SceneManager.LoadSceneAsync(newName);

            // Start the time.
            Time.timeScale = 1.0f;
        }

    }

}
