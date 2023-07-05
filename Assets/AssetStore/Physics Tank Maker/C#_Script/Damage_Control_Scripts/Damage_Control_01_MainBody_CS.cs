using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Damage_Control_01_MainBody_CS : Damage_Control_00_Base_CS
	{

        Transform bodyTransform;


        protected override void Start()
        {
            centerScript = GetComponentInParent<Damage_Control_Center_CS>();

            bodyTransform = transform;
        }


        void Update()
        {
            // Check the rollover.
            if (Mathf.Abs(Mathf.DeltaAngle(0.0f, bodyTransform.eulerAngles.z)) > 90.0f)
            {
                centerScript.Receive_Damage(Mathf.Infinity, 0, 0); // type = 0 (MainBody), index =0 (useless)
            }
        }


        public override bool Get_Damage(float damage, int bulletType)
		{ // Called from "Bullet_Control_CS", when the bullet hits this collider.
			// Send the damage value to the "Damage_Control_Center_CS".
			return centerScript.Receive_Damage(damage, 0, 0); // type = 0 (MainBody), index =0 (useless)
		}


        void MainBody_Destroyed_Linkage()
		{ // Called from "Damage_Control_Center_CS", when the MainBody has been destroyed.
			Destroy (this);
		}

	}

}
