using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour {

    private GameObject parent;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private GameObject border;

    private Vector3 lastposition, difference, defaultPosition;
    private float defaultScale;

    private bool isMouseOverObject = false;

    private float maxSize = 0f;
    [SerializeField] private float minSize = 200f;
    [SerializeField] private float zoomSpeed = 100f;


    public Vector3 screenPosition;
    public Vector3 toWorld;
    public Plane m_CanvasPlane;



    private void Start() {
        defaultPosition = mapCamera.transform.position;
        defaultScale = mapCamera.orthographicSize;
        parent = GameObject.FindGameObjectsWithTag(GameTagsUtils.MAP).First();

        minSize = mapCamera.orthographicSize - minSize;
        maxSize = mapCamera.orthographicSize;
    }

    private void Update() {
        screenPosition = Input.mousePosition;
        //toWorld = mapCamera.ScreenToWorldPoint(screenPosition);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(mapCanvas.GetComponent<RectTransform>(), Input.mousePosition, mapCamera, out toWorld);
    }

    //private void LateUpdate() {
    //    Move();
    //    Zoom();
    //    ResetMap();
    //}

    private void Move() {
        if (Input.GetMouseButtonDown(0) && IsMouseOverObject()) {
            isMouseOverObject = true;
            lastposition = GetMousePosition();
        } else if (Input.GetMouseButtonUp(0)) {
            isMouseOverObject = false;
        }
        if (Input.GetMouseButton(0) && isMouseOverObject) {
            difference = GetMousePosition() - lastposition;

            CorrectPosition();

            lastposition = GetMousePosition();
        }
    }

    private void Zoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && IsMouseOverObject()) {
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize - scroll * zoomSpeed, minSize, maxSize);

            if (scroll < 0) {
                CorrectPosition();
            }
        }
    }

    private void CorrectPosition() {
        Vector2 bortderSize = border.GetComponent<RectTransform>().sizeDelta * transform.localScale.x / 2;
        Vector3 newPosition = new Vector3(mapCamera.transform.position.x + difference.x, mapCamera.transform.position.y, mapCamera.transform.position.z + difference.y);
        Vector3 objectSize = new Vector3(mapCamera.orthographicSize * 2f * mapCamera.aspect, 0f, mapCamera.orthographicSize * 2f);

        newPosition.x = Mathf.Clamp(newPosition.x, (objectSize.x - bortderSize.x) / 2, (-objectSize.x + bortderSize.x) / 2);
        newPosition.z = Mathf.Clamp(newPosition.z, (objectSize.z - bortderSize.y) / 2, (-objectSize.z + bortderSize.y) / 2);
        mapCamera.transform.position = newPosition;
        Debug.Log("CAMERA" + mapCamera.transform.position);
    }

    private void ResetMap() {
        if (Input.GetMouseButtonDown(1)) {
            mapCamera.transform.position = defaultPosition;
            mapCamera.orthographicSize = defaultScale;
        }
    }

    private bool IsMouseOverObject() {
        return RectTransformUtility.RectangleContainsScreenPoint(parent.GetComponent<RectTransform>(), Input.mousePosition);
    }

    private Vector3 GetMousePosition() {
        return Input.mousePosition;
    }

}
