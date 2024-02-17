using FishNet;
using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour {

    private GameObject renderImage;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private LayerMask layerMask;

    private LobbyManager lobbyManager;

    private void Start() {
        if (InstanceFinder.IsClient) {
            lobbyManager = FindObjectOfType<LobbyManager>();
        }
    }

    private void Update() {
        Vector3 newPosition = TransformPosition();

        if (Input.GetMouseButtonDown(0)) {
            Ray rayCamera = new Ray(newPosition, Vector3.up);
            //Debug.DrawRay(rayCamera.origin, rayCamera.direction * 300, Color.red);
            if (Physics.Raycast(rayCamera, out RaycastHit hit, layerMask)) {
                Debug.Log($"HIT: {hit.transform.gameObject.name}");
                lobbyManager.ChangePosition(hit.transform.gameObject.name);
            }
        }
    }

    private Vector3 TransformPosition() {
        return TransformPosition(Input.mousePosition);
    }

    private Vector3 TransformPosition(Vector3 position) {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapCanvas.GetComponent<RectTransform>(), position, mapCamera, out localPosition);
        return new Vector3(localPosition.x, 0, localPosition.y);
    }

    public void SetPosition() {
        //Correct position of the map
        renderImage = GameObject.FindGameObjectsWithTag(GameTagsUtils.MAP).First();
        transform.position = TransformPosition(renderImage.transform.position);
    }

}
