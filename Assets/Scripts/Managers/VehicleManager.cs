using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleManager : NetworkBehaviour {

    [Header("UI")]
    public GameObject crewInfoPrefab;

    [Header("Tank")]
    public GameObject tankPrefab;
    private GameObject actualTank;
    [SyncVar, HideInInspector] public string tankName;

    private NetworkObject networkObject;
    [SyncVar] private Vector3 spawnpointPosition;

    [Header("Crew")]
    //KEY - index | VALUE - CrewData
    [SyncObject, HideInInspector] public readonly SyncDictionary<int, CrewData> tankCrew = new SyncDictionary<int, CrewData>();
    private List<GameObject> crewButtons = new List<GameObject>();
    private int maxCrewCount;

    private void Awake() {
        PresetDropdown.OnPresetChange += ChangePreset;
        UTW.SceneManager.OnClientDisconnectLobby += Destroy;
    }

    #region Crew
    #region Server-Crew
    public void SetCrewData(Preset preset, NetworkConnection conn, Vector3 spawnpoint) {
        networkObject = GetComponent<NetworkObject>();
        this.spawnpointPosition = spawnpoint;

        //TODO Set this data from preset on VM creation
        tankCrew.Add(0, new CrewData(TankPositions.DRIVER));
        tankCrew.Add(1, new CrewData(TankPositions.GUNNER));
        tankCrew.Add(2, new CrewData(TankPositions.GUNNER));
        maxCrewCount = tankCrew.Count;

        SpawnTank(preset);
    }

    public bool JoinCrew(NetworkConnection joiningClientConn) {
        if (tankCrew.Any(x => x.Value.empty)) {
            RegisterOnChange(joiningClientConn, true);
            CrewData data = tankCrew.First(x => x.Value.conn is null).Value;
            data.conn = joiningClientConn;
            data.empty = false;
            tankCrew.Dirty(data);
            return !tankCrew.Any(x => x.Value.empty);
        }
        return true;
    }

    public bool LeaveCrew(NetworkConnection leavingClientConn) {
        RegisterOnChange(leavingClientConn, false);
        CrewData data = tankCrew.First(x => x.Value.conn == leavingClientConn).Value;
        data.conn = null;
        data.empty = true;
        data.swapRequest = false;
        tankCrew.Dirty(data);
        return CrewIsEmpty();
    }

    public bool CrewIsEmpty() {
        return tankCrew.Count(x => x.Value.empty) == maxCrewCount;
    }

    public bool IsInCrew(NetworkConnection conn) {
        return tankCrew.Any(client => client.Value.conn == conn);
    }

    #region Swap
    private void Swap(NetworkConnection requestConn, int key) {
        int oldKey = GetKey(requestConn);
        if (oldKey == -1) return;
        CrewData oldData = tankCrew[oldKey];
        CrewData requestedData = tankCrew[key];

        var conn = requestedData.conn;
        oldData.conn = conn;
        oldData.empty = conn is null;

        requestedData.conn = requestConn;
        requestedData.empty = false;

        tankCrew.Dirty(key);
        tankCrew.Dirty(oldKey);

        SetSwapping(key, oldKey, false);
        Debug.Log($"Client ID: {requestConn.ClientId} swapped position to {requestedData.tankPosition}");
        LogResponse(requestConn, $"Position changed to {requestedData.tankPosition}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwapRequest(NetworkConnection requestConn, int key) {
        CrewData data = tankCrew[key];
        int oldKey = GetKey(requestConn);
        if (tankCrew[oldKey].swapRequest) {
            LogResponse(requestConn, "Cannot swap while requesting another position.");
            return;
        }
        if (data.conn == requestConn) {
            LogResponse(requestConn, "Already in this position.");
            return;
        }
        if (data.empty) {
            Swap(requestConn, key);
            return;
        }
        if (data.swapRequest) {
            LogResponse(requestConn, "Another client is requesting this position.");
            return;
        }
        SetSwapping(key, oldKey, true);
        SwapRequestPopup(data.conn, requestConn, key, oldKey);
        Debug.Log($"Client ID: {requestConn.ClientId} requesting position {key} in tank.");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwapRequestResponse(NetworkConnection requestConn, int key, int oldKey, bool swap) {
        if (swap) {
            Swap(requestConn, key);
            return;
        }
        SetSwapping(key, oldKey, false);
        LogResponse(requestConn, "Client declined swap");
    }

    private void SetSwapping(int key, int oldKey, bool isSwapping) {
        tankCrew[key].swapRequest = isSwapping;
        tankCrew[oldKey].swapRequest = isSwapping;
        tankCrew.Dirty(key);
        tankCrew.Dirty(oldKey);
    }
    #endregion Swap
    #endregion Server-Crew

    #region Client-Crew
    private void OnChange(SyncDictionaryOperation op, int key, CrewData value, bool asServer) {
        switch (op) {
            case SyncDictionaryOperation.Add: { //This event catch only client that created this VM.
                    CreateCrewButton(key, value);
                }
                break;
            case SyncDictionaryOperation.Set: {
                    crewButtons[key].GetComponentInChildren<TextMeshProUGUI>().text = GetName(value);
                }
                break;
            case SyncDictionaryOperation.Remove: {
                    crewButtons[key].GetComponentInChildren<TextMeshProUGUI>().text = GetName(value);
                }
                break;
            case SyncDictionaryOperation.Clear: {

                }
                break;
        }
    }

    private void CreateCrewButton(int key, CrewData data) {
        GameObject parent = GameObject.FindGameObjectWithTag(GameTagsUtils.CREW_GRID);
        GameObject go = Instantiate(crewInfoPrefab, parent.transform);
        go.GetComponentInChildren<TextMeshProUGUI>().text = GetName(data);
        go.GetComponentInChildren<Button>().onClick.AddListener(() => SwapRequest(LocalConnection, key));
        crewButtons.Insert(key, go);
    }

    private void CreateCrewButtonOnJoin() {
        foreach (var pair in tankCrew) {
            CreateCrewButton(pair.Key, pair.Value);
        }
    }

    private void DestroyCrewButton() {
        for (int i = 0; i < crewButtons.Count; i++) {
            Destroy(crewButtons[i]);
        }
        crewButtons.Clear();
    }

    public string GetName(CrewData data) {
        return data.empty ? "EMPTY" : "Client_" + data.conn.ClientId;
    }

    [TargetRpc]
    private void RegisterOnChange(NetworkConnection conn, bool register) {
        if (LocalConnection != conn) return;
        if (register) {
            CreateCrewButtonOnJoin();
            tankCrew.OnChange += OnChange;
        } else {
            tankCrew.OnChange -= OnChange;
            DestroyCrewButton();
        }
    }

    [TargetRpc]
    private void SwapRequestPopup(NetworkConnection targetConn, NetworkConnection requestConn, int key, int oldKey) {
        //TODO Add countdown (after countdown send decline)
        //TODO Fix client leaving during swap request
        FindObjectOfType<LobbyController>().SetSwapData(this, requestConn, key, oldKey);
    }
    #endregion Client-Crew
    #endregion Crew

    #region Tank
    #region Server-Tank
    [ServerRpc(RequireOwnership = false)]
    private void SpawntankServer(Preset preset) {
        SpawnTank(preset);
    }

    private void SpawnTank(Preset preset) {
        if (actualTank is not null) Despawn(actualTank);
        tankName = preset.tankName;
        actualTank = Instantiate(tankPrefab, spawnpointPosition, Quaternion.identity);
        NetworkObject no = actualTank.GetComponent<NetworkObject>();
        no.SetParent(networkObject);
        Spawn(no);
        BuildTank(preset, actualTank);
    }
    #endregion Server-Tank

    #region Client-Tank
    [ObserversRpc(BufferLast = true)]
    private void BuildTank(Preset preset, GameObject actualTank) {
        //TODO Build tank

        //TODO Just for testing purpose, delete in future
        Color color = Color.grey;
        if (preset.color != 0)
            color = preset.color == 1 ? Color.green : Color.yellow;
        //

        actualTank.GetComponentInChildren<MeshRenderer>().material.color = color;
        actualTank.name = preset.tankName;
    }

    public void ChangePreset(NetworkConnection conn, Preset preset) {
        SpawntankServer(preset);
    }
    #endregion Client-Tank
    #endregion Tank

    private int GetKey(NetworkConnection conn) {
        try {
            return tankCrew.First(key => key.Value.conn == conn).Key;
        } catch {
            return -1;
        }
    }

    private void Destroy(NetworkConnection conn) {
        if (Owner != conn) return;
        Despawn(gameObject);
    }

    [TargetRpc]
    public void LogResponse(NetworkConnection conn, string text) {
        Debug.Log(text);
    }

    private void OnDestroy() {
        PresetDropdown.OnPresetChange -= ChangePreset;
        UTW.SceneManager.OnClientDisconnectLobby -= Destroy;
    }

}
