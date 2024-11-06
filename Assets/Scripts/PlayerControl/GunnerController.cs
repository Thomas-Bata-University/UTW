using System;
using FishNet;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.Serialization;

public class GunnerController : PlayerController
{
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;
    [SerializeField] private GameObject CannonBase;

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    [Range(0, 30)]
    public float rotationSpeed = 20f;

    public float rightMaxRotation = 90f;
    public float leftMaxRotation = -90f;

    public float minElevation = -5f;
    public float maxElevation = 10f;

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
        //float horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime * rotationSpeed;
        //float verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime * rotationSpeed;
        
        tankPart.transform.Rotate(new Vector3(0, rotationSpeed * Input.GetAxisRaw("Horizontal"), 0 ) * Time.deltaTime);

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            var elevationDelta = rotationSpeed * Input.GetAxisRaw("Vertical") * -1 * Time.deltaTime;
            var currentElevation = CannonBase.transform.rotation.eulerAngles.x;
            currentElevation = (currentElevation > 180) ? currentElevation -= 360 : currentElevation; 
            elevationDelta = Math.Clamp(elevationDelta, -maxElevation - currentElevation, -minElevation - currentElevation);

            CannonBase.transform.Rotate(new Vector3(elevationDelta, 0,0 ));
        }
    }
}