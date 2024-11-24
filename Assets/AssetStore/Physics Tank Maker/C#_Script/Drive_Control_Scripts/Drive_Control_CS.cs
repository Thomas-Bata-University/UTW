using System.Collections;
using FishNet.Object;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace ChobiAssets.PTM
{

    public class Drive_Control_CS : NetworkBehaviour
    {

        /*
		 * This script is attached to the "MainBody" of the tank.
		 * This script controls the driving of the tank, such as speed, torque, acceleration and so on.
		 * This script works in combination with "Drive_Wheel_Parent_CS" in the 'Create_##Wheels objects', and "Drive_Wheel_CS" in the drive wheels.
		*/

        // User options >>
        public float Torque = 2000.0f;
        public float Max_Speed = 8.0f;
        public float Turn_Brake_Drag = 150.0f;
        public float Switch_Direction_Lag = 0.5f;
        public bool Allow_Pivot_Turn;
        public float Pivot_Turn_Rate = 0.3f;

        public bool Acceleration_Flag = false;
        public float Acceleration_Time = 4.0f;
        public float Deceleration_Time = 0.1f;
        public AnimationCurve Acceleration_Curve;

        public bool Torque_Limitter = false;
        public float Max_Slope_Angle = 45.0f;

        public float Parking_Brake_Velocity = 0.5f;
        public float Parking_Brake_Angular_Velocity = 0.1f;

        public bool Use_AntiSlip = false;
        public float Ray_Distance = 1.0f;
        public float AntiSlip_Min_Speed = 2.0f;
        public float AntiSlip_Max_Speed = 32.0f;

        public bool Use_Downforce = false;
        public float Downforce = 25000.0f;
        public AnimationCurve Downforce_Curve;

        public bool Sync_Speed_Rate;
        public float Actual_Speed_Offset_Rate = 1.0f;
        public float Actual_Speed_Tolerance_Rate = 0.2f;

        public bool Support_Turning = true;
        public float Support_Turning_Force = 10000.0f;
        // << User options


        // Set by "inputType_Settings_CS".
        public int inputType = 0;

        // Set by "Drive_Control_Input_##_##_CS" scripts.
        public bool Stop_Flag = true; // Referred to from "Steer_Wheel_CS".
        public float L_Input_Rate;
        public float R_Input_Rate;
        public float Turn_Brake_Rate;
        public bool Pivot_Turn_Flag;
        public bool Apply_Brake;

        // Referred to from "Drive_Wheel_Parent_CS".
        public float Speed_Rate; // Referred to from also "inputType_Settings_CS".
        public float L_Brake_Drag;
        public float R_Brake_Drag;
        public float Left_Torque;
        public float Right_Torque;

        // Referred to from "Fix_Shaking_Rotation_CS".
        public bool Parking_Brake;

        // Referred to from "AI_CS", "AI_Hand_CS", "UI_Speed_Indicator_Control_CS".
        public float Current_Velocity;

        Transform thisTransform;
        Rigidbody thisRigidbody;
        float leftSpeedRate;
        float rightSpeedRate;
        float defaultTorque;
        float acceleRate;
        float deceleRate;
        int currentStep;
        bool switchDirectionTimerFlag;

        bool isSelected;

        protected Drive_Control_Input_00_Base_CS inputScript;

        public Static_Wheel_Parent_CS sprocketWheel;
        public Static_Wheel_Parent_CS idlerWheel;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisTransform = transform.parent;
            thisRigidbody = GetComponent<Rigidbody>();
            defaultTorque = Torque;

            // Get the input type.
            if (inputType != 10)
            { // This tank is not an AI tank.
                inputType = General_Settings_CS.Input_Type;
            }

            // Set the acceleration rates.
            if (Acceleration_Flag)
            {
                acceleRate = 1.0f / Acceleration_Time;
                deceleRate = 1.0f / Deceleration_Time;
            }

            // Check the 'Downforce_Curve'.
            if (Use_Downforce && Downforce_Curve.keys.Length < 2)
            { // The 'Downforce_Curve' is not set yet.
                Downforce_Curve = Create_Curve();
            }

            // Check the 'Acceleration_Curve'.
            if (Acceleration_Flag && Acceleration_Curve.keys.Length < 2)
            { // The 'Acceleration_Curve' is not set yet.
                Acceleration_Curve = Create_Curve();
            }

            // Set the input script.
            Set_Input_Script(inputType);

            // Prepare the input script.
            if (inputScript != null)
            {
                inputScript.Prepare(this);
            }

            sprocketWheel.isOwner = IsOwner;
            idlerWheel.isOwner = IsOwner;
        }


        protected virtual void Set_Input_Script(int type)
        {
            switch (type)
            {
                case 0: // Mouse + Keyboard (Stepwise)
                    inputScript = gameObject.AddComponent<Drive_Control_Input_01_Keyboard_Stepwise_CS>();
                    break;

                case 1: // Mouse + Keyboard (Pressing)
                    inputScript = gameObject.AddComponent<Drive_Control_Input_02_Keyboard_Pressing_CS>();
                    break;

                case 2: // Gamepad (Single stick)
                    inputScript = gameObject.AddComponent<Drive_Control_Input_03_Single_Stick_CS>();
                    break;

                case 3: // Gamepad (Twin sticks)
                    inputScript = gameObject.AddComponent<Drive_Control_Input_04_Twin_Sticks_CS>();
                    break;

                case 4: // Gamepad (Triggers)
                    inputScript = gameObject.AddComponent<Drive_Control_Input_05_Triggers_CS>();
                    break;

                case 10: // AI
                    inputScript = gameObject.AddComponent<Drive_Control_Input_99_AI_CS>();
                    break;
            }
        }


        AnimationCurve Create_Curve()
        { // Create a temporary AnimationCurve.
            Debug.LogWarning("'Curve' is not set correctly in 'Drive_Control_CS'.");
            Keyframe key1 = new Keyframe(0.0f, 0.0f, 1.0f, 1.0f);
            Keyframe key2 = new Keyframe(1.0f, 1.0f, 1.0f, 1.0f);
            return new AnimationCurve(key1, key2);
        }

        [ServerRpc(RequireOwnership = false)]
        private void Sync(float vertical, float horizontal, Vector2 sprocket, Vector2 idler) {
            SyncClient(vertical, horizontal, sprocket, idler);
        }

        [ObserversRpc]
        private void SyncClient(float vertical, float horizontal, Vector2 sprocket, Vector2 idler) {
            if (IsOwner || inputScript is null) return;
            inputScript.vertical = vertical;
            inputScript.horizontal = horizontal;

            sprocketWheel.Left_Angle_Y = sprocket.X;
            sprocketWheel.Right_Angle_Y = sprocket.Y;
            idlerWheel.Left_Angle_Y = idler.X;
            idlerWheel.Right_Angle_Y = idler.Y;
        }

        void Update()
        {
            inputScript.Drive_Input(IsOwner && isSelected);

            if (isSelected || inputType == 10)
            { // The tank is selected, or AI.
                // inputScript.Drive_Input(IsOwner);
            }

            if (IsClient) {
                Vector2 sprocket = new Vector2(sprocketWheel.Left_Angle_Y, sprocketWheel.Right_Angle_Y);
                Vector2 idler = new Vector2(idlerWheel.Left_Angle_Y, idlerWheel.Right_Angle_Y);

                Sync(inputScript.vertical, inputScript.horizontal, sprocket, idler);
            }

            // Set the driving values, such as speed rate, brake drag and torque.
            Set_Driving_Values();
        }



        void FixedUpdate()
        {
            // Get the current velocity values;
            Current_Velocity = thisRigidbody.velocity.magnitude;

            // Control the automatic parking brake.
            Control_Parking_Brake();

            // Call anti-spinning function.
            Anti_Spin();

            // Call anti-slipping function.
            if (Use_AntiSlip)
            {
                Anti_Slip();
            }

            // Limit the torque in slope.
            if (Torque_Limitter)
            {
                Limit_Torque();
            }

            // Add downforce.
            if (Use_Downforce)
            {
                Add_Downforce();
            }

            // Support turning.
            if (Support_Turning)
            {
                Support_Brake_Turn();
            }
        }

        
        void Set_Driving_Values()
        {
            if (Acceleration_Flag)
            {
                // Synchronize the virtual speed and the actual speed.
                if (Sync_Speed_Rate)
                {
                    Synchronize_Speed_Rate();
                }

                // Set the "leftSpeedRate" and "rightSpeedRate".
                leftSpeedRate = Calculate_Speed_Rate(leftSpeedRate, -L_Input_Rate);
                rightSpeedRate = Calculate_Speed_Rate(rightSpeedRate, R_Input_Rate);
                if (inputType == 10)
                { // AI tank.

                    // Synchronize the left and right speed rates to improve the straightness.
                    if (Stop_Flag == false && L_Input_Rate == -R_Input_Rate)
                    { // Not stopping && Going straight.

                        // Set the average value to the both sides.
                        float averageRate = (leftSpeedRate + rightSpeedRate) * 0.5f;
                        leftSpeedRate = averageRate;
                        rightSpeedRate = averageRate;
                    }
                }
            }
            else
            {
                leftSpeedRate = -L_Input_Rate;
                rightSpeedRate = R_Input_Rate;
            }

            // Set the "Speed_Rate" value.
            Speed_Rate = Mathf.Max(Mathf.Abs(leftSpeedRate), Mathf.Abs(rightSpeedRate));
            Speed_Rate = Acceleration_Curve.Evaluate(Speed_Rate);

            // Check the pivot-turn.
            if (Pivot_Turn_Flag)
            { // The tank is doing pivot-turn.

                // Clamp the speed rate.
                Speed_Rate = Mathf.Clamp(Speed_Rate, 0.0f, Pivot_Turn_Rate);
            }

            // Set the "L_Brake_Drag" and "R_Brake_Drag".
            L_Brake_Drag = Mathf.Clamp(Turn_Brake_Drag * -Turn_Brake_Rate, 0.0f, Turn_Brake_Drag);
            R_Brake_Drag = Mathf.Clamp(Turn_Brake_Drag * Turn_Brake_Rate, 0.0f, Turn_Brake_Drag);

            // Set the "Left_Torque" and "Right_Torque".
            Left_Torque = Torque * -Mathf.Sign(leftSpeedRate) * Mathf.Ceil(Mathf.Abs(leftSpeedRate)); // (Note.) When the "leftSpeedRate" is zero, the torque will be set to zero.
            Right_Torque = Torque * Mathf.Sign(rightSpeedRate) * Mathf.Ceil(Mathf.Abs(rightSpeedRate));
        }


        void Synchronize_Speed_Rate()
        {
            // Synchronize the virtual speed and the actual speed.

            if (Stop_Flag == false && Pivot_Turn_Flag == false && Turn_Brake_Rate == 0.0f)
            { // Not stopping, Not pivot-turning, Not brake-turning.

                // Get the current actual speed rate.
                var currentActualSpeedRate = Current_Velocity / Max_Speed;

                // Check the difference between the virtual speed and the actual speed.
                var diffValue = (Speed_Rate * Actual_Speed_Offset_Rate) - currentActualSpeedRate;
                if (diffValue > Actual_Speed_Tolerance_Rate)
                { // The difference is large.

                    // Check the tilt.
                    var currentTiltAngle = Mathf.Abs(Mathf.DeltaAngle(thisTransform.eulerAngles.x, 0.0f));
                    if (currentTiltAngle < 5.0f)
                    { // Almost horizontal.

                        // Make the virtual speed approximate to the actual speed.
                        if (leftSpeedRate != 0.0f)
                        {
                            leftSpeedRate = Mathf.MoveTowards(leftSpeedRate, currentActualSpeedRate * Mathf.Sign(leftSpeedRate), 40.0f * Time.deltaTime);
                        }
                        if (rightSpeedRate != 0.0f)
                        {
                            rightSpeedRate = Mathf.MoveTowards(rightSpeedRate, currentActualSpeedRate * Mathf.Sign(rightSpeedRate), 40.0f * Time.deltaTime);
                        }

                        // Debug.Log("Speed adjusted. " + diffValue);
                    }
                }
            }
        }


        float Calculate_Speed_Rate(float currentRate, float targetRate)
        {
            if (switchDirectionTimerFlag)
            {
                return 0.0f;
            }

            if (currentRate == targetRate)
            {
                return currentRate;
            }

            float moveRate;
            if (Apply_Brake)
            {
                moveRate = deceleRate * 10.0f;
            }
            else if (targetRate == 0.0f)
            {
                moveRate = deceleRate;
            }
            else if (Mathf.Sign(targetRate) == Mathf.Sign(currentRate))
            { // The both rates have the same direction.
                if (Mathf.Abs(targetRate) > Mathf.Abs(currentRate))
                { // It should be in acceleration.
                    moveRate = acceleRate;
                }
                else
                { // It should be in deceleration.
                    moveRate = deceleRate;
                }
            }
            else
            { // The both rates have different directions. >> It should be in deceleration until the currentRate becomes zero.
                
                // Decrease the speed rapidly like a brake.
                moveRate = deceleRate * 10.0f;

                // Stop the tank while switching the direction.
                if (inputType != 10)
                { // Not AI tank.
                    var tempRate = Mathf.MoveTowards(currentRate, targetRate, moveRate * Time.deltaTime);
                    if ((currentRate > 0.0f && tempRate <= 0.0f) || (currentRate <= 0.0f && tempRate > 0.0f))
                    { // From forward to backward, or from backward to forward.
                        StartCoroutine("Switch_Direction_Timer");
                        return tempRate;
                    }
                }
            }

            return Mathf.MoveTowards(currentRate, targetRate, moveRate * Time.deltaTime);
        }


        IEnumerator Switch_Direction_Timer()
        {
            switchDirectionTimerFlag = true;
            var count = 0.0f;
            while (count < Switch_Direction_Lag)
            {
                count += Time.deltaTime;
                yield return null;
            }
            switchDirectionTimerFlag = false;
        }


        void Control_Parking_Brake()
        {
            // Check there is no input for driving.
            if (Stop_Flag)
            {
                // Get the angular velocity of the Rigidbody.
                var currentAngularVelocityMagnitude = thisRigidbody.angularVelocity.magnitude;

                // Check the parking brake is working now.
                if (Parking_Brake)
                {
                    // Check the tank dose not stop.
                    if (Current_Velocity > Parking_Brake_Velocity || currentAngularVelocityMagnitude > Parking_Brake_Angular_Velocity)
                    {
                        // Release the parking brake.
                        Parking_Brake = false;
                        thisRigidbody.constraints = RigidbodyConstraints.None;
                        return;
                    }
                    return;
                }
                else
                { // The parking brake is not working now.

                    // Check the tank almost stops.
                    if (Current_Velocity < Parking_Brake_Velocity && currentAngularVelocityMagnitude < Parking_Brake_Angular_Velocity)
                    {
                        // Put on the parking brake.
                        Parking_Brake = true;
                        thisRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
                        leftSpeedRate = 0.0f;
                        rightSpeedRate = 0.0f;
                        return;
                    }
                    return;
                }
            }
            else
            { // There is any inputs for driving.

                // Check the parking brake is working now.
                if (Parking_Brake)
                {
                    // Release parking brake.
                    Parking_Brake = false;
                    thisRigidbody.constraints = RigidbodyConstraints.None;
                }
            }
        }


        void Anti_Spin()
        {
            // Reduce the spinning motion by controling the angular velocity of the Rigidbody.
            if (Pivot_Turn_Flag == false && Turn_Brake_Rate == 0.0f)
            { // The tank should not be doing pivot-turn or brake-turn.

                // Reduce the angular velocity on Y-axis.
                var currentAngularVelocity = thisRigidbody.angularVelocity;
                currentAngularVelocity.y *= 0.9f;

                // Set the new angular velocity.
                thisRigidbody.angularVelocity = currentAngularVelocity;
            }
        }



        void Anti_Slip()
        {
            // Reduce the slippage by controling the velocity of the Rigidbody.

            // Cast a ray downward to detect the ground.
            var ray = new Ray();
            ray.origin = thisTransform.position;
            ray.direction = -thisTransform.up;
            if (Physics.Raycast(ray, Ray_Distance, Layer_Settings_CS.Anti_Slipping_Layer_Mask) == true)
            { // The ray hits the ground.

                // Control the velocity of the Rigidbody.
                Vector3 tempVelocity = thisRigidbody.velocity;
                if (Speed_Rate == 0.0f)
                { // The tank stops.

                    // Reduce the Rigidbody velocity gradually.
                    tempVelocity.x *= 0.9f;
                    tempVelocity.z *= 0.9f;
                }
                else
                { // The tank should been driving.
                    
                    // Get the direction from the rigidbody velocity.
                    var sign = Mathf.Sign(thisTransform.InverseTransformDirection(thisRigidbody.velocity).z);

                    // Change the velocity of the Rigidbody forcibly.
                    var maxDistanceDelta = Mathf.Lerp(AntiSlip_Max_Speed, AntiSlip_Min_Speed, Current_Velocity / Max_Speed) * Time.fixedDeltaTime;
                    tempVelocity = Vector3.MoveTowards(tempVelocity, thisTransform.forward * sign * Current_Velocity, maxDistanceDelta);
                }

                // Set the new velocity.
                thisRigidbody.velocity = tempVelocity;
            }
        }


        void Limit_Torque()
        {
            // Reduce the torque according to the angle of the slope.
            var torqueRate = Mathf.DeltaAngle(thisTransform.eulerAngles.x, 0.0f) / Max_Slope_Angle;
            if (leftSpeedRate > 0.0f && rightSpeedRate > 0.0f)
            { // The tank should be going forward.
                Torque = Mathf.Lerp(defaultTorque, 0.0f, torqueRate);
            }
            else
            { // The tank should be going backward.
                Torque = Mathf.Lerp(defaultTorque, 0.0f, -torqueRate);
            }
        }


        void Add_Downforce()
        {
            // Add downforce.
            var downforceRate = Downforce_Curve.Evaluate(Current_Velocity / Max_Speed);
            thisRigidbody.AddRelativeForce(Vector3.up * (-Downforce * downforceRate));
        }


        void Support_Brake_Turn()
        {
            // Check brake-turning now, or trying pivot-turn at overspeed.
            if (Turn_Brake_Rate == 0.0f || (L_Input_Rate == 0.0f && R_Input_Rate == 0.0f))
            {
                return;
            }

            // Set the position to add force.
            var supportPos = thisTransform.position;
            supportPos += -thisTransform.right * (8.0f * Mathf.Sign(Turn_Brake_Rate));

            // Set the support force according to the speed. (Low speed >> High support)
            var supportForceRate = (1.0f - Speed_Rate) * -Mathf.Sign(L_Input_Rate - R_Input_Rate);

            // Add the support force.
            thisRigidbody.AddForceAtPosition(thisTransform.forward * Support_Turning_Force * supportForceRate, supportPos);

            //Debug.Log(supportForceRate);
        }


        void Call_Indicator()
        {
            // Call "UI_Speed_Indicator_Control_CS" in the scene.
            if (UI_Speed_Indicator_Control_CS.Instance)
            {
                bool isManual = (General_Settings_CS.Input_Type == 0); // "Mouse + Keyboard (Stepwise)".
                UI_Speed_Indicator_Control_CS.Instance.Get_Drive_Script(this, isManual, currentStep);
            }
        }


        public void Shift_Gear(int currentStep)
        { // Called from "Drive_Control_Input_01_Keyboard_Stepwise_CS".
            this.currentStep = currentStep;

            // Call "UI_Speed_Indicator_Control_CS" in the scene.
            if (UI_Speed_Indicator_Control_CS.Instance)
            {
                UI_Speed_Indicator_Control_CS.Instance.Get_Current_Step(currentStep);
            }
        }


        void Get_AI_CS()
        { // Called from "AI_CS".
            inputType = 10;
        }


        public void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            this.isSelected = isSelected;

            if (isSelected)
            {
                Call_Indicator();
            }
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(inputScript as Object);
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}