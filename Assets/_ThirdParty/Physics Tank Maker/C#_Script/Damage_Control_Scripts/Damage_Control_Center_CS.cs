using UnityEngine;
using System.Collections;
using FishNet.Object;
using UnityEngine.AI;

namespace ChobiAssets.PTM
{

	[ System.Serializable]
	public struct TurretDamageControlProp
	{
		public Transform turretBaseTransform;
		public float hitPoints;
		public float damageThreshold;
		public bool blowOff;
		public float mass;
		public GameObject destroyedEffect;
	}


	public class Damage_Control_Center_CS : NetworkBehaviour
	{
		/* 
		 * This script is attached to the "MainBody" in the tank.
		 * This script controls the hit points of the tank parts, and their destruction processes.
		 * This script works in combination with "Damage_Control_##_##_CS" scripts in the tank parts.
		*/


		// User options >>
		public float MainBody_HP = 1000.0f;
		public float Left_Track_HP = 1000.0f;
		public float Right_Track_HP = 1000.0f;

		public float MainBody_Damage_Threshold = 100.0f;
		public float Track_Damage_Threshold = 50.0f;

        public bool Repairable_Track = true;
        public float Track_Repairing_Time = 20.0f;
        public float Track_Repairing_Velocity_Limit = 0.5f;

        public GameObject Destroyed_Effect;
        public Vector3 Destroyed_Effect_Offset;

        public int Turret_Num = 1;
		public TurretDamageControlProp[] Turret_Props;
		// << User options
		

		// Referred to from "UI_HP_Bars_Self_CS" and "UI_HP_Bars_Target_CS".
		public float Initial_Body_HP;
		public float Initial_Turret_HP;
		public float Initial_Left_Track_HP;
		public float Initial_Right_Track_HP;

		Transform bodyTransform;
        AI_CS aiScript;
        bool isDead;


        void Start()
		{
			Initialize();
		}


        void Initialize()
        {
            bodyTransform = transform;

            // Store the initial HP values for the "UI_HP_Bars_Self_CS" and "UI_HP_Bars_Target_CS".
            Initial_Body_HP = MainBody_HP;
            Initial_Turret_HP = Turret_Props[0].hitPoints;
            Initial_Left_Track_HP = Left_Track_HP;
            Initial_Right_Track_HP = Right_Track_HP;
        }

        public bool Receive_Damage(float damage, int type, int index)
        { // Called from "Damage_Control_##_##_CS" scripts in the tank.
            if (aiScript)
            { // AI tank.
                // Call "AI_CS" to disable the dead angle.
                aiScript.StartCoroutine("Wake_Up_Timer");
            }

            switch (type)
            {
                case 0: // MainBody
                    return MainBody_Damaged(damage);

                case 1: // Turret
                    return Turret_Damaged(damage, index);

                case 2: // Physics_Track piece
                case 3: // Track_Collider
                    return Track_Damaged(damage, index);

                default:
                    return false;
            }
        }


        bool MainBody_Damaged(float damage)
        {
            /*
            if (damage < MainBody_Damage_Threshold)
            { // Never receive any damage under the threshold value.
                return false;
            }

            MainBody_HP -= damage;
            if (MainBody_HP <= 0)
            {
                MainBody_Destroyed();
                return true;
            }
            return false;
            */

            MainBody_DamagedClientRpc(damage);
            if (MainBody_HP <= 0) return true;
            else return false;
        }
        [ObserversRpc]
        void MainBody_DamagedClientRpc(float damage)
        {
            if (damage < MainBody_Damage_Threshold)
            { // Never receive any damage under the threshold value.
                return;
            }

            MainBody_HP -= damage;
            if (MainBody_HP <= 0)
            {
                MainBody_Destroyed();
            }
        }


        bool Turret_Damaged(float damage, int index)
        {
            if (Turret_Props[index].turretBaseTransform == null)
            { // The turret had already been destroyed.
                return false;
            }

            if (damage < Turret_Props[index].damageThreshold)
            { // Never receive any damage under the threshold value.
                return false;
            }

            Turret_Props[index].hitPoints -= damage;
            if (Turret_Props[index].hitPoints <= 0)
            {
                if (index == 0)
                { // The main turret has been destroyed.
                    MainBody_Destroyed();
                }
                else
                {
                    Turret_Destroyed(index);
                }
                return true;
            }
            return false;
        }


