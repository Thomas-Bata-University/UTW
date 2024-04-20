using UnityEngine;

public class DriverController : PlayerController {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    private float moveSpeed = 10f;

    private bool autoDrive = false;

    protected override void Start() {
        tankPosition = TankPositions.DRIVER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
    }

    private void Update() {
        MouseLook(100f, 100f, 80f);
        Move();

        if (Input.GetKeyDown(KeyCode.K)) {
            autoDrive = !autoDrive;
        }

        if (autoDrive) {
            tankPart.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    protected override void Move() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        tankPart.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

}//END