using System.Collections.Generic;
using UnityEngine;

namespace ChobiAssets.PTM
{

    public class ID_Manager_CS : MonoBehaviour
    {
        /*
		 * This script is attached to "Game_Controller" in the scene.
		 * This script works in combination with "ID_Settings_CS" in tanks in the scene.
		 * This script gives proper ID number to each "ID_Settings_CS".
         * Also, the "ID_Settings_CS" scripts are listed in this script as "ID_Scripts_List".
		 * The player can select the tank by pressing numpad "+", "-" and "Enter" keys.
		*/

        List<bool> usedIDList = new List<bool>();
        int currentID = 1;

        public List<ID_Settings_CS> ID_Scripts_List = new List<ID_Settings_CS>(); // Referred to from "RC_Camera_CS".

        public static ID_Manager_CS Instance;


        void Awake()
        {
            Instance = this;
        }


        void Receive_ID_Script(ID_Settings_CS idScript)
        { // Called from "ID_Settings_CS" in tanks in the scene, when the tank is spawned.

            // Store the "ID_Settings_CS".
            ID_Scripts_List.Add(idScript);

            // Give the proper ID.
            idScript.Tank_ID = Get_Proper_ID(idScript);
        }


        int Get_Proper_ID(ID_Settings_CS idScript)
        {
            // Check the ID.
            if (idScript.Tank_ID == 0)
            { // The tank is not selectable. >> It it not neccesary to give a new number.
                return 0;
            }

            // Check the size of "usedIDList", and increase the size if needed.
            if (usedIDList.Count < idScript.Tank_ID)
            { // The list size is small.
                // Increase the list size.
                int additionalElementCount = idScript.Tank_ID - usedIDList.Count;
                for (int i = 0; i < additionalElementCount; i++)
                {
                    usedIDList.Add(false);
                }
            }

            // Check duplication of the ID numbers.
            if (usedIDList[idScript.Tank_ID - 1] == true)
            { // The number is already used.
                Debug.LogWarning("'Tank ID' is duplicated. It will be changed to proper value by 'ID_Manager_CS' script.");
                
                // Get an empty number.
                for (int i = 0; i < usedIDList.Count; i++)
                {
                    if (usedIDList[i] == false)
                    { // Empty number is found.
                        usedIDList[i] = true;
                        // Give the new number.
                        return i + 1;
                    }
                } // The list is full. >> The list should be small.

                // Add one element, and set it to true.
                usedIDList.Add(true);

                // Give the new number.
                return usedIDList.Count;
            }
            else
            { // The number is not used yet. >> It it not neccesary to give a new number.
                usedIDList[idScript.Tank_ID - 1] = true;
                return idScript.Tank_ID;
            }
        }


        void Update()
        {
            Select_Tank();
        }


        void Select_Tank()
        {
            // Change the current ID to "1".
            if (Input.GetKeyDown(General_Settings_CS.Select_Default_Tank_Key))
            {
                if (usedIDList[0] == true)
                {
                    currentID = 1;
                    Broadcast_Current_ID();
                }
                return;
            }

            // Increase the current ID.
            if (Input.GetKeyDown(General_Settings_CS.Increase_ID_Key))
            {
                for (int i = 0; i < usedIDList.Count; i++)
                {
                    currentID += 1;
                    if (currentID > usedIDList.Count)
                    {
                        currentID = 1;
                    }
                    if (usedIDList[currentID - 1] == true)
                    {
                        Broadcast_Current_ID();
                        break;
                    }
                }
                return;
            }

            // Decrease the current ID.
            if (Input.GetKeyDown(General_Settings_CS.Decrease_ID_Key))
            {
                for (int i = 0; i < usedIDList.Count; i++)
                {
                    currentID -= 1;
                    if (currentID < 1)
                    {
                        currentID = usedIDList.Count;
                    }
                    if (usedIDList[currentID - 1] == true)
                    {
                        Broadcast_Current_ID();
                        break;
                    }
                }
                return;
            }
        }


        void Broadcast_Current_ID()
        {
            // Send the current ID to all the "ID_Settings_CS".
            for(int i = 0; i < ID_Scripts_List.Count; i++)
            {
                ID_Scripts_List[i].Receive_Current_ID(currentID);
            }
        }


        public void Remove_ID(ID_Settings_CS idScript)
        { // Called from "ID_Settings_CS", just before the tank is removed from the scene.

            // Set the element to false.
            if (idScript.Tank_ID != 0)
            { // The tank is selectable.
                usedIDList[idScript.Tank_ID - 1] = false;
            }

            // Remove the "ID_Settings_CS" from the list.
            ID_Scripts_List.Remove(idScript);

            // Check the ID.
            if (idScript.Tank_ID == currentID)
            { // The current selected tank will be removed soon.
                // Change the current ID to "1".
                currentID = 1; // (Note.) Please do not remove the tank with ID "1".
                Broadcast_Current_ID();
            }
        }

    }

}