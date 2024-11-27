using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Extra_Collider_Collision_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Extra_Collier" in the tank.
		*/


        public float Collision_Impact_Force;


        Drive_Control_CS driveScript;
        Transform bodyTransform;


        void Start()
        {
            GetComponent<Collider>().isTrigger = true;

            driveScript = GetComponentInParent<Drive_Control_CS>();
            bodyTransform = driveScript.transform;
        }


        void OnTriggerStay(Collider collider)
        {
            var dir = (collider.attachedRigidbody.position - bodyTransform.position).normalized;
            collider.attachedRigidbody.AddForce(dir * Collision_Impact_Force * (driveScript.Current_Velocity / driveScript.Max_Speed), ForceMode.Impulse);
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            Destroy(this.gameObject);
        }
    }

}