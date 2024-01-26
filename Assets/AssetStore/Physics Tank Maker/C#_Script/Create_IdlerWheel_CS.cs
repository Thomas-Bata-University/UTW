using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Create_IdlerWheel_CS : MonoBehaviour
	{
		/* 
		 * This script is attached to the "Create_IdlerWheel" in the tank.
		 * This script is used for creating and setting the idler wheels by the editor script.
		*/

		public bool Static_Flag = false;
		public float Radius_Offset;
		public bool Invisible_Physics_Wheel = false;

		public bool Arm_Flag = false;
		public float Arm_Distance = 2.11f;
		public float Arm_Length = 0.25f;
		public float Arm_Angle = 110.0f;
		public Mesh Arm_L_Mesh;
		public Mesh Arm_R_Mesh;
		public int Arm_Materials_Num = 1;
		public Material[] Arm_Materials;
		public Material Arm_L_Material; // for old versions.
		public Material Arm_R_Material; // for old versions.
	
		public float Wheel_Distance = 2.7f;
		public float Wheel_Mass = 30.0f;
		public float Wheel_Radius = 0.35f;
		public PhysicMaterial Collider_Material;
		public Mesh Wheel_Mesh;
		public int Wheel_Materials_Num = 1;
		public Material[] Wheel_Materials;
		public Material Wheel_Material; // for old versions.
		public bool Drive_Wheel = true;
		public bool Wheel_Resize = true;
		public float ScaleDown_Size = 0.7f;
		public float Return_Speed = 0.05f;

        // For editor script.
        public bool Has_Changed;
    }

}