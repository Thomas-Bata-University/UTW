using System.Collections.Generic;
using System.IO;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using Factions;
using UnityEngine;

namespace Managers
{
    public sealed class GameManager : NetworkBehaviour
    {
        private const string DataPath = "Assets/Resources/Users";

        public static GameManager Instance { get; private set; }

        private readonly Dictionary<string, PlayerData> _playersData = new();

        [SyncObject] public readonly SyncList<Player> players = new();


        private void Awake()
        {
            Instance = this;
            LoadUsers();
        }

        private void Update()
        {
            if (!IsServer) return;
        }

        private void LoadUsers()
        {
            var dir = new DirectoryInfo(DataPath);
            var info = dir.GetFiles("*.*");

            foreach (var fi in info)
            {
                var reader = new StreamReader(DataPath);

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
            var writer = new StreamWriter(DataPath + $"/{data.PlayerName}.json");
            writer.Write(json);
            return data;
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
        }
    }
}