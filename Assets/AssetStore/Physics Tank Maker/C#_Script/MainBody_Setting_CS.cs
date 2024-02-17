using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(MeshFilter))]
	[ RequireComponent (typeof(MeshRenderer))]
	[ RequireComponent (typeof(Rigidbody))]

	public class MainBody_Setting_CS : MonoBehaviour
	{
		/*
		 * This script is attached to the MainBody of the tank.
		 * These variables are used for setting the appearance and physics of the MainBody of the tank.
		 * 
		 * Also this script works in the rutime to control the rigidbody settings.
		 * And gives the visible-invisilbe boolean to the several scripts. 
		*/


		// User options >>
		public float Body_Mass = 2000.0f;
		public Mesh Body_Mesh;
		public int Materials_Num = 1;
		public Material[] Materials;
		public int Colliders_Num;
		public Mesh[] Colliders_Mesh;
		public Mesh Collider_Mesh; // for old versions.
		public Mesh Sub_Collider_Mesh; // for old versions.
		public int SIC = 14;
		public bool Soft_Landing_Flag;
		public float Landing_Drag = 20.0f;
		public float Landing_Time = 1.5f;
        public Vector3 Mass_Center_Offset = Vector3.zero;
        public float AI_Upper_Offset = 1.5f; // for old versions.
		public float AI_Lower_Offset = 0.3f; // for old versions.
		public bool Use_Damage_Control = true;
        // << User options

        // For editor script.
        public bool Has_Changed;

        // Referred to from some scripts in the child objects.
        public bool Visible_Flag; // Referred to from "Static_Track_Parent_CS", "Static_Wheel_Parent_CS", "Track_Deform_CS", "Track_Scroll_CS", "Track_Joint_CS".


        void Start()
        {
            // Set the Solver Iterations Count of the rigidbody.
            Rigidbody thisRigidbody = GetComponent<Rigidbody>();
            thisRigidbody.solverIterations = SIC; // (Note.) The "solverIterations" must be set in the runtime.

            // Set the center of mass to zero.
            thisRigidbody.centerOfMass = Mass_Center_Offset; // (Note.) The "centerOfMass" must be set in the runtime.

            // Fall down slowly at the opening.
            if (Soft_Landing_Flag)
            {
                StartCoroutine("Soft_Landing", thisRigidbody);
            }

            // Set the Layer.
            gameObject.layer = Layer_Settings_CS.Body_Layer;
        }


        IEnumerator Soft_Landing(Rigidbody tempRigidbody)
        {
            // Increase the drag, and set the constraints so that the tank does not bank.
            float defaultDrag = tempRigidbody.drag;
            tempRigidbody.drag = Landing_Drag;
            tempRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            // Wait.
            yield return new WaitForSeconds(Landing_Time);

            // Return them.
            tempRigidbody.drag = defaultDrag;
            tempRigidbody.constraints = RigidbodyConstraints.None;
        }


        void OnBecameVisible()
        { // The tank is visible by any camera.
            Visible_Flag = true;
        }


        void OnBecameInvisible()
        { // The tank is not visible by any camera.
            Visible_Flag = false;
        }

    }

}