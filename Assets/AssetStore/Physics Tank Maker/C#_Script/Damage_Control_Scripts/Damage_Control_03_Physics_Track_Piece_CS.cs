using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Damage_Control_03_Physics_Track_Piece_CS : Damage_Control_00_Base_CS
    {

        public int Track_Index; // 0 = Left, 1 = Right.


        public override bool Get_Damage(float damage, int bulletType)
        { // Called from "Bullet_Control_CS", when the bullet hits this collider.

            // Send the damage value to the "Damage_Control_Center_CS".
            if (centerScript.Receive_Damage(damage, 2, Track_Index) == true)
            { // type = 2 (Physics_Track piece), index = Track_Index (0 = Left, 1 = Right). true = The track has been destroyed.
                
                // Break the track by removing the HingeJoint.
                HingeJoint hingeJoint = GetComponent<HingeJoint>();
                if (hingeJoint)
                {
                    Destroy(hingeJoint);
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

            // Change the hierarchy.
            transform.parent = transform.root;

            // Remove from the scene in 20 seconds.
            Destroy(this.gameObject, 20.0f);
        }


        void OnJointBreak()
        { // Called when the HingeJoint in this piece has broken physically.

            // Send an infinite damage value to the "Damage_Control_Center_CS".
            centerScript.Receive_Damage(Mathf.Infinity, 2, Track_Index); // type = 2 (Physics_Track piece), index = Track_Index (0 = Left, 1 = Right).
        }
    }

}
