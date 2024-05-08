using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public abstract class PlayerController : NetworkBehaviour
{
    private bool isRotating = false;

    protected TankPositions tankPosition;

    public GameObject tankPart;
    public GameObject object2Rotate;
    public Vector3 seatPosition;

    protected void Awake()
    {
        enabled = false;
    }

    protected abstract void Start();

    public void SetPosition()
    {
        tankPart.transform.position = seatPosition;
    }

    protected virtual void MouseLook(float mouseSpeed, float viewAngleX, float viewAngleY)
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (isRotating)
        {
            float rotationX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

            rotationX = Mathf.Clamp(rotationX, -viewAngleX, viewAngleX);
            rotationY = Mathf.Clamp(rotationY, -viewAngleY, viewAngleY);

            object2Rotate.transform.Rotate(Vector3.up, rotationX, Space.World);
            object2Rotate.transform.Rotate(Vector3.left, rotationY, Space.Self);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    protected void Exit(NetworkConnection conn, UTW.SceneManager sceneManager)
    {
        FindObjectOfType<LobbyManager>().LeaveSpawnpoint(conn);
        sceneManager.Disconnect(conn);
    }

    protected abstract void Move();
}