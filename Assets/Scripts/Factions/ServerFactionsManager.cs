using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Factions
{
    //TODO Database assembly???!
    internal class DatabaseMock
    {
        public List<GameObject> hulls = new();
        public List<GameObject> turrets = new();
        public UnityEvent isDbInitialized = new();
    }

    public class ServerFactionsManager : FactionsManager, IFactionManagerAssetHandler
    {
        private const string DataPath = "Assets/Resources/Factions/Factions.json";

        //TODO Database assembly???!
        private DatabaseMock _database;

        private readonly Dictionary<Guid, Faction> _factions = new();

        public int CountOfFactions => _factions.Count;

        // Load assets from files, create "DB"
        public override void Initialize()
        {
            LoadFactionsFromCsv();
            //LoadFactionAssets();
        }

        public Faction GetFactionById(Guid guid) => _factions[guid];

        private void LoadFactionAssets()
        {
            foreach (var hull in _database.hulls)
            {
                //var hullFaction = Factions[hull.FactionId];
                // TODO how to know hulls or other assets faction?
                var hullFaction = _factions[Guid.NewGuid()];
                hullFaction.Hulls.Add(hull);
            }

            foreach (var turret in _database.turrets)
            {
                //var turretFaction = Factions[turret.FactionId];
                // TODO how to know turret or other assets faction?
                var turretFaction = _factions[Guid.NewGuid()];
                turretFaction.Turrets.Add(turret);
            }
        }

        private void LoadFactionsFromCsv()
        {
            var reader = new StreamReader(DataPath);

            var jsonString = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<FactionsData>(jsonString);

            foreach (var faction in data.Factions)
            {
                _factions[faction.Id] = faction;
            }
        }

        //TODO store type on object?, wrapper around GameObject?
        public void OnSaveAsset(GameObject asset, AssetType type)
        {
            //TODO save asset file on server?  _database.SaveAsset(asset, type);
        }
    }
}