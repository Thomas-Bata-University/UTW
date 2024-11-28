using System;
using System.Collections.Generic;
using System.Linq;
using ChobiAssets.PTM;
using Enum;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Preset;

public class VehicleManager : NetworkBehaviour
{
    [Header("UI")]
    public GameObject crewInfoPrefab;

    [Header("Tank")]
    public GameObject visorPrefab;
    [SyncVar] private GameObject actualTank;
    [SyncVar, HideInInspector] public string tankName;

    private NetworkObject networkObject;
    [SyncVar] private Vector3 spawnpointPosition;

    private Database _assetDatabase;

    [Header("Crew")]
    //KEY - index | VALUE - CrewData
    [SyncObject, HideInInspector] public readonly SyncDictionary<int, CrewData> _tankCrew = new SyncDictionary<int, CrewData>();
    private List<GameObject> crewButtons = new List<GameObject>();
    private int maxCrewCount;

    [HideInInspector] public LobbyManager lobbyManager;

    //TODO: Get this shit away after Den Rozglábených futer
    public GameObject Destroyed_Effect;

    private void Awake()
    {
        PresetDropdown.OnPresetChange += ChangePreset; //Client side - use this after joining crew
        UTW.SceneManager.OnClientDisconnectLobby += Destroy;

        _assetDatabase = FindObjectOfType<Database>();
    }

    #region Crew
    #region Server-Crew
    public void SetCrewData(Preset preset, NetworkConnection conn, Vector3 spawnpoint)
    {
        networkObject = GetComponent<NetworkObject>();
        spawnpointPosition = spawnpoint;
        SpawnTank(preset);
    }

    public bool JoinCrew(NetworkConnection joiningClientConn)
    {
        if (_tankCrew.Any(x => x.Value.empty))
        {
            RegisterOnChange(joiningClientConn, true);
            CrewData data = _tankCrew.First(x => x.Value.conn is null).Value;
            data.conn = joiningClientConn;
            data.empty = false;
            data.tankPart.GiveOwnership(joiningClientConn);
            _tankCrew.Dirty(data);
            return !_tankCrew.Any(x => x.Value.empty);
        }
        return true;
    }

    public bool LeaveCrew(NetworkConnection leavingClientConn)
    {
        RegisterOnChange(leavingClientConn, false);
        CrewData data = _tankCrew.First(x => x.Value.conn == leavingClientConn).Value;
        data.conn = null;
        data.empty = true;
        data.swapRequest = false;
        data.tankPart.RemoveOwnership();
        _tankCrew.Dirty(data);
        return CrewIsEmpty();
    }

    public bool CrewIsEmpty()
    {
        return _tankCrew.Count(x => x.Value.empty) == maxCrewCount;
    }

    public bool IsInCrew(NetworkConnection conn)
    {
        return _tankCrew.Any(client => client.Value.conn == conn);
    }

