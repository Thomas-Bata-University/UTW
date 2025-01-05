using UnityEngine;
using UnityEngine.EventSystems;

namespace ChobiAssets.PTM
{
    public class Menu_Camera_CS : MonoBehaviour
    {
        /*
		 * This script controls the camera in the menu scenes.
		*/


        Camera thisCamera;
        Transform thisTransform;
        float currentOrthographicSize;
        float targetOrthographicSize;
        Vector3 currentPos;
        Vector3 targetPos;


        void Start()
        {
            thisCamera = GetComponent<Camera>();
            thisTransform = transform;

            currentOrthographicSize = thisCamera.orthographicSize;
            targetOrthographicSize = currentOrthographicSize;
            currentPos = thisTransform.position;
            targetPos = currentPos;
        }


        void Update()
        {
            Rotaion();
            Zoom();
            Move();

            // Quit
            if (General_Settings_CS.Allow_Instant_Quit && Input.GetKeyDown(General_Settings_CS.Quit_Key))
            {
                Application.Quit();
            }
        }


        void Rotaion()
        {
            if (Input.GetMouseButton(1))
            {
                var horizontal = Input.GetAxis("Mouse X") * 4.0f;
                var vertical = -Input.GetAxis("Mouse Y") * 4.0f;

                var currentAngles = thisTransform.eulerAngles;
                currentAngles.y += horizontal;
                currentAngles.x += vertical;
                currentAngles.x = Mathf.Clamp(currentAngles.x, 1.0f, 90.0f);
                thisTransform.eulerAngles = currentAngles;
            }
        }


        void Zoom()
        {
            if (Input.GetMouseButton(1))
            {
                var wheelInput = -Input.GetAxis("Mouse ScrollWheel");
                targetOrthographicSize *= 1.0f + (wheelInput * 2.0f);
                targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, 10.0f, 1280.0f);
            }

            currentOrthographicSize = Mathf.MoveTowards(currentOrthographicSize, targetOrthographicSize, 640.0f * Time.deltaTime);
            thisCamera.orthographicSize = currentOrthographicSize;
        }


        void Move()
        {
            if (Input.GetMouseButtonDown(2) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                var ray = thisCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, Layer_Settings_CS.Layer_Mask))
                {
                    targetPos = raycastHit.point;
                }
            }

            currentPos = Vector3.MoveTowards(currentPos, targetPos, 5.0f * thisCamera.orthographicSize * Time.deltaTime);
            thisTransform.position = currentPos;
        }

    }
}
