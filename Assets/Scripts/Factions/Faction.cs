using System;
using System.Collections.Generic;
using UnityEngine;

namespace Factions
{
    public record Faction
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<GameObject> Hulls { get; set; } = new();
        public List<GameObject> Turrets { get; set; } = new();
    }
}