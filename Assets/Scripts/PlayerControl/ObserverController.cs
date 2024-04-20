using UnityEngine;

public class ObserverController : PlayerController {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    public float rotationSpeed = 20f;

    public float rightMaxRotation = 45f;
    public float leftMaxRotation = -45f;

    public float downMaxRotation = 5f;
    public float upMaxRotation = -10f;

    private float xRotation;
    private float yRotation;

    protected override void Start() {
        tankPosition = TankPositions.OBSERVER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
    }

    private void Update() {
        MouseLook(100f, 100f, 80f);
        Move();
    }

    protected override void Move() {
        float horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime * rotationSpeed;
        float verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime * rotationSpeed;

        yRotation += horizontalInput;
        yRotation = Mathf.Clamp(yRotation, leftMaxRotation, rightMaxRotation);

        xRotation -= verticalInput;
        xRotation = Mathf.Clamp(xRotation, upMaxRotation, downMaxRotation);

        tankPart.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

}//END