using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Damage_Control_04_Track_Collider_CS : Damage_Control_00_Base_CS
    {

        public int Track_Index; // 0 = Left, 1 = Right.
        public Static_Track_Piece_CS Linked_Piece_Script;

        // For editor script.
        public bool Has_Changed;


        void Awake()
        { // (Note.) The hierarchy must be changed before "Start()", because this gameobject might be inactivated by the "Track_LOD_Control_CS" in the "Start()".

            // Change the hierarchy. (Make this Track_Collider a child of the MainBody.)
            transform.parent = transform.parent.parent;
        }


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

            // Find the "Damage_Control_Center_CS" script in the MainBody.
            centerScript = GetComponentInParent<Damage_Control_Center_CS>();
        }


        public override bool Get_Damage(float damage, int bulletType)
        { // Called from "Bullet_Control_CS", when the bullet hits this collider.

            // Send the damage value to the "Damage_Control_Center_CS".
            if (centerScript.Receive_Damage(damage, 3, Track_Index) == true)
            { // type = 3 (Track_Collider), index = Track_Index (0 = Left, 1 = Right). true = The track has been destroyed.
                
                // Breaking the track by calling the "Linked_Piece_Script". ("Static_Track_Piece_CS" script in the track piece.)
                if (Linked_Piece_Script)
                {
                    Linked_Piece_Script.Start_Breaking(centerScript.Track_Repairing_Time);
                }
                return true;
            }
            else
            { // The track has not been destroyed.
                return false;
            }
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS", when the track has broken.
            if ((isLeft && Track_Index != 0) || (isLeft == false && Track_Index != 1))
            { // The direction is different.
                return;
            }

            // Disable the collider.
            GetComponent<Collider>().enabled = false;
        }


        void Track_Repaired_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if ((isLeft && Track_Index != 0) || (isLeft == false && Track_Index != 1))
            { // The direction is different.
                return;
            }

            // Enable the collider.
            GetComponent<Collider>().enabled = true;
        }
    }

}
