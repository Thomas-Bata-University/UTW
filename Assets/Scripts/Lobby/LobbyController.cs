using FishNet;
using UnityEngine;

public class LobbyController : MonoBehaviour {

    public void DisconnectFromLobby() {
        SceneManager.Instance.DisconnectLobby(InstanceFinder.ClientManager.Connection);
    }

}