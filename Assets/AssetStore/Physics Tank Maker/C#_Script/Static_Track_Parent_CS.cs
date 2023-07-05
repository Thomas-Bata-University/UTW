using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Static_Track_Parent_CS : MonoBehaviour
    {
        /*
		 * This script controls the position and rotation of Static_Track pieces.
		 * This script works in combination with "Static_Track_Piece_CS" in the track pieces.
		*/


        // User options >>
        public Transform Reference_L;
        public Transform Reference_R;
        public string Reference_Name_L;
        public string Reference_Name_R;
        public string Reference_Parent_Name_L;
        public string Reference_Parent_Name_R;
        public float Length;
        public float Radius_Offset;
        public float Mass = 30.0f;
        public Mesh Track_L_Shadow_Mesh;
        public Mesh Track_R_Shadow_Mesh;
        public float RoadWheel_Effective_Range = 0.4f;
        public float SwingBall_Effective_Range = 0.15f;
        public float Anti_Stroboscopic_Min = 0.125f;
        public float Anti_Stroboscopic_Max = 0.375f;
        // << User options

        // For editor script.
        public bool Has_Changed;

        // Set by "Create_TrackBelt_CSEditor".
        public RoadWheelsProp[] RoadWheelsProp_Array;
        public float Stored_Body_Mass;

        // Referred to from "Static_Wheel_Parent_CS".
        public float Reference_Radius_L;
        public float Reference_Radius_R;
        public float Delta_Ang_L;
        public float Delta_Ang_R;

        // Referred to from "Static_Track_Piece_CS".
        public float Rate_L;
        public float Rate_R;
        public bool Is_Visible;

        // Referred to from "Static_Track_Switch_Mesh_CS".
        public bool Switch_Mesh_L;
        public bool Switch_Mesh_R;

        float leftPreviousAng;
        float rightPreviousAng;
        float leftAngRate;
        float rightAngRate;
        MainBody_Setting_CS bodyScript;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            var bodyTransform = transform.parent;
            bodyScript = bodyTransform.GetComponent<MainBody_Setting_CS>();

            // Find the reference wheels.
            if (Reference_L == null)
            { // The left reference wheel has been lost by modifying.
                if (string.IsNullOrEmpty(Reference_Name_L) == false && string.IsNullOrEmpty(Reference_Parent_Name_L) == false)
                {
                    Reference_L = bodyTransform.Find(Reference_Parent_Name_L + "/" + Reference_Name_L);
                }
            }
            if (Reference_R == null)
            { // The right reference wheel has been lost by modifying.
                if (string.IsNullOrEmpty(Reference_Name_R) == false && string.IsNullOrEmpty(Reference_Parent_Name_R) == false)
                {
                    Reference_R = bodyTransform.Find(Reference_Parent_Name_R + "/" + Reference_Name_R);
                }
            }

            // Set "Reference_Radius_#" for "Static_Wheel_Parent_CS", and set the angle rate for controlling the speed.
            if (Reference_L && Reference_R)
            {
                Reference_Radius_L = Reference_L.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x + Radius_Offset;
                leftAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_L) / Length);
                Reference_Radius_R = Reference_R.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x + Radius_Offset;
                rightAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_R) / Length);
            }
            else
            {
                Debug.LogError("'Reference wheels' for Static_Track cannot be found.");
                this.enabled = false;
                return;
            }

            // Send this reference to all the "Static_Wheel_Parent_CS" in the tank.
            var staticWheelParentScripts = bodyTransform.GetComponentsInChildren<Static_Wheel_Parent_CS>();
            for (int i = 0; i < staticWheelParentScripts.Length; i++)
            {
                staticWheelParentScripts[i].Prepare_With_Static_Track(this);
            }
        }


        void Update()
        {
            // Check the tank is visible by any camera.
            Is_Visible = bodyScript.Visible_Flag;

            if (Is_Visible)
            {
                Speed_Control();
            }
        }


        void Speed_Control()
        {
            // Left
            var currentAng = Reference_L.localEulerAngles.y;
            Delta_Ang_L = Mathf.DeltaAngle(currentAng, leftPreviousAng);
            var tempClampRate = Random.Range(Anti_Stroboscopic_Min, Anti_Stroboscopic_Max);
            Delta_Ang_L = Mathf.Clamp(Delta_Ang_L, -leftAngRate * tempClampRate, leftAngRate * tempClampRate); // Anti Stroboscopic Effect.
            Rate_L += Delta_Ang_L / leftAngRate;
            if (Rate_L > 1.0f)
            {
                Rate_L %= 1.0f;
                Switch_Mesh_L = !Switch_Mesh_L;
            }
            else if (Rate_L < 0.0f)
            {
                Rate_L = 1.0f + (Rate_L % 1.0f);
                Switch_Mesh_L = !Switch_Mesh_L;
            }
            leftPreviousAng = currentAng;

            // Right
            currentAng = Reference_R.localEulerAngles.y;
            Delta_Ang_R = Mathf.DeltaAngle(currentAng, rightPreviousAng);
            Delta_Ang_R = Mathf.Clamp(Delta_Ang_R, -rightAngRate * tempClampRate, rightAngRate * tempClampRate); // Anti Stroboscopic Effect.
            Rate_R += Delta_Ang_R / rightAngRate;
            if (Rate_R > 1.0f)
            {
                Rate_R %= 1.0f;
                Switch_Mesh_R = !Switch_Mesh_R;
            }
            else if (Rate_R < 0.0f)
            {
                Rate_R = 1.0f + (Rate_R % 1.0f);
                Switch_Mesh_R = !Switch_Mesh_R;
            }
            rightPreviousAng = currentAng;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }

    }

}