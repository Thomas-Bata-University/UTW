using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Delete_Timer_CS : MonoBehaviour
    {
        /*
		 * This script is used for destroying NavMeshObstacle object instantiated by "AI_CS" in the runtime.
		*/

        public float Count;

        void Start()
        {
            Destroy(this.gameObject, Count);
        }

    }

}