using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public delegate void GameStart();
    public event GameStart OnGameStart;
    public GameObject UI;
    public GameObject LobbyManager;
    public void DisconnectFromLobby()
    {
        SceneManager.Instance.DisconnectLobby(InstanceFinder.ClientManager.Connection);
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