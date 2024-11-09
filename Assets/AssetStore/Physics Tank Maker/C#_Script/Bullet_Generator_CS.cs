using FishNet.Object;
using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Bullet_Generator_CS : NetworkBehaviour
    {
        /* 
		 * This script is attached to the "Bullet_Generator" under the "Barrel_Base" in the tank.
		 * This script instantiates the bullet prefab and shoot it from the muzzle.
		 * Also you can create a prefab for the bullet using this script.
		*/

        // User options >>
        public GameObject AP_Bullet_Prefab;
        public GameObject HE_Bullet_Prefab;
        public GameObject MuzzleFire_Object;
        public float Attack_Point = 500.0f;
        public float Attack_Point_HE = 500.0f;
        public float Initial_Velocity = 500.0f;
        public float Initial_Velocity_HE = 500.0f;

        public float Life_Time = 5.0f;
        public int Initial_Bullet_Type = 0;
        public float Offset = 0.5f;
        public bool Debug_Flag = false;
        // << User options

        public float Attack_Multiplier = 1.0f; // Set by "Special_Settings_CS".
        public int Barrel_Type = 0; // Set by "Barrel_Base". (0 = Single barrel, 1 = Left of twins, 2 = Right of twins)
        public float Current_Bullet_Velocity; // Referred to from "Turret_Horizontal_CS", "Cannon_Vertical_CS", "UI_Lead_Marker_Control_CS".
        int currentBulletType;
        Transform thisTransform;

        // Only for AI tank.
        public bool Can_Aim; // Set by "AI_CS", and referred to from "Cannon_Fire_Input_99_AI_CS" script.


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisTransform = transform;

            // Switch the bullet type at the first time.
            currentBulletType = Initial_Bullet_Type - 1; // (Note.) The "currentBulletType" value is added by 1 soon in the "Switch_Bullet_Type()".

            Switch_Bullet_Type();
        }


        public void Switch_Bullet_Type()
        { // Called from "Cannon_Fire_Input_##_##" scripts.
            currentBulletType += 1;
            if (currentBulletType > 1)
            {
                currentBulletType = 0;
            }

            // Set the bullet velocity.
            switch (currentBulletType)
            {
                case 0: // AP
                    Current_Bullet_Velocity = Initial_Velocity;
                    break;

                case 1: // HE
                    Current_Bullet_Velocity = Initial_Velocity_HE;
                    break;
            }
        }

        public void Fire_Linkage(int direction)
        { // Called from "Cannon_Fire_CS".
            if (Barrel_Type == 0 || Barrel_Type == direction)
            { // Single barrel, or the same direction.

                // Generate the bullet and shoot it.
                //FireServerRpc(OwnerClientId);
            }
        }

        [ObserversRpc]
        public void MuzzleFlashClientRpc()
        {
            Instantiate(MuzzleFire_Object, thisTransform.position, thisTransform.rotation, thisTransform);
            GetComponentInParent<Recoil_Brake_CS>().Fire_Linkage(1);
        }

        [ServerRpc]
        public void FireServerRpc()
        {
            // Generate the muzzle fire.
            if (MuzzleFire_Object)
            {
                MuzzleFlashClientRpc();
            }

            // Generate the bullet.
            GameObject bulletObject = null;
            float attackPoint = 0;

            switch (currentBulletType)
            {
                case 0: // AP
                    if (AP_Bullet_Prefab == null)
                    {
                        Debug.LogError("'AP_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                        break;
                    }
                    bulletObject = Instantiate(AP_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation);
                    Spawn(bulletObject.GetComponent<NetworkObject>());
                   // if (bulletObject.GetComponent<NetworkObject>().IsNetworkVisibleTo(callerID)) Debug.Log("This object is visible to client: " + callerID);

                    attackPoint = Attack_Point;
                    break;

                case 1: // HE
                    if (HE_Bullet_Prefab == null)
                    {
                        Debug.LogError("'HE_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                        break;
                    }
                    bulletObject = Instantiate(HE_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation);
                   Spawn(bulletObject.GetComponent<NetworkObject>());
                    attackPoint = Attack_Point_HE;
                    break;

                default:
                    break;
            }
            Destroy(bulletObject, Life_Time);
            
            new WaitForFixedUpdate();
            Rigidbody rigidbody = bulletObject.GetComponent<Rigidbody>();
            Vector3 currentVelocity = bulletObject.transform.forward * Initial_Velocity;
            rigidbody.velocity = currentVelocity;
            
        }



        /*
                public void Fire_Linkage(int direction)
                { // Called from "Cannon_Fire_CS".
                    if (Barrel_Type == 0 || Barrel_Type == direction)
                    { // Single barrel, or the same direction.

                        // Generate the bullet and shoot it.
                        StartCoroutine("Generate_Bullet");
                    }
                }

                IEnumerator Generate_Bullet()
                {
                    // Generate the muzzle fire.
                    if (MuzzleFire_Object)
                    {
                        Instantiate(MuzzleFire_Object, thisTransform.position, thisTransform.rotation, thisTransform);
                    }

                    // Generate the bullet.
                    GameObject bulletObject;
                    float attackPoint = 0;
                    switch (currentBulletType)
                    {
                        case 0: // AP
                            if (AP_Bullet_Prefab == null)
                            {
                                Debug.LogError("'AP_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                                yield break;
                            }
                            bulletObject = Instantiate(AP_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation) as GameObject;
                            attackPoint = Attack_Point;
                            break;

                        case 1: // HE
                            if (HE_Bullet_Prefab == null)
                            {
                                Debug.LogError("'HE_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                                yield break;
                            }
                            bulletObject = Instantiate(HE_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation) as GameObject;
                            attackPoint = Attack_Point_HE;
                            break;

                        default:
                            yield break;
                    }

                    // Set values of "Bullet_Control_CS" in the bullet.
                    Bullet_Control_CS bulletScript = bulletObject.GetComponent<Bullet_Control_CS>();
                    bulletScript.Attack_Point = attackPoint;
                    bulletScript.Initial_Velocity = Current_Bullet_Velocity;
                    bulletScript.Life_Time = Life_Time;
                    bulletScript.Attack_Multiplier = Attack_Multiplier;
                    bulletScript.Debug_Flag = Debug_Flag;

                    // Set the tag.
                    bulletObject.tag = "Finish"; // (Note.) The ray cast for aiming does not hit any object with "Finish" tag.

                    // Set the layer.
                    bulletObject.layer = Layer_Settings_CS.Bullet_Layer;

                    // Shoot.
                    yield return new WaitForFixedUpdate();
                    Rigidbody rigidbody = bulletObject.GetComponent<Rigidbody>();
                    Vector3 currentVelocity = bulletObject.transform.forward * Current_Bullet_Velocity;
                    rigidbody.velocity = currentVelocity;
                }


        */







        /*
        public void Fire_Linkage(int direction)
        { // Called from "Cannon_Fire_CS".
            if (Barrel_Type == 0 || Barrel_Type == direction)
            { // Single barrel, or the same direction.

                // Generate the bullet and shoot it.
                FireServerRpc();
            }
        }

        [ClientRpc]
        public void MuzzleFlashClientRpc()
        {
            Instantiate(MuzzleFire_Object, thisTransform.position, thisTransform.rotation, thisTransform);
        }

        [ServerRpc]
        public void FireServerRpc()
        {
            // Generate the muzzle fire.
            if (MuzzleFire_Object)
            {
                MuzzleFlashClientRpc();
            }

            // Generate the bullet.
            GameObject bulletObject = null;
            float attackPoint = 0;

            switch (currentBulletType)
            {
                case 0: // AP
                    if (AP_Bullet_Prefab == null)
                    {
                        Debug.LogError("'AP_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                        break;
                    }
                    bulletObject = Instantiate(AP_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation);
                    bulletObject.GetComponent<NetworkObject>().Spawn();
                    attackPoint = Attack_Point;
                    break;

                case 1: // HE
                    if (HE_Bullet_Prefab == null)
                    {
                        Debug.LogError("'HE_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                        break;
                    }
                    bulletObject = Instantiate(HE_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation);
                    bulletObject.GetComponent<NetworkObject>().Spawn();
                    attackPoint = Attack_Point_HE;
                    break;

                default:
                    break;
            }

            // Set values of "Bullet_Control_CS" in the bullet.
            Bullet_Control_CS bulletScript = bulletObject.GetComponent<Bullet_Control_CS>();
            bulletScript.Attack_Point = attackPoint;
            bulletScript.Initial_Velocity = Current_Bullet_Velocity;
            bulletScript.Life_Time = Life_Time;
            bulletScript.Attack_Multiplier = Attack_Multiplier;
            bulletScript.Debug_Flag = Debug_Flag;

            // Set the tag.
            bulletObject.tag = "Finish"; // (Note.) The ray cast for aiming does not hit any object with "Finish" tag.

            // Set the layer.
            bulletObject.layer = Layer_Settings_CS.Bullet_Layer;

            // Shoot.
            new WaitForFixedUpdate();
            Rigidbody rigidbody = bulletObject.GetComponent<Rigidbody>();
            Vector3 currentVelocity = bulletObject.transform.forward * Current_Bullet_Velocity;
            rigidbody.velocity = currentVelocity;
        }
        */



        /*
        [ServerRpc]
        public void FireServerRpc()
        {
            StartCoroutine("Generate_Bullet");
        }
        [ClientRpc]
        public void MuzzleFlashClientRpc()
        {
            Instantiate(MuzzleFire_Object, thisTransform.position, thisTransform.rotation, thisTransform);
        }

        IEnumerator Generate_Bullet()
        {
            // Generate the muzzle fire.
            if (MuzzleFire_Object)
            {
                MuzzleFlashClientRpc();
            }

            // Generate the bullet.
            GameObject bulletObject;
            float attackPoint = 0;
            switch (currentBulletType)
            {
                case 0: // AP
                    if (AP_Bullet_Prefab == null)
                    {
                        Debug.LogError("'AP_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                        yield break;
                    }
                    bulletObject = Instantiate(AP_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation);
                    bulletObject.GetComponent<NetworkObject>().Spawn();
                    attackPoint = Attack_Point;
                    break;

                case 1: // HE
                    if (HE_Bullet_Prefab == null)
                    {
                        Debug.LogError("'HE_Bullet_Prefab' is not assigned in the 'Bullet_Generator'.");
                        yield break;
                    }
                    bulletObject = Instantiate(HE_Bullet_Prefab, thisTransform.position + (thisTransform.forward * Offset), thisTransform.rotation);
                    bulletObject.GetComponent<NetworkObject>().Spawn();
                    attackPoint = Attack_Point_HE;
                    break;

                default:
                    yield break;
            }

            // Set values of "Bullet_Control_CS" in the bullet.
            Bullet_Control_CS bulletScript = bulletObject.GetComponent<Bullet_Control_CS>();
            bulletScript.Attack_Point = attackPoint;
            bulletScript.Initial_Velocity = Current_Bullet_Velocity;
            bulletScript.Life_Time = Life_Time;
            bulletScript.Attack_Multiplier = Attack_Multiplier;
            bulletScript.Debug_Flag = Debug_Flag;

            // Set the tag.
            bulletObject.tag = "Finish"; // (Note.) The ray cast for aiming does not hit any object with "Finish" tag.

            // Set the layer.
            bulletObject.layer = Layer_Settings_CS.Bullet_Layer;

            // Shoot.
            yield return new WaitForFixedUpdate();
            Rigidbody rigidbody = bulletObject.GetComponent<Rigidbody>();
            Vector3 currentVelocity = bulletObject.transform.forward * Current_Bullet_Velocity;
            rigidbody.velocity = currentVelocity;
        }
        */
    }

}