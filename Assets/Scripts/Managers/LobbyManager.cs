using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour {

    [Header("UI")]
    public GameObject defaultMap;
    public Material lockedSpawnpoint;
    public Material unlockedSpawnpoint;

    [SerializeField]
    private GameObject vehicleManagerPrefab;

    private GameObject activeMap;

    //KEY - name of the gameobject | VALUE - MapSpawnpointData
    [SyncObject]
    private readonly SyncDictionary<string, MapSpawnpointData> spawnpoints = new();

    private void Awake() {
        if (InstanceFinder.IsServer) {
            UTW.SceneManager.OnClientJoinLobby += ClientJoin;
            UTW.SceneManager.OnClientDisconnectLobby += ClientDisconnect;
        } else {
            spawnpoints.OnChange += OnChange;
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
                    value.spawnpoint.GetComponent<MeshRenderer>().material = value.locked ? lockedSpawnpoint : unlockedSpawnpoint;
                }
                break;
            case SyncDictionaryOperation.Set: {
                    value.spawnpoint.GetComponent<MeshRenderer>().material = value.locked ? lockedSpawnpoint : unlockedSpawnpoint;
                }
                break;
            case SyncDictionaryOperation.Remove: spawnpoints.Remove(key); break;
            case SyncDictionaryOperation.Clear: spawnpoints.Clear(); break;
        }
    }

    /// <summary>
    /// Create map using prefab. There must be only ONE spawned map with this tag for EACH lobby!
    /// </summary>
    /// <param name="mapToSpawn">Prefab</param>
    private void InitializeMap(GameObject mapToSpawn) {
        GameObject map = Instantiate(mapToSpawn);
        activeMap = map;

        Spawn(map, null, gameObject.scene);

        InitializeSpawnpoints();
    }

    private void InitializeSpawnpoints() {
        GameObject[] spawnpoints = GameObject
            .FindGameObjectsWithTag(GameTagsUtils.MAP_SPAWNPOINT)
            .Where(x => x.scene.handle == gameObject.scene.handle).ToArray();
        Array.Sort(spawnpoints.Where(x => x.scene.handle == gameObject.scene.handle).ToArray(), (a, b) => a.name.CompareTo(b.name));

        for (int i = 0; i < spawnpoints.Length; i++) {
            string key = spawnpoints[i].name;
            GameObject spawnpoint = spawnpoints[i];
            this.spawnpoints.Add(key, new MapSpawnpointData(spawnpoint, spawnpoint.transform));
        }
    }

    //Call this when owner change map
    private void OnMapChange() {
        Despawn(activeMap);
    }

    public void ChangePosition(string name) {
        ChangePosition(LocalConnection, name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePosition(NetworkConnection conn, string key) {
        if (spawnpoints[key].conn == conn) {
            ChangePositionResponse(conn, "You have already selected this spawnpoint.");
            return;
        }
        if (spawnpoints[key].locked) {
            ChangePositionResponse(conn, "Spawnpoint is selected by another client.");
            return;
        }
        Debug.Log($"Client ID: {conn.ClientId} changed position to {spawnpoints[key].position.position}.");
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
        spawnpoints[key].locked = true;
        spawnpoints[key].conn = conn;
        spawnpoints.Dirty(key);
        ChangePositionResponse(conn, $"Spawnpoint changed to {spawnpoints[key].position.position}");
    }

    /// <summary>
    /// Unlock spawnpoint.
    /// </summary>
    /// <param name="conn"></param>
    private void Unlock(NetworkConnection conn) {
        if (spawnpoints.Any(x => x.Value.conn == conn)) {
            string oldKey = spawnpoints.First(x => x.Value.conn == conn).Key;
            MapSpawnpointData data = spawnpoints[oldKey];
            data.locked = false;
            data.conn = null;
            spawnpoints.Dirty(oldKey);
        }
    }
    #endregion

    #region Client
    private void ClientJoin(NetworkConnection conn) {
        Scene scene = GetComponent<NetworkObject>().gameObject.scene;
        if (conn.Scenes.First().handle == scene.handle) {
            InitializeVehicleManager(conn, scene);
        }
    }

    private void ClientDisconnect(NetworkConnection conn) {
        Unlock(conn);
    }

    private void InitializeVehicleManager(NetworkConnection conn, Scene scene) {
        GameObject go = Instantiate(vehicleManagerPrefab);
        go.name = vehicleManagerPrefab.name;
        Spawn(go, conn, scene);
        Debug.Log($"{go.name} successfully initialized.");
    }

    public bool CanSpawnTank(NetworkConnection conn) {
        return spawnpoints.Any(x => x.Value.conn == conn);
    }
    #endregion

    private void OnDestroy() {
        if (InstanceFinder.IsServer) {
            UTW.SceneManager.OnClientJoinLobby -= ClientJoin;
            UTW.SceneManager.OnClientDisconnectLobby -= ClientDisconnect;
        } else {
            spawnpoints.OnChange -= OnChange;
        }
    }

}
