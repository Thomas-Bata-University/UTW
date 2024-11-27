using UnityEngine;


namespace ChobiAssets.PTM
{


    public class Utility_Simple_Rename_CS : MonoBehaviour
    {
       
        [SerializeField] string Base_Name = "Tank";


        [ContextMenu("Rename")]


        void Rename()
        {

            var childTransforms = GetComponentsInChildren<Transform>();
            for (int i = 0; i < childTransforms.Length; i++)
            {
                if (childTransforms[i] == this.transform)
                {
                    continue;
                }

                childTransforms[i].name = Base_Name + " (" + i + ")";
            }

        }


    }

}
