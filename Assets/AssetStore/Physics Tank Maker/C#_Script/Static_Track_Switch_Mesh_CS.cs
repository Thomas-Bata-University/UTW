using UnityEngine;

namespace ChobiAssets.PTM
{
    public class Static_Track_Switch_Mesh_CS : MonoBehaviour
    {
        /*
		 * This script switches the mesh of Static Track pieces while drivning.
		 * This script works in combination with "Static_Track_Parent_CS" in the parent object, and "Static_Track_Piece_CS" in the piece.
		*/

        // User options >>
        public Static_Track_Piece_CS Piece_Script;
        public Static_Track_Parent_CS Parent_Script;
        public MeshFilter This_MeshFilter;
        public Mesh Mesh_A;
        public Mesh Mesh_B;
        public bool Is_Left;
        // << User options

        Mesh currentMesh;
        bool isRepairing;


        void Start()
        {
            currentMesh = Mesh_A;
        }


        void LateUpdate()
        {
            if (Piece_Script.enabled == false)
            {
                return;
            }

            if (Is_Left)
            { // Left
                if (Parent_Script.Switch_Mesh_L)
                {
                    currentMesh = Mesh_A;
                }
                else
                {
                    currentMesh = Mesh_B;
                }
            }
            else
            { // Right
                if (Parent_Script.Switch_Mesh_R)
                {
                    currentMesh = Mesh_A;
                }
                else
                {
                    currentMesh = Mesh_B;
                }
            }

            This_MeshFilter.mesh = currentMesh;
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Disable this script.
            this.enabled = false;

            // Switch the flag.
            isRepairing = true;
        }


        void Track_Repaired_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Enable this script.
            this.enabled = true;

            // Switch the flag.
            isRepairing = false;
        }


        void Duplicated()
        { // Called from "Static_Track_Piece_CS" (this).

            // Destroy this script.
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".

            // Check the track is being repaired.
            if (isRepairing)
            {
                return;
            }

            this.enabled = !isPaused;
        }

    }
}
