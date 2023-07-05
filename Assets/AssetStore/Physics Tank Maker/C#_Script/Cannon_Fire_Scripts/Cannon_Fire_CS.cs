using UnityEngine;
using System.Collections;
using Unity.Netcode;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_CS : NetworkBehaviour
	{
		/*
		 * This script is attached to the "Cannon_Base" in the tank.
		 * This script controls the firining of the tank.
		 * When firing, this script calls "Bullet_Generator_CS" and "Recoil_Brake_CS" scripts placed under this object in the hierarchy.
		 * In case of AI tank, this script works in combination with "AI_CS", "Turret_Horizontal_CS", "Cannon_Vertical_CS" and "Aiming_Control_CS".
		*/

		// User options >>
		public float Reload_Time = 2.0f;
		public float Recoil_Force = 5000.0f;
		// << User options


		// Set by "inputType_Settings_CS".
		public int inputType = 0;

		// Referred to from "UI_Reloading_Circle_CS".
		public float Loading_Count;
		public bool Is_Loaded = true;

        Rigidbody bodyRigidbody;
        Transform thisTransform;
        int direction = 1; // For twin barrels, 1 = left, 2 = right.
		public Bullet_Generator_CS[] Bullet_Generator_Scripts; // Referred to from "Cannon_Fire_Input_##_###".
        Recoil_Brake_CS[] recoilScripts;

        protected Cannon_Fire_Input_00_Base_CS inputScript;

        bool isSelected;


        void Start()
		{
        }

        public void OnSpawnRPC()
        {
            if (IsOwner) Initialize();
            else enabled = false;
        }


        public void Initialize()
        { // This function must be called in Start() after changing the hierarchy.
            thisTransform = transform;
            Bullet_Generator_Scripts = GetComponentsInChildren<Bullet_Generator_CS>();
            recoilScripts = thisTransform.parent.GetComponentsInChildren<Recoil_Brake_CS>();
            bodyRigidbody = GetComponentInParent<Rigidbody>();

            // Get the input type.
            if (inputType != 10)
            { // This tank is not an AI tank.
                inputType = General_Settings_CS.Input_Type;
            }

            // Set the "inputScript".
            Set_Input_Script(inputType);

            // Prepare the "inputScript".
            if (inputScript != null)
            {
                inputScript.Prepare(this);
            }

            if(inputScript == null) Debug.LogError("Cannon_Fire_Input_Script failed to load!");
            else Debug.Log("Cannon_Fire_Input_Script loaded successfully!");
            if (Bullet_Generator_Scripts.Length < 1) Debug.LogError("No Bullet_Generator_Scripts found!");
            else Debug.Log("Bullet_Generator_Scripts loaded successfully!");
        }


        protected virtual void Set_Input_Script(int type)
        {
            switch (type)
            {
                case 0: // Mouse + Keyboard (Stepwise)
                case 1: // Mouse + Keyboard (Pressing)
                    inputScript = gameObject.AddComponent<Cannon_Fire_Input_01_Mouse_CS>();
                    break;

                case 2: // GamePad (Single stick)
                case 3: // GamePad (Twin stick)
                    inputScript = gameObject.AddComponent<Cannon_Fire_Input_02_For_Sticks_Drive_CS>();
                    break;

                case 4: // GamePad (Triggers)
                    inputScript = gameObject.AddComponent<Cannon_Fire_Input_03_For_Triggers_Drive_CS>();
                    break;

                case 10: // AI
                    inputScript = gameObject.AddComponent<Cannon_Fire_Input_99_AI_CS>();
                    break;
            }
        }


        void Update()
        {
            if (Is_Loaded == false)
            {
                return;
            }
            else inputScript.Get_Input();

            /*
            if (isSelected || inputType == 10)
            { // The tank is selected, or AI.
                Debug.LogError("Pew!");
                inputScript.Get_Input();
            }
            */
        }


        public void Fire()
        { // Called from "Cannon_Fire_Input_##_###".
            // Call all the "Bullet_Generator_CS".


            for (int i = 0; i < Bullet_Generator_Scripts.Length; i++)
            {
                Bullet_Generator_Scripts[i].Fire_Linkage(direction);
            }

            // Call all the "Recoil_Brake_CS".
            for (int i = 0; i < recoilScripts.Length; i++)
            {
                recoilScripts[i].Fire_Linkage(direction);
            }

            // Add recoil shock force to the MainBody.
            bodyRigidbody.AddForceAtPosition(-thisTransform.forward * Recoil_Force, thisTransform.position, ForceMode.Impulse);

            // Reload.
            StartCoroutine("Reload");
        }

        public IEnumerator Reload()
        { // Called also from "Cannon_Fire_Input_##_###".
            Is_Loaded = false;
            Loading_Count = 0.0f;

            while (Loading_Count < Reload_Time)
            {
                Loading_Count += Time.deltaTime;
                yield return null;
            }

            Is_Loaded = true;
            Loading_Count = Reload_Time;

            // Set direction for twin barrels, 1 = left, 2 = right.
            if (direction == 1)
            {
                direction = 2;
            }
            else
            {
                direction = 1;
            }
        }


        void Get_AI_CS()
        { // Called from "AI_CS".
            inputType = 10;
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            this.isSelected = isSelected;
        }


        public void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}