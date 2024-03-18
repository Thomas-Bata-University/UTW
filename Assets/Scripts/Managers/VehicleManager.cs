using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VehicleManager : NetworkBehaviour {

    [Header("UI")]
    public GameObject crewInfoPrefab;
    private GameObject parent;

    //Crew
    private int maxCrewCount;
    [SyncObject] public readonly SyncDictionary<int, CrewData> tankCrew = new SyncDictionary<int, CrewData>();
    private List<GameObject> crewButtons = new List<GameObject>();

    private void Awake() {
        PresetDropdown.OnPresetChange += SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby += Destroy;
        parent = GameObject.FindGameObjectWithTag(GameTagsUtils.CREW_GRID);
    }

    #region Crew
    #region Server
    public void SetCrewData(Preset preset) {
        //TODO Set this data from preset on VM creation
        tankCrew.Add(0, new CrewData(TankPositions.Driver));
        tankCrew.Add(1, new CrewData(TankPositions.Shooter));
        tankCrew.Add(2, new CrewData(TankPositions.Shooter));
        maxCrewCount = tankCrew.Count;
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
        tankCrew.Dirty(data);
        return CrewIsEmpty();
    }

    public bool CrewIsEmpty() {
        return tankCrew.Count(x => x.Value.empty) == maxCrewCount;
    }

    public bool IsInCrew(NetworkConnection conn) {
        return tankCrew.Any(client => client.Value.conn == conn);
    }
    #endregion Server

    #region Client
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
        GameObject go = Instantiate(crewInfoPrefab, parent.transform);
        go.GetComponentInChildren<TextMeshProUGUI>().text = GetName(data);
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

    private string GetName(CrewData data) {
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

    private void SwapRequest(NetworkConnection conn) {
        Debug.Log($"Client ID: {conn.ClientId} requesting for position swap.");
    }
    #endregion Client
    #endregion Crew

    #region Tank
    public void SpawnTank(NetworkConnection conn, Preset preset) {
        if (!IsOwner) return;
        SpawnTankServerRpc(conn, preset);
    }

    [ServerRpc]
    public void SpawnTankServerRpc(NetworkConnection conn, Preset preset) { //TODO Delete tank owner

    }
    #endregion Tank

    private void Destroy(NetworkConnection conn) {
        if (Owner != conn) return;
        Despawn(gameObject);
    }

    [TargetRpc]
    public void LogResponse(NetworkConnection conn, string text) {
        Debug.Log(text);
    }

    private void OnDestroy() {
        PresetDropdown.OnPresetChange -= SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby -= Destroy;
    }

}
