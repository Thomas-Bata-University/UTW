using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Extra_Collider_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Extra_Collier" in the tank.
		 * This script only sets the Layer of this gameobject.
		*/


        void Start()
        {
            // Set the layer.
            gameObject.layer = Layer_Settings_CS.Extra_Collider_Layer;

            Destroy(this);
        }

    }

}