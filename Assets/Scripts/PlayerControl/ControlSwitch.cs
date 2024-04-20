using FishNet;
using UnityEngine;

public class ControlSwitch : MonoBehaviour {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    private VehicleManager vehicleManager;

    private void Start() {
        vehicleManager = GetComponent<VehicleManager>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeSeat();
        }
    }

    private void ChangeSeat() {
        vehicleManager.InGameSwap(InstanceFinder.ClientManager.Connection);
    }

}//END