using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyManager : NetworkBehaviour {

    [Header("UI")]
    public GameObject defaultMap;
    public Material lockedSpawnpoint;
    public Material unlockedSpawnpoint;
    public Material readySpawnpoint;
    public Material fullSpawnpoint;

    [SerializeField]
    private GameObject vehicleManagerPrefab;

    private GameObject activeMap;

    private string activeSpawnpointKey; //Local client spawnpoint key

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
    #region Server
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
            string newKey = spawnpoints[i].name;
            GameObject spawnpoint = spawnpoints[i];
            this.spawnpoints.Add(newKey, new MapSpawnpointData(spawnpoint, spawnpoint.transform));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectSpawnpoint(NetworkConnection conn, string newKey, string activeKey) {
        switch (spawnpoints[newKey].spawnpointState) {
            case SpawnpointState.EMPTY: {
                    ChangeSpawnpoint(conn, newKey, activeKey);
                }
                break;
            case SpawnpointState.LOCKED: {
                    ClientResponse(conn, "Cannot join... CREW is not ready.", null);
                }
                break;
            case SpawnpointState.UNLOCKED: {
                    JoinSpawnpoint(conn, newKey, activeKey);
                }
                break;
            case SpawnpointState.FULL: {
                    ClientResponse(conn, "Cannot join... CREW is full.", null);
                }
                break;
        }
    }

    private void ChangeSpawnpoint(NetworkConnection conn, string newKey, string activeKey) {
        if (activeKey is not null)
            return;

        if (activeKey is null) {
            InitializeVehicleManager(conn, newKey);
            return;
        }
        if (!activeKey.Equals(newKey)) {
            Lock(conn, newKey, activeKey);
            return;
        }
    }

    private void JoinSpawnpoint(NetworkConnection conn, string newKey, string activeKey) {
        if (activeKey is not null && spawnpoints[activeKey].vehicleManager.IsInCrew(conn)) {
            ClientResponse(conn, "Cannot join... Already in CREW.", activeKey);
            return;
        }
        if (activeKey is not null)
            LeaveSpawnpoint(conn, activeKey);
        if (spawnpoints[newKey].vehicleManager.JoinCrew(conn))
            SetSpawnpointFull(newKey);

        Debug.Log($"Client ID: {conn.ClientId} joined CREW at position {spawnpoints[newKey].position.position}.");
        ClientResponse(conn, "CREW joined.", newKey);
    }

    //Call this when owner change map
    private void OnMapChange() {
        Despawn(activeMap);
    }

    private void SetSpawnpointFull(string newKey) {
        Debug.Log("CREW is FULL");
        spawnpoints[newKey].spawnpointState = SpawnpointState.FULL;
        spawnpoints.Dirty(newKey);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSpawnpointReady(string newKey) {
        if (newKey is null) return;
        spawnpoints[newKey].spawnpointState = SpawnpointState.UNLOCKED;
        spawnpoints.Dirty(newKey);
    }

    private void LeaveSpawnpoint(NetworkConnection conn, string activeKey) {
        if (activeKey is null) return;
        MapSpawnpointData data = spawnpoints[activeKey];
        if (data.vehicleManager.LeaveCrew(conn)) {
            data.vehicleManager.Despawn();
            Unlock(conn, activeKey);
        } else {
            data.spawnpointState = SpawnpointState.UNLOCKED;
            spawnpoints.Dirty(activeKey);
        }
        Debug.Log($"Client ID: {conn.ClientId} left CREW at position {data.position.position}.");
        ClientResponse(conn, $"CREW left", null);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveSpawnpointRpc(NetworkConnection conn, string activeKey) {
        LeaveSpawnpoint(conn, activeKey);
    }

    /// <summary>
    /// Lock new and unlock old spawnpoint position.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="key"></param>
    private void Lock(NetworkConnection conn, string newKey, string activeKey) {
        MapSpawnpointData data = spawnpoints[newKey];
        data.locked = true;
        data.spawnpointState = SpawnpointState.LOCKED;
        data.vehicleManager = spawnpoints[activeKey].vehicleManager;
        spawnpoints.Dirty(newKey);
        Unlock(conn, activeKey);

        Debug.Log($"Client ID: {conn.ClientId} LOCKED position {data.position.position}.");
        ClientResponse(conn, $"Spawnpoint LOCKED at position {data.position.position}", newKey);
    }

    /// <summary>
    /// Unlock spawnpoint.
    /// </summary>
    /// <param name="conn"></param>
    private void Unlock(NetworkConnection conn, string activeKey) {
        MapSpawnpointData data = spawnpoints[activeKey];
        if (activeKey == null || !data.vehicleManager.CrewIsEmpty()) return;
        data.vehicleManager = null;
        data.locked = false;
        data.spawnpointState = SpawnpointState.EMPTY;
        spawnpoints.Dirty(activeKey);

        Debug.Log($"Client ID: {conn.ClientId} UNLOCKED position {data.position.position}.");
        ClientResponse(conn, $"Spawnpoint UNLOCKED at position {data.position.position}", null);
    }

    private void InitializeVehicleManager(NetworkConnection conn, string newKey) {
        GameObject go = Instantiate(vehicleManagerPrefab);
        Spawn(go, conn, gameObject.scene);
        Debug.Log($"{go.name} successfully initialized.");

        VehicleManager vehicleManager = go.GetComponent<VehicleManager>();
        vehicleManager.SetCrewData(null);
        vehicleManager.JoinCrew(conn);

        MapSpawnpointData data = spawnpoints[newKey];
        data.vehicleManager = vehicleManager;
        data.spawnpointState = SpawnpointState.LOCKED;
        data.locked = true;
        spawnpoints.Dirty(newKey);

        Debug.Log($"Client ID: {conn.ClientId} created CREW at position {data.position.position}.");
        ClientResponse(conn, "CREW created.", newKey);
    }

    private void ClientJoin(NetworkConnection conn) {

    }

    private void ClientDisconnect(NetworkConnection conn) {

    }
    #endregion Server

    #region Client
    private void OnChange(SyncDictionaryOperation op, string key, MapSpawnpointData value, bool asServer) {
        switch (op) {
            case SyncDictionaryOperation.Add: {
                    SetSpawnpointMaterial(value);
                }
                break;
            case SyncDictionaryOperation.Set: {
                    SetSpawnpointMaterial(value);
                }
                break;
            case SyncDictionaryOperation.Remove: spawnpoints.Remove(key); break;
            case SyncDictionaryOperation.Clear: spawnpoints.Clear(); break;
        }
    }

    private void SetSpawnpointMaterial(MapSpawnpointData value) {
        if (value.spawnpoint is null) return;
        switch (value.spawnpointState) {
            case SpawnpointState.EMPTY: {
                    value.spawnpoint.GetComponent<MeshRenderer>().material = unlockedSpawnpoint;
                }
                break;
            case SpawnpointState.LOCKED: {
                    value.spawnpoint.GetComponent<MeshRenderer>().material = lockedSpawnpoint;
                }
                break;
            case SpawnpointState.UNLOCKED: {
                    value.spawnpoint.GetComponent<MeshRenderer>().material = readySpawnpoint;
                }
                break;
            case SpawnpointState.FULL: {
                    value.spawnpoint.GetComponent<MeshRenderer>().material = fullSpawnpoint;
                }
                break;
        }
    }

    public void SelectSpawnpoint(string name) {
        SelectSpawnpoint(LocalConnection, name, activeSpawnpointKey);
    }

    public void SetSpawnpointReady() {
        SetSpawnpointReady(activeSpawnpointKey);
    }

    [TargetRpc]
    private void ClientResponse(NetworkConnection conn, string reason, string newKey) {
        Debug.Log(reason);
        activeSpawnpointKey = newKey;
    }

    public void LeaveSpawnpoint(NetworkConnection conn) {
        LeaveSpawnpointRpc(conn, activeSpawnpointKey);
    }

    public List<CrewData> GetCrewData(string key) {
        if (spawnpoints[key].vehicleManager is null) return null;
        return spawnpoints[key].vehicleManager.tankCrew.Values.ToList();
    }
    #endregion Client
    #endregion Map

    private void OnDestroy() {
        if (InstanceFinder.IsServer) {
            UTW.SceneManager.OnClientJoinLobby -= ClientJoin;
            UTW.SceneManager.OnClientDisconnectLobby -= ClientDisconnect;
        } else {
            spawnpoints.OnChange -= OnChange;
        }
    }

}
