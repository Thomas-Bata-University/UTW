using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Turret_Finishing_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Turret Objects" in the tank.
		 * This script change the hierarchy of the child objects such as "Turret_Base", "Cannon_Base" and "Barrel_Base" at the first time.
		*/


        // User options >>
        public bool Multiple_barrels_Flag = false;
        public bool Child_Flag = false;
        public Transform Parent_Transform;
        // << User options


        Transform thisTransform;
        Transform turretBase;
        Transform cannonBase;
        Transform barrelBase;


        void Awake()
        { // These function must be called before "Start()".
            thisTransform = transform;
            if (Multiple_barrels_Flag == true)
            {
                Multiple_Barrels();
            }
            else
            {
                Single_Barrel();
            }
        }


        void Single_Barrel()
        {
            turretBase = thisTransform.Find("Turret_Base");
            cannonBase = thisTransform.Find("Cannon_Base");
            barrelBase = thisTransform.Find("Barrel_Base");
            if (turretBase && cannonBase && barrelBase)
            {
                // Change the hierarchy.
                barrelBase.parent = cannonBase;
                cannonBase.parent = turretBase;
                Finishing();
            }
            else
            {
                Error_Message();
            }
        }


        void Multiple_Barrels()
        {
            turretBase = thisTransform.Find("Turret_Base");
            cannonBase = thisTransform.Find("Cannon_Base");
            if (turretBase && cannonBase)
            {
                // Change the hierarchy.
                cannonBase.parent = turretBase;
            }
            else
            {
                Error_Message();
            }

            var childCount = thisTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                barrelBase = thisTransform.Find("Barrel_Base");
                if (barrelBase)
                {
                    // Change the hierarchy.
                    barrelBase.parent = cannonBase;
                }
            }

            Finishing();
        }


        void Finishing()
        {
            if (Child_Flag == false)
            { // The turret is not a child turret.
                Destroy(this);
            }
        }


        void Start()
        { // Called only when this turret is a child turret.

            // Change the hierarchy of the child turret.
            if (Parent_Transform)
            {
                thisTransform.parent = Parent_Transform.Find("Turret_Base");
            }
            else
            {
                Debug.LogError("'Parent_Transform' for the child Turret is not assigned.");
            }

            Destroy(this);
        }


        void Error_Message()
        {
            Debug.LogError("'Turret_Finishing_CS' could not change the hierarchy of the turret.");
            Debug.LogWarning("Make sure the names of 'Turret_Base', 'Cannon_Base' and 'Barrel_Base'.");
            Destroy(this);
        }

    }

}