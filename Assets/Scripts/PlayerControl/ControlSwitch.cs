using System.Collections.Generic;
using UnityEngine;

public class ControlSwitch : MonoBehaviour {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    PlayerController playerController;

    DriverController driverController;
    GunnerController observerController;

    private List<PlayerController> playerControllers;

    private void Start() {
        driverController = FindObjectOfType<DriverController>();
        observerController = FindObjectOfType<GunnerController>();

        playerControllers = new List<PlayerController> { driverController, observerController };

        //TODO Enable controller by chosen role.
        playerController = driverController;

        playerController.enabled = true;
        playerController.SetPosition();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeSeat(TankPositions.DRIVER);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeSeat(TankPositions.OBSERVER);
        } else if (!Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeSeat(TankPositions.GUNNER);
        }
    }

    private void ChangeSeat(TankPositions seat) {
        playerControllers.ForEach(controller => controller.enabled = false);

        switch (seat) {
            case TankPositions.DRIVER: playerController = driverController; break;
            case TankPositions.OBSERVER: playerController = observerController; break;
        }

        playerController.enabled = true;
        playerController.SetPosition();
    }

}//END