using Unity.Netcode;
using UnityEngine;


namespace ChobiAssets.PTM
{

    public class Turret_Horizontal_CS : NetworkBehaviour
    {
        /* 
		 * This script rotates the turret horizontally.
		 * This script works in combination with "Aiming_Control_CS" in the MainBody.
		*/


        // User options >>
        public bool Limit_Flag;
        public float Max_Right = 170.0f;
        public float Max_Left = 170.0f;
        public float Speed_Mag = 10.0f;
        public float Acceleration_Time = 0.5f;
        public float Deceleration_Time = 0.5f;
        public Bullet_Generator_CS Bullet_Generator_Script;
        // << User options


        Transform thisTransform;
        Transform parentTransform;
        Aiming_Control_CS aimingScript;
        bool isTurning;
        bool isTracking;
        float angleY;
        Vector3 currentLocalAngles;
        public float Turn_Rate; // Referred to from "Sound_Control_Motor_CS".
        float previousTurnRate;
        float bulletVelocity;
        public bool Is_Ready = true; // Referred to from "Cannon_Fire_CS".


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
            parentTransform = thisTransform.parent;
            aimingScript = GetComponentInParent<Aiming_Control_CS>();

            if (aimingScript != null) Debug.Log("Turret Horizontal: Loaded aiming control script!");

            currentLocalAngles = thisTransform.localEulerAngles;
            angleY = currentLocalAngles.y;
            Max_Right = angleY + Max_Right;
            Max_Left = angleY - Max_Left;

            // Get the "Bullet_Generator_CS".
            if (Bullet_Generator_Script == null)
            {
                Bullet_Generator_Script = GetComponentInChildren<Bullet_Generator_CS>();
            }
            if (Bullet_Generator_Script == null)
            {
                Debug.LogWarning("'Bullet_Generator_CS' cannot be found. The cannon cannot get the bullet velocity.");
                // Set the fake value.
                bulletVelocity = 250.0f;
            }
        }


        public void Start_Tracking()
        { // Called from "Aiming_Control_CS".
            isTracking = true;
            isTurning = true;
        }


        public void Stop_Tracking()
        { // Called from "Aiming_Control_CS".
            isTracking = false;
        }


        void FixedUpdate()
        {
                if (aimingScript.Use_Auto_Turn)
                {
                    Auto_Turn();
                }
                else
                {
                    Manual_Turn();
                }
        }


        void Auto_Turn()
        {
            if (isTurning == false)
            {
                return;
            }

            // Calculate the target angle.
            float targetAngle;
            if (isTracking)
            { // Tracking the target.

                // Get the target position.
                Vector3 targetPosition = aimingScript.Target_Position;
                if (aimingScript.Target_Rigidbody && aimingScript.Use_Auto_Lead)
                { // The target has a rigidbody, and the "Use_Auto_Lead" option is enabled.

                    // Calculate the lead angle to the target.
                    float distance = Vector3.Distance(thisTransform.position, targetPosition);
                    if (Bullet_Generator_Script)
                    {
                        bulletVelocity = Bullet_Generator_Script.Current_Bullet_Velocity;
                    }
                    targetPosition += aimingScript.Target_Rigidbody.velocity * aimingScript.Aiming_Blur_Multiplier * (distance / bulletVelocity);
                }

                // Calculate the target angle.
                Vector3 targetLocalPos = parentTransform.InverseTransformPoint(targetPosition);
                Vector2 targetLocalPos2D;
                targetLocalPos2D.x = targetLocalPos.x;
                targetLocalPos2D.y = targetLocalPos.z;
                targetAngle = Vector2.Angle(Vector2.up, targetLocalPos2D) * Mathf.Sign(targetLocalPos.x);
                if (Limit_Flag)
                {
                    targetAngle -= angleY;
                }
                else
                {
                    targetAngle = Mathf.DeltaAngle(angleY, targetAngle);
                }
                targetAngle += aimingScript.Adjust_Angle.x;
            }
            else
            { // Not tracking. >> Return to the initial angle.
                targetAngle = Mathf.DeltaAngle(angleY, 0.0f);
                if (Mathf.Abs(targetAngle) < 0.01f)
                {
                    isTurning = false;
                }
            }

            // Calculate the "Turn_Rate".
            float sign = Mathf.Sign(targetAngle);
            targetAngle = Mathf.Abs(targetAngle);
            float currentSlowdownAng = Mathf.Abs(Speed_Mag * previousTurnRate) * Deceleration_Time;
            float targetTurnRate = Mathf.Lerp(0.0f, 1.0f, targetAngle / (Speed_Mag * Time.fixedDeltaTime + currentSlowdownAng)) * sign;
            if (targetAngle > currentSlowdownAng)
            {
                Turn_Rate = Mathf.MoveTowards(Turn_Rate, targetTurnRate, Time.fixedDeltaTime / Acceleration_Time);
            }
            else
            {
                Turn_Rate = Mathf.MoveTowards(Turn_Rate, targetTurnRate, Time.fixedDeltaTime / Deceleration_Time);
            }
            previousTurnRate = Turn_Rate;

            // Rotate.
            angleY += Speed_Mag * Turn_Rate * Time.fixedDeltaTime * aimingScript.Turret_Speed_Multiplier;
            if (Limit_Flag)
            {
                angleY = Mathf.Clamp(angleY, Max_Left, Max_Right);
                if (angleY <= Max_Left || angleY >= Max_Right)
                {
                    Turn_Rate = 0.0f;
                }
            }
            currentLocalAngles.y = angleY;
            thisTransform.localEulerAngles = currentLocalAngles;

            // Set the "Is_Ready".
            if (targetAngle <= aimingScript.OpenFire_Angle)
            {
                Is_Ready = true; // Referred to from "Cannon_Fire_CS".
            }
            else
            {
                Is_Ready = false; // Referred to from "Cannon_Fire_CS".
            }
        }


        void Manual_Turn()
        {
            if (aimingScript.Turret_Turn_Rate != 0.0f)
            {
                isTurning = true;
            }

            if (isTurning == false)
            {
                return;
            }

            // Calculate the "Turn_Rate".
            float targetTurnRate = aimingScript.Turret_Turn_Rate;
            if (targetTurnRate != 0.0f)
            {
                Turn_Rate = Mathf.MoveTowards(Turn_Rate, targetTurnRate, Time.fixedDeltaTime / Acceleration_Time);
            }
            else
            {
                Turn_Rate = Mathf.MoveTowards(Turn_Rate, targetTurnRate, Time.fixedDeltaTime / Deceleration_Time);
            }
            if (Turn_Rate == 0.0f)
            {
                isTurning = false;
            }

            // Rotate.
            angleY += Speed_Mag * Turn_Rate * Time.fixedDeltaTime;
            if (Limit_Flag)
            {
                angleY = Mathf.Clamp(angleY, Max_Left, Max_Right);
            }
            currentLocalAngles.y = angleY;
            thisTransform.localEulerAngles = currentLocalAngles;
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}