using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Parts;
using UnityEngine;

public class Database : MonoBehaviour, IDatabase
{
    public List<GameObject> hulls;
    public List<GameObject> turrets;
    public List<Preset> presetList;

    public Preset SelectedPreset;

    private Dictionary<string, string> _moduleHashes;

    private void Start()
    {
        DontDestroyOnLoad(this);
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
            var fileName = Path.GetFileName(file);
            var hashString = GenerateHash(File.ReadAllBytes(file));

            var loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, file));

            if (loadedBundle is null)
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
                _moduleHashes.TryAdd(fileName, hashString);

                if (asset.name.EndsWith("Hull"))
                    hulls.Add(asset); //YIRO-TODO return MainBody

                if (asset.name.EndsWith("Turret"))
                    turrets.Add(asset);
            }
        }
    }

    public void AddAll(Preset[] presetList)
    {
        foreach (var preset in presetList)
        {
            this.presetList.Add(preset);
            Debug.Log($"Loaded preset: {preset}");
        }
    }
    
    public string GenerateHash(byte[] fileContent)
    {
        using var sha256 = SHA256.Create();

        var hashBytes = sha256.ComputeHash(fileContent);
        var hash = new StringBuilder(hashBytes.Length * 2);

        foreach (var b in hashBytes)
            hash.Append(b.ToString("X2"));

        return hash.ToString();
    }

    public bool ApprovalCheck(IEnumerable<string> requiredHashesFromServer)
        => requiredHashesFromServer.All(requiredHash => _moduleHashes.ContainsValue(requiredHash));

    public string GetModuleDataForServer()
        => JsonUtility.ToJson(_moduleHashes);
}