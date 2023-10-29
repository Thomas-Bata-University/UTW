using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Factions
{
    [Serializable]
    public record Faction
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<string> PresetNames { get; set; } = new();

        [XmlIgnore] [field: NonSerialized] public List<Preset> Presets { get; set; } = new();
    }
}