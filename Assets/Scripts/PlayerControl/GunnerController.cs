using FishNet;
using FishNet.Connection;
using UnityEngine;

public class GunnerController : PlayerController
{
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    [Range(0, 30)]
    public float rotationSpeed = 20f;

    public float rightMaxRotation = 90f;
    public float leftMaxRotation = -90f;

    public float downMaxRotation = 5f;
    public float upMaxRotation = -10f;

    private float xRotation;
    private float yRotation;

    protected override void Start()
    {
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;

        tankPosition = TankPositions.GUNNER;
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
    }

    protected override void Move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime * rotationSpeed;
        float verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime * rotationSpeed;

        yRotation += horizontalInput;
        yRotation = Mathf.Clamp(yRotation, leftMaxRotation, rightMaxRotation);

        xRotation -= verticalInput;
        xRotation = Mathf.Clamp(xRotation, upMaxRotation, downMaxRotation);

        tankPart.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}