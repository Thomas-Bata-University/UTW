using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(Dropdown))]
    [DefaultExecutionOrder(+1)] // (Note.) This script must be executed after the old "Menu_Dictionary_CS" was destroyed in "Awake()" by the new "Menu_Dictionary_CS".
    public class Menu_Dropdown_CS : MonoBehaviour
    {
        /*
		 * This script is attached to each dropdown for selecting the tank prefab in the menu scene.
		 * This script is used for creating the 'Tank Dictionary' in the "Menu_Dictionary_CS".
		*/

        // User options >>
        public Dropdown This_Dropdown;
        public Text Title_Text;
        public string Key_Name = "";
        public int Num;
        public GameObject[] Prefabs_Array;
        public int Default_Value = 0;
        public Transform Symbol_Transform;
        public float Offset;
        // << User options


        Transform thisTransform;
        bool isPrepared;


        void Awake()
        { // This function is called after the old "Menu_Dictionary_CS" was destroyed in "Awake()" by the new "Menu_Dictionary_CS".
            // These values must be setup before "OnSceneLoaded()", because the "Menu_Dictionary_CS" uses these values in "OnSceneLoaded()".
            thisTransform = transform;
            if (This_Dropdown == null)
            {
                This_Dropdown = GetComponent<Dropdown>();
            }

            // Setup the dropdown.
            Setup_Dropdown();

            // Set the initial selection.
            if (Prefabs_Array.Length > Default_Value)
            {
                This_Dropdown.value = Default_Value;
            }

            // Set the title text.
            if (Title_Text)
            {
                Title_Text.text = this.name;
            }

            // Set the "Key_Name".
            if (string.IsNullOrEmpty(Key_Name))
            {
                Key_Name = this.name;
            }

            // Send this reference to "Menu_Dictionary_CS".
            if (Menu_Dictionary_CS.Instance)
            {
                Menu_Dictionary_CS.Instance.Get_Dropdown_Script(this);
            }

            // Prepared.
            isPrepared = true;
        }


        protected virtual void Setup_Dropdown()
        {
            // Setup the dropdown.
            This_Dropdown.ClearOptions();
            for (int i = 0; i < Prefabs_Array.Length; i++)
            {
                var prefab = Prefabs_Array[i];
                if (prefab)
                { // The prefab is assigned.
                    // Split the name by "_". (e.g. "Tiger-I_AI" >> "Tiger-I" and "AI")
                    var splittedNames = prefab.name.Split('_');

                    // Set the first name to the dropdown.
                    This_Dropdown.options.Add(new Dropdown.OptionData { text = splittedNames[0] });
                }
                else
                { // The prefab is not assigned.
                    // Set "Empty" to the dropdown.
                    This_Dropdown.options.Add(new Dropdown.OptionData { text = "Empty" });
                }
            }
            This_Dropdown.RefreshShownValue();
        }


        public void On_Value_Changed()
        { // Called from the dropdown, when the value has been changed.

            // Check it is called after "Awake()".
            if (isPrepared == false)
            { // Called in "Awake()" to set the initial selection. >> The stored values in "Menu_Dictionary_CS" will be over written.
                return;
            }

            // Send the new value to "Menu_Dictionary_CS".
            if (Menu_Dictionary_CS.Instance)
            {
                Menu_Dictionary_CS.Instance.Update_Tank_Disctionary(Key_Name, Prefabs_Array[This_Dropdown.value]);
            }
        }


        void LateUpdate()
        {
            // Set the position.
            if (Symbol_Transform)
            {
                Vector3 tempPos = Camera.main.WorldToScreenPoint(Symbol_Transform.position + Symbol_Transform.forward * Offset);
                thisTransform.position = tempPos;
            }
        }

    }
}
