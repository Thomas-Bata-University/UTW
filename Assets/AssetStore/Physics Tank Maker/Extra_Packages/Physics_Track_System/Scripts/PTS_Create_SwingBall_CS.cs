using UnityEngine;

namespace ChobiAssets.PTS
{

	public class PTS_Create_SwingBall_CS : MonoBehaviour
	{
        /* 
		 * This script is attached to the "Create_SwingBall" in the tank.
		 * This script is used for creating and setting the swing balls by the editor script.
		 * The swing balls are used for giving a dynamic sag to the "Static_Track".
		*/

        public float Distance = 2.7f;
		public int Num = 1;
		public float Spacing = 1.7f;
		public bool Set_Individually = false;
		public Vector3 [] Pos_Array;
		public float Mass = 10.0f;
		public bool Gravity = false;
		public float Radius = 0.1f;
		public float Range = 0.3f;
		public float Spring = 500.0f;
		public float Damper = 100.0f;
		public int Layer = 0;
		public PhysicMaterial Collider_Material;

        // For editor script.
        public bool Has_Changed;


        void OnDrawGizmosSelected ()
		{
            Gizmos.color = Color.green;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                Vector3 pos = childTransform.position;
                Gizmos.DrawSphere(pos, Radius);
                ConfigurableJoint joint = childTransform.GetComponent<ConfigurableJoint>();
                if (joint.connectedBody)
                {
                    Transform bodyTransform = joint.connectedBody.transform;
                    Vector3 anchorPos = bodyTransform.position + (bodyTransform.right * joint.connectedAnchor.x) + (bodyTransform.up * joint.connectedAnchor.y) + (bodyTransform.forward * joint.connectedAnchor.z);
                    Gizmos.DrawLine(anchorPos - (childTransform.up * Range), anchorPos + (childTransform.up * Range));
                }
            }
        }

	}

}