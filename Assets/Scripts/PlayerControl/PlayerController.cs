using UnityEngine;

public abstract class PlayerController : MonoBehaviour {

    private float actualAngleX = 0f;
    private float actualAngleY = 0f;

    public GameObject player;
    public Vector3 seatPosition;

    protected void Awake() {
        enabled = false;
    }

    public void SetPosition() {
        player.transform.position = seatPosition;
    }

    protected virtual void MouseLook(float mouseSpeed, float viewAngleX, float viewAngleY) {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * x * mouseSpeed);

        actualAngleX -= y * mouseSpeed;
        float angleX = viewAngleX / 2;
        actualAngleX = Mathf.Clamp(actualAngleX, -angleX, angleX);

        actualAngleY -= y * mouseSpeed;
        float angleY = viewAngleY / 2;
        actualAngleY = Mathf.Clamp(actualAngleY, -angleY, angleY);

        transform.localRotation = Quaternion.Euler(actualAngleX, actualAngleY, 0f);
    }

}//END