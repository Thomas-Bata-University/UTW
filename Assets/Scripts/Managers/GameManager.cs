using System.Collections.Generic;
using System.IO;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using Factions;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncObject] private readonly SyncDictionary<string, PlayerData> _playersData = new();
    [SyncObject] private readonly SyncDictionary<int, Faction> _factions = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        LoadFactionsFromJson();
        LoadFactionPresets();
        LoadUsers();

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
                p.ClientConnectionId = ConnectionCodes.OFFLINE_CODE;
                // ClearPresetsForClient(conn);

                UpdateDictionary(p.PlayerName);
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Couldn't find a matching connection.");
            }
        }
    }

    // TODO
    // This will work once we catch the disconnecting player while he's still on the server
    // As of now this will be called once he's already disconnected which is "k prdu"
    [TargetRpc]
    private void ClearPresetsForClient(NetworkConnection conn)
    {
        FindObjectOfType<Database>().RemoveAllPresets();
        Debug.Log("Clearing presets!");
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
            AssignFactionToPlayer(data);
        }
    }

    public PlayerData CreateOrSelectPlayer(string playerName)
    {
        if (_playersData.TryGetValue(playerName, out var existingPlayerData))
        {
            return existingPlayerData;
        }

        var data = _playersData[playerName] = CreatePlayerData(playerName);
        AssignFactionToPlayer(data);
        return data;
    }

    private PlayerData CreatePlayerData(string playerName)
    {
        var player = new PlayerData(playerName, 1);
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
        try
        {
            var files = Directory.GetFiles(Application.streamingAssetsPath + "/Presets/", "*.json");
            var presets = new List<Preset>();

            foreach (var file in files)
            {
                try
                {
                    var jsonString = File.ReadAllText(file);
                    var preset = JsonUtility.FromJson<Preset>(jsonString);

                    if (preset != null)
                        presets.Add(preset);
                    else
                        Debug.LogWarning($"Failed to deserialize preset from file: {file}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error processing preset file {file}: {ex.Message}");
                }
            }

            foreach (var faction in _factions.Values)
            {
                faction.Presets ??= new List<Preset>();

                var factionPresets = presets.Where(preset => preset.faction == faction.Id).ToList();
                faction.Presets.AddRange(factionPresets);

                Debug.Log($"Loaded {factionPresets.Count} presets for faction {faction.Name} (ID: {faction.Id})");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading faction presets: {ex.Message}");
        }
    }

    private void AssignFactionToPlayer(PlayerData player) =>
        _playersData[player.PlayerName].Faction = _factions[player.FactionId];

    public PlayerData GetPlayerByConnection(int clientId) =>
        _playersData.Values.First(playerData => playerData.ClientConnectionId.Equals(clientId));

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