using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Factions
{
    public class ServerFactionsManager : FactionsManager
    {
        private const string DataPath = "Assets/Resources/Factions/Factions.csv";
       
        //TODO Database assembly???!
        private dynamic _database;

        public readonly Dictionary<Guid, Faction> Factions = new();

        // Load assets from files, create "DB"
        public override void Initialize()
        {
            LoadFactionsFromCsv();
           // LoadFactionAssets();
        }

        private void LoadFactionAssets()
        {
            throw new NotImplementedException();
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
                Factions[Guid.NewGuid()] = new Faction
                {
                    Id = Guid.Parse(list.First()),
                    Name = list.Last()
                };
            }
        }
    }
}