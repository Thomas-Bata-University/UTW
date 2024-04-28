using FishNet;
using System.Text;
using TMPro;
using UnityEngine;

public class MapController : MonoBehaviour
{

    [SerializeField] private LayerMask mapLayerMask, spawnpointLayerMask;
    [SerializeField] private float zoomSpeed, minSize;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Camera mapCamera;
    private GameObject popupPanel;
    private float maxSize;

    private LobbyManager lobbyManager;

    private Vector3 lastPosition, defaultPosition;
    private bool isMouseOverObject = false;

    private void Start()
    {
        if (InstanceFinder.IsClient)
        {
            lobbyManager = FindObjectOfType<LobbyManager>();
        }
        popupPanel = GameObject.FindGameObjectWithTag(GameTagsUtils.CREW_INFO_POPUP);
        defaultPosition = mapCamera.transform.position;
        maxSize = mapCamera.orthographicSize;
    }

    private void Update()
    {
        Move();
        Zoom();
        ResetMap();
    }

    private void Move()
    { //TODO - fix map moving
        GameObject spawnpoint = IsMouseOverObject(spawnpointLayerMask);
        if (Input.GetMouseButtonDown(0) && spawnpoint != null)
        {
            lobbyManager.SelectSpawnpoint(spawnpoint.name);
        }
        else if (Input.GetMouseButtonDown(0) && IsMouseOverObject(mapLayerMask) != null)
        {
            isMouseOverObject = true;
            lastPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseOverObject = false;
        }

        if (Input.GetMouseButton(0) && isMouseOverObject)
        {
            Vector2 canvasSize = mapCanvas.GetComponent<RectTransform>().sizeDelta;
            float x = Mathf.Clamp(mapCamera.transform.position.x, -canvasSize.x / 2, canvasSize.x / 2);
            float z = Mathf.Clamp(mapCamera.transform.position.z, -canvasSize.y / 2, canvasSize.y / 2);
            Vector3 diff = lastPosition - Input.mousePosition;
            mapCamera.transform.position = new Vector3(x + diff.x, mapCamera.transform.position.y, z + diff.y);
            lastPosition = Input.mousePosition;
        }
    }

    private void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && IsMouseOverObject(mapLayerMask) != null)
        {
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize - scroll * zoomSpeed, minSize, maxSize);
        }
    }

    private GameObject IsMouseOverObject(LayerMask layer)
    {
        RaycastHit hit;
        var ray = mapCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            var spawnpoint = hit.transform.gameObject;
            if (layer == spawnpointLayerMask)
                ShowPopup(spawnpoint.name, Input.mousePosition);
            return spawnpoint;
        }
        popupPanel.SetActive(false);
        return null;
    }

    private void ShowPopup(string key, Vector3 position)
    {
        VehicleManager vehicleManager = FindObjectOfType<LobbyManager>().GetCrewData(key);
        if (vehicleManager is null) return;
        SetPopupData(vehicleManager);
        popupPanel.SetActive(true);
        popupPanel.transform.parent.position = position;
    }

    private void SetPopupData(VehicleManager vehicleManager)
    {
        popupPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Tank: {vehicleManager.tankName}";
        StringBuilder info = new StringBuilder();
        foreach (CrewData data in vehicleManager.tankCrew.Values)
        {
            info.AppendLine($"{data.tankPosition}: {vehicleManager.GetName(data)}");
        }
        popupPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = info.ToString();
    }

    private void ResetMap()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mapCamera.transform.position = defaultPosition;
            mapCamera.orthographicSize = maxSize;
        }
    }

}
