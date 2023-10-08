using System;
using System.Collections.Generic;

namespace Factions
{
    [Serializable]
    public record FactionsData
    {
        public IReadOnlyCollection<Faction> Factions { get; set; } = new List<Faction>().AsReadOnly();
    }
}