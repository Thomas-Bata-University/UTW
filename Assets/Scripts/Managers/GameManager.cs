using System.Collections.Generic;
using System.IO;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using Factions;
using UnityEngine;
using Utils;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncObject]
    private readonly SyncDictionary<string, PlayerData> _playersData = new();
    [SyncObject]
    private readonly SyncDictionary<int, Faction> _factions = new();

    public int CountOfFactions => _factions.Count;

    private void Awake()
    {
        Instance = this;
        LoadUsers();
        LoadFactionsFromJson();
        LoadFactionPresets();

        InstanceFinder.NetworkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
    }

    private void ServerManager_OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            Debug.Log($"The user with id: {conn.ClientId} has disconnected!");

            try
            {
                PlayerData p = GetPlayerByConnection(conn.ClientId);
                p.ClientConnection = -2;

                UpdateDictionary(p.PlayerName);
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Couldn't find a matching connection.");
            }
        }
    }

    private void Update()
    {
        if (!IsServer) return;
    }

    public void ListAllUsers()
    {
        foreach (var p in _playersData.Values)
        {
            Debug.Log($"Name: {p.PlayerName} | Conn: {p.ClientConnection}");
        }
    }

    [Server]
    public void UpdateDictionary(string name)
    {
        _playersData.Dirty(name);
    }

    private void LoadUsers()
    {
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Users/", "*.json");

        foreach (var fi in files)
        {
            var reader = new StreamReader(fi);

            var jsonString = reader.ReadToEnd();
            var data = JsonUtility.FromJson<PlayerData>(jsonString);
            _playersData[data.PlayerName] = data;
            _playersData[data.PlayerName].ClientConnection = -2;
        }
    }

    public PlayerData CreateOrSelectPlayer(string playerName)
    {
        // If found fill player with its json data
        if (_playersData.TryGetValue(playerName, out var existingPlayerData))
        {
            return existingPlayerData;
        }

        var data = _playersData[playerName] = CreatePlayerData(playerName);
        return data;
    }

    private PlayerData CreatePlayerData(string playerName)
    {
        var player = new PlayerData(playerName, -2, string.Empty);
        var json = JsonUtility.ToJson(player);
        var writer = new StreamWriter(Application.streamingAssetsPath + "/Users/" + $"/{player.PlayerName}.json");
        writer.Write(json);
        writer.Close();
        return player;
    }

    private void LoadFactionsFromJson()
    {
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Factions/", "*.json");
        var reader = new StreamReader(files.First());
        var jsonString = reader.ReadToEnd();
        var data = JsonUtility.FromJson<FactionsData>(jsonString);

        foreach (var faction in data.Factions)
        {
            _factions[faction.Id] = faction;
        }
    }

    private void LoadFactionPresets()
    {
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Presets/", "*.xml");

        var presets = files.Select(SerializationUtils.DeserializeXml<Preset>).ToList();

        foreach (var faction in _factions.Values)
        {
            //Genius serialization utility in Unity...just dont ask
            faction.Presets ??= new List<Preset>();
            faction.Presets.AddRange(
                presets.Where(preset => preset.faction.Equals(faction.Id)));
        }
    }

    public PlayerData GetPlayerByConnection(int clientId) =>
        _playersData.Values.First(playerData => playerData.ClientConnection.Equals(clientId));

    public PlayerData GetPlayerByName(string clientName) =>
        _playersData.Values.First(playerData => playerData.PlayerName.Equals(clientName));

    public Faction GetFactionById(int guid) => _factions[guid];

    public Faction GetFactionByName(string factionName) =>
        _factions.FirstOrDefault(part => part.Value.Name.Equals(factionName)).Value;

    private void OnDestroy()
    {
        InstanceFinder.NetworkManager.ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;
    }
}