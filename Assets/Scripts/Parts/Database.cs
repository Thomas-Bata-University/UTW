using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Den.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Database : MonoBehaviour
{
    public List<GameObject> hulls;
    public List<GameObject> turrets;
    public UnityEvent isDbInitialized;
    
    public Preset SelectedPreset = null;
    
    private Dictionary<string, string> _moduleHashes;
    
    private const string MainBodySuffix = "MainBody";
    private const string TurretSuffix = "Turret";

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        hulls = new List<GameObject>();
        turrets = new List<GameObject>();
        _moduleHashes = new Dictionary<string, string>();

        var files = Directory.GetFiles(Application.streamingAssetsPath, "*.");
        
        foreach (var file in files)
        {
            var loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, file));

            if (loadedBundle == null)
            {
                Debug.Log($"Failed to load AssetBundle from file: '{file}'");
                continue;
            }

            var assets = loadedBundle.LoadAllAssets<GameObject>();
            loadedBundle.Unload(false);
            
            if (assets == null)
                continue;

            foreach (var asset in assets)
            {
                var hash = GenerateHash(asset);

                if (asset.name.EndsWith(MainBodySuffix))
                {
                    hulls.Add(asset);
                    _moduleHashes[asset.name] = hash;
                }
                else if (asset.name.EndsWith(TurretSuffix))
                {
                    turrets.Add(asset);
                    _moduleHashes[asset.name] = hash;
                }
            }
        }

        isDbInitialized.Invoke();
    }

    private string GenerateHash(GameObject asset)
    {
        var assetName = asset.name;
        
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(assetName));
            var hash = new StringBuilder(hashBytes.Length * 2);

            foreach (var b in hashBytes)
                hash.AppendFormat("{0:X2}", b);

            return hash.ToString();
        }
    }

    // Prototype
    private bool ApprovalCheck(IEnumerable<string> requiredHashesFromServer)
    {
        // Check if all required hashes are present in the client's DB.
        return requiredHashesFromServer
            .All(requiredHash => _moduleHashes.ContainsValue(requiredHash));
    }

    public string GetModuleDataForServer() 
        => JsonUtility.ToJson(_moduleHashes);
}