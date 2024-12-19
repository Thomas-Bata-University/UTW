using UnityEngine;
using System.Collections.Generic;


namespace ChobiAssets.PTM
{

    public class Game_Controller_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Game_Controller" in the scene.
		 * This script controls the physics settings, the layers collision settings and the cursor state in the scene.
		 * Also the general functions such as quit and pause are controlled by this script.
		*/


        // User options >>
        public bool Fix_Frame_Rate = true;
        public int Target_Frame_Rate = 50;
        public float Fixed_TimeStep = 0.02f;
        public float Sleep_Threshold = 0.5f;
        public Canvas Pause_Canvas;
        // << User options

        public bool Allow_Pause = true; // Controlled by some events.
        List<ID_Settings_CS> idScriptsList = new List<ID_Settings_CS>();
        bool isPaused;
        bool storedCursorVisible;


        public static Game_Controller_CS Instance;


        void Awake()
        {
            Instance = this;
        }


        void Start()
        {
            // Physics settings.
            Time.fixedDeltaTime = Fixed_TimeStep;
            Physics.sleepThreshold = Sleep_Threshold;

            // Layer settings.
            Layer_Settings_CS.Layers_Collision_Settings();

            // Set the frame rate.
            if (Fix_Frame_Rate)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = Target_Frame_Rate;
            }
            
            // Disable the pause canvas.
            if (Pause_Canvas)
            {
                Pause_Canvas.enabled = false;
            }
        }


        void Receive_ID_Script(ID_Settings_CS idScript)
        { // Called from "ID_Settings_CS" in tanks in the scene, when the tank is spawned.

            // Store the "ID_Settings_CS".
            idScriptsList.Add(idScript);
        }


        void Update()
        {
            General_Functions();
        }


        void General_Functions()
        {
            if (Input.anyKeyDown == false)
            {
                return;
            }

            // Reload the scene.
            if (General_Settings_CS.Allow_Reload_Scene && Input.GetKeyDown(General_Settings_CS.Reload_Scene_Key))
            {
                Time.timeScale = 1.0f;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            // Quit.
            if (General_Settings_CS.Allow_Instant_Quit && Input.GetKeyDown(General_Settings_CS.Quit_Key))
            {
                Application.Quit();
                return;
            }

            // Cursor state.
            if (General_Settings_CS.Allow_Switch_Cursor && Input.GetKeyDown(General_Settings_CS.Switch_Cursor_Key))
            {
                Switch_Cursor(Cursor.visible == false);
                return;
            }

            // Pause.
            if (Allow_Pause && Input.GetKeyDown(General_Settings_CS.Pause_Key))
            {
                Pause();
                return;
            }
        }


        void Switch_Cursor(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        public void Pause()
        { // Called also from the button.

            isPaused = !isPaused;

            // Set the time scale, and the cursor state.
            if (isPaused)
            {
                Time.timeScale = 0.0f;
                storedCursorVisible = Cursor.visible;
                Switch_Cursor(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                Switch_Cursor(storedCursorVisible);
            }

            // Control the pause canvas.
            if (Pause_Canvas)
            {
                Pause_Canvas.enabled = isPaused;
            }

            // Send "Pause" message to all the tank parts via "ID_Settings_CS" attached to the tanks.
            for (int i = 0; i < idScriptsList.Count; i++)
            {
                idScriptsList[i].BroadcastMessage("Pause", isPaused, SendMessageOptions.DontRequireReceiver);
            }
        }


        public void Remove_ID(ID_Settings_CS idScript)
        { // Called from "ID_Settings_CS", just before the tank is removed from the scene.
            
            // Remove the "ID_Settings_CS" from the list.
            idScriptsList.Remove(idScript);
        }

    }

}