using System;
using System.Collections.Generic;

namespace Factions
{
    [Serializable]
    public class FactionsData
    {
        public Faction[] Factions;

        public FactionsData(Faction[] factions)
        {
            Factions = factions;
        }
    }
}