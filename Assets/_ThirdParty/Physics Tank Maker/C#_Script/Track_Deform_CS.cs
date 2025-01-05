using UnityEngine;

namespace ChobiAssets.PTM
{

    [System.Serializable]
    public class IntArray
    {
        public int[] intArray;
        public IntArray(int[] newIntArray)
        {
            intArray = newIntArray;
        }
    }


    public class Track_Deform_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Scroll_Track" in the tank.
		 * This script controls the deforming of the Scroll_Track.
		*/


        // User options >>
        public int Anchor_Num = 1;
        public Transform[] Anchor_Array;
        public string[] Anchor_Names;
        public string[] Anchor_Parent_Names;
        public float[] Width_Array;
        public float[] Height_Array;
        public float[] Offset_Array;
        public float[] Initial_Pos_Array;
        public Vector3[] Initial_Vertices;
        public IntArray[] Movable_Vertices_List;
        // << User options

        // For editor script.
        public bool Has_Changed;

        Mesh thisMesh;
        MainBody_Setting_CS bodyScript;
        Vector3[] currentVertices;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            // Check the anchor wheels.
            for (int i = 0; i < Anchor_Array.Length; i++)
            {
                if (Anchor_Array[i] == null)
                {
                    // Find the anchor wheel with reference to the name.
                    if (string.IsNullOrEmpty(Anchor_Names[i]) == false && string.IsNullOrEmpty(Anchor_Parent_Names[i]) == false)
                    {
                        Anchor_Array[i] = transform.parent.Find(Anchor_Parent_Names[i] + "/" + Anchor_Names[i]);
                    }
                    else
                    {
                        Debug.LogError("Anchor wheels of Scroll Track are not assigned.");
                        Destroy(this);
                        return;
                    }
                }
            }

            thisMesh = GetComponent<MeshFilter>().mesh;
            currentVertices = new Vector3[Initial_Vertices.Length];
            bodyScript = GetComponentInParent<MainBody_Setting_CS>();
        }


        void Update()
        {
            // Check the tank is visible by any camera.
            if (bodyScript.Visible_Flag)
            {
                Deform();
            }
        }


        void Deform()
        {
            Initial_Vertices.CopyTo(currentVertices, 0);
            for (int i = 0; i < Anchor_Array.Length; i++)
            {
                if (Anchor_Array[i] == null)
                {
                    continue;
                }

                var dist = Anchor_Array[i].localPosition.x - Initial_Pos_Array[i];
                for (int j = 0; j < Movable_Vertices_List[i].intArray.Length; j++)
                {
                    currentVertices[Movable_Vertices_List[i].intArray[j]].y += dist;
                }
            }
            thisMesh.vertices = currentVertices;
        }


        void OnDrawGizmosSelected()
        {
            // Draw the square.
            if (Anchor_Array != null && Anchor_Array.Length != 0 && Offset_Array != null && Offset_Array.Length != 0)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < Anchor_Array.Length; i++)
                {
                    if (Anchor_Array[i] != null)
                    {
                        var tempSize = new Vector3(0.0f, Height_Array[i], Width_Array[i]);
                        var tempCenter = Anchor_Array[i].position;
                        tempCenter.y += Offset_Array[i];
                        Gizmos.DrawWireCube(tempCenter, tempSize);
                    }
                }
            }

            // Draw the vertices.
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter && meshFilter.sharedMesh)
            {
                Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, transform.localScale);
                var vertices = meshFilter.sharedMesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(vertices[i] + transform.position, 0.01f);
                }
            }
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }


        void OnDestroy()
        { // Avoid memory leak.
            if (thisMesh)
            {
                Destroy(thisMesh);
            }
        }

    }

}