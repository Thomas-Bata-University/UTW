using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ChobiAssets.PTM
{

    public class Menu_Dictionary_CS : MonoBehaviour
    {
        /*
         * This script is attached to "Menu_Dictionary" object in menu scenes.
		 * This script creates three dictionaries (Tanks, Attack Multiplier, Defence Multiplier) in the menu scene.
         * And sends them to the battle scene using 'DontDestroyOnLoad()'.
         * The dictionaries are used for overwriting "Event_Controller_CS" set for spawning a tank in the battle scene.
		 * 
		*/

        // User options >>
        public int Scene_Type = 0;
        public string Battle_Scene_Name;
        // << User options


        protected List<Menu_Dropdown_CS> dropdownScriptsList = new List<Menu_Dropdown_CS>();
        protected List<Menu_Slider_CS> attackSliderScriptsList = new List<Menu_Slider_CS>();
        protected List<Menu_Slider_CS> defenceSliderScriptsList = new List<Menu_Slider_CS>();

        protected Dictionary<string, GameObject> tankDictionary;
        protected Dictionary<string, float> attackMultiplierDictionary;
        protected Dictionary<string, float> defenceMultiplierDictionary;

        string birthplaceSceneName;
        string battleSceneName;


        public static Menu_Dictionary_CS Instance;


        void Awake()
        { // This function is called only once at the first time. It is not called after the this object moves to other scenes by using 'DontDestroyOnLoad()'.
            
            // Store the birthplace scene name.
            //birthplaceSceneName = SceneManager.GetActiveScene().name;

            // Get the battle scene name.
            switch (Scene_Type)
            {
                case 0: // Input Manually.
                    battleSceneName = Battle_Scene_Name;
                    break;

                case 1: // Battle Scene.
                    battleSceneName = Set_Battle_Scene_Name(birthplaceSceneName);
                    break;
            }

            // Find another "Menu_Dictionary_CS" in the scene, and check the birthplace scene.
            if (Instance)
            { // There is another "Menu_Dictionary_CS" in the scene.
                if (Instance.birthplaceSceneName == birthplaceSceneName)
                { // Another one has the same birthplace. >> It returned from the battle scene.

                    // Copy the values from the old one.
                    Copy_Values();

                    // Destroy the old one.
                    DestroyImmediate(Instance.gameObject);

                } // Another one has different birthplace. >> It will be destroyed in "OnSceneLoaded()".
            }

            // Store this instance.
            Instance = this;

            // Keep this object even if the scene has been changed.
            DontDestroyOnLoad(this.gameObject);
        }


        protected virtual string Set_Battle_Scene_Name(string currentSceneName)
        {
            var tempSceneName = currentSceneName.Replace("_Menu", "");
            tempSceneName = tempSceneName.Replace("_00_", "_01_");
            return tempSceneName;
        }


        protected virtual void Copy_Values()
        {
            // Copy the values from the old one.
            tankDictionary = Instance.tankDictionary;
            attackMultiplierDictionary = Instance.attackMultiplierDictionary;
            defenceMultiplierDictionary = Instance.defenceMultiplierDictionary;
        }


        public void Get_Dropdown_Script(Menu_Dropdown_CS dropdownScript)
        { // Called from "Menu_Dropdown_CS" before "Awake()" of this script.
            dropdownScriptsList.Add(dropdownScript);
        }


        public void Get_Attack_Slider_Script(Menu_Slider_CS sliderScript)
        { // Called from "Menu_Slider_CS" before "Awake()" of this script.
            attackSliderScriptsList.Add(sliderScript);
        }


        public void Get_Defence_Slider_Script(Menu_Slider_CS sliderScript)
        { // Called from "Menu_Slider_CS" before "Awake()" of this script.
            defenceSliderScriptsList.Add(sliderScript);
        }


        void OnEnable()
        {
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }


        void OnDisable()
        {
            //SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        { // This function is called after "Awake()" at the first time, and each time the scnene has been changed.

            // Check the current scene is related to this.
            if (scene.name == birthplaceSceneName)
            {  // The current scene is this birthplace.
                // Create new dictionaries, or setup the dropdowns and sliders.
                Control_Menu_Components();
                return;
            }

            if (scene.name == battleSceneName)
            { // The current scene is the specified battle scene.
                // Overwrite "Event_Controller_CS" in the scene.
                Overwrite_Values();
                return;
            }

            // The current scene is not related to this.
            // This object is no longer needed.
            Destroy(this.gameObject);
        }


        protected virtual void Control_Menu_Components()
        {
            // Tank Dictionary.
            if (tankDictionary == null)
            { // Tank Dictionary does not exist.
                // Create a new tank dictionary.
                Create_Tank_Dictionary();
            }
            else
            { // Tank Dictionary exists. >> This object returned from the battle scene.
                // Setup the dropdowns using the stored values.
                Setup_Tank_Dropdown();
            }

            // Attack Multiplier Dictionary.
            if (attackMultiplierDictionary == null)
            { // Attack Multiplier Dictionary does not exist.
                // Create a new Attack Multiplier dictionary.
                Create_Attack_Multiplier_Dictionary();
            }
            else
            { // Attack Multiplier Dictionary exists. >> This object returned from the battle scene.
                // Setup the slider using the stored value.
                Setup_Attack_Multiplier_Slider();
            }

            // Defence Multiplier Dictionary.
            if (defenceMultiplierDictionary == null)
            { // Defence Multiplier Dictionary does not exist.
                // Create a new Defence Multiplier dictionary.
                Create_Defence_Multiplier_Dictionary();
            }
            else
            { // Defence Multiplier Dictionary exists. >> This object returned from the battle scene.
                 // Setup the slider using the stored value.
                Setup_Defence_Multiplier_Slider();
            }
        }


        void Create_Tank_Dictionary()
        {
            // Create Tank Dictionary.
            tankDictionary = new Dictionary<string, GameObject>();
            for (int i = 0; i < dropdownScriptsList.Count; i++)
            {
                tankDictionary.Add(dropdownScriptsList[i].Key_Name, dropdownScriptsList[i].Prefabs_Array[dropdownScriptsList[i].This_Dropdown.value]);
            }
        }


        void Create_Attack_Multiplier_Dictionary()
        {
            // Create Attack Multiplier Dictionary.
            attackMultiplierDictionary = new Dictionary<string, float>();
            for (int i = 0; i < attackSliderScriptsList.Count; i++)
            {
                attackMultiplierDictionary.Add(attackSliderScriptsList[i].Key_Name, attackSliderScriptsList[i].This_Slider.value);
            }
        }


        void Create_Defence_Multiplier_Dictionary()
        {
            // Create Defence Multiplier Dictionary.
            defenceMultiplierDictionary = new Dictionary<string, float>();
            for (int i = 0; i < defenceSliderScriptsList.Count; i++)
            {
                defenceMultiplierDictionary.Add(defenceSliderScriptsList[i].Key_Name, defenceSliderScriptsList[i].This_Slider.value);
            }
        }


        void Setup_Tank_Dropdown()
        {
            // Setup the "Menu_Dropdown_CS" in the scene.
            for (int i = 0; i < dropdownScriptsList.Count; i++)
            {
                // Get the index that matches between the dictionary and the dropdown.
                for (int j = 0; j < dropdownScriptsList[i].Prefabs_Array.Length; j++)
                {
                    if (dropdownScriptsList[i].Prefabs_Array[j] == tankDictionary[dropdownScriptsList[i].Key_Name])
                    { // The both tank prefabs are the same.
                        // Set the dropdown value.
                        dropdownScriptsList[i].This_Dropdown.value = j;
                        continue;
                    }
                }
            }
        }


        void Setup_Attack_Multiplier_Slider()
        {
            // Setup the "Menu_Slider_CS" in the scene.
            for (int i = 0; i < attackSliderScriptsList.Count; i++)
            {
                // Set the slider value.
                attackSliderScriptsList[i].This_Slider.value = attackMultiplierDictionary[attackSliderScriptsList[i].Key_Name];
            }
        }


        void Setup_Defence_Multiplier_Slider()
        {
            // Setup the value in the "Menu_Slider_CS" in the scene.
            for (int i = 0; i < defenceSliderScriptsList.Count; i++)
            {
                // Set the slider value.
                defenceSliderScriptsList[i].This_Slider.value = defenceMultiplierDictionary[defenceSliderScriptsList[i].Key_Name];
            }
        }


        public void Update_Tank_Disctionary(string keyName, GameObject prefab)
        { // Called from "Menu_Dropdown_CS", when the dropdown value has been changed.
            // Update the Tank Dictionary.
            if (tankDictionary != null && tankDictionary.ContainsKey(keyName))
            {
                tankDictionary[keyName] = prefab;
            }
        }


        public virtual void Update_Slider_Disctionary(int type, string keyName, float value)
        { // Called from "Menu_Slider_CS", when the slider value has been changed.
            switch (type)
            {
                case 0: // for Attack Multiplier.
                    // Update the Attack Multiplier Dictionary.
                    if (attackMultiplierDictionary.ContainsKey(keyName))
                    {
                        attackMultiplierDictionary[keyName] = value;
                    }
                    break;

                case 1: // for Defence Multiplier.
                        // Update the Defence Multiplier Dictionary.
                    if (defenceMultiplierDictionary.ContainsKey(keyName))
                    {
                        defenceMultiplierDictionary[keyName] = value;
                    }
                    break;
            }
        }


        protected virtual void Overwrite_Values()
        {
            // Overwrite values in "Event_Controller_CS" in the battle scene.
            var eventScripts = FindObjectsOfType<Event_Controller_CS>();
            for (int i = 0; i < eventScripts.Length; i++)
            {
                if (eventScripts[i].Event_Type == 0)
                { // Spawn Tank Event.
                    // Overwrite "Prefab_Object".
                    if (tankDictionary != null && tankDictionary.ContainsKey(eventScripts[i].Key_Name))
                    {
                        eventScripts[i].Prefab_Object = tankDictionary[eventScripts[i].Key_Name];
                    }

                    // Overwrite "Attack_Multiplier".
                    if (attackMultiplierDictionary != null && attackMultiplierDictionary.ContainsKey(eventScripts[i].Key_Name))
                    {
                        eventScripts[i].Attack_Multiplier = attackMultiplierDictionary[eventScripts[i].Key_Name];
                    }

                    // Overwrite "Defence_Multiplier".
                    if (defenceMultiplierDictionary != null && defenceMultiplierDictionary.ContainsKey(eventScripts[i].Key_Name))
                    {
                        eventScripts[i].Defence_Multiplier = defenceMultiplierDictionary[eventScripts[i].Key_Name];
                    }
                }
            }
        }

    }

}
