using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Damage_Control_09_Armor_Collider_CS : Damage_Control_00_Base_CS
    {

        public float Damage_Multiplier = 1.0f;


        Damage_Control_00_Base_CS parentDamageScript;


        protected override void Start()
        {
            // Set the layer.
            gameObject.layer = Layer_Settings_CS.Armor_Collider_Layer;

            // Make this invisible.
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                meshRenderer.enabled = false;
            }

            // Find the "Damage_Control_##_##_CS" script in the parent object.
            parentDamageScript = transform.parent.GetComponent<Damage_Control_00_Base_CS>();
            if (parentDamageScript == null)
            {
                Destroy(this.gameObject);
            }
        }


        public override bool Get_Damage(float damage, int bulletType)
        { // Called from "Bullet_Control_CS".

            // Apply the multiplier.
            if (bulletType == 0)
            { // AP
                damage *= Damage_Multiplier;
            }

            // Send the damage value to the parent "Damage_Control_##_##_CS" script.
            if (parentDamageScript == null)
            {
                return false;
            }
            return parentDamageScript.Get_Damage(damage, bulletType);
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS", when the MainBody has been destroyed.
            Destroy(this.gameObject);
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS", when this turret or the parent turret has been destroyed.
            Destroy(this.gameObject);
        }

    }

}
