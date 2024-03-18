using System.Collections.Generic;
using UnityEngine;

public class ControlSwitch : MonoBehaviour {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    PlayerController playerController;

    DriverController driverController;
    ObserverController observerController;

    private List<PlayerController> playerControllers;

    private void Start() {
        driverController = FindObjectOfType<DriverController>();
        observerController = FindObjectOfType<ObserverController>();

        playerControllers = new List<PlayerController> { driverController, observerController };

        //TODO Enable controller by chosen role.
        playerController = driverController;

        playerController.enabled = true;
        playerController.SetPosition();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeSeat(TankPositions.Driver);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeSeat(TankPositions.Observer);
        } else if (!Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeSeat(TankPositions.Shooter);
        }
    }

    private void ChangeSeat(TankPositions seat) {
        playerControllers.ForEach(controller => controller.enabled = false);

        switch (seat) {
            case TankPositions.Driver: playerController = driverController; break;
            case TankPositions.Observer: playerController = observerController; break;
        }

        playerController.enabled = true;
        playerController.SetPosition();
    }

}//END