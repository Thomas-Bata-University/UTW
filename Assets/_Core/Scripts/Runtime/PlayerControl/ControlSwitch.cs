using FishNet;
using System.Collections.Generic;
using UnityEngine;

public class ControlSwitch : MonoBehaviour
{
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    private VehicleManager vehicleManager;

    private void Start()
    {
        vehicleManager = GetComponent<VehicleManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSeat(TankPositions.DRIVER);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSeat(TankPositions.GUNNER);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSeat(TankPositions.OBSERVER);
        }
    }

    private void ChangeSeat(TankPositions tankPosition)
    {
        vehicleManager.InGameSwap(InstanceFinder.ClientManager.Connection, tankPosition);
    }
}