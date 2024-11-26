using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Factions;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Database : MonoBehaviour {

    public NetworkManager networkManager;

    [Serializable]
    public struct Data {

        public string key;
        public GameObject prefab;

        public Data(string key, GameObject prefab) {
            this.key = key;
            this.prefab = prefab;
        }

    }

    //TODO Refactor to dictionary for better performance
    public List<Data> hulls;
    public List<Data> turrets;

    //KEY - faction | VALUE - Preset
    public Dictionary<int, List<Preset>> presetList = new();
    public static List<string> hashes { get; private set; }

    public Preset SelectedPreset = null;

    void Start()
    {
        DontDestroyOnLoad(this);
        Initialize();
    }

    private void Initialize()
    {
        string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.");
        foreach (string file in files)
        {
            var loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, file));

            if (loadedBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var assets = loadedBundle.LoadAllAssets<GameObject>();
            foreach (var asset in assets) {
                var data = new Data(asset.name, asset);
                Debug.Log(asset.name);

                if (asset.name.ToLower().Contains("hull")) hulls.Add(data);
                if (asset.name.ToLower().Contains("turret")) turrets.Add(data);

                networkManager.SpawnablePrefabs.AddObject(asset.GetComponent<NetworkObject>());
            }
        }

        LoadHashes(files);
    }

    private void LoadHashes(string[] files)
    {
        List<string> fileHashes = new List<string>();

        foreach (var file in files)
        {
            string hash = ComputeSHA256Hash(file);
            fileHashes.Add(hash);
            Debug.Log($"File:\t{Path.GetFileName(file)}\tHash: {hash}");
        }

        hashes = fileHashes;
        Debug.Log($"AssetDB succesfully hashed! Count: {hashes.Count}");
    }

    private string ComputeSHA256Hash(string filePath)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public void AddAllPresets(Faction faction)
    {
        Debug.Log($"Loaded {faction.Presets.Count} presets for fraction {faction.Id}");
        presetList.Add(faction.Id, faction.Presets);
    }

    public void AddAllPresets(SyncDictionary<int, Faction> factions)
    {
        foreach (var kv in factions) {
            presetList.Add(kv.Key, kv.Value.Presets);
            Debug.Log($"Saved: {kv.Key} - {kv.Value.Presets.Count} presets to server Database");
        }
    }

    public GameObject FindHullByKey(string key) {
        return FindByKey(hulls, key);
    }

    public GameObject FindTurretByKey(string key) {
        return FindByKey(turrets, key);
    }

    private GameObject FindByKey(List<Data> list, string key) {
        return list.Find(data => data.key.Equals(key)).prefab;
    }

    public void RemoveAllPresets()
    {
        presetList.Clear();
    }
  
}