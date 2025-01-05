using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Create_RoadWheel_Type89_CS : MonoBehaviour
	{
		/* 
		 * This script is attached to the "Create_RoadWheel_Type89" in type 89 tank.
		 * This script is dedicated for type 89 tank.
		 * This script is used for creating and setting the road wheels and the suspension arms by the editor script.
		*/

		public bool Fit_ST_Flag = false;

		public float Distance = 1.78f;
		public int ParentArm_Num = 2;
		public float ParentArm_Spacing = 1.6f;
		public float ParentArm_Offset_Y = 0.0f;
		public float ParentArm_Mass = 100.0f;
        public float ParentArm_LinearLimit = 0.1f;
        public float ParentArm_AngleLimit = 15.0f;
		public float ParentArm_Spring = 400000.0f;
		public float ParentArm_Damper = 40000.0f;
		public Mesh ParentArm_L_Mesh;
		public Mesh ParentArm_R_Mesh;
		public Material ParentArm_L_Material;
		public Material ParentArm_R_Material;
	
		public int ChildArm_Num = 2;
		public float ChildArm_Spacing = 0.8f;
		public 	float ChildArm_Offset_Y = 0.0f;
		public float ChildArm_Mass = 100.0f;
		public float ChildArm_AngleLimit = 15.0f;
		public float ChildArm_Damper = 100.0f;
		public Mesh ChildArm_L_Mesh;
		public Mesh ChildArm_R_Mesh;
		public Material ChildArm_L_Material;
		public Material ChildArm_R_Material;
	
		public int Wheel_Num = 2;
		public float Wheel_Spacing = 0.4f;
		public float Wheel_Offset_Y = 0.0f;
		public float Wheel_Mass = 100.0f;
		public float Wheel_Radius = 0.12f;
		public PhysicMaterial Wheel_Collider_Material;
		public Mesh Wheel_Mesh;
		public Material Wheel_Material;

		public bool Drive_Wheel = true;
		public bool Wheel_Resize = false;
		public float ScaleDown_Size = 0.5f;
		public float Return_Speed = 0.05f;

        // For editor script.
        public bool Has_Changed;
    }

}