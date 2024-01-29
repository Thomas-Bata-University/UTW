using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UTW {
    public class SceneManager : NetworkBehaviour {

        public static SceneManager Instance;

        public static UnityAction<NetworkConnection> OnClientJoinLobby;

        private Dictionary<int, SceneData> lobbyData = new Dictionary<int, SceneData>();

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
        public void InitializeLobbyManager(GameObject lobbyManagerPrefab, NetworkConnection conn) {
            Scene scene = GetSceneForClient(conn);

            if (lobbyData.ContainsKey(scene.handle)) return;

            CreateNewLobbyData(conn, scene);

            GameObject go = Instantiate(lobbyManagerPrefab);
            go.name = lobbyManagerPrefab.name;
            InstanceFinder.ServerManager.Spawn(go, null, scene);
            Debug.Log($"{go.name} successfully initialized.");
        }

        [ServerRpc(RequireOwnership = false)]
        public void Connected(NetworkConnection conn) {
            UpdatePlayerCount(conn, 1);
            ClientConnected(conn);
        }

        public void ClientConnected(NetworkConnection conn) {
            OnClientJoinLobby?.Invoke(conn);
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartGame(NetworkConnection conn) {
            LoadScene(InstanceFinder.SceneManager.SceneConnections.First(pair => pair.Value.Contains(conn)).Value.ToArray(),
                new SceneLookupData(GameSceneUtils.GAME_SCENE),
                false);
            RemoveLobbyData(conn);
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

        #region Lobby data
        public void CreateNewLobbyData(NetworkConnection owner, Scene scene) {
            SceneData data = new SceneData(scene.handle, scene.name, null, owner);
            lobbyData.Add(scene.handle, data);
        }

        public void UpdatePlayerCount(NetworkConnection conn, int count) {
            Scene scene = GetSceneForClient(conn);
            foreach (var dataPair in lobbyData) {
                if (dataPair.Key == scene.handle) {
                    dataPair.Value.playerCount += count;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveLobbyData(NetworkConnection conn) {
            foreach (var dataPair in lobbyData) {
                if (dataPair.Value.lobbyOwner == conn) {
                    Debug.Log($"Removing data for owner: {conn.ClientId}");
                    lobbyData.Remove(GetSceneForClient(conn).handle);
                    return;
                }
            }
            UpdatePlayerCount(conn, -1);
            Debug.Log($"Updating data for cliend ID: {conn.ClientId}");
        }

        [ServerRpc(RequireOwnership = false)]
        public void GetLobbyData(NetworkConnection conn) {
            GetLobbyDataResponse(conn, lobbyData);
        }

        [TargetRpc]
        public void GetLobbyDataResponse(NetworkConnection conn, Dictionary<int, SceneData> lobbyData) {
            FindObjectOfType<ShardController>().CreateLobbyButtons(lobbyData);
        }
        #endregion

        /// <summary>
        /// Load scene.
        /// </summary>
        /// <param name="conn">Load scene for one Network connection</param>
        /// <param name="lookupData">Data to find a scene</param>
        /// <param name="allowStacking">True - create new scene instance | False - find existing scene</param>
        /// PreferredActiveScene - Set active scene for spawning NO on client.
        private void LoadScene(NetworkConnection conn, SceneLookupData lookupData, bool allowStacking) {
            SceneLoadData sceneLoadData = new SceneLoadData(lookupData);
            sceneLoadData.Options.AllowStacking = allowStacking;
            sceneLoadData.ReplaceScenes = ReplaceOption.OnlineOnly;
            sceneLoadData.PreferredActiveScene = lookupData;
            InstanceFinder.SceneManager.LoadConnectionScenes(conn, sceneLoadData);
        }

        private void LoadScene(NetworkConnection[] conns, SceneLookupData lookupData, bool allowStacking) {
            SceneLoadData sceneLoadData = new SceneLoadData(lookupData);
            sceneLoadData.Options.AllowStacking = allowStacking;
            sceneLoadData.ReplaceScenes = ReplaceOption.OnlineOnly;
            sceneLoadData.PreferredActiveScene = lookupData;
            InstanceFinder.SceneManager.LoadConnectionScenes(conns, sceneLoadData);
        }

        public Scene GetSceneForClient(NetworkConnection conn) {
            Scene scene = conn.Scenes.First();
            return scene;
        }

        private void OnDestroy() {
            InstanceFinder.NetworkManager.ServerManager.OnRemoteConnectionState -= MoveClientToShardScene;
        }

    }
}
