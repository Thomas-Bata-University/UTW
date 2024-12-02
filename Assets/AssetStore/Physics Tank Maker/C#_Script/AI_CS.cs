using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace ChobiAssets.PTM
{
	
	public class AI_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the "AI_Core" in the AI tank.
		 * This script controls the behavior of the AI tank.
		 * This script works in combination with "AI_Settings_CS" in the top object, and "AI_Hand_CS" in the child object.
		 * This script requires also "AI_Headquaters_CS" in the scene to get the taget information.
		*/


		// User options >>
		// Drive settings.
		public float WayPoint_Radius = 10.0f;
		public float Pivot_Turn_Angle = 45.0f;
		public float Min_Target_Angle = 2.5f;
		public float Min_Turn_Rate = 0.0f;
		public float Max_Turn_Rate = 1.0f;
		public float Min_Speed_Rate = 0.0f;
		public float Max_Speed_Rate = 1.0f;
		public float SlowDown_Range = 15.0f;
		public GameObject NavMeshAgent_Prefab;
		public float Agent_Distance = 5.0f;
		public float Agent_Additional_Distance = 5.0f;
		public GameObject Obstacle_Prefab;
		public float Stuck_Count = 2.0f; // Referred to from "AI_Hand"
		// Combat settings.
		public float Fire_Angle = 2.0f; // Referred to from "Aiming_Control_CS".
		public bool Direct_Fire = true; // Referred to from "Cannon_Fire_CS".
		public float Fire_Count = 0.5f; // Referred to from "Cannon_Fire_CS".
		// << User options


		Transform thisTransform;
        Transform eyeTransform;
        NavMeshAgent navAgent;
        Transform navAgentTransform;
        Aiming_Control_CS aimingScript;
		public AI_Settings_CS Settings_Script; // Referred to from "AI_Headquaters_CS".
        float navAgentDistance;
        float updateDestinationCount;
        float updateDestinationPeriod = 1.0f;
        public bool Is_Sharing_Target; // Set by "AI_Share_Target_CS", and referred to from "AI_Headquaters_CS" in the scene.

		// Target informations.
		public Transform Target_Transform; // Referred to from "AI_Share_Target_CS".
        Transform targetRootTransform;
        float targetUpperOffset;

		// For actions.
		public int Action_Type; // 0 = Defensive, 1 = Offensive. // Referred to from "UI_PosMarker_Control_CS" and "UI_AIState_Control_CS".
        int defensiveType; // 0 = Waypoint mode, 1 = Follow mode.
        Transform[] wayPoints;
        int nextWayPoint = -1;
        Vector3 lookAtPosition;
        bool isStaying = false;
        Transform followBodyTransform;
        float followDistance = 15.0f;

        // For searching the target.
        float searchingCount;
        float searchingInterval = 1.0f;
        float targetDistance;
		public bool Detect_Flag; // Referred to from "UI_AIState_Control_CS", "AI_Headquaters_CS".
		public float Losing_Count; // Referred to from "UI_AIState_Control_CS".
        public bool Wakeful_Flag;
        int Wake_Up_Timer_ID;

        // For offensive actions.
        public bool OpenFire_Flag; // Referred to from "Cannon_Fire_CS".
        bool GunsCanAim;
        Bullet_Generator_CS[] bulletGeneratorScripts;
        float castRayCount;
        bool hasApproached;

        // For driving.
        AI_Hand_CS handScript;
        bool isEscapingFromStuck = false;
        float drivingTargetAngle;
        float drivingTargetDistance;
        float drivingNextCornerAngle;
        public float Speed_Order; // Referred to from "Drive_Control_CS".
		public float Turn_Order; // Referred to from "Drive_Control_CS".
        float currentMaxSpeedRate;

        // For navigation.
        Drive_Control_CS driveControlScript;


        void Awake()
        {
            // Broadcast this reference to all the tank parts from the top object.
            transform.parent.parent.BroadcastMessage("Get_AI_CS", this, SendMessageOptions.DontRequireReceiver);
        }


        void Start()
		{
			Initialize();
		}


        void Initialize ()
		{
			thisTransform = transform;

            // Get "AI_Settings_CS" in the top object.
            Settings_Script = GetComponentInParent<AI_Settings_CS>();
            if (Settings_Script == null)
            {
                Debug.LogError("'AI_CS' requires 'AI_Settings_CS' in the top object.");
                Destroy(this);
                return;
            }

            // Get "Aiming_Control_CS".
            aimingScript = GetComponentInParent<Aiming_Control_CS>();

            // Get "AI_Eye"
            eyeTransform = thisTransform.Find("AI_Eye"); // (Note.) Do not rename "AI_Eye".
            if (eyeTransform == null)
            {
                Debug.LogError("'AI_Eye' can not be found. ");
                Destroy(this);
            }

            // Get "AI_Hand_CS" script.
            handScript = thisTransform.GetComponentInChildren<AI_Hand_CS>();

            // Get all the "Bullet_Generator_CS" in the tank.
            bulletGeneratorScripts = thisTransform.parent.GetComponentsInChildren<Bullet_Generator_CS>();

            // Instantiate NavMeshAgent object.
            GameObject agentObject;
            if (NavMeshAgent_Prefab)
            {
                agentObject = Instantiate(NavMeshAgent_Prefab, thisTransform.position, thisTransform.rotation, thisTransform.parent.parent) as GameObject;
                navAgent = agentObject.GetComponent<NavMeshAgent>();
            }
            else
            {
                agentObject = new GameObject("AI_NavMeshAgent_Object");
                agentObject.transform.SetPositionAndRotation(thisTransform.position, thisTransform.rotation);
                agentObject.transform.parent = thisTransform.parent.parent;
                navAgent = agentObject.AddComponent<NavMeshAgent>();
            }
            navAgentTransform = agentObject.transform;
            navAgent.acceleration = 120.0f;

            // 'Follow_Target' settings.
            if (Settings_Script.Follow_Target)
            {
                Get_Follow_Transform();
            }
            else
            { //  // The "Follow_Target" is not set.
                // Set up the waypoints.
                Set_WayPoints();
                // Set the first waypoint.
                Update_Next_WayPoint();
            }

            // 'AI_State_Text' settings.
            if (Settings_Script.AI_State_Text)
            {
                UI_AIState_Control_CS stateScript = Settings_Script.AI_State_Text.GetComponent<UI_AIState_Control_CS>();
                if (stateScript == null)
                {
                    stateScript = Settings_Script.AI_State_Text.gameObject.AddComponent<UI_AIState_Control_CS>();
                }
                stateScript.Get_AI_Script(this);
            }

            // 'Commander' settings.
            if (Settings_Script.Commander)
            {
                AI_Share_Target_CS shareTargetScript = gameObject.AddComponent<AI_Share_Target_CS>();
                shareTargetScript.AI_Script = this;
            }

            // Get the "Drive_Control_CS".
            driveControlScript = GetComponentInParent<Drive_Control_CS>();

            // Set the current max speed rate.
            currentMaxSpeedRate = Max_Speed_Rate * Settings_Script.Patrol_Speed_Rate;
        }


        void Set_WayPoints ()
		{
            if (Settings_Script.WayPoint_Pack)
            {
                int childCount = Settings_Script.WayPoint_Pack.transform.childCount;
                if (childCount > 1)
                { // The "WayPoint_Pack" has more than two waypoints.
                    wayPoints = new Transform[childCount];
                    for (int i = 0; i < childCount; i++)
                    {
                        wayPoints[i] = Settings_Script.WayPoint_Pack.transform.GetChild(i);
                    }
                    return;
                }
                else if (childCount == 1)
                { // The "WayPoint_Pack" has only one waypoint.
                    wayPoints = new Transform[1];
                    wayPoints[0] = Settings_Script.WayPoint_Pack.transform.GetChild(0);
                    return;
                }
            }
            // The "WayPoint_Pack" has no point, or is not assigined.

            // Create a new waypoint.
            wayPoints = new Transform[1];
            GameObject newWayPoint = new GameObject("Waypoint (1)");
            newWayPoint.transform.parent = thisTransform.root;
            newWayPoint.transform.position = thisTransform.position;
            newWayPoint.transform.rotation = thisTransform.rotation;
            wayPoints[0] = newWayPoint.transform;
        }


        void Get_Follow_Transform()
        { // This function is called also when the follow target tank has been respawned.
            if (Settings_Script.Follow_Target == null)
            { // The "Follow_Target" might have been removed from the scene.
                // Change to waypoint mode.
                defensiveType = 0; // Waypoint mode.
                isStaying = false;
                // Set up the waypoints.
                Set_WayPoints();
                // Set the first waypoint.
                Update_Next_WayPoint();
                return;
            }

            // Get the MainBody of the follow target tank.
            Rigidbody followRigidbody = Settings_Script.Follow_Target.GetComponentInChildren<Rigidbody>();
            if (followRigidbody)
            { // The "Follow_Target" has a rigidbody. >> The rigidbody should be in the MainBody of the follow target tank.
                defensiveType = 1; // Follow mode.
                // Assign the MainBody of the follow target tank to the "followBodyTransform".
                followBodyTransform = followRigidbody.transform;
            }
            else
            { // The "Follow_Target" has no rigidbody.
                Debug.LogError("'Follow Target' has no rigidbody. AI cannot follow the target tank.");
                // Change to waypoint mode.
                Update_Next_WayPoint();
            }
        }


        void Update_Next_WayPoint()
		{
            switch (Settings_Script.Patrol_Type)
            {
                case 0: // Order
                    nextWayPoint += 1;
                    if (nextWayPoint >= wayPoints.Length)
                    {
                        nextWayPoint = 0;
                    }
                    break;

                case 1: // Random
                    nextWayPoint = Random.Range(0, wayPoints.Length);
                    break;
            }

            // Update the destination of the NavMeshAgent.
            navAgent.SetDestination(wayPoints[nextWayPoint].position);
        }


        void Update()
		{
            // Control the NavMeshAgent.
            navAgentDistance = Vector3.Distance(navAgentTransform.position, thisTransform.position);
			if (navAgentDistance > Agent_Distance + Agent_Additional_Distance)
            { // The NavMeshAgent is too far.
                // Reset the position.
                navAgent.nextPosition = thisTransform.position;
			}
			else
            {
                // Control the speed to keep the distance from the tank.
                float tempRate = Mathf.Pow(driveControlScript.Current_Velocity / driveControlScript.Max_Speed, 2.0f);
                tempRate = Mathf.Clamp(tempRate, 0.0f, 1.0f);
                navAgent.speed = Mathf.Lerp(64.0f, 0.0f, navAgentDistance / (Agent_Distance + (Agent_Additional_Distance * tempRate)));
			}

            // Search the target.
            if (Target_Transform)
            {
                Search_Target();
            }

            // Action.
            switch (Action_Type)
            {
                case 0: // Defensive.
                    switch (defensiveType)
                    {
                        case 0: // Waypoint mode.
                            WayPoint_Mode();
                            break;
                        case 1: // Follow mode.
                            Follow_Mode();
                            break;
                    }
                    break;

                case 1: // Offensive.
                    if (Target_Transform)
                    { // The target exists.
                        if (Settings_Script.Breakthrough == true)
                        {
                            // Keep going around the waypoaints while attacking the target.
                            Breakthrough_Mode();
                        }
                        else
                        {
                            // Chase the target while attaking it.
                            Chase_Mode();
                        }
                    }
                    else
                    { // The target does not exist. >> The target might be respawned, or be removed from the scene.
                        Lost_Target();
                        return;
                    }
                    break;
            }

            // Auto brake function.
            if (handScript && handScript.Is_Working)
            { // An obstacle is detected in fornt of the tank.
                drivingTargetDistance = 0.0f; // The tank will stop, and will be allowed only to turn.
            }

            // Calculate the "Speed_Order" and "Turn_Order" for driving.
            Auto_Drive();
        }


        void Search_Target()
		{
            // Check the interval.
            searchingCount -= Time.deltaTime;
            if (searchingCount > 0.0f)
            {
                return;
            }
            searchingCount = searchingInterval;

            // Check the target state.
            if (targetRootTransform.tag == "Finish")
            { // The target is already dead.
                Lost_Target();
                // Call "AI_Headquaters_CS" to get a new order.
                if (AI_Headquaters_CS.Instance)
                {
                    AI_Headquaters_CS.Instance.Give_Order();
                }
                return;
            }

            // Detect the target.
            Vector3 targetPosition = Target_Transform.position + (Target_Transform.up * targetUpperOffset);
            targetDistance = Vector3.Distance(eyeTransform.position, targetPosition);
            if (targetDistance < Settings_Script.Visibility_Radius || Wakeful_Flag || Is_Sharing_Target)
            { // The target is within the "Visibility_Radius", or right after being shot.
                if (Cast_Line_For_Searching(targetPosition) == true || Is_Sharing_Target)
                { // The target is detected from the "AI_Eye".
                    Detect_Flag = true;
                    switch (Action_Type)
                    {
                        case 0: // Defensive.
                            Action_Type = 1;
                            if (Settings_Script.Breakthrough == false)
                            { // Not breakthrough >> Chase the target.
                                isStaying = false;
                                // Move the NavMeshAgent to this position, so that the NavMeshAgent can find a new path smoothly.
                                navAgent.Warp(thisTransform.position);
                                // Set the 'updateDestinationCount', so that the destination of the NavMeshAgent can be updated soon.
                                updateDestinationCount = Mathf.Infinity;
                                // Change the current max speed rate.
                                currentMaxSpeedRate = Max_Speed_Rate * Settings_Script.Combat_Speed_Rate;
                            }

                            // Call the "Aiming_Control_CS" to lock on the target.
                            if (aimingScript)
                            {
                                aimingScript.AI_Lock_On(Target_Transform);
                            }
                            break;

                        case 1: // Offensive..
                            // Continue to chase the target.
                            Losing_Count = Settings_Script.Lost_Count;
                            break;
                    }
                    return;
                }
                // The target is not detected from the "AI_Eye".

            }
            // The target is out of "Visibility_Radius".

            // The target cannot be detected.
            Detect_Flag = false;
            switch (Action_Type)
            {
                case 0: // Defensive.
                    break;

                case 1: // Offensive..
                    Losing_Count -= Time.deltaTime + searchingInterval;
                    if (Losing_Count < 0.0f)
                    { // The AI has lost the target.
                        Lost_Target();
                    }
                    break;
            }
        }


        bool Cast_Line_For_Searching(Vector3 targetPosition)
		{
            // Check the dead angle.
            if (Action_Type == 0 && Wakeful_Flag == false)
            { // Defensive && not attacked.
                // Get the angle to the target.
                float theta = Mathf.Abs(Calculate_2D_Angle(targetPosition));
                if (180.0f - theta < Settings_Script.Dead_Angle)
                { // The target is in the dead angle.
                    return false;
                }
            }

            // Cast a line from the "AI_Eye" to the target.
            RaycastHit raycastHit;
            if (Physics.Linecast(eyeTransform.position, targetPosition, out raycastHit, Layer_Settings_CS.Layer_Mask))
            {
                if (raycastHit.transform.root == targetRootTransform)
                { // The ray hits the target.
                    return true;
                }
                else
                { // The ray hits other object.
                    return false;
                }
            }
            else
            { // The ray does not hit anything. >> There is no obstacle between the eye and the target.
                return true;
            }
        }


        public IEnumerator Wake_Up_Timer()
        { // Called from "Damage_Control_Center_CS", when the AI tank is attacked.
            Wakeful_Flag = true;
            Wake_Up_Timer_ID += 1;
            int thisTimerID = Wake_Up_Timer_ID;

            float count = 0.0f;
            while (count < 5.0f)
            {
                if (thisTimerID != Wake_Up_Timer_ID)
                { // The tank should be attacked again.
                    yield break;
                }

                count += Time.deltaTime;
                yield return null;
            }

            Wake_Up_Timer_ID = 0;
            Wakeful_Flag = false;
        }


        void WayPoint_Mode()
		{
            // Update the destination.
            updateDestinationCount += Time.deltaTime;
            if (updateDestinationCount > updateDestinationPeriod)
            {
                navAgent.SetDestination(wayPoints[nextWayPoint].position);
                updateDestinationCount = 0.0f;
            }

            // Check the distance to the waypoint.
            if (wayPoints.Length > 1)
            { // There are more than two waypoints.
                if (Vector3.Distance(thisTransform.position, wayPoints[nextWayPoint].position) < WayPoint_Radius)
                { // The tank has arriveded at the next waypoint.
                    // Update the next waypoint.
                    Update_Next_WayPoint();
                }
                else
                { // The tank does not arrive at the next waypoint.
                    // Move to the NavMeshAgent.
                    Set_Driving_Target_Angle_And_Distance();
                }
            }
            else
            { // There is only one waypoint.
                float distanceToWaypoint = Vector3.Distance(thisTransform.position, wayPoints[0].position);
                if (isStaying)
                { // The tank is staying now.
                    if (distanceToWaypoint > WayPoint_Radius + 5.0f)
                    { // The tank has moved away from the waypoint.
                        isStaying = false;
                    }
                }
                else
                { // The tank is not staying now.
                    if (distanceToWaypoint < WayPoint_Radius)
                    { // The tank has arrived at the waypoint.
                        // Set the "lookAtPosition".
                        lookAtPosition = wayPoints[0].position + (wayPoints[0].forward * 100.0f);
                        isStaying = true;
                    }
                }

                // Set the "drivingTargetAngle" and "drivingTargetDistance".
                if (isStaying)
                { // The tank is near the waypoint.
                    // Face the same direction as the waypoint.
                    drivingTargetAngle = Calculate_2D_Angle(lookAtPosition);
                    // Stay.
                    drivingTargetDistance = 0.0f;
                }
                else
                { // The tank is away from the waypoint.
                    // Move to the NavMeshAgent.
                    Set_Driving_Target_Angle_And_Distance();
                }
            }
        }


        void Follow_Mode()
        {
            // Check the follow target exists.
            if (followBodyTransform == null)
            {
                Get_Follow_Transform(); // The "followBodyTransform" might have been lost by respawning.
                return;
            }

            // Update the destination.
            updateDestinationCount += Time.deltaTime;
            if (updateDestinationCount > updateDestinationPeriod)
            {
                navAgent.SetDestination(followBodyTransform.position);
                updateDestinationCount = 0.0f;
            }

            // Check the distance to the follow target.
            float distanceToFollowTatget = Vector3.Distance(thisTransform.position, followBodyTransform.position);
            if (isStaying)
            { // The tank is staying now.
                if (distanceToFollowTatget < followDistance + 5.0f)
                { // The tank is staying near the target.
                    // Keep staying.
                    isStaying = true;
                    // Update the "lookAtPosition", so that the tank faces the same direction as the follow target.
                    lookAtPosition = followBodyTransform.position + (followBodyTransform.forward * 100.0f);
                    // Hold the NavMeshAgent in this position, so that the tank smoothly start following again.
                    navAgent.Warp(thisTransform.position);
                }
                else
                { // The tank has moved away from the follow target.
                    isStaying = false;
                }
            }
            else
            { // The tank is not staying now.
                if (distanceToFollowTatget < followDistance)
                { // The tank has arrived at near the follow target.
                    isStaying = true;
                    // Set the "lookAtPosition", so that the tank faces the same direction as the follow target.
                    lookAtPosition = followBodyTransform.position + (followBodyTransform.forward * 100.0f);
                }
            }

            // Set the "drivingTargetAngle" and "drivingTargetDistance".
            if (isStaying)
            { // The tank is staying near the target.
                // Face the same direction as the follow target.
                drivingTargetAngle = Calculate_2D_Angle(lookAtPosition);
                // Stay.
                drivingTargetDistance = 0.0f;
            }
            else
            { // The tank is away from the follow target.
                // Move to the NavMeshAgent.
                Set_Driving_Target_Angle_And_Distance();
                if (navAgent.path.corners.Length < 3)
                { // The next corner position must be the target position.
                    // Reduce the target distance so that the tank can follow the target smoothly.
                    drivingTargetDistance -= 15.0f;
                    drivingNextCornerAngle = 180.0f;
                }
            }
        }


        void Breakthrough_Mode()
		{
            // Keep the behavior in the defensive condition.
            switch (defensiveType)
            {
                case 0: // Waypoint mode.
                    WayPoint_Mode();
                    break;
                case 1: // Follow mode.
                    Follow_Mode();
                    break;
            }

            // Set the "OpenFire_Flag" referred to from "Cannon_Fire_CS".
            if (Detect_Flag == true)
            { // The target is detected.
                // Check the target is within the "OpenFire_Distance" or not.
                OpenFire_Flag = (targetDistance < Settings_Script.OpenFire_Distance);
            }
            else
            { // The target is not detected.
                OpenFire_Flag = false;
            }

            // Set "Can_Aim" in all the "Bullet_Generator_CS" referred to from "Cannon_Fire_CS".
            if (OpenFire_Flag == true)
            { // The target is detected and is within the "OpenFire_Distance"
                if (Direct_Fire == true)
                { // The tank aims a target directly.
                    castRayCount += Time.fixedDeltaTime;
                    if (castRayCount > 2.0f)
                    {
                        castRayCount = 0.0f;
                        Set_Can_Aim();
                    }
                }
            }
        }


        void Chase_Mode()
        {
            // Update the destination.
            updateDestinationCount += Time.deltaTime;
            if (updateDestinationCount > updateDestinationPeriod)
            {
                navAgent.SetDestination(Target_Transform.position);
                updateDestinationCount = 0.0f;
            }

            // Set the "OpenFire_Flag" referred to from "Cannon_Fire_CS".
            if (Detect_Flag == true)
            { // The target is detected.
                // Check the target is within the "OpenFire_Distance" or not.
                OpenFire_Flag = (targetDistance < Settings_Script.OpenFire_Distance);
            }
            else
            { // The target is not detected.
                OpenFire_Flag = false;
            }

            // Check all the guns can aim the target or not.
            if (OpenFire_Flag == true)
            { // The target is detected and is within the "OpenFire_Distance"
                if (Direct_Fire == true)
                { // The tank aims a target directly.
                    castRayCount += Time.fixedDeltaTime;
                    if (castRayCount > 2.0f)
                    {
                        castRayCount = 0.0f;
                        GunsCanAim = Set_Can_Aim();
                    }
                }
                else
                { // The tank aims a target indirectly like a howitzer.
                    GunsCanAim = true;
                }
            }
            else
            { // The target is not detected.
                GunsCanAim = false;
            }

            // Check the tank is within the "Approach_Distance" or not.
            if (targetDistance < Settings_Script.Approach_Distance)
            { // The target is within the "Approach_Distance".
                if (GunsCanAim || Settings_Script.Approach_Distance == Mathf.Infinity)
                { // At least one of the guns can aim the target, or the "Approach_Distance" is set to infinity.
                    hasApproached = true;
                }
                else
                { // Any gun can not aim the tagert.
                    hasApproached = false;
                }
            }
            else
            { // The target is out of 'Approach_Distance'.
                hasApproached = false;
            }

            // Set the "drivingTargetAngle" and "drivingTargetDistance".
            if (hasApproached)
            { // The tank has approached the target and can aim it.
                // Stay.
                drivingTargetDistance = 0.0f;

                // Get the angle to the target.
                drivingTargetAngle = Calculate_2D_Angle(Target_Transform.position);
                if(Settings_Script.Approach_Distance != Mathf.Infinity && Mathf.Abs(drivingTargetAngle) < 60.0f)
                { // The target is almost in front.
                    // Need not to face the target.
                    drivingTargetAngle = 0.0f;
                }
                else
                {
                    // Face the target.
                    drivingTargetAngle -= Settings_Script.Face_Offest_Angle * Mathf.Sign(drivingTargetAngle);
                }
            }
            else
            { // The tank has not approached the target yet, or cannot aim it.
                // Move to the NavMeshAgent.
                Set_Driving_Target_Angle_And_Distance();
            }
        }


        bool Set_Can_Aim()
		{
            // Set "Can_Aim" in all the "Bullet_Generator_CS" referred to from "Cannon_Fire_CS".
            // And check at least one of the guns can aim the target or not.
            bool flag = false;
            for (int i = 0; i < bulletGeneratorScripts.Length; i++)
            {
                if (bulletGeneratorScripts[i] == null)
                {
                    continue;
                }

                // Cast a line from the "Bullet_Generator" to the target.
                RaycastHit raycastHit;
                if (Physics.Linecast(bulletGeneratorScripts[i].transform.position, aimingScript.Target_Position, out raycastHit, Layer_Settings_CS.Layer_Mask))
                {
                    if (raycastHit.transform.root == targetRootTransform)
                    { // The line hits the target.
                        bulletGeneratorScripts[i].Can_Aim = true;
                        flag = true; // At least one of the "Bullet_Generator" can aim the target.
                    }
                    else
                    { // The line hits other object.
                        bulletGeneratorScripts[i].Can_Aim = false;
                    }
                }
                else
                { // The line does not hit anyhing. >> There is no obstacle between the muzzle and the target.
                    bulletGeneratorScripts[i].Can_Aim = true;
                    flag = true; // At least one of the "Bullet_Generator" can aim the target.
                }
            }
            return flag; // At least one of the "Bullet_Generator" can aim the target or not.
        }


        float Calculate_2D_Angle(Vector3 targetPosition)
        {
            // Calculate the angle to the target for driving.
            Vector3 localPosition3D = thisTransform.InverseTransformPoint(targetPosition);
            Vector2 localPosition2D;
            localPosition2D.x = localPosition3D.x;
            localPosition2D.y = localPosition3D.z;
            return Vector2.Angle(Vector2.up, localPosition2D) * Mathf.Sign(localPosition2D.x);
        }


        void Set_Driving_Target_Angle_And_Distance()
		{
            // Check the state of the NavMeshAgent.
            if (navAgent.path.corners.Length == 0)
            { // Something wrong in the NavMeshAgent.
                // Do not move.
                drivingTargetAngle = 0.0f;
                drivingTargetDistance = 0.0f;
                return;
            }

            // Get the angle to the NavMeshAgent.
            drivingTargetAngle = Calculate_2D_Angle(navAgentTransform.position);

            // Get the distance to the next corner.
            if (navAgent.path.corners.Length > 1)
            { // The corners [0] should be the NavMeshAgent it self.
                drivingTargetDistance = Vector3.Distance(thisTransform.position, navAgent.path.corners[1]);
                if (navAgent.path.corners.Length > 2)
                { // The next corner (corners [1]) should not be the destination.
                    // Get the angle of the corners [1]. (this position >> corners [1] >> corners [2]")
                    Vector3 vecA = thisTransform.position - navAgent.path.corners[1];
                    Vector3 vecB = navAgent.path.corners[2] - navAgent.path.corners[1];
                    float theta = Mathf.Acos(Vector3.Dot(vecA, vecB) / (vecA.magnitude * vecB.magnitude)) * Mathf.Rad2Deg;
                    drivingNextCornerAngle = 180.0f - theta;
                }
                else
                { // The next corner (corners [1]) should be the destination (the next waypoint).
                    drivingNextCornerAngle = 180.0f;
                }
            }
            else
            { // The corners [0] should be the destination (the next waypoint).
                drivingTargetDistance = Vector3.Distance(thisTransform.position, navAgent.path.corners[0]);
                drivingNextCornerAngle = 180.0f;
            }
        }


        public void Set_Target (AI_Headquaters_Helper_CS targetAIHelperScript)
        { // Called from "AI_Headquaters_CS" in the scene.
            if (Target_Transform == targetAIHelperScript.Body_Transform)
            { // The sent target is the same as the current target.
                return;
            }
            // Reset the values.
            Lost_Target();
            Target_Transform = targetAIHelperScript.Body_Transform;
            targetRootTransform = targetAIHelperScript.Body_Transform.root;
            targetUpperOffset = targetAIHelperScript.Visibility_Upper_Offset;
        }


        public void Lost_Target()
        { // Called also from "AI_Share_Target_CS".
            if (Settings_Script.Breakthrough == false)
            { // Not breakthrough >> The tank is chasing the target until now.
                // Move the NavMeshAgent to this position, so that the NavMeshAgent can find a new path smoothly.
                navAgent.Warp(thisTransform.position);
            }

            // Reset the values.
            Action_Type = 0;
            updateDestinationCount = 0.0f;
            searchingCount = 0.0f;
            Target_Transform = null;
            targetRootTransform = null;
            Detect_Flag = false;
            OpenFire_Flag = false;
            Losing_Count = Settings_Script.Lost_Count;
            hasApproached = false;
            isStaying = false;
            currentMaxSpeedRate = Max_Speed_Rate * Settings_Script.Patrol_Speed_Rate;

            // Call the "Aiming_Control_CS" to lock off the target.
            if (aimingScript)
            {
                aimingScript.AI_Lock_Off();
            }

            if (defensiveType == 0)
            { // Waypoint mode.
                // Update the destination of the NavMeshAgent.
                navAgent.SetDestination(wayPoints[nextWayPoint].position);
            }
        }


        void Auto_Drive()
        {
            // Calculate "Speed_Order" and "Turn_Order".
            float sign = Mathf.Sign(drivingTargetAngle);
            drivingTargetAngle = Mathf.Abs(drivingTargetAngle);
            if (drivingTargetAngle > Min_Target_Angle)
            { // Turn.
                if (drivingTargetAngle > Pivot_Turn_Angle)
                { // Pivot turn.
                    Turn_Order = 1.0f * sign;
                    Speed_Order = 0.0f;
                    return;
                }
                // Brake turn.
                Turn_Order = Mathf.Lerp(Min_Turn_Rate, Max_Turn_Rate, drivingTargetAngle / Pivot_Turn_Angle) * sign;
                if (drivingTargetDistance == 0.0f)
                {
                    Speed_Order = 0.0f;
                    return;
                }
                Speed_Order = 1.0f - Turn_Order;
                float currentMinSpeedRate = Mathf.Lerp(Max_Speed_Rate, Min_Speed_Rate, drivingNextCornerAngle / 45.0f);
                Speed_Order *= Mathf.Lerp(currentMinSpeedRate, Max_Speed_Rate, drivingTargetDistance / SlowDown_Range);
                Speed_Order = Mathf.Clamp(Speed_Order, Min_Speed_Rate, currentMaxSpeedRate);
                return;
            }
            else
            { // No turn.
                Turn_Order = 0.0f;
                if (drivingTargetDistance == 0.0f)
                {
                    Speed_Order = 0.0f;
                    return;
                }
                float currentMinSpeedRate = Mathf.Lerp(Max_Speed_Rate, Min_Speed_Rate, drivingNextCornerAngle / 30.0f);
                Speed_Order = Mathf.Lerp(currentMinSpeedRate, Max_Speed_Rate, drivingTargetDistance / SlowDown_Range);
                Speed_Order = Mathf.Clamp(Speed_Order, Min_Speed_Rate, currentMaxSpeedRate);
                return;
            }
        }


        public void Escape_From_Stuck()
        { // Called from "AI_Hand" when the tank gets stuck.
            // Move the NavMeshAgent to this position, so that the NavMeshAgent can find a new path smoothly.
            navAgent.Warp(thisTransform.position);

            switch (Action_Type)
            {
                case 0: // Defensive.
                    switch (defensiveType)
                    {
                        case 0: // Waypoint mode.
                            if (isEscapingFromStuck == false && Random.Range(0, 3) == 0)
                            {
                                StartCoroutine("Create_NavMeshObstacle_Object");
                            }
                            else
                            {
                                Update_Next_WayPoint();
                            }
                            break;

                        case 1: // Follow mode.
                            // Stop only.
                            return;
                    }
                    break;

                case 1: // Offensive.
                    if (Settings_Script.Breakthrough == true)
                    { // The AI tank never chase the target.
                        switch (defensiveType)
                        {
                            case 0: // Waypoint mode.
                                if (isEscapingFromStuck == false && Random.Range(0, 3) == 0)
                                {
                                    StartCoroutine("Create_NavMeshObstacle_Object");
                                }
                                else
                                {
                                    Update_Next_WayPoint();
                                }
                                break;

                            case 1: // Follow mode.
                                // Stop only.
                                break;
                        }
                    }
                    else
                    { // The AI tank is chasing the target.
                        if (isEscapingFromStuck == false && Random.Range(0, 2) == 0)
                        {
                            StartCoroutine("Create_NavMeshObstacle_Object");
                        }
                    }
                    break;
            }
        }


        IEnumerator Create_NavMeshObstacle_Object()
        {
            if (Obstacle_Prefab == null)
            {
                yield break;
            }
            isEscapingFromStuck = true;

            // Spawn the "Obstacle_Prefab".
            Instantiate(Obstacle_Prefab, thisTransform.position, thisTransform.rotation);

            // Wait.
            yield return new WaitForSeconds(20.0f);

            isEscapingFromStuck = false;
        }


        public bool Check_for_Assigning(AI_Headquaters_Helper_CS targetAIHelperScript)
        { // Called from "AI_Headquaters_CS" in the scene.
            // Check this tank can detect the target or not, by casting a line.
            Vector3 tempTargetPosition = targetAIHelperScript.Body_Transform.position + (targetAIHelperScript.Body_Transform.up * targetAIHelperScript.Visibility_Upper_Offset);
            RaycastHit raycastHit;
            if (Physics.Linecast(eyeTransform.position, tempTargetPosition, out raycastHit, Layer_Settings_CS.Layer_Mask))
            { // The line hits anything.
                if (raycastHit.transform.root == targetAIHelperScript.Body_Transform.root)
                { // The line hits the target.
                    return true;
                }
                else
                { // The line hits other object.
                    return false;
                }
            }
            else
            { // The line does not hit anything. >> There is no obstacle between this eye and the target.
                return true;
            }
        }


        public void Reset_Settings()
        { // Called from "AI_Settings_CS", after the AI settings has been changed by events.
            if (Settings_Script.Follow_Target)
            {
                Get_Follow_Transform();
            }
            else
            {
                // Set up the waypoints.
                Set_WayPoints();
                // Set the first waypoint.
                nextWayPoint = -1;
                Update_Next_WayPoint();
            }
            Lost_Target();
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(navAgent.gameObject);
            Destroy(this.gameObject);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}