using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using Managers;
using UnityEngine;

public class LobbyController : MonoBehaviour {

    public GameObject lobbyManagerPrefab;

    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    private void Awake() {
        if (InstanceFinder.IsServer) return;

        InstanceFinder.SceneManager.OnLoadEnd += SceneLoadEnd;
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;
    }

    private void SceneLoadEnd(SceneLoadEndEventArgs args) {

        sceneManager.InitializeLobbyManager(lobbyManagerPrefab, conn);
        sceneManager.Connected(conn);
    }

    public void DisconnectFromLobby() {
        sceneManager.RemoveLobbyData(conn);
        sceneManager.DisconnectLobby(conn);
    }

    public void StartGame() { //TODO-YIRO show button only to lobby owner
        sceneManager.StartGame(conn);
    }

    private void OnDestroy() {
        if (InstanceFinder.IsServer) return;
        InstanceFinder.SceneManager.OnLoadEnd -= SceneLoadEnd;
    }

}