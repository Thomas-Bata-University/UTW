using System;

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