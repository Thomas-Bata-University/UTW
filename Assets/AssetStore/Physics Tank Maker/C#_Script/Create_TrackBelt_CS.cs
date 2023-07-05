using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Create_TrackBelt_CS : MonoBehaviour
	{
		/* 
		 * This script is attached to the "Create_TrackBelt" in the tank.
		 * This script is used for creating and setting the "Physics_Track" by the editor script.
		*/

		public bool Rear_Flag = false;
		public int SelectedAngle = 3600;
		public int Angle_Rear = 4500;
		public int Number_Straight = 17;
		public float Spacing = 0.3f;
		public float Distance = 2.7f;
		public float Track_Mass = 30.0f;
		public float Angular_Drag = 50.0f;
		public Bounds Collider_Info = new Bounds (new Vector3 (0.0f, -0.016f, 0.0f), new Vector3 (0.65f, 0.08f, 0.3f));
		public PhysicMaterial Collider_Material;
		public bool Use_BoxCollider;

		public Mesh Track_R_Mesh;
		public Mesh Track_L_Mesh;
		public int Track_Materials_Num = 1;
		public Material[] Track_Materials;
		public bool Use_2ndPiece = false;
		public Mesh Track_R_2nd_Mesh;
		public Mesh Track_L_2nd_Mesh;
		public int Track_2nd_Materials_Num = 1;
		public Material[] Track_2nd_Materials;
		public bool Use_ShadowMesh = false;
		public Mesh Track_R_Shadow_Mesh;
		public Mesh Track_L_Shadow_Mesh;

		public int SubJoint_Type = 1;
		public float Reinforce_Radius = 0.3f;

		public bool Use_Joint = false;
		public float Joint_Offset;
		public Mesh Joint_R_Mesh;
		public Mesh Joint_L_Mesh;
		public int Joint_Materials_Num = 1;
		public Material[] Joint_Materials;
		public bool Joint_Shadow = true;

		public float BreakForce = 5000.0f;

		public bool Static_Flag = false;

        // For editor script.
        public bool Has_Changed;


        void Start()
        {
            if (Static_Flag)
            { // For creating Static_Track.
                Control_Rigidbody();
            }
            else
            {
                Destroy(this);
            }
        }


        void Control_Rigidbody()
        {
            Rigidbody parentRigidbody = transform.parent.GetComponent<Rigidbody>();
            parentRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            parentRigidbody.drag = 15.0f;
        }

    }

}