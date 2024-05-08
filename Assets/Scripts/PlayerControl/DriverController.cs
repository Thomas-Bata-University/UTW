using FishNet;
using FishNet.Connection;
using UnityEngine;

public class DriverController : PlayerController
{
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    [Range(0, 20)]
    private float moveSpeed = 5f;
    [SerializeField]
    [Range(0, 30)]
    private float rotationSpeed = 15f;

    private bool autoDrive = false;

    protected override void Start()
    {
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;

        tankPosition = TankPositions.DRIVER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
    }

    private void Update()
    {
        MouseLook(100f, 100f, 80f);
        Move();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit(conn, sceneManager);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            autoDrive = !autoDrive;
        }

        if (autoDrive)
        {
            tankPart.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    protected override void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput;
        tankPart.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;
        tankPart.transform.Rotate(0, rotationAmount, 0, Space.World);
    }
}