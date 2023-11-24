using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FishNet.Object;
using UnityEngine;
using Utils;

namespace Factions
{
    public class FactionsManager : NetworkBehaviour
    {
        public static FactionsManager Instance { get; private set; }


        private const string DataPath = "Assets/Resources/Factions/Factions.json";

        private readonly Dictionary<int, Faction> _factions = new();

        public int CountOfFactions => _factions.Count;

        private void Awake()
        {
            Instance = this;
            Initialize();
        }

        private void Update()
        {
            if (!IsServer) return;
        }

        public void Initialize()
        {
            LoadFactionsFromJson();
            LoadFactionPresets();
        }

        public Faction GetFactionById(int guid) => _factions[guid];

        public Faction GetFactionByName(string factionName) =>
            _factions.FirstOrDefault(part => part.Value.Name.Equals(factionName)).Value;

        private void LoadFactionsFromJson()
        {
            var reader = new StreamReader(DataPath);

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
                    presets.Where(preset => faction.PresetNames.Contains(preset.presetName)));
            }
        }
    }
}