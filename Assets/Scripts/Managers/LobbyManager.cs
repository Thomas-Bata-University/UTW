using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour {

    [Header("UI")]
    public GameObject defaultMap;
    public Button spawnpointButtonPrefab;
    public Color lockedSpawnpoint;
    public Color unlockedSpawnpoint;

    [SerializeField]
    private GameObject vehicleManagerPrefab;

    private GameObject activeMap;

    [SyncObject]
    private readonly SyncDictionary<string, MapSpawnpointData> positions = new();

    private void Awake() {
        if (InstanceFinder.IsServer) {
            UTW.SceneManager.OnClientJoinLobby += ClientJoin;
            UTW.SceneManager.OnClientDisconnectLobby += ClientDisconnect;
        } else {
            positions.OnChange += OnChange;
        }
    }

    private void Start() {
        if (InstanceFinder.IsServer) {
            InitializeMap(defaultMap);
        }
    }

    #region Map
    private void OnChange(SyncDictionaryOperation op, string key, MapSpawnpointData value, bool asServer) {
        switch (op) {
            case SyncDictionaryOperation.Add: {
                    Button button = value.button.GetComponent<Button>();
                    button.onClick.AddListener(() => ChangePosition(LocalConnection, key));
                    button.image.color = value.locked ? lockedSpawnpoint : unlockedSpawnpoint;
                }
                break;
            case SyncDictionaryOperation.Set: {
                    Button button = value.button.GetComponent<Button>();
                    button.image.color = value.locked ? lockedSpawnpoint : unlockedSpawnpoint;
                }
                break;
            case SyncDictionaryOperation.Remove: positions.Remove(key); break;
            case SyncDictionaryOperation.Clear: positions.Clear(); break;
        }
    }

    /// <summary>
    /// Create map using prefab. There must be only ONE spawned map with this tag for EACH lobby!
    /// </summary>
    /// <param name="mapToSpawn">Prefab</param>
    private void InitializeMap(GameObject mapToSpawn) {
        GameObject parent = GameObject.FindGameObjectsWithTag(GameTagsUtils.MAP).First(x => x.scene.handle == gameObject.scene.handle);

        GameObject map = Instantiate(mapToSpawn, parent.transform);
        activeMap = map;
        map.GetComponent<NetworkObject>().SetParent(parent.GetComponent<NetworkObject>());
        Spawn(map);

        InitializeButtons();
    }

    private void InitializeButtons() {
        GameObject[] spawnpointButtons = GameObject
            .FindGameObjectsWithTag(GameTagsUtils.SPAWNPOINT_BUTTON)
            .Where(x => x.scene.handle == gameObject.scene.handle).ToArray();
        Array.Sort(spawnpointButtons.Where(x => x.scene.handle == gameObject.scene.handle).ToArray(), (a, b) => a.name.CompareTo(b.name));

        for (int i = 0; i < spawnpointButtons.Length; i++) {
            string key = spawnpointButtons[i].name;
            GameObject button = spawnpointButtons[i];
            positions.Add(key, new MapSpawnpointData(button, button.transform));
        }
    }

    //Call this when owner change map
    private void OnMapChange() {
        Despawn(activeMap);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePosition(NetworkConnection conn, string key) {
        if (positions[key].conn == conn) {
            ChangePositionResponse(conn, "You have already selected this spawnpoint.");
            return;
        }
        if (positions[key].locked) {
            ChangePositionResponse(conn, "Spawnpoint is selected by another client.");
            return;
        }
        Debug.Log($"Client ID: {conn.ClientId} changed position to {positions[key].position.position}.");
        Lock(conn, key);
    }

    [TargetRpc]
    private void ChangePositionResponse(NetworkConnection conn, string reason) {
        Debug.Log(reason);
    }

    /// <summary>
    /// Lock new and unlock old spawnpoint position.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="key"></param>
    private void Lock(NetworkConnection conn, string key) {
        Unlock(conn);
        positions[key].locked = true;
        positions[key].conn = conn;
        positions.Dirty(key);
        ChangePositionResponse(conn, $"Spawnpoint changed to {positions[key].position.position}");
    }

    /// <summary>
    /// Unlock spawnpoint.
    /// </summary>
    /// <param name="conn"></param>
    private void Unlock(NetworkConnection conn) {
        if (positions.Any(x => x.Value.conn == conn)) {
            string oldKey = positions.First(x => x.Value.conn == conn).Key;
            MapSpawnpointData data = positions[oldKey];
            data.locked = false;
            data.conn = null;
            positions.Dirty(oldKey);
        }
    }
    #endregion

    private void ClientJoin(NetworkConnection conn) {
        Scene scene = GetComponent<NetworkObject>().gameObject.scene;
        if (conn.Scenes.First().handle == scene.handle) {
            InitializeVehicleManager(conn, scene);
        }
    }

    private void ClientDisconnect(NetworkConnection conn) {
        //TODO if client is lobby owner, disconnect all clients
        Unlock(conn);
    }

    private void InitializeVehicleManager(NetworkConnection conn, Scene scene) {
        GameObject go = Instantiate(vehicleManagerPrefab);
        go.name = vehicleManagerPrefab.name;
        Spawn(go, conn, scene);
        Debug.Log($"{go.name} successfully initialized.");
    }

    public bool CanSpawnTank(NetworkConnection conn) {
        return positions.Any(x => x.Value.conn == conn);
    }

    private void OnDestroy() {
        if (InstanceFinder.IsServer) {
            UTW.SceneManager.OnClientJoinLobby -= ClientJoin;
        }
    }

}
