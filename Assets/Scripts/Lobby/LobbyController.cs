using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;

public class LobbyController : MonoBehaviour {

    public GameObject lobbyManagerPrefab;

    [SerializeField] private GameObject startButton;
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    private void Awake() {
        if (InstanceFinder.IsServer) return;

        InstanceFinder.SceneManager.OnLoadEnd += SceneLoadEnd;
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;
        startButton.SetActive(false);
    }

    private void SceneLoadEnd(SceneLoadEndEventArgs args) {
        sceneManager.InitializeLobbyManager(lobbyManagerPrefab, conn);
        sceneManager.Connected(conn);
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