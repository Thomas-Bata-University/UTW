using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Turret_Base_CS : MonoBehaviour
	{
		/* 
		 * This script is attached to the "Turret_Base" in the tank.
		 * This script is used for creating and setting the turret by the editor script.
		*/

		public Mesh Part_Mesh;

		public int Colliders_Num;
		public Mesh[] Colliders_Mesh;
		public Mesh Collider_Mesh; // for old versions.
		public Mesh Sub_Collider_Mesh; // for old versions.

		public int Materials_Num = 1;
		public Material[] Materials;
		public Material Part_Material;

		public float Offset_X = 0.0f;
		public float Offset_Y = 0.0f;
		public float Offset_Z = 0.0f;

		public bool Use_Damage_Control = true;
		public int Turret_Index;

		public GameObject Damage_Effect_Object; // for old versions.


        // For editor script.
        public bool Has_Changed;


        void Start ()
		{
			Destroy (this);
		}

	}

}