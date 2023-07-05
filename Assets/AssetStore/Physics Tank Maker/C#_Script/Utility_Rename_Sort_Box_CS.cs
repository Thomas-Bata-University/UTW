using System.Collections.Generic;
using UnityEngine;


namespace ChobiAssets.PTM
{


    public class Utility_Rename_Sort_Box_CS : MonoBehaviour
    {

        [SerializeField] Mesh Mesh_1 = default;
        [SerializeField] Mesh Mesh_2 = default;
        [SerializeField] Mesh Mesh_3 = default;

        [SerializeField] string Name_1 = "Box_1";
        [SerializeField] string Name_2 = "Box_2";
        [SerializeField] string Name_3 = "Box_3";


        [ContextMenu("Rename")]


        void Rename()
        {
            var childTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform tempTransform in childTransforms)
            {
                if (tempTransform == this.transform)
                {
                    continue;
                }

                var tempMesh = tempTransform.GetComponent<MeshFilter>().sharedMesh;

                if (tempMesh == Mesh_1)
                {
                    tempTransform.name = Name_1;
                    continue;
                }

                if (tempMesh == Mesh_2)
                {
                    tempTransform.name = Name_2;
                    continue;
                }

                if (tempMesh == Mesh_3)
                {
                    tempTransform.name = Name_3;
                    continue;
                }
            }

            Sort();
        }


        void Sort()
        {
            List<Transform> childrenList = new List<Transform>();

            var children = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                childrenList.Add(child);
            }

            childrenList.Sort((child1, child2) => string.Compare(child1.name, child2.name));

            foreach (Transform child in childrenList)
            {
                child.SetAsLastSibling();
            }
        }

    }

}
