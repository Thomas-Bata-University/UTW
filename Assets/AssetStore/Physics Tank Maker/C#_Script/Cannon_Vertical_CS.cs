using FishNet.Object;
using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Cannon_Vertical_CS : NetworkBehaviour
    {
        /* 
		 * This script is attached to the "Cannon_Base" in the tank.
		 * This script rotates the cannon vertically.
		 * This script works in combination with "Aiming_Control_CS" in the MainBody.
		*/

        // User options >>
        public float Max_Elevation = 13.0f;
        public float Max_Depression = 7.0f;
        public float Speed_Mag = 5.0f;
        public float Acceleration_Time = 0.1f;
        public float Deceleration_Time = 0.1f;
        public bool Upper_Course = false;
        public Bullet_Generator_CS Bullet_Generator_Script;
        // << User options


        Transform thisTransform;
        Transform turretBaseTransform;
        Aiming_Control_CS aimingScript;
        bool isTurning;
        bool isTracking;
        float angleX;
        Vector3 currentLocalAngles;
        public float Turn_Rate; // Referred to from "Sound_Control_Motor_CS".
        float previousTurnRate;
        float bulletVelocity;
        public bool Is_Ready; // Referred to from "Cannon_Fire".


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
                turretBaseTransform = thisTransform.parent;
                aimingScript = GetComponentInParent<Aiming_Control_CS>();

                if (aimingScript != null) Debug.Log("Cannon Vertical: Loaded aiming control script!");

                currentLocalAngles = thisTransform.localEulerAngles;
                angleX = currentLocalAngles.x;
                Max_Elevation = angleX - Max_Elevation;
                Max_Depression = angleX + Max_Depression;

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
                // Calculate the target angle.
                targetAngle = Auto_Elevation_Angle();
                targetAngle += Mathf.DeltaAngle(0.0f, angleX) + aimingScript.Adjust_Angle.y;
            }
            else
            { // Not tracking. >> Return to the initial angle.
                targetAngle = -Mathf.DeltaAngle(angleX, 0.0f);
                if (Mathf.Abs(targetAngle) < 0.01f)
                {
                    isTurning = false;
                }
            }

            // Calculate the "Turn_Rate".
            float sign = Mathf.Sign(targetAngle);
            targetAngle = Mathf.Abs(targetAngle);
            float currentSlowdownAng = Mathf.Abs(Speed_Mag * previousTurnRate) * Deceleration_Time;
            float targetTurnRate = -Mathf.Lerp(0.0f, 1.0f, targetAngle / (Speed_Mag * Time.fixedDeltaTime + currentSlowdownAng)) * sign;
            if (targetAngle > currentSlowdownAng)
            {
                Turn_Rate = Mathf.MoveTowards(Turn_Rate, targetTurnRate, Time.fixedDeltaTime / Acceleration_Time);
            }
            else
            {
                Turn_Rate = Mathf.MoveTowards(Turn_Rate, targetTurnRate, Time.fixedDeltaTime / Deceleration_Time);
            }
            angleX += Speed_Mag * Turn_Rate * Time.fixedDeltaTime * aimingScript.Turret_Speed_Multiplier;
            previousTurnRate = Turn_Rate;

            // Rotate
            angleX = Mathf.Clamp(angleX, Max_Elevation, Max_Depression);
            currentLocalAngles.x = angleX;
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


        float Auto_Elevation_Angle()
        {
            // Calculate the proper angle.
            float properAngle;
            Vector2 targetPos2D;
            targetPos2D.x = aimingScript.Target_Position.x;
            targetPos2D.y = aimingScript.Target_Position.z;
            Vector2 thisPos2D;
            thisPos2D.x = thisTransform.position.x;
            thisPos2D.y = thisTransform.position.z;
            Vector2 dist;
            dist.x = Vector2.Distance(targetPos2D, thisPos2D);
            dist.y = aimingScript.Target_Position.y - thisTransform.position.y;
            if (Bullet_Generator_Script)
            {
                bulletVelocity = Bullet_Generator_Script.Current_Bullet_Velocity;
            }
            float posBase = (Physics.gravity.y * Mathf.Pow(dist.x, 2.0f)) / (2.0f * Mathf.Pow(bulletVelocity, 2.0f));
            float posX = dist.x / posBase;
            float posY = (Mathf.Pow(posX, 2.0f) / 4.0f) - ((posBase - dist.y) / posBase);
            if (posY >= 0.0f)
            {
                if (Upper_Course)
                {
                    properAngle = Mathf.Rad2Deg * Mathf.Atan(-posX / 2.0f + Mathf.Pow(posY, 0.5f));
                }
                else
                {
                    properAngle = Mathf.Rad2Deg * Mathf.Atan(-posX / 2.0f - Mathf.Pow(posY, 0.5f));
                }
            }
            else
            { // The bullet cannot reach the target.
                properAngle = 45.0f;
            }

            // Add the tilt angle of the tank.
            Vector3 forwardPos = turretBaseTransform.forward;
            Vector2 forwardPos2D;
            forwardPos2D.x = forwardPos.x;
            forwardPos2D.y = forwardPos.z;
            properAngle -= Mathf.Rad2Deg * Mathf.Atan(forwardPos.y / Vector2.Distance(Vector2.zero, forwardPos2D));
            return properAngle;
        }


        float Manual_Elevation_Angle()
        {
            // Simply face the target.
            float directAngle;
            Vector3 localPos = turretBaseTransform.InverseTransformPoint(aimingScript.Target_Position);
            directAngle = Mathf.Rad2Deg * (Mathf.Asin((localPos.y - thisTransform.localPosition.y) / Vector3.Distance(thisTransform.localPosition, localPos)));
            return directAngle;
        }


        void Manual_Turn()
        {
            if (aimingScript.Cannon_Turn_Rate != 0.0f)
            {
                isTurning = true;
            }

            // Calculate the "Turn_Rate".
            float targetTurnRate = aimingScript.Cannon_Turn_Rate;
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
            angleX += Speed_Mag * Turn_Rate * Time.fixedDeltaTime;
            angleX = Mathf.Clamp(angleX, Max_Elevation, Max_Depression);
            currentLocalAngles.x = angleX;
            thisTransform.localEulerAngles = currentLocalAngles;
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            // Depress the cannon.
            currentLocalAngles.x = Max_Depression;
            thisTransform.localEulerAngles = currentLocalAngles;

            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}