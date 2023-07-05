using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Static_Track_Setting_CS : MonoBehaviour
    {
        /*
		 * This script is used for converting Physics_Track into Static_Track by "Create_TrackBelt_CSEditor" in the editor.
		 * This script is attached to all the track piece while the converting.
		 * In the runtime, this script set the type of the piece while detecting the wheel touching this track piece.
		 * Also, this script attaches required scripts, and removes the useless components in the track piece.
		*/


        public bool Use_2ndPiece; // Set by "Create_TrackBelt_CSEditor".

        int type;
        Transform anchorTransform;

        Transform thisTransform;
        Transform rootTransform;
        Rigidbody bodyRigidbody;
        float count;


        void Start()
        {
            thisTransform = transform;
            rootTransform = thisTransform.root;
            bodyRigidbody = rootTransform.GetComponentInChildren<Rigidbody>();
        }


        void Update()
        {
            // Check the track is not broken.
            if (thisTransform.parent == rootTransform)
            { // The track has been broken.
                Destroy(this);
                return;
            }

            // Set the type.
            Set_Type();

            // Check the tank stands still.
            if (bodyRigidbody.velocity.magnitude < 0.1f)
            { // The tank almost stops.

                count += Time.deltaTime;
                if (count > 1.0f)
                {
                    // Stop the time.
                    Time.timeScale = 0.0f;

                    // Show the cursor.
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }


        void Set_Type()
        {
            // Detect the wheel touching this track piece.
            var hitColliders = Physics.OverlapSphere(thisTransform.position, 0.1f, Layer_Settings_CS.Layer_Mask);
            foreach (Collider hitCollider in hitColliders)
            {
                var hitColliderParent = hitCollider.transform.parent;
                if (hitColliderParent == null)
                {
                    continue;
                }

                if (hitColliderParent.GetComponent<Create_RoadWheel_CS>() || hitColliderParent.GetComponent<Create_RoadWheel_Type89_CS>())
                { // The hit collider must be a roadwheel that can move up and down with the suspension.
                    type = 1; // Anchor type
                    anchorTransform = hitCollider.transform;
                    return;
                }

                if (hitColliderParent.GetComponent<Create_SprocketWheel_CS>() || hitColliderParent.GetComponent<Create_IdlerWheel_CS>() || hitColliderParent.GetComponent<Create_SupportWheel_CS>())
                { // The hit collider must be a wheel without any suspension.
                    type = 0; // Static type
                    anchorTransform = null;
                    return;
                }
            }

            // There is no wheel touching this track piece.
            type = 2; // Dynamic type
            anchorTransform = null;
        }


        void Set_Child_Scripts(Static_Track_Parent_CS parentScript)
        { // Called from "Create_TrackBelt_CSEditor".

            // Add "Static_Track_Piece_CS" and set the values.
            var pieceScript = gameObject.AddComponent<Static_Track_Piece_CS>();

            // Set the "This_Transform".
            pieceScript.This_Transform = thisTransform;

            // Set the "Parent_Script".
            pieceScript.Parent_Script = parentScript;

            // Set the "Is_Left".
            pieceScript.Is_Left = (thisTransform.localPosition.y > 0.0f);

            // Set the "Invert_Angle".
            var localAngleY = thisTransform.localEulerAngles.y;
            if (localAngleY > 90.0f && localAngleY < 270.0f)
            { // Upper piece.
                pieceScript.Invert_Angle = 180.0f;
            }
            else
            { // Lower piece
                pieceScript.Invert_Angle = 0.0f;
            }

            // Set the "Half_Length".
            pieceScript.Half_Length = pieceScript.Parent_Script.Length * 0.5f;

            // Set the "Pieces_Count".
            pieceScript.Pieces_Count = thisTransform.parent.childCount / 2;

            // Find the front and rear pieces using this name.
            var baseName = this.name.Substring(0, 12); // e.g. "TrackBelt_L_"
            var thisNum = int.Parse(this.name.Substring(12)); // e.g. "1"

            // Find the front piece.
            Transform frontTransform = thisTransform.parent.Find(baseName + (thisNum + 1)); // Find a piece having next number.
            if (frontTransform == null)
            { // It must be the last piece.
                frontTransform = thisTransform.parent.Find(baseName + 1); // The 1st piece.
            }
            pieceScript.Front_Transform = frontTransform;

            // Find the rear piece.
            Transform rearTransform = thisTransform.parent.Find(baseName + (thisNum - 1)); // Find a piece having previous number.
            if (rearTransform == null)
            { // It must be the 1st piece.
                rearTransform = thisTransform.parent.Find(baseName + (thisTransform.parent.childCount / 2)); // The last piece.
            }
            pieceScript.Rear_Transform = rearTransform;

            // Set the "Type".
            switch (type)
            {
                case 0: // Static
                    if (frontTransform.GetComponent<Static_Track_Setting_CS>().type == 1)
                    { // The front piece is Anchor type.
                        // Change to Dynamic type.
                        pieceScript.Type = 2;
                    }
                    else if (rearTransform.GetComponent<Static_Track_Setting_CS>().type == 1)
                    { // The rear piece is Anchor type.
                        // Change to Dynamic type.
                        pieceScript.Type = 2;
                    }
                    else
                    {
                        pieceScript.Type = 0; // Static.
                    }
                    break;

                case 1: // Anchor
                    pieceScript.Type = 1;
                    pieceScript.Anchor_Name = anchorTransform.name;
                    pieceScript.Anchor_Parent_Name = anchorTransform.parent.name;
                    break;

                case 2: // Dynamic
                    pieceScript.Type = 2;
                    break;
            }


            // Add "Static_Track_Switch_Mesh_CS".
            if (Use_2ndPiece)
            {
                var switchScript = gameObject.AddComponent<Static_Track_Switch_Mesh_CS>();
                switchScript.Piece_Script = pieceScript;
                switchScript.Parent_Script = parentScript;
                switchScript.This_MeshFilter = switchScript.GetComponent<MeshFilter>();
                switchScript.Mesh_A = switchScript.This_MeshFilter.sharedMesh;
                switchScript.Mesh_B = switchScript.Piece_Script.Front_Transform.GetComponent<MeshFilter>().sharedMesh;
                switchScript.Is_Left = switchScript.Piece_Script.Is_Left;
            }


            // Remove the useless components.
            var hingeJoint = GetComponent<HingeJoint>();
            if (hingeJoint)
            {
                DestroyImmediate(hingeJoint);
            }
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody)
            {
                DestroyImmediate(rigidbody);
            }
            var damageScript = GetComponent<Damage_Control_03_Physics_Track_Piece_CS>();
            if (damageScript)
            {
                DestroyImmediate(damageScript);
            }
            var stabilizerScript = GetComponent<Stabilizer_CS>();
            if (stabilizerScript)
            {
                DestroyImmediate(stabilizerScript);
            }


            // Disable the Colliders.
            var colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }


            // Remove the child objects such as Reinforce piece.
            for (int i = 0; i < transform.childCount; i++)
            {
                var childObject = transform.GetChild(i).gameObject;
                if (childObject.layer == Layer_Settings_CS.Reinforce_Layer)
                { // Reinforce.
                    DestroyImmediate(childObject);
                }
            }
        }


        void Set_Front_And_Rear_Scripts()
        { // Called from "Create_TrackBelt_CSEditor".
            // Set the "Front_Script" and "Rear_Script" in the "Static_Track_Piece_CS".
            var pieceScript = gameObject.GetComponent<Static_Track_Piece_CS>();
            pieceScript.Front_Script = pieceScript.Front_Transform.GetComponent<Static_Track_Piece_CS>();
            pieceScript.Rear_Script = pieceScript.Rear_Transform.GetComponent<Static_Track_Piece_CS>();
        }


        void Remove_Setting_Script()
        { // Called from "Create_TrackBelt_CSEditor".
            DestroyImmediate(this);
        }

    }

}