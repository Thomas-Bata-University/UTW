using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace Factions
{
    public class ClientFactionsManager : FactionsManager
    {
        public Guid FactionId { get; set; }
        public ServerFactionsManager ServerFactionsManager { get; set; }

        [CanBeNull] private Faction _faction;

        public List<GameObject> AvailableHulls => _faction?.Hulls ?? new List<GameObject>();
        public List<GameObject> AvailableTurrets => _faction?.Turrets ?? new List<GameObject>();

        public override void Initialize()
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
    }
}