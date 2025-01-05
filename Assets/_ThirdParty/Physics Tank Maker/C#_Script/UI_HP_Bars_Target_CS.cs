using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	[DefaultExecutionOrder (+2)] // (Note.) This script is executed after the main camera is moved, in order to move the bars smoothly.
	[ RequireComponent (typeof(Canvas))]
	public class UI_HP_Bars_Target_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "Canvas_HP_Bars (Target)" in the scene.
		 * This script controls the Hit Points Bars for displaying the target tank's hit points values.
		 * This script works in combination with "Aiming_Control_CS" in the current selected tank, and "Damage_Control_Center_CS" in the target tank.
		*/


		// User options >>
		public Canvas This_Canvas;
		public CanvasScaler This_Canvas_Scaler;
		public Transform Bars_Parent_Transform;
		public Image Body_Bar;
		public Image Turret_Bar;
		public Image Left_Track_Bar;
		public Image Right_Track_Bar;
		public float Flash_Time = 1.0f;
        public float Normal_Alpha = 0.4f;
        public Color Friend_Color = Color.blue;
        public Color Hostile_Color = Color.red;
        // << User options


        Aiming_Control_CS aimingScript;
		Image[] bodyBarImages;
		Image[] turretBarImages;
		Image[] leftTrackBarImages;
		Image[] rightTrackBarImages;
		float previousBodyHP;
		float previousTurretHP;
		float previousLeftTrackHP;
		float previousRightTrackHP;
		int flashCancelID;
		Rigidbody currentTargetRigidbody;
		Damage_Control_Center_CS targetDamageScript;


        public static UI_HP_Bars_Target_CS Instance;


        void Awake()
        {
            Instance = this;
        }


        void Start()
		{
			Initialize();
		}


		void Initialize()
		{
			if (This_Canvas == null) {
				This_Canvas = GetComponent <Canvas>();
			}

			if (This_Canvas_Scaler == null) {
				This_Canvas_Scaler = GetComponent <CanvasScaler>();
			}

			if (Bars_Parent_Transform == null) {
				Debug.LogWarning("'Bars_Parent_Transform'' is not assigned in '" + this.name + "'.");
				Destroy(this);
				return;
			}

			// Get all the child images.
			bodyBarImages = Body_Bar.GetComponentsInChildren <Image>();
			turretBarImages = Turret_Bar.GetComponentsInChildren <Image>();
			leftTrackBarImages = Left_Track_Bar.GetComponentsInChildren <Image>();
			rightTrackBarImages = Right_Track_Bar.GetComponentsInChildren <Image>();
		}


        void LateUpdate()
        {
            if (aimingScript == null)
            { // The tank has been destroyed.
                Disable_Canvas(true);
                return;
            }

            if (aimingScript.Is_Selected == false)
            { // The tank is not selected now.
                Disable_Canvas(true);
                return;
            }

            if (aimingScript.Target_Rigidbody && aimingScript.Target_Rigidbody != currentTargetRigidbody)
            { // The target has been changed.
                // Update the target information.
                Update_Traget_Information();
                return;
            }

            if (currentTargetRigidbody == null)
            { // The target has been removed from the scene.
                Disable_Canvas(true);
                return;
            }

            if (targetDamageScript == null)
            { // The target has no "Damage_Control_Center_CS".
                Disable_Canvas(true);
                return;
            }

            if (targetDamageScript.MainBody_HP <= 0.0f)
            { // The target has been destroyed.
                Disable_Canvas(true);
                return;
            }


            if (aimingScript.Target_Rigidbody == null)
            { // The tank is not locking on anything.
                // Disable the canvas after a while.
                Disable_Canvas(false);
            }

            // Set the appearance and the position of all the bars.
            Set_Appearance_And_Position();

            // Control the appearance of each bar.
            Control_Bars();
        }


        void Enable_Canvas()
        {
            // Cancel the "Disable_Timer".
            cancelDisableTimer = true;

            if (This_Canvas.enabled)
            {
                return;
            }

            // Enable the canvas.
            This_Canvas.enabled = true;
        }


        void Disable_Canvas(bool isImmediate)
        {
            if (This_Canvas.enabled == false)
            {
                return;
            }

            // Disable immediately.
            if (isImmediate)
            {
                This_Canvas.enabled = false;
                return;
            }

            // Disable the canvas after a while.
            StartCoroutine("Disable_Timer");
        }


        bool cancelDisableTimer;
        IEnumerator Disable_Timer()
        {
            var count = 0.0f;
            while(count < 2.0f)
            {
                if (This_Canvas.enabled == false)
                { // The canvas has been disabled.
                    yield break;
                }

                if (cancelDisableTimer)
                { // The timer has been canceled.
                    cancelDisableTimer = false;
                    yield break;
                }

                count += Time.deltaTime;
                yield return null;
            }

            This_Canvas.enabled = false;
        }


        void Update_Traget_Information()
		{
            // Set the target transform.
            if (aimingScript.Target_Rigidbody)
            {
                currentTargetRigidbody = aimingScript.Target_Rigidbody;
            }
            else
            {
                currentTargetRigidbody = null;
                return;
            }

            // Get the "Damage_Control_Center_CS" in the target.
            targetDamageScript = currentTargetRigidbody.GetComponentInParent<Damage_Control_Center_CS>();
            if (targetDamageScript == null)
            {
                return;
            }

            // Store the HP values.
            previousBodyHP = targetDamageScript.MainBody_HP;
            previousTurretHP = targetDamageScript.Turret_Props[0].hitPoints;
            previousLeftTrackHP = targetDamageScript.Left_Track_HP;
            previousRightTrackHP = targetDamageScript.Right_Track_HP;

            // Set the color.
            var targetIDScript = currentTargetRigidbody.GetComponentInParent<ID_Settings_CS>();
            if (targetIDScript)
            {
                // Set the color according to the relationship.
                Color color = Color.white;
                switch (targetIDScript.Relationship)
                {
                    case 0: // Friend.
                        color = Friend_Color;
                        break;

                    case 1: // Hostile.
                        color = Hostile_Color;
                        break;
                }
                color.a = Normal_Alpha;

                // Change the color of all the images.
                Set_Color(bodyBarImages, color);
                Set_Color(turretBarImages, color);
                Set_Color(leftTrackBarImages, color);
                Set_Color(rightTrackBarImages, color);
            }
        }


        void Set_Color(Image[] images, Color color)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = color;
            }
        }


        void Set_Appearance_And_Position()
		{
            var mainCamera = Camera.main;

            // Set the appearance and the position.
            Vector3 currentPosition = mainCamera.WorldToScreenPoint(currentTargetRigidbody.position);
            if (currentPosition.z < 0.0f)
            { // Behind of the camera.
                // Disable the canvas.
                Disable_Canvas(true);
                return;
            } // In front of the camera.

            // Enable the canvas.
            if (aimingScript.Target_Transform)
            {
                Enable_Canvas();
            }

            // Set the scale.
            float frustumHeight = 2.0f * Vector3.Distance(mainCamera.transform.position, currentTargetRigidbody.position) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            Bars_Parent_Transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.5f, 1.0f / frustumHeight);

            // Set the position.
            float resolutionOffset = Screen.width / This_Canvas_Scaler.referenceResolution.x;
            currentPosition.y += Mathf.Lerp(64.0f, 640.0f, Mathf.Sqrt(1.0f / frustumHeight)) * resolutionOffset;
            currentPosition.z = 128.0f;
            Bars_Parent_Transform.position = currentPosition;
        }


		void Control_Bars()
		{
            // MainBody
            Body_Bar.fillAmount = targetDamageScript.MainBody_HP / targetDamageScript.Initial_Body_HP;
            if (previousBodyHP != targetDamageScript.MainBody_HP)
            {
                flashCancelID = 1;
                StartCoroutine(Flash(bodyBarImages, 1));
            }

            // Turret
            Turret_Bar.fillAmount = targetDamageScript.Turret_Props[0].hitPoints / targetDamageScript.Initial_Turret_HP;
            if (previousTurretHP != targetDamageScript.Turret_Props[0].hitPoints)
            {
                flashCancelID = 2;
                StartCoroutine(Flash(turretBarImages, 2));
            }

            // Left Track
            Left_Track_Bar.fillAmount = targetDamageScript.Left_Track_HP / targetDamageScript.Initial_Left_Track_HP;
            if (previousLeftTrackHP != targetDamageScript.Left_Track_HP)
            {
                flashCancelID = 3;
                StartCoroutine(Flash(leftTrackBarImages, 3));
            }

            // Right Track
            Right_Track_Bar.fillAmount = targetDamageScript.Right_Track_HP / targetDamageScript.Initial_Right_Track_HP;
            if (previousRightTrackHP != targetDamageScript.Right_Track_HP)
            {
                flashCancelID = 4;
                StartCoroutine(Flash(rightTrackBarImages, 4));
            }

            // Store the HP values.
            previousBodyHP = targetDamageScript.MainBody_HP;
            previousTurretHP = targetDamageScript.Turret_Props[0].hitPoints;
            previousLeftTrackHP = targetDamageScript.Left_Track_HP;
            previousRightTrackHP = targetDamageScript.Right_Track_HP;
        }


        IEnumerator Flash(Image[] images, int id)
        {
            flashCancelID = 0;
            float count = 0.0f;
            while (count < Flash_Time)
            {
                if (id == flashCancelID)
                {
                    yield break;
                }
                for (int i = 0; i < images.Length; i++)
                {
                    var currentColor = images[i].color;
                    currentColor.a = Mathf.Lerp(1.0f, Normal_Alpha, count / Flash_Time);
                    images[i].color = currentColor;
                }
                count += Time.deltaTime;
                yield return null;
            }
        }


        public void Get_Aiming_Script(Aiming_Control_CS tempAimingScript)
        { // Called from "Aiming_Control_CS".
            aimingScript = tempAimingScript;

            // Update the target information.
            Update_Traget_Information();
        }

    }

}