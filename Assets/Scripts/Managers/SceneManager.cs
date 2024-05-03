using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UTW
{
    public class SceneManager : NetworkBehaviour
    {
        public static SceneManager Instance;

        public static UnityAction<NetworkConnection> OnClientJoinLobby;
        public static UnityAction<NetworkConnection> OnClientDisconnectLobby;

        //KEY - scene handle | value - SceneData
        private Dictionary<int, SceneData> lobbyData = new Dictionary<int, SceneData>();

        //KEY - NetworkConnection | value - Spawned ChatManager
        private Dictionary<NetworkConnection, GameObject> chatManagers = new Dictionary<NetworkConnection, GameObject>();

        private void Start()
        {
            if (Instance == null)
                Instance = this;

            InstanceFinder.NetworkManager.ServerManager.OnAuthenticationResult += MoveClientToShardScene;
        }

        private void MoveClientToShardScene(NetworkConnection conn, bool passed)
        {
            if (!passed) return;

            SceneLoadData data = new SceneLoadData(GameSceneUtils.SHARD_SCENE);
            data.ReplaceScenes = ReplaceOption.All;

            InstanceFinder.SceneManager.LoadConnectionScenes(conn, data);
        }

        #region Lobby scene
        [ServerRpc(RequireOwnership = false)]
        public void CreateLobby(NetworkConnection conn)
        {
            Debug.Log($"Creating new lobby by client ID: {conn.ClientId}...");
            LoadScene(conn, new SceneLookupData(GameSceneUtils.LOBBY_SCENE), true);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ConnectToLobby(NetworkConnection conn, int handle)
        {
            Debug.Log($"Connecting client ID: {conn.ClientId} to lobby...");

            if (lobbyData.TryGetValue(handle, out SceneData sceneData))
            {
                if (sceneData.lobbyState == LobbyState.ONGOING)
                {
                    LogResponse(conn, "This lobby is ongoing!");
                    return;
                }

                LoadScene(conn, new SceneLookupData(handle), false);
            }
            else
            {
                LogResponse(conn, "Cannot connect to lobby.");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void InitializeLobbyManagers(GameObject lobbyManagerPrefab, GameObject chatPrefab, NetworkConnection conn)
        {
            Scene scene = GetSceneForClient(conn, GameSceneUtils.LOBBY_SCENE);

            if (lobbyData.ContainsKey(scene.handle)) return;

            CreateNewLobbyData(conn, scene);

            GameObject go = Instantiate(lobbyManagerPrefab);
            go.name = lobbyManagerPrefab.name;
            lobbyData[scene.handle].lobbyManager = go.GetComponent<LobbyManager>();
            InstanceFinder.ServerManager.Spawn(go, conn, scene);
            Debug.Log($"{go.name} successfully initialized.");

            go = Instantiate(chatPrefab);
            go.name = chatPrefab.name;
            InstanceFinder.ServerManager.Spawn(go, conn, scene);
            Debug.Log($"{go.name} successfully initialized.");

            ActivateStartButtonForLobbyOwner(conn);
        }

        [ServerRpc(RequireOwnership = false)]
        public void InitializeChatManager(GameObject chatManagerPrefab, NetworkConnection conn)
        {
            Scene scene = GetSceneForClient(conn, GameSceneUtils.LOBBY_SCENE);
            GameObject go = Instantiate(chatManagerPrefab);
            go.name = chatManagerPrefab.name;

            InstanceFinder.ServerManager.Spawn(go, conn, scene);
            chatManagers[conn] = go;

            Debug.Log($"{go.name} successfully initialized.");
        }

        [Server]
        public void DespawnChatManager(NetworkConnection conn)
        {
            if (chatManagers.TryGetValue(conn, out GameObject chatManager))
            {
                InstanceFinder.ServerManager.Despawn(chatManager);
                chatManagers.Remove(conn);

                Debug.Log($"{chatManager.name} has been despawned.");
            }
            else
            {
                Debug.LogWarning("No ChatManager found for the given connection.");
            }
        }

        [Server]
        public void DespawnChatManager(List<NetworkConnection> conns)
        {
            foreach (var c in conns)
            {
                DespawnChatManager(c);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void Connected(NetworkConnection conn)
        {
            AddClientData(conn);
            OnClientJoinLobby?.Invoke(conn);
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartGame(NetworkConnection conn)
        {
            SceneData data = GetData(conn);
            DespawnChatManager(data.clients);
            data.lobbyManager.StartGame();
            HideLobbyData(data);
        }

        [TargetRpc]
        private void ActivateStartButtonForLobbyOwner(NetworkConnection conn)
        {
            FindObjectOfType<LobbyController>().ActivateStartButton();
        }
        #endregion

        #region Lobby data
        public void CreateNewLobbyData(NetworkConnection owner, Scene scene)
        {
            SceneData data = new SceneData(scene.handle, scene.name, null, owner);
            lobbyData.Add(scene.handle, data);
        }

        /// <summary>
        /// Remove data from the list of lobbies after starting game to not allow another clients connect this lobby.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>If conn is owner</returns>
        private bool RemoveLobbyData(NetworkConnection conn, SceneData data)
        {
            if (data != null && data.lobbyOwner == conn)
            {
                Debug.Log($"Removing data for owner: {conn.ClientId}");
                lobbyData.Remove(data.handle);
                return true;
            }
            return false;
        }

        private void HideLobbyData(SceneData data)
        {
            if (data == null) return;

            lobbyData[data.handle].lobbyState = LobbyState.ONGOING;
        }

        private void AddClientData(NetworkConnection conn)
        {
            var data = GetData(conn);
            if (data == null) return;
            data.playerCount++;
            data.clients.Add(conn);
        }

        private void RemoveClientData(NetworkConnection conn)
        {
            var data = GetData(conn);
            if (data == null) return;
            data.playerCount--;
            data.clients.Remove(conn);
        }

        private SceneData GetData(NetworkConnection conn)
        {
            Scene scene = GetSceneForClient(conn, GameSceneUtils.LOBBY_SCENE);
            foreach (var dataPair in lobbyData)
            {
                if (dataPair.Key == scene.handle)
                {
                    return dataPair.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if conn is owner, disconnect all clients if TRUE, disconnect only conn if FALSE
        /// </summary>
        /// <param name="conn"></param>
        [ServerRpc(RequireOwnership = false)]
        public void Disconnect(NetworkConnection conn)
        {
            var data = GetData(conn);
            if (RemoveLobbyData(conn, data))
            {
                Debug.Log($"Disconnecting all clients from lobby...");
                LoadScene(data.clients.ToArray(), new SceneLookupData(GameSceneUtils.SHARD_SCENE), false);
                return;
            }

            Debug.Log($"Updating data for cliend ID: {conn.ClientId}");
            RemoveClientData(conn);

            Debug.Log($"Despawning ChatManager for cliend ID: {conn.ClientId}");
            DespawnChatManager(conn);

            Debug.Log($"Disconnecting client ID: {conn.ClientId} from lobby...");
            LoadScene(conn, new SceneLookupData(GameSceneUtils.SHARD_SCENE), false);

            OnClientDisconnectLobby?.Invoke(conn);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisconnectFromShard(NetworkConnection conn)
        {
            conn.Disconnect(true);
        }

        [ServerRpc(RequireOwnership = false)]
        public void GetLobbyData(NetworkConnection conn)
        {
            GetLobbyDataResponse(conn, lobbyData);
        }

        [TargetRpc]
        public void GetLobbyDataResponse(NetworkConnection conn, Dictionary<int, SceneData> lobbyData)
        {
            ShardController sc = FindObjectOfType<ShardController>();
            if (sc != null)
            {
                sc.CreateLobbyButtons(lobbyData);
            }
        }
        #endregion

        /// <summary>
        /// Load scene.
        /// </summary>
        /// <param name="conn">Load scene for one Network connection</param>
        /// <param name="lookupData">Data to find a scene</param>
        /// <param name="allowStacking">True - create new scene instance | False - find existing scene</param>
        /// PreferredActiveScene - Set active scene for spawning NO on client.
        private void LoadScene(NetworkConnection conn, SceneLookupData lookupData, bool allowStacking)
        {
            SceneLoadData sceneLoadData = new SceneLoadData(lookupData);
            sceneLoadData.Options.AllowStacking = allowStacking;
            sceneLoadData.ReplaceScenes = ReplaceOption.OnlineOnly;
            sceneLoadData.PreferredActiveScene = lookupData;
            InstanceFinder.SceneManager.LoadConnectionScenes(conn, sceneLoadData);
        }

        private void LoadScene(NetworkConnection[] conns, SceneLookupData lookupData, bool allowStacking)
        {
            SceneLoadData sceneLoadData = new SceneLoadData(lookupData);
            sceneLoadData.Options.AllowStacking = allowStacking;
            sceneLoadData.ReplaceScenes = ReplaceOption.OnlineOnly;
            sceneLoadData.PreferredActiveScene = lookupData;
            InstanceFinder.SceneManager.LoadConnectionScenes(conns, sceneLoadData);
        }

        /// <summary>
        /// To safely get the scene for conn, filter with required sceneName.
        /// This is because of loading and unloading scenes.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public Scene GetSceneForClient(NetworkConnection conn, string sceneName)
        {
            return conn.Scenes.First(x => x.name.Equals(sceneName));
        }

        [TargetRpc]
        public void LogResponse(NetworkConnection conn, string text)
        {
            Debug.Log(text);
        }

        private void OnDestroy()
        {
            InstanceFinder.NetworkManager.ServerManager.OnAuthenticationResult -= MoveClientToShardScene;
        }
    }
}
