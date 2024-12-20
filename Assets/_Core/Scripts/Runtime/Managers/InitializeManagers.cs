using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;

public class InitializeManagers : MonoBehaviour {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    private List<GameObject> prefabList;

    private void Start() {
        InstanceFinder.NetworkManager.ServerManager.OnServerConnectionState += InitializeAllManagers;
    }

    private void InitializeAllManagers(ServerConnectionStateArgs args) {
        if (args.ConnectionState != LocalConnectionState.Started) return;

        Debug.Log("Server started... Initializing all managers");

        foreach (GameObject prefab in prefabList) {
            Initialize(prefab);
        }
    }

    public void Initialize(GameObject prefab) {
        GameObject go = Instantiate(prefab);
        go.name = prefab.name;
        NetworkObject no = go.GetComponent<NetworkObject>();
        InstanceFinder.NetworkManager.ServerManager.Spawn(no);
        Debug.Log($"{prefab.name} successfully initialized.");
    }

}
