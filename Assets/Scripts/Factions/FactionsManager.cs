using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FishNet.Object;
using UnityEngine;

namespace Factions
{
    public class FactionsManager : NetworkBehaviour
    {
        public static FactionsManager Instance { get; private set; }


        private const string DataPath = "Assets/Resources/Factions/Factions.json";

        private readonly Dictionary<Guid, Faction> _factions = new();

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
            LoadFactionsFromCsv();
            LoadFactionPresets();
        }

        public Faction GetFactionById(Guid guid) => _factions[guid];

        public Faction GetFactionByName(string factionName) =>
            _factions.FirstOrDefault(part => part.Value.Name.Equals(factionName)).Value;

        private void LoadFactionsFromCsv()
        {
            var reader = new StreamReader(DataPath);

            var jsonString = reader.ReadToEnd();
            var data = new FactionsData() /*  TODO JsonConvert.DeserializeObject<FactionsData>(jsonString)*/;

            foreach (var faction in data.Factions)
            {
                _factions[faction.Id] = faction;
            }
        }

        private void LoadFactionPresets()
        {
            var files = Directory.GetFiles(Application.streamingAssetsPath + "/Presets/", "*.xml");

            var presets = files.Select(Deserialize).ToList();

            foreach (var faction in _factions.Values)
            {
                faction.Presets.AddRange(
                    presets.Where(preset => faction.PresetNames.Contains(preset.presetName)));
            }
        }

        private static Preset Deserialize(string path)
        {
            var serializer = new XmlSerializer(typeof(Preset));
            var reader = new StreamReader(path);
            var deserialized = (Preset)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        }
    }
}