using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace Factions
{
    public class ClientFactionsManager : FactionsManager, IFactionManagerAssetHandler
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

        public void OnSaveAsset(GameObject asset, AssetType type)
        {
            OnClientAddAssetToServerRpc(asset, type);
        }


        // Save assets to server?
        [ServerRpc]
        private void OnClientAddAssetToServerRpc(GameObject asset, AssetType type)
        {
            ServerFactionsManager.OnSaveAsset(asset, type);
        }
    }
}