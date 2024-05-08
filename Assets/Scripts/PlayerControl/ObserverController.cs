using FishNet;
using FishNet.Connection;
using UnityEngine;

public class ObserverController : PlayerController
{
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    protected override void Start()
    {
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;

        tankPosition = TankPositions.OBSERVER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
    }

    private void Update()
    {
        MouseLook(500f, 100f, 100f);
        Move();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit(conn, sceneManager);
        }
    }

    protected override void Move()
    {

    }
}