using System;
using System.Collections.Generic;
using FishNet.Object;
using JetBrains.Annotations;
using UnityEditor.Presets;
using UnityEngine;

namespace Factions
{
    public class ClientFactionsManager :  NetworkBehaviour,
        IFactionManager
    {
        public Guid FactionId { get; set; }
        public ServerFactionsManager ServerFactionsManager { get; set; }

        [CanBeNull] private Faction _faction;

        public List<Preset> AvailablePresets => _faction?.Presets ?? new List<Preset>();

        public void Initialize()
        {
            OnClientInitializedServerRpc();
        }

        // Retrieve assets from server
        [ServerRpc]
        private void OnClientInitializedServerRpc()
        {
            RetrieveAssetsFromServer();
        }

        private void RetrieveAssetsFromServer()
        {
            _faction = ServerFactionsManager.GetFactionById(FactionId);
        }

        public Faction GetFactionById(Guid guid)
            => ServerFactionsManager.GetFactionById(guid);

        public Faction GetFactionByName(string factionName)
            => ServerFactionsManager.GetFactionByName(factionName);
    }
}