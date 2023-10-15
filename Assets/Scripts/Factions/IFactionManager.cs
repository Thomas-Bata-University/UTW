using System;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;

namespace Factions
{
    public interface IFactionManager
    {
        public void Initialize();
        
        public Faction GetFactionById(Guid guid);

        [CanBeNull]
        public Faction GetFactionByName(string factionName);
    }
}