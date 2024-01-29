using System.Collections.Generic;
using System.IO;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using Factions;
using UnityEngine;
using Utils;

namespace Managers
{
    public sealed class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        private readonly Dictionary<string, PlayerData> _playersData = new();

        [SyncObject] public readonly SyncList<Player> players = new();


        [SyncObject] private readonly SyncDictionary<int, Faction> _factions = new();

        public int CountOfFactions => _factions.Count;

        private void Awake()
        {
            Instance = this;
            LoadUsers();
            LoadFactionsFromJson();
            LoadFactionPresets();
        }

        private void Update()
        {
            if (!IsServer) return;
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
            }
        }

        public PlayerData CreateOrSelectPlayer(Player player)
        {
            // If found fill player with its json data
            if (_playersData.TryGetValue(player.PlayerName, out var existingPlayerData))
            {
                players.Add(player);
                return existingPlayerData;
            }

            // If not found, create and return new json
            players.Add(player);
            return _playersData[player.PlayerName] = CreatePlayerData(player);
        }

        private PlayerData CreatePlayerData(Player player)
        {
            var data = new PlayerData(player.name, (ulong)player.ObjectId, string.Empty);
            var json = JsonUtility.ToJson(data);
            var writer = new StreamWriter(Application.streamingAssetsPath + "/Users/" + $"/{data.PlayerName}.json");
            writer.Write(json);
            return data;
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
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


        public Faction GetFactionById(int guid) => _factions[guid];

        public Faction GetFactionByName(string factionName) =>
            _factions.FirstOrDefault(part => part.Value.Name.Equals(factionName)).Value;
    }
}