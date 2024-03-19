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

    [Header("Tank")]
    public GameObject tankPrefab;
    private GameObject actualTank;

    private NetworkObject networkObject;
    [SyncVar] private Vector3 spawnpointPosition;

    //Crew
    private int maxCrewCount;
    [SyncObject] public readonly SyncDictionary<int, CrewData> tankCrew = new SyncDictionary<int, CrewData>();
    private List<GameObject> crewButtons = new List<GameObject>();

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
        tankCrew.Add(0, new CrewData(TankPositions.Driver));
        tankCrew.Add(1, new CrewData(TankPositions.Shooter));
        tankCrew.Add(2, new CrewData(TankPositions.Shooter));
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
        tankCrew.Dirty(data);
        return CrewIsEmpty();
    }

    public bool CrewIsEmpty() {
        return tankCrew.Count(x => x.Value.empty) == maxCrewCount;
    }

    public bool IsInCrew(NetworkConnection conn) {
        return tankCrew.Any(client => client.Value.conn == conn);
    }
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
    #endregion Client-Crew
    #endregion Crew

    #region Tank
    #region Server-Tank
    private void SpawnTank(Preset preset) {
        actualTank = Instantiate(tankPrefab, spawnpointPosition, Quaternion.identity);
        NetworkObject no = actualTank.GetComponent<NetworkObject>();
        no.SetParent(networkObject);
        Spawn(no);
    }
    #endregion Server-Tank

    #region Client-Tank
    public void ChangePreset(NetworkConnection conn, Preset preset) {
        if (!IsOwner) return;
        Debug.Log("Preset changed."); //TODO Change preset request
    }
    #endregion Client-Tank
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
        PresetDropdown.OnPresetChange -= ChangePreset;
        UTW.SceneManager.OnClientDisconnectLobby -= Destroy;
    }

}
