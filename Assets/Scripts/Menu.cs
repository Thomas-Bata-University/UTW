using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class Menu : MonoBehaviour {

    [SerializeField] private GameObject networkManager;
    [SerializeField] private GameObject presetManager;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject factionManager;
    private Tugboat _tugboat;

    private void Start() {
        if (networkManager.TryGetComponent(out Tugboat t)) {
            _tugboat = t;
        } else {
            Debug.LogError("Couldn't find Tugboat component!");
        }
        InstanceFinder.NetworkManager.ServerManager.OnServerConnectionState += SpawnPresetManagerOnServer;
    }

    public void Host() {
        _tugboat.StartConnection(true);
    }

    /// <summary>
    /// If STATE is STARTED (server started) then spawn PRESET MANAGER only on SERVER.
    /// </summary>
    /// <param name="args"></param>
    private void SpawnPresetManagerOnServer(ServerConnectionStateArgs args) {
        if (args.ConnectionState == LocalConnectionState.Started) {
            var presetManagerInstance = Instantiate(presetManager);
            var gameManagerInstance = Instantiate(gameManager);
            var factionManagerInstance = Instantiate(factionManager);
            presetManagerInstance.GetComponent<NetworkObject>().SetIsGlobal(true);
            presetManagerInstance.GetComponent<NetworkObject>().SetIsNetworked(true);
            gameManagerInstance.GetComponent<NetworkObject>().SetIsGlobal(true);
            gameManagerInstance.GetComponent<NetworkObject>().SetIsNetworked(true);
            factionManagerInstance.GetComponent<NetworkObject>().SetIsGlobal(true);
            factionManagerInstance.GetComponent<NetworkObject>().SetIsNetworked(true);
            InstanceFinder.ServerManager.Spawn(presetManagerInstance);
            InstanceFinder.ServerManager.Spawn(gameManagerInstance);
            InstanceFinder.ServerManager.Spawn(factionManagerInstance);
        }
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Join() {
        _tugboat.StartConnection(false);
    }

}
