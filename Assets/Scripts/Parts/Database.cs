using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Database : MonoBehaviour
{
    public List<GameObject> hulls;
    public List<GameObject> turrets;
    public List<Preset> presetList;
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
            foreach (var asset in assets)
            {
                if (asset.name.EndsWith("Hull")) hulls.Add(asset); //YIRO-TODO return MainBody
                if (asset.name.EndsWith("Turret")) turrets.Add(asset);
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

    public void AddAll(Preset[] presetList)
    {
        foreach (var preset in presetList)
        {
            this.presetList.Add(preset);
        }
    }
}