using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class LobbyController : MonoBehaviour {

    public GameObject lobbyManagerPrefab;
    public GameObject swapRequestPanel;

    [SerializeField] private GameObject startButton;
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    private void Awake() {
        if (InstanceFinder.IsServer) return;

        InstanceFinder.SceneManager.OnLoadEnd += SceneLoadEnd;
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;
        startButton.SetActive(false);
        swapRequestPanel.SetActive(false);
    }

    private void SceneLoadEnd(SceneLoadEndEventArgs args) {
        sceneManager.InitializeLobbyManager(lobbyManagerPrefab, conn);
        sceneManager.Connected(conn);
    }

    public void SetSwapData(VehicleManager vehicleManager, NetworkConnection requestConn, int key, int oldKey) {
        swapRequestPanel.SetActive(true);
        swapRequestPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Request from client ID: {requestConn.ClientId}";
        swapRequestPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Swap from {vehicleManager.tankCrew[key].tankPosition} to {vehicleManager.tankCrew[oldKey].tankPosition}?";
        OnClick(2).AddListener(() => Swap(vehicleManager, requestConn, key, oldKey, true));
        OnClick(3).AddListener(() => Swap(vehicleManager, requestConn, key, oldKey, false));
    }

    private ButtonClickedEvent OnClick(int childIndex) {
        return swapRequestPanel.transform.GetChild(childIndex).GetComponent<Button>().onClick;
    }

    private void Swap(VehicleManager vehicleManager, NetworkConnection requestConn, int key, int oldKey, bool swap) {
        vehicleManager.SwapRequestResponse(requestConn, key, oldKey, swap);
        swapRequestPanel.SetActive(false);
    }

    public void SpawnpointReady() {
        FindObjectOfType<LobbyManager>().SetSpawnpointReady();
    }

    public void LeaveSpawnpoint() {
        FindObjectOfType<LobbyManager>().LeaveSpawnpoint(conn);
    }

    public void DisconnectFromLobby() {
        FindObjectOfType<LobbyManager>().LeaveSpawnpoint(conn);
        sceneManager.Disconnect(conn);
    }

    public void ActivateStartButton() {
        startButton.SetActive(true);
    }

    public void StartGame() {
        sceneManager.StartGame(conn);
    }

    private void OnDestroy() {
        if (InstanceFinder.IsServer) return;
        InstanceFinder.SceneManager.OnLoadEnd -= SceneLoadEnd;
    }

}