        bool Track_Damaged(float damage, int index)
        {
            if (damage < Track_Damage_Threshold)
            { // Never receive any damage under the threshold value.
                return false;
            }

            switch (index)
            {
                case 0: // Left track
                    if (Left_Track_HP <= 0.0f)
                    { // The track is already broken.
                        return false;
                    }

                    Left_Track_HP -= damage;
                    if (Left_Track_HP <= 0)
                    {
                        Track_Destroyed(true);
                        return true;
                    }
                    return false;

                case 1: // Right track

                    if (Right_Track_HP <= 0.0f)
                    { // The track is already broken.
                        return false;
                    }

                    Right_Track_HP -= damage;
                    if (Right_Track_HP <= 0)
                    {
                        Track_Destroyed(false);
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        }


        void MainBody_Destroyed()
        {
            // Check the tank has already been dead or not.

            if (isDead)
            { // (Note.) When the tank has been destroyed by an explosion, this function might be called several times in the one frame.
                return;
            }
            isDead = true;

            // Set the HP value to zero.
            MainBody_HP = 0.0f;

            // Set the tag.
            bodyTransform.root.tag = "Finish";

            // Destroy all the turret.
            for (int i = 0; i < Turret_Props.Length; i++)
            {
                Turret_Destroyed(i);
            }

            // Create the destroyed effect.
            if (Destroyed_Effect)
            {
                Vector3 pos = bodyTransform.position + (bodyTransform.right * Destroyed_Effect_Offset.x) + (bodyTransform.up * Destroyed_Effect_Offset.y) + (bodyTransform.forward * Destroyed_Effect_Offset.z);
                Instantiate(Destroyed_Effect, pos, bodyTransform.rotation, bodyTransform);
            }

            // Send Message to "Damage_Control_00_MainBody_CS", "Damage_Control_01_Turret_CS", "Respawn_Controller_CS", "AI_CS", "UI_Aim_Marker_Control_CS", "Aiming_Marker_Control_CS", "Drive_Control_CS", "Drive_Wheel_Parent_CS", "Drive_Wheel_CS", "Steer_Wheel_CS", "Stabilizer_CS", "Fix_Shaking_Rotation_CS", "Sound_Control_##_CS".
            bodyTransform.parent.BroadcastMessage("MainBody_Destroyed_Linkage", SendMessageOptions.DontRequireReceiver);

            // Add NavMeshObstacle to the MainBody.
            NavMeshObstacle navMeshObstacle = bodyTransform.gameObject.AddComponent<NavMeshObstacle>();
            navMeshObstacle.shape = NavMeshObstacleShape.Capsule;
            navMeshObstacle.carving = true;
            navMeshObstacle.carvingMoveThreshold = 1.0f;

            /// Release the parking brake, and Destroy this script.
            StartCoroutine("Disable_MainBody_Constraints");
        }


        IEnumerator Disable_MainBody_Constraints()
        {
            // Disable the rigidbody constraints in the MainBody in order to release the parking brake.
            yield return new WaitForFixedUpdate(); // This wait is required for PhysX.
            Rigidbody bodyRigidBody = bodyTransform.GetComponent<Rigidbody>();
            bodyRigidBody.constraints = RigidbodyConstraints.None;
        }


        void Turret_Destroyed(int index)
        {
            transform.GetComponentInChildren<Cannon_Fire_CS>().Turret_Destroyed_Linkage();
            if (Turret_Props[index].turretBaseTransform == null)
            { // The turret had already been destroyed.
                return;
            }
            // Set the HP value to zero.
            Turret_Props[index].hitPoints = 0.0f;

            // Create the destroyed effect.
            if (Turret_Props[index].destroyedEffect)
            {
                Instantiate(Turret_Props[index].destroyedEffect, transform.Find("Turret_Base").position, transform.Find("Turret_Base").rotation, transform.Find("Turret_Base"));
            }

            // Send Message to "Damage_Control_01_Turret_CS", "Turret_Horizontal_CS", "Cannon_Vertical_CS", "Cannon_Fire_CS", "Gun_Camera_CS", "Recoil_Brake_CS", "Sound_Control_Motor_CS".
            transform.Find("Turret_Base").BroadcastMessage("Turret_Destroyed_Linkage", SendMessageOptions.DontRequireReceiver);

            // Blow off the turret.
            if (Turret_Props[index].blowOff == true)
            {
                Rigidbody turretRigidbody = transform.Find("Turret_Base").gameObject.AddComponent<Rigidbody>();
                turretRigidbody.mass = Turret_Props[index].mass;
                Vector3 addForceOffset;
                addForceOffset.x = Random.Range(-2.0f, 2.0f);
                addForceOffset.y = 0.0f;
                addForceOffset.z = Random.Range(-2.0f, 2.0f);
                turretRigidbody.AddForceAtPosition(transform.Find("Turret_Base").up * Random.Range(Turret_Props[index].mass * 5.0f, Turret_Props[index].mass * 15.0f), Turret_Props[index].turretBaseTransform.position + addForceOffset, ForceMode.Impulse);
                // Change the hierarchy.
                Turret_Props[index].turretBaseTransform.parent = bodyTransform.parent; // Make it a child of the top object.
            }

            // Remove the "turretBaseTransform" from the array element.
            Turret_Props[index].turretBaseTransform = null;
        }


        void Track_Destroyed(bool isLeft)
        {
            // Send message to "Damage_Control_02_Physics_Track_Piece_CS", "Fix_Shaking_Rotation_CS", "Damage_Control_04_Track_Collider_CS", "Static_Track_Piece_CS", "Track_Joint_CS", "Stabilizer_CS", "Drive_Wheel_CS", "Static_Wheel_CS", "Track_Scroll_CS", "Track_LOD_Control_CS", "Static_Track_Switch_Mesh_CS".
            bodyTransform.BroadcastMessage("Track_Destroyed_Linkage", isLeft, SendMessageOptions.DontRequireReceiver);

            // Start repairing the tracks.
            if (Repairable_Track)
            {
                // Check the tank has "Static_Track" or "Scroll_Track".
                if (GetComponentInChildren<Static_Track_Parent_CS>() || GetComponentInChildren<Track_Scroll_CS>())
                {
                    StartCoroutine(Track_Repairing_Timer(isLeft));
                }
            }
        }


        IEnumerator Track_Repairing_Timer(bool isLeft)
        {
            var bodyRigidBody = bodyTransform.GetComponent<Rigidbody>();
            var count = 0.0f;
            while (count < Track_Repairing_Time)
            {
                count += Time.deltaTime;

                // Check the tank has moved.
                if (bodyRigidBody.velocity.magnitude > Track_Repairing_Velocity_Limit)
                {
                    count = 0.0f;
                }

                // Set the HP.
                if (isLeft)
                {
                    Left_Track_HP = Initial_Left_Track_HP * (count / Track_Repairing_Time);
                }
                else
                {
                    Right_Track_HP = Initial_Right_Track_HP * (count / Track_Repairing_Time);
                }

                yield return null;
            }

            // Check the tank is still alive.
            if (isDead)
            {
                yield break;
            }

            // Repair the tracks.
            Track_Repaired(isLeft);
        }


        void Track_Repaired(bool isLeft)
        {
            // Send message to "Damage_Control_04_Track_Collider_CS", "Static_Track_Piece_CS", "Stabilizer_CS", "Drive_Wheel_CS", "Static_Wheel_CS", "Track_Scroll_CS", "Track_LOD_Control_CS", "Static_Track_Switch_Mesh_CS".
            bodyTransform.BroadcastMessage("Track_Repaired_Linkage", isLeft, SendMessageOptions.DontRequireReceiver);
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            if (isSelected == false)
            {
                return;
            } // This tank is selected.

            // Send this reference to the "UI_HP_Bars_Self_CS" in the scene.
            if (UI_HP_Bars_Self_CS.Instance)
            {
                UI_HP_Bars_Self_CS.Instance.Get_Damage_Script(this);
            }
        }


        void Get_AI_CS(AI_CS aiScript)
        { // Called from "AI_CS".
            this.aiScript = aiScript;
        }

    }

}