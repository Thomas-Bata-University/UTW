using FishNet;
using UnityEngine;

public class LobbyController : MonoBehaviour {

    public void DisconnectFromLobby() {
        SceneManager.Instance.DisconnectLobby(InstanceFinder.ClientManager.Connection);
    }

    public void StartGame() { //TODO-YIRO show button only to lobby owner
        SceneManager.Instance.StartGame(InstanceFinder.ClientManager.Connection);
    }

}