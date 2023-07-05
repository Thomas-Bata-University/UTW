using UnityEngine;
using System.Collections.Generic;


namespace ChobiAssets.PTM
{

    public class Static_Track_Piece_CS : MonoBehaviour
    {
        /*
		 * This script controls the position and rotation of Static_Track pieces.
		 * This script works in combination with "Static_Track_Parent_CS" in the parent object.
		*/


        // User options >>
        public Transform This_Transform;
        public Static_Track_Parent_CS Parent_Script;
        public int Type; // 0=Static, 1=Anchor, 2=Dynamic.
        public Transform Front_Transform;
        public Transform Rear_Transform;
        public Static_Track_Piece_CS Front_Script;
        public Static_Track_Piece_CS Rear_Script;
        public string Anchor_Name;
        public string Anchor_Parent_Name;
        public Transform Anchor_Transform;
        public bool Simple_Flag;
        public bool Is_Left;
        public float Invert_Angle; // Lower piece => 0.0f, Upper pieces => 180.0f.
        public float Half_Length;
        public int Pieces_Count;
        // << User options

        // For editor script.
        public bool Has_Changed;

        Vector3 invisiblePos;
        float invisibleAngY;
        float initialPosX; // Only for anchor type.
        float anchorInitialPosX; // Only for anchor type.
        Vector3 currentAngles = new Vector3(0.0f, 0.0f, 270.0f);
        bool isRepairing;


        void Start()
        {
            switch (Type)
            {
                case 0: // Static.
                    Basic_Settings();
                    break;

                case 1: // Anchor.
                    Find_Anchor();
                    Basic_Settings();
                    break;

                case 2: // Dynamic.
                    Basic_Settings();
                    break;
            }
        }


        void Basic_Settings()
        {
            // Set the initial position and angle.
            invisiblePos = This_Transform.localPosition;
            invisibleAngY = This_Transform.localEulerAngles.y;
        }


        void Find_Anchor()
        {
            if (Anchor_Transform == null)
            { // The "Anchor_Transform" should have been lost by modifying.
                // Get the "Anchor_Transform" with reference to the name.
                if (string.IsNullOrEmpty(Anchor_Name) == false && string.IsNullOrEmpty(Anchor_Parent_Name) == false)
                {
                    Anchor_Transform = This_Transform.parent.parent.Find(Anchor_Parent_Name + "/" + Anchor_Name);
                }
            }

            // Set the initial hight. (Axis X = hight)
            if (Anchor_Transform)
            {
                initialPosX = This_Transform.localPosition.x;
                anchorInitialPosX = Anchor_Transform.localPosition.x;
            }
            else
            {
                Debug.LogWarning("'Anchor_Transform' cannot be found in " + This_Transform.name + ".");
                // Change this piece to "Dynamic" type.
                Type = 2;
            }
        }


        void LateUpdate()
        {
            // Check the tank is visible by any camera.
            if (Parent_Script.Is_Visible)
            {
                switch (Type)
                {
                    case 0: // Static.
                        Slide_Control();
                        break;

                    case 1: // Anchor.
                        Anchor_Control();
                        Slide_Control();
                        break;

                    case 2: // Dynamic.
                        Dynamic_Control();
                        Slide_Control();
                        break;
                }
            }
        }


        void Slide_Control()
        {
            if (Is_Left)
            { // Left
                This_Transform.localPosition = Vector3.Lerp(invisiblePos, Rear_Script.invisiblePos, Parent_Script.Rate_L);
                currentAngles.y = Mathf.LerpAngle(invisibleAngY, Rear_Script.invisibleAngY, Parent_Script.Rate_L);
                This_Transform.localRotation = Quaternion.Euler(currentAngles);
            }
            else
            { // Right
                This_Transform.localPosition = Vector3.Lerp(invisiblePos, Rear_Script.invisiblePos, Parent_Script.Rate_R);
                currentAngles.y = Mathf.LerpAngle(invisibleAngY, Rear_Script.invisibleAngY, Parent_Script.Rate_R);
                This_Transform.localRotation = Quaternion.Euler(currentAngles);
            }
        }


