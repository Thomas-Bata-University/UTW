using UnityEngine;

namespace ChobiAssets.PTM
{
	[ System.Serializable]
	public class CreatingScrollTrackInfo
	{
		public Vector3 position;
		public float radius;
		public float startAngle;
		public float endAngle;
	}

	public class Create_ScrollTrack_CS : MonoBehaviour
	{
		/* 
		 * This script is attached to the "Create_ScrollTrack" that is used for creating the mesh for "Scroll_Track".
		*/

		public bool Is_Left = true;
		public float Pos_X;
		public float Width = 0.36f;
		public float Height = 0.07f;
		public Material Mat;
		public float Scale = 0.8f;
		public float Line_A = 0.8f;
		public float Line_B = 0.75f;
		public float Line_C = 0.75f;
		public float Line_D = 0.5f;
		public float Line_E = 0.5f;
		public float Line_F = 0.45f;
		public float Line_G = 0.45f;
		public float Line_H = 0.2f;
		public int Guide_Count = 1;
		public float Guide_Height = 0.07f;
		public float[] Guide_Positions;
		public float Guide_Line_Top = 0.13f;
		public float Guide_Line_Bottom = 0.08f;

		public Vector3[] Position_Array;
		public float [] Radius_Array;
		public float [] Start_Angle_Array;
		public float [] End_Angle_Array;

		public bool Show_Texture = false;

        public Mesh Saved_Mesh;

        // For editor script.
        public bool Has_Changed;


        void Start()
		{
			Check_Mesh();
		}


        void Check_Mesh()
        {
            // Check the mesh had been saved or not.
            MeshFilter thisMeshFilter = GetComponent<MeshFilter>();
            if (thisMeshFilter && thisMeshFilter.sharedMesh)
            {
                if (thisMeshFilter.sharedMesh.name == " Instance")
                {
                    Debug.LogWarning("The mesh of '" + this.name + "' is not saved yet.");
                }
            }
            else
            {
                Debug.LogWarning("The mesh of '" + this.name + "' is not created yet.");
            }
            Destroy(this);
        }


        void OnDrawGizmosSelected()
        {
            MeshFilter thisFilter = GetComponent<MeshFilter>();
            if (thisFilter && thisFilter.sharedMesh)
            {
                // Draw vertices.
                Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, transform.localScale);
                Vector3[] vertices = thisFilter.sharedMesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(vertices[i] + transform.position, 0.01f);
                }

                // Draw UV lines.
                if (Show_Texture)
                {
                    Texture tex = GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
                    float size = 2.0f;
                    Gizmos.DrawGUITexture(new Rect(size / 2.0f, size / 2.0f, -size, -size), tex);
                    float[] heightArray = {
                        Line_A,
                        Line_B,
                        Line_C,
                        Line_D,
                        Line_E,
                        Line_F,
                        Line_G,
                        Line_H,
                        Guide_Line_Top,
                        Guide_Line_Bottom
                    };
                    Gizmos.color = Color.yellow;
                    for (int i = 0; i < heightArray.Length; i++)
                    {
                        Vector3 tempPos = new Vector3(size / 2.0f, -size / 2.0f + (heightArray[i] * size), 0.0f);
                        for (int j = 0; j < 6; j++)
                        { // Make the lines strong.
                            Gizmos.DrawLine(tempPos, tempPos + Vector3.left * size);
                        }
                    }
                }
            }
        }

    }

}
