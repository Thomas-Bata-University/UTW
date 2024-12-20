using FishNet;
using FishNet.Connection;
using UnityEngine;
using UTW;

public class DriverController : PlayerController {

    private SceneManager sceneManager;
    private NetworkConnection conn;

    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    [Range(0, 20)]
    private float moveSpeed = 5f;
    [SerializeField]
    [Range(0, 30)]
    private float rotationSpeed = 15f;
    [SerializeField]
    private float acceleration = 3f;
    [SerializeField]
    private float deceleration = 5f;
    [SerializeField]
    private float maxTrackSpeed = 0.5f;
    private float currentSpeed = 0f;
    private float currentRotationSpeed = 0f;

    private bool autoDrive = false;

    protected override void Start() {
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;

        tankPosition = TankPositions.DRIVER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
    }

    private void Update() {
        MouseLook(100f, 100f, 80f);
        Move();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Exit(conn, sceneManager);
        }

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

        if (verticalInput != 0) {
            currentSpeed = Mathf.MoveTowards(currentSpeed, verticalInput * moveSpeed, acceleration * Time.deltaTime);
        }
        else {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        if (horizontalInput != 0) {
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, horizontalInput * moveSpeed,
                acceleration * Time.deltaTime);
        }
        else {
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, 0, deceleration * Time.deltaTime);
        }

        Vector3 moveDirection = transform.forward * currentSpeed;
        tankPart.transform.Translate(moveDirection * Time.deltaTime, Space.World);

        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;
        tankPart.transform.Rotate(0, rotationAmount, 0, Space.World);
    }

} //END