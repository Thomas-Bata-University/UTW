using System;
using System.Collections.Generic;

namespace Factions
{
    [Serializable]
    public class Faction
    {
        public int Id;
        public string Name;
        [field: NonSerialized] public List<Preset> Presets = new();

        public Faction()
        {
        }

        public Faction(int id, string name, string[] presetNames)
        {
            Id = id;
            Name = name;
        }
    }
}