        void Dynamic_Control()
        {
            if (Simple_Flag)
            {
                invisiblePos = Vector3.Lerp(Rear_Script.invisiblePos, Front_Script.invisiblePos, 0.5f);
                invisibleAngY = Mathf.LerpAngle(Rear_Script.invisibleAngY, Front_Script.invisibleAngY, 0.5f);
            }
            else
            {
                // Calculate the end positions.
                var tempRad = Rear_Script.invisibleAngY * Mathf.Deg2Rad;
                var rearEndPos = Rear_Script.invisiblePos + new Vector3(Half_Length * Mathf.Sin(tempRad), 0.0f, Half_Length * Mathf.Cos(tempRad));
                tempRad = Front_Script.invisibleAngY * Mathf.Deg2Rad;
                var frontEndPos = Front_Script.invisiblePos - new Vector3(Half_Length * Mathf.Sin(tempRad), 0.0f, Half_Length * Mathf.Cos(tempRad));
                
                // Set the position and angle.
                invisiblePos = Vector3.Lerp(rearEndPos, frontEndPos, 0.5f);
                invisibleAngY = Mathf.Rad2Deg * Mathf.Atan((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + Invert_Angle;
            }
        }


        void Anchor_Control()
        {
            // Set the position. (Axis X = hight)
            invisiblePos.x = initialPosX + (Anchor_Transform.localPosition.x - anchorInitialPosX);

            if (Simple_Flag)
            {
                return;
            }

            // Calculate the end positions.
            var tempRad = Rear_Script.invisibleAngY * Mathf.Deg2Rad;
            var rearEndPos = Rear_Script.invisiblePos + new Vector3(Half_Length * Mathf.Sin(tempRad), 0.0f, Half_Length * Mathf.Cos(tempRad));
            tempRad = Front_Script.invisibleAngY * Mathf.Deg2Rad;
            var frontEndPos = Front_Script.invisiblePos - new Vector3(Half_Length * Mathf.Sin(tempRad), 0.0f, Half_Length * Mathf.Cos(tempRad));
            
            // Set the angle.
            invisibleAngY = Mathf.Rad2Deg * Mathf.Atan((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + Invert_Angle;
        }

        
        public void Start_Breaking(float lifeTime)
        { // Called from "Damage_Control_04_Track_Collider_CS" in the Track_Collider.

            // Reset the rate values in the parent script.
            if (Parent_Script)
            {
                Parent_Script.Rate_L = 0.0f;
                Parent_Script.Rate_R = 0.0f;
            }

            // Create the pieces list.
            var clonePiecesList = new List<GameObject>();
            var pieceScript = this;
            for (int i = 0; i < Pieces_Count; i++)
            {
                // Create the clone piece.
                var clonePieceObject = Instantiate(pieceScript.gameObject, This_Transform.parent);

                // Send message to "Static_Track_Piece_CS", "Track_Joint_CS", "Static_Track_Switch_Mesh_CS".
                clonePieceObject.BroadcastMessage("Duplicated", SendMessageOptions.DontRequireReceiver);

                // Add to the list.
                clonePiecesList.Add(clonePieceObject);

                // Set the life time.
                Destroy(clonePieceObject, lifeTime);

                // Set the next piece.
                pieceScript = pieceScript.Front_Script;
            }

            // Connect the clone pieces by HingeJoint.
            for (int i = 0; i < clonePiecesList.Count - 1; i++)
            {
                var hingeJoint = clonePiecesList[i].AddComponent<HingeJoint>();
                hingeJoint.connectedBody = clonePiecesList[i + 1].GetComponent<Rigidbody>();
                hingeJoint.anchor = new Vector3(0.0f, 0.0f, Half_Length);
                hingeJoint.axis = new Vector3(1.0f, 0.0f, 0.0f);
                hingeJoint.breakForce = 100000.0f;
            }
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Make this joint invisible.
            GetComponent<Renderer>().enabled = false;

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

            // Make this piece visible.
            GetComponent<Renderer>().enabled = true;

            // Enable this script.
            this.enabled = true;

            // Switch the flag.
            isRepairing = false;
        }


        void Duplicated()
        { // Called from "Static_Track_Piece_CS" (this).

            // Make this piece visible.
            GetComponent<Renderer>().enabled = true;

            // Add rigidbody.
            var rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.mass = Parent_Script.Mass;

            // Enable the colliders.
            var colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }

            // Add reinforce object.
            var reinforceObject = new GameObject("Reinforce");
            reinforceObject.transform.parent = This_Transform;
            reinforceObject.transform.localPosition = Vector3.zero;
            var sphereCollider = reinforceObject.AddComponent<SphereCollider>();
            sphereCollider.radius = Half_Length * 2.0f;
            reinforceObject.layer = Layer_Settings_CS.Reinforce_Layer;

            // Change the hierarchy.
            This_Transform.parent = null;

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