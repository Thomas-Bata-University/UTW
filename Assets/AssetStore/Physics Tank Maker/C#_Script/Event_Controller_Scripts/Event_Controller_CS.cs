using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	public class Event_Controller_CS : MonoBehaviour
	{
		/*
		 * This script controls the events in the scene, such as spawning a tank, displaying a message and so on.
		*/


		// User options >>

		// Trigger settings.
		public int Trigger_Type; // 0=Timer, 1=Destroy, 2=Trigger_Collider
		public float Trigger_Time;
		public int Trigger_Setting_Type = 0; // 0=Set Manually, 1=Any Hostile tank, 2=Any Friendly tank.
		public int Trigger_Num = 1;
		public Transform[] Trigger_Tanks;
		public int Operator_Type; // 0=AND, 1=OR.
		public bool All_Trigger_Flag = true;
		public int Necessary_Num = 1;
		public int Trigger_Collider_Num = 1;
		public Trigger_Collider_CS[] Trigger_Collider_Scripts;
		public int Useless_Event_Num;
		public Event_Controller_CS[] Useless_Events;
		public int Disabled_Event_Num;
		public Event_Controller_CS[] Disabled_Events;
		public int Event_Type; // 0="Spawn Tank" , 1="Show Message" , 2="Change AI Settings" , 3="Remove Tank" , 4="Artillery Fire" ,  , 10="None".
	

		// For "Spawn Tank" event.
		public string Key_Name;
		// Variables for "ID_Settings_CS".
		public GameObject Prefab_Object;
		public int Tank_ID = 1;
		public int Relationship;
		// Variables for "Respawn_Controller_CS".
		public int Respawn_Times = 0;
		public float Auto_Respawn_Interval = 10.0f;
		public bool Remove_After_Death = false;
		public float Remove_Interval = Mathf.Infinity;
        public Transform Respawn_Point_Pack;
        public Transform Respawn_Target;
        // Variable for "Special_Settings_CS".
        public float Attack_Multiplier = 1.0f;
        public float Defence_Multiplier = 1.0f;
        // Variables for "AI_Settings_CS".
        public GameObject WayPoint_Pack;
		public int Patrol_Type = 1; // 0 = Order, 1 = Random.
		public Transform Follow_Target;
		public bool No_Attack = false;
		public bool Breakthrough = false;
		public Transform Commander;
		public float Visibility_Radius = 512.0f;
		public float Approach_Distance = 256.0f;
		public float OpenFire_Distance = 512.0f;
		public float Lost_Count = 20.0f;
		public float Face_Offest_Angle = 0.0f;
        public float Dead_Angle = 30.0f;
        public float Patrol_Speed_Rate = 1.0f;
        public float Combat_Speed_Rate = 1.0f;
        public Text AI_State_Text;
		public string Tank_Name;

		// For "Show Message" event.
		public Text Event_Text;
		public string Event_Message;
		public Color Event_Message_Color = Color.white;
		public float Event_Message_Time = 3.0f;

		// For "Change AI settings" event.
		public int Target_Num = 1;
		public Transform[] Target_Tanks;
		// Variables for "AI_Settings_CS".
		public GameObject New_WayPoint_Pack;
		public int New_Patrol_Type = 1;
		public Transform New_Follow_Target;
		public bool New_No_Attack = false;
		public bool New_Breakthrough = false;
		public float New_Visibility_Radius = 512.0f;
		public float New_Approach_Distance = 256.0f;
		public float New_OpenFire_Distance = 512.0f;
		public float New_Lost_Count = 20.0f;
		public float New_Face_Offest_Angle = 0.0f;
        public float New_Dead_Angle = 30.0f;

        // For "Artillery Fire" event.
        public Artillery_Fire_CS Artillery_Script;
		public Transform Artillery_Target;
		public int Artillery_Num;

        // For "Show Result Canvas" event.
        public Canvas Result_Canvas;

		// Only for "Trigger_Collider" trigger type, and only for "Change AI settings", "Remove Tank" and "Destroy" event types.
		public bool Trigger_Itself_Flag = true;
		public bool Wait_For_All_Triggers = true;

		// << User options


		public Event_Trigger_00_Base_CS Trigger_Script; // Controlled by trigger scripts.
		protected Event_Event_00_Base_CS eventScript;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            // Detach the parent.
            transform.parent = null;

            // Set "Trigger_Script".
            Set_Trigger_Script();

            // Prepare the triggers.
            if (Trigger_Script != null)
            {
                Trigger_Script.Prepare_Trigger(this);
            }

            // Set "eventScript".
            Set_Event_Script();

            // Prepare the event.
            if (eventScript != null)
            {
                eventScript.Prepare_Event(this);
            }
        }


        protected void Set_Trigger_Script()
        {
            switch (Trigger_Type)
            {
                case 0: // Timer
                    Trigger_Script = gameObject.AddComponent<Event_Trigger_01_Timer_CS>();
                    break;

                case 1: // Destroy
                    switch (Operator_Type)
                    {
                        case 0: // AND
                            if (All_Trigger_Flag)
                            {
                                Trigger_Script = gameObject.AddComponent<Event_Trigger_02_01_Destroy_AND_All_CS>();
                            }
                            else
                            {
                                Trigger_Script = gameObject.AddComponent<Event_Trigger_02_02_Destroy_AND_Necessary_CS>();
                            }
                            break;

                        case 1: // OR
                            Trigger_Script = gameObject.AddComponent<Event_Trigger_02_03_Destroy_OR_CS>();
                            break;
                    }
                    break;

                case 2: // Trigger_Collider
                    switch (Trigger_Setting_Type)
                    {
                        case 0: // 'Set manually'
                            switch (Operator_Type)
                            {
                                case 0: // AND
                                    Trigger_Script = gameObject.AddComponent<Event_Trigger_03_01_TriggerCollider_AND_CS>();
                                    break;
                                case 1: // OR
                                    Trigger_Script = gameObject.AddComponent<Event_Trigger_03_02_TriggerCollider_OR_CS>();
                                    break;
                            }
                            break;

                        case 1: // "Any hostile tank"
                        case 2: // "Any friendly tank"
                        case 3: // "Any tank"
                            Trigger_Script = gameObject.AddComponent<Event_Trigger_03_03_TriggerCollider_Any_CS>();
                            break;
                    }
                    break;
            }
        }


        protected void Set_Event_Script()
        {
            switch (Event_Type)
            {
                case 0: // Spawn Tank
                    eventScript = gameObject.AddComponent<Event_Event_01_Spawn_Tank_CS>();
                    break;

                case 1: // Show Message
                    eventScript = gameObject.AddComponent<Event_Event_02_Show_Message_CS>();
                    break;

                case 2: // Change AI Settings
                    eventScript = gameObject.AddComponent<Event_Event_03_Change_AI_Settings_CS>();
                    break;

                case 3: // Remove Tank
                    eventScript = gameObject.AddComponent<Event_Event_04_Remove_Tank_CS>();
                    break;

                case 4: // Artillery Fire
                    eventScript = gameObject.AddComponent<Event_Event_05_Artillery_Fire_CS>();
                    break;

                case 5: // Destroy Tank
                    eventScript = gameObject.AddComponent<Event_Event_06_Destroy_Tank_CS>();
                    break;

                case 6: // Show Result Canvas
                    eventScript = gameObject.AddComponent<Event_Event_07_Show_Result_Canvas_CS>();
                    break;

                case 10: // None
                    break;
            }
        }


        void Update()
        {
            // Check the trigger.
            if (Trigger_Script != null)
            {
                Trigger_Script.Check_Trigger();
            }
        }


        public void Start_Event()
        { // Called from trigger scripts.

            // Control other triggers.
            Destroy_Useless_Events();
            Enable_Disabled_Events();

            // Start Event.
            if (eventScript != null)
            {
                eventScript.Execute_Event();
            }
        }


        void Destroy_Useless_Events()
        {
            for (int i = 0; i < Useless_Events.Length; i++)
            {
                if (Useless_Events[i])
                {
                    Destroy(Useless_Events[i].gameObject);
                    Useless_Events[i] = null;
                }
            }
        }


        void Enable_Disabled_Events()
        {
            for (int i = 0; i < Disabled_Events.Length; i++)
            {
                if (Disabled_Events[i])
                {
                    Disabled_Events[i].enabled = true;
                    Disabled_Events[i] = null;
                }
            }
        }


        public void Overwrite_Values(Event_Controller_CS eventScript)
        { // Called from "Event_Event_03_Change_AI_Settings_CS".
            WayPoint_Pack = eventScript.New_WayPoint_Pack;
            Patrol_Type = eventScript.New_Patrol_Type;
            Follow_Target = eventScript.New_Follow_Target;
            No_Attack = eventScript.New_No_Attack;
            Breakthrough = eventScript.New_Breakthrough;
            Visibility_Radius = eventScript.New_Visibility_Radius;
            Approach_Distance = eventScript.New_Approach_Distance;
            OpenFire_Distance = eventScript.New_OpenFire_Distance;
            Lost_Count = eventScript.New_Lost_Count;
            Face_Offest_Angle = eventScript.New_Face_Offest_Angle;
            Dead_Angle = eventScript.New_Dead_Angle;
        }

    }

}