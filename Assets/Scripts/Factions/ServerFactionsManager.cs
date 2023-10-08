using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public class ServerFactionsManager : FactionsManager
    {
        private const string DataPath = "Assets/Resources/Factions/Factions.csv";

        //TODO Database assembly???!
        private DatabaseMock _database;

        private readonly Dictionary<Guid, Faction> _factions = new();

        public int CountOfFactions => _factions.Count;

        // Load assets from files, create "DB"
        public override void Initialize()
        {
            LoadFactionsFromCsv();
            LoadFactionAssets();
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

            var dataset = reader.ReadToEnd();
            var lines = dataset.Split('\n');
            var lists = new List<List<string>>();
            var columns = 0;
            foreach (var t in lines)
            {
                var data = t.Split(',');
                var list = new List<string>(data); // turn this into a list
                lists.Add(list); // add this list into a big list
                columns = Mathf.Max(columns,
                    list.Count); // this way we can tell what's the max number of columns in data
            }

            foreach (var list in lists)
            {
                _factions[Guid.NewGuid()] = new Faction
                {
                    Id = Guid.Parse(list.First()),
                    Name = list.Last()
                };
            }
        }
    }
}