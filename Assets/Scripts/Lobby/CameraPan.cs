using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 lastposition;
    private Vector3 difference;
    private Vector3 ResetCamera;

    private bool drag = false;



    private void Start()
    {
        ResetCamera = transform.position;
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (drag == false)
            {
                drag = true;
            }
        }
        else
        {
            drag = false;
            lastposition = Input.mousePosition;
        }

        if (drag)
        {
            difference = Input.mousePosition - lastposition;
            transform.position = new Vector3(transform.position.x + difference.x, transform.position.y, transform.position.z+difference.y);
            lastposition = Input.mousePosition;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + Input.mouseScrollDelta.y*100, transform.position.z);
        if (Input.GetMouseButton(1))
            transform.position = ResetCamera;

    }
}
