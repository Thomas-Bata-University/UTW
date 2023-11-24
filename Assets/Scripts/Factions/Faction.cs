using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Factions
{
    [Serializable]
    public class Faction
    {
        public int Id;
        public string Name;
        public string[] PresetNames;
        [field: NonSerialized] public List<Preset> Presets = new();

        public Faction(int id, string name, string[] presetNames)
        {
            Id = id;
            Name = name;
            PresetNames = presetNames;
        }
    }
}