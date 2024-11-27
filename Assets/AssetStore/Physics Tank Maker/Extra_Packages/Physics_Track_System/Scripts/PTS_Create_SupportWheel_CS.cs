using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Create_SupportWheel_CS : MonoBehaviour
	{
        /* 
		 * This script is attached to the "Create_SupportWheel" in the tank.
		 * This script is used for creating and setting the support wheels (return rollers) by the editor script.
		*/

        public float Wheel_Distance = 2.7f;
		public int Num = 3;
		public float Spacing = 1.7f;
		public float Wheel_Mass = 100.0f;
		public float Wheel_Radius = 0.21f;
		public PhysicMaterial Collider_Material;
		public Mesh Wheel_Mesh;
		public int Wheel_Materials_Num = 1;
		public Material[] Wheel_Materials;
		public Material Wheel_Material;
		public bool Drive_Wheel = true;
		public bool Wheel_Resize = false;
		public float ScaleDown_Size = 0.5f;
		public float Return_Speed = 0.05f;
		public bool Static_Flag = false;
		public float Radius_Offset;

        // For editor script.
        public bool Has_Changed;
    }

}