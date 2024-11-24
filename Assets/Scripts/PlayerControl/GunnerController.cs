using System;
using System.Collections;
using ChobiAssets.PTM;
using FishNet;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.Serialization;

public class GunnerController : PlayerController
{
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;
    [SerializeField] private GameObject gunCamera;
    [SerializeField] private GameObject bulletGenerator;
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
    
    public float Reload_Time = 2.0f;
    public float Recoil_Force = 5000.0f;

    private float xRotation;
    private float yRotation;

    public bool isInScope = false;
    private float Loading_Count;
    private bool Is_Loaded = true;

    protected override void Start()
    {
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;

        tankPosition = TankPositions.GUNNER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
        
    }

    private void Update()
    {
        if(isInScope == false) MouseLook(100f, 100f, 80f);
        Move();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit(conn, sceneManager);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeViewpoint();
        }
        if (Input.GetKeyDown(KeyCode.Space) && Is_Loaded)
        {
            Fire();
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

    protected void ChangeViewpoint()
    {
        isInScope = !isInScope;
        gunCamera.SetActive(isInScope);
        object2Rotate.SetActive(!isInScope);
    }
    
    protected void Fire()
    {
        var bulletGeneratorScript = bulletGenerator.GetComponent<Bullet_Generator_CS>();
        bulletGeneratorScript.FireServerRpc();
        StartCoroutine(Reload());
    }
    
    public IEnumerator Reload()
    { // Called also from "Cannon_Fire_Input_##_###".
        Is_Loaded = false;
        Loading_Count = 0.0f;

        while (Loading_Count < Reload_Time)
        {
            Loading_Count += Time.deltaTime;
            yield return null;
        }

        Is_Loaded = true;
        Loading_Count = Reload_Time;
        
    }
    
}