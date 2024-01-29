using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
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

    public void StartGame()
    { //TODO-YIRO show button only to lobby owner
        UI.SetActive(false);
        OnGameStart.Invoke();
//        SceneManager.Instance.StartGame(InstanceFinder.ClientManager.Connection);
    }
    public void Start()
    {
        UI = GameObject.Find("UI");
        if (InstanceFinder.IsServer)
        {
            GameObject go = Instantiate(LobbyManager);
            go.name = LobbyManager.name;
            NetworkObject no = go.GetComponent<NetworkObject>();
            InstanceFinder.NetworkManager.ServerManager.Spawn(no);
            Debug.Log($"{LobbyManager.name} successfully initialized.");
        }
    }
}