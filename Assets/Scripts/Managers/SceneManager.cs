using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : NetworkBehaviour {

    public static SceneManager Instance;

    private List<SceneData> sceneDataList = new List<SceneData>();

    private void Start() {
        if (Instance == null)
            Instance = this;
        InstanceFinder.NetworkManager.ServerManager.OnRemoteConnectionState += MoveClientToShardScene;
    }

    #region Lobby scene
    [ServerRpc(RequireOwnership = false)]
    public void CreateLobby(NetworkConnection conn) {
        Debug.Log($"Creating new lobby by client ID: {conn.ClientId}...");
        LoadScene(conn, new SceneLookupData(GameSceneUtils.LOBBY_SCENE), true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectToLobby(NetworkConnection conn, int handle) {
        Debug.Log($"Connecting client ID: {conn.ClientId} to lobby...");
        LoadScene(conn, new SceneLookupData(handle), false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisconnectLobby(NetworkConnection conn) {
        Debug.Log($"Disconnecting client ID: {conn.ClientId} from lobby...");
        LoadScene(conn, new SceneLookupData(GameSceneUtils.SHARD_SCENE), false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshLobby(NetworkConnection conn) {
        sceneDataList.Clear();
        foreach (var pair in InstanceFinder.SceneManager.SceneConnections) {
            if (pair.Key.name == GameSceneUtils.LOBBY_SCENE) {
                SceneData data = new SceneData(pair.Key.handle, pair.Key.name, null, pair.Value.Count); //TODO-YIRO add LOBBY NAME
                sceneDataList.Add(data);
            }
        }
        RefreshLobbyResponse(conn, sceneDataList);
    }

    [TargetRpc]
    public void RefreshLobbyResponse(NetworkConnection conn, List<SceneData> sceneDataList) {
        FindObjectOfType<ShardController>().CreateLobbyButtons(sceneDataList);
    }
    #endregion

    #region Shard scene
    //Called on SERVER
    private void MoveClientToShardScene(NetworkConnection conn, RemoteConnectionStateArgs args) {
        if (args.ConnectionState != RemoteConnectionState.Started) return;

        SceneLoadData data = new SceneLoadData(GameSceneUtils.SHARD_SCENE);
        data.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadConnectionScenes(conn, data);
    }
    #endregion

    /// <summary>
    /// Load scene.
    /// </summary>
    /// <param name="conn">Load scene for one Network connection</param>
    /// <param name="lookupData">Data to find a scene</param>
    /// <param name="allowStacking">True - create new scene instance | False - find existing scene</param>
    private void LoadScene(NetworkConnection conn, SceneLookupData lookupData, bool allowStacking) {
        SceneLoadData sceneLoadData = new SceneLoadData(lookupData);
        sceneLoadData.Options.AllowStacking = allowStacking;
        sceneLoadData.ReplaceScenes = ReplaceOption.OnlineOnly;
        InstanceFinder.SceneManager.LoadConnectionScenes(conn, sceneLoadData);
    }

    private void LoadScene(NetworkConnection[] conns, SceneLookupData lookupData, bool allowStacking) {
        SceneLoadData sceneLoadData = new SceneLoadData(lookupData);
        sceneLoadData.Options.AllowStacking = allowStacking;
        sceneLoadData.ReplaceScenes = ReplaceOption.OnlineOnly;
        InstanceFinder.SceneManager.LoadConnectionScenes(conns, sceneLoadData);
    }

    private void OnDestroy() {
        InstanceFinder.NetworkManager.ServerManager.OnRemoteConnectionState -= MoveClientToShardScene;
    }

}
