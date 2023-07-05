using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Drive_Wheel_CS : MonoBehaviour
    {
        /*
		 * This script is attached to all the drive wheels in the tank.
		 * This script controls the angular velocity and angular drug of the rigidbody in the wheel, and give it the torque force.
		 * This script works in combination with "Drive_Control_CS" in the MainBody, and "Drive_Wheel_Parent_CS" in the parent object.
		*/

        // User options >>
        public Rigidbody This_Rigidbody;
        public bool Is_Left;
        public Drive_Wheel_Parent_CS Parent_Script;
        // << User options


        float storedSphereColliderRadius;


        void FixedUpdate()
        {
            Control_Rigidbody();
        }


        void Control_Rigidbody()
        {
            // Set the "maxAngularVelocity" of the rigidbody.
            This_Rigidbody.maxAngularVelocity = Parent_Script.Max_Angular_Velocity;

            // Set the "angularDrag" of the rigidbody, and add torque to it.
            if (Is_Left)
            { // Left
                This_Rigidbody.angularDrag = Parent_Script.Left_Angular_Drag;
                This_Rigidbody.AddRelativeTorque(0.0f, Parent_Script.Left_Torque, 0.0f);
            }
            else
            { // Right
                This_Rigidbody.angularDrag = Parent_Script.Right_Angular_Drag;
                This_Rigidbody.AddRelativeTorque(0.0f, Parent_Script.Right_Torque, 0.0f);
            }
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Lock the wheel.
            This_Rigidbody.angularDrag = Mathf.Infinity;

            // Disable this script. (Note.) Don't remove this script, because if the tracks are still alive, the "Track_Destroyed_Linkage()" might been called.
            this.enabled = false;
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Lock the wheels.
            This_Rigidbody.angularDrag = Mathf.Infinity;

            // Store the radius of the SphereCollider.
            var sphereCollider = GetComponent<SphereCollider>();
            storedSphereColliderRadius = sphereCollider.radius;

            // Resize the SphereCollider.
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter && meshFilter.sharedMesh)
            {
                sphereCollider.radius = meshFilter.sharedMesh.bounds.extents.x;
            }

            // Disable this script.
            this.enabled = false;
        }


        void Track_Repaired_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Resize the SphereCollider.
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = storedSphereColliderRadius;

            // Enable this script.
            this.enabled = true;
        }

    }

}