    #region Swap
    private void Swap(NetworkConnection requestConn, int key)
    {
        int oldKey = GetKey(requestConn);
        if (oldKey == -1) return;
        CrewData oldData = _tankCrew[oldKey];
        CrewData requestedData = _tankCrew[key];

        var conn = requestedData.conn;
        oldData.conn = conn;
        oldData.empty = conn is null;
        oldData.tankPart.GiveOwnership(conn);

        requestedData.conn = requestConn;
        requestedData.empty = false;
        requestedData.tankPart.GiveOwnership(requestConn);

        _tankCrew.Dirty(key);
        _tankCrew.Dirty(oldKey);

        SetSwapping(key, oldKey, false);
        Debug.Log($"Client ID: {requestConn.ClientId} swapped position to {requestedData.tankPosition}");
        LogResponse(requestConn, $"Position changed to {requestedData.tankPosition}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwapRequest(NetworkConnection requestConn, int key)
    {
        CrewData data = _tankCrew[key];
        int oldKey = GetKey(requestConn);
        if (_tankCrew[oldKey].swapRequest)
        {
            LogResponse(requestConn, "Cannot swap while requesting another position.");
            return;
        }
        if (data.conn == requestConn)
        {
            LogResponse(requestConn, "Already in this position.");
            return;
        }
        if (data.empty)
        {
            Swap(requestConn, key);
            return;
        }
        if (data.swapRequest)
        {
            LogResponse(requestConn, "Another client is requesting this position.");
            return;
        }
        SetSwapping(key, oldKey, true);
        SwapRequestPopup(data.conn, requestConn, key, oldKey);
        Debug.Log($"Client ID: {requestConn.ClientId} requesting position {key} in tank.");
    }

    [ServerRpc(RequireOwnership = false)]
    public void InGameSwap(NetworkConnection requestConn, TankPositions tankPosition)
    {
        try
        {
            int oldKey = GetKey(requestConn);
            int key = 0;

            switch (tankPosition)
            {
                case TankPositions.DRIVER:
                    key = _tankCrew.First(x => x.Value.empty && x.Value.tankPosition == TankPositions.DRIVER).Key;
                    break;
                case TankPositions.GUNNER:
                    key = _tankCrew.First(x => x.Value.empty && x.Value.tankPosition == TankPositions.GUNNER).Key;
                    break;
                // case TankPositions.OBSERVER:
                //     key = _tankCrew.First(x => x.Value.empty && x.Value.tankPosition == TankPositions.OBSERVER).Key;
                //     break;
                default:
                    break;
            }

            //int key = _tankCrew.First(x => x.Value.empty).Key;
            Swap(requestConn, key);
            ActivateController(requestConn, _tankCrew[oldKey], _tankCrew[key]);
        }
        catch
        {
            LogResponse(requestConn, "No position to swap.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwapRequestResponse(NetworkConnection requestConn, int key, int oldKey, bool swap)
    {
        if (swap)
        {
            Swap(requestConn, key);
            return;
        }
        SetSwapping(key, oldKey, false);
        LogResponse(requestConn, "Client declined swap");
    }

    private void SetSwapping(int key, int oldKey, bool isSwapping)
    {
        _tankCrew[key].swapRequest = isSwapping;
        _tankCrew[oldKey].swapRequest = isSwapping;
        _tankCrew.Dirty(key);
        _tankCrew.Dirty(oldKey);
    }
    #endregion Swap
    #endregion Server-Crew

    #region Client-Crew
    private void OnChange(SyncDictionaryOperation op, int key, CrewData value, bool asServer)
    {
        switch (op)
        {
            case SyncDictionaryOperation.Add:
                { //This event catch only client that created this VM.
                    FindObjectOfType<LobbyController>().HideObjects(false);
                    CreateCrewButton(key, value);
                }
                break;
            case SyncDictionaryOperation.Set:
                {
                    crewButtons[key].GetComponentInChildren<TextMeshProUGUI>().text = GetName(value);
                }
                break;
            case SyncDictionaryOperation.Remove:
                {
                    crewButtons[key].GetComponentInChildren<TextMeshProUGUI>().text = GetName(value);
                }
                break;
            case SyncDictionaryOperation.Clear:
                {
                    Debug.Log("Destroying buttons");
                    DestroyCrewButton();
                }
                break;
        }
    }

    private void CreateCrewButton(int key, CrewData data)
    {
        GameObject parent = GameObject.FindGameObjectWithTag(GameTagsUtils.CREW_GRID);
        GameObject go = Instantiate(crewInfoPrefab, parent.transform);
        go.GetComponentInChildren<TextMeshProUGUI>().text = GetName(data);
        go.GetComponentInChildren<Button>().onClick.AddListener(() => SwapRequest(LocalConnection, key));
        crewButtons.Insert(key, go);
    }

    private void CreateCrewButtonOnJoin()
    {
        foreach (var pair in _tankCrew)
        {
            CreateCrewButton(pair.Key, pair.Value);
        }
    }

    private void DestroyCrewButton()
    {
        for (int i = 0; i < crewButtons.Count; i++)
        {
            Destroy(crewButtons[i]);
        }
        crewButtons.Clear();
    }

    public string GetName(CrewData data)
    {
        return data.empty ? "EMPTY" : GameManager.Instance.GetPlayerByConnection(data.conn.ClientId).PlayerName;
    }

    [TargetRpc]
    private void RegisterOnChange(NetworkConnection conn, bool register)
    {
        if (LocalConnection != conn) return;
        if (register)
        {
            CreateCrewButtonOnJoin();
            _tankCrew.OnChange += OnChange;
        }
        else
        {
            _tankCrew.OnChange -= OnChange;
            DestroyCrewButton();
        }
    }

    [TargetRpc]
    private void SwapRequestPopup(NetworkConnection targetConn, NetworkConnection requestConn, int key, int oldKey)
    {
        //TODO Fix client leaving during swap request
        FindObjectOfType<LobbyController>().SetSwapData(this, requestConn, key, oldKey);
    }

    public KeyValuePair<int, CrewData> GetActualTankPosition()
    {
        foreach (var pair in _tankCrew)
        {
            if (pair.Value.conn == LocalConnection)
            {
                return pair;
            }
        }
        throw new Exception("Player position not found.");
    }
    #endregion Client-Crew
    #endregion Crew

    #region Tank
    #region Server-Tank
    [ServerRpc(RequireOwnership = false)]
    private void ChangePresetServer(NetworkConnection conn, Preset preset)
    {
        //TODO Refactor this shit
        LeaveCrew(conn);
        _tankCrew.Clear();
        SpawnTank(preset);
        JoinCrew(conn);
    }

    private void SpawnTank(Preset preset)
    {
        if (actualTank is not null) Despawn(actualTank);
        tankName = preset.tankName;
        SpawnTankParts(preset.mainPart);
        BuildTank(preset, actualTank);
    }

    private void SpawnTankParts(MainPart mainPart) {
        Debug.Log($"Spawning part {mainPart.mainData.partName}");
        GameObject tankPrefab = _assetDatabase.FindHullByKey(mainPart.mainData.databaseKey);
        Vector3 spawnPosition = new Vector3(spawnpointPosition.x, mainPart.mainData.partPosition.y, spawnpointPosition.z);
        actualTank = Instantiate(tankPrefab, spawnPosition, Quaternion.identity);
        NetworkObject tankNo = actualTank.GetComponent<NetworkObject>();
        actualTank.name = mainPart.mainData.partName;
        tankNo.SetParent(networkObject);
        Spawn(tankNo);

        _tankCrew.Add(mainPart.mainData.key, new CrewData(mainPart.mainData.tankPosition, tankNo, 0));

        int childIndex = 2; // child index 0 is camera

        foreach (var part in mainPart.parts)
        {
            TankData data = part.partData;
            Debug.Log($"Spawning part {data.partName}");
            GameObject tankPart = Instantiate(SelectPrefab(data), data.partPosition + transform.position, Quaternion.identity, actualTank.transform);
            tankPart.name = data.partName;
            NetworkObject partNo = tankPart.GetComponent<NetworkObject>();
            partNo.SetParent(tankNo);
            tankPart.transform.localPosition = data.partPosition;
            Spawn(partNo);

            _tankCrew.Add(data.key, new CrewData(data.tankPosition, partNo, childIndex));
            childIndex++;
        }

        maxCrewCount = _tankCrew.Count;
    }

    private GameObject SelectPrefab(TankData tankData) {
        switch (tankData.partType) {
            case PartType.TURRET:
                return _assetDatabase.FindTurretByKey(tankData.databaseKey);
            case PartType.WEAPONRY:
                break;
            case PartType.SUSPENSION:
                break;
            case PartType.OTHER:
                return visorPrefab;
        }

        return null;
    }
    #endregion Server-Tank

    #region Client-Tank
    [ObserversRpc(BufferLast = true)]
    private void BuildTank(Preset preset, GameObject actualTank)
    {
        //TODO Just for testing purpose, delete in future
        //Color color = Color.grey;
        //if (preset.color != 0)
        //    color = preset.color == 1 ? Color.green : Color.yellow;

        //actualTank.GetComponentInChildren<MeshRenderer>().material.color = color;
        actualTank.name = preset.tankName;
    }

    public void ChangePreset(NetworkConnection conn, Preset preset)
    { //TODO cannot swap preset if there are more clients
        Debug.Log($"Calling for server to change preset {conn}");
        ChangePresetServer(conn, preset);
    }

    public void StartGame()
    {
        FindObjectOfType<LobbyController>().menuPanel.SetActive(false);
        ActivateController(GetActualTankPosition().Value, true);
    }

    [TargetRpc]
    private void ActivateController(NetworkConnection conn, CrewData oldData, CrewData newData)
    {
        //TODO Can we do it better?
        ActivateController(oldData, false);
        ActivateController(newData, true);
    }
    private void ActivateController(CrewData data, bool active)
    {
        Transform tankPart = actualTank.transform;
        Debug.Log($"Index {data.childIndex}");

        if (!data.tankPosition.Equals(TankPositions.DRIVER))
            tankPart = tankPart.GetChild(data.childIndex);

        EnableController(tankPart, active);
    }

    private void EnableController(Transform tankPart, bool active)
    {
        if (tankPart.TryGetComponent(out PlayerController playerController)) playerController.enabled = active;
        if(tankPart.GetComponentInChildren<Camera>() != null) tankPart.GetComponentInChildren<Camera>().enabled = active;
        if(tankPart.GetComponentInChildren<AudioListener>() != null) tankPart.GetComponentInChildren<AudioListener>().enabled = active;
        if(tankPart.GetComponentInChildren<Drive_Control_CS>() != null) tankPart.GetComponentInChildren<Drive_Control_CS>().Selected(active);
        if (TryGetComponent(out ControlSwitch cswitch)) cswitch.enabled = active;
        if (tankPart.TryGetComponent(out SoundControl soundControl)) soundControl.EnableSound();

        if (tankPart.Find("Gun_Camera") != null)
        {
            tankPart.Find("Gun_Camera").gameObject.SetActive(false);
            if (tankPart.TryGetComponent(out GunnerController gunnerController)) gunnerController.isInScope = false;
        }

            // tankPart.GetComponent<PlayerController>().enabled = active;
    }
    #endregion Client-Tank
    #endregion Tank

    private int GetKey(NetworkConnection conn)
    {
        try
        {
            return _tankCrew.First(key => key.Value.conn == conn).Key;
        }
        catch
        {
            return -1;
        }
    }

    private void Destroy(NetworkConnection conn)
    {
        if (Owner != conn) return;
        Despawn(gameObject);
    }

    [TargetRpc]
    public void LogResponse(NetworkConnection conn, string text)
    {
        Debug.Log(text);
    }

    private void OnDestroy()
    {
        PresetDropdown.OnPresetChange -= ChangePreset;
        UTW.SceneManager.OnClientDisconnectLobby -= Destroy;
    }


    // Temporary function for simplified damage handling
    public void ShellHitsVehicle()
    {
        if (!IsServer) return;

        var connList = _tankCrew.Select(x => x.Value.conn).Where(x => x != null).ToArray();
        var roundSystem = lobbyManager.gameObject.GetComponent<RoundSystem>();
        if (roundSystem != null)
        {
            roundSystem.OnTankDestroyed(connList);
        }
        BlowUpHull();
    }

    [ObserversRpc]
    void BlowUpHull()
    {
        Instantiate(Destroyed_Effect, transform);
    }
}