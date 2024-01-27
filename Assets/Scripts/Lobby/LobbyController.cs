using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;

public class LobbyController : MonoBehaviour {

    public GameObject lobbyManagerPrefab;
    private LobbyManager lobbyManager;

    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    private void Awake() {
        if (InstanceFinder.IsServer) return;

        InstanceFinder.SceneManager.OnLoadEnd += SceneLoadEnd;
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;
    }

    private void SceneLoadEnd(SceneLoadEndEventArgs args) {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait() {
        yield return new WaitForSeconds(3);
        lobbyManager = FindObjectOfType<LobbyManager>();

        if (lobbyManager == null) {
            sceneManager.InitializeLobbyManager(lobbyManagerPrefab, conn);
        }
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