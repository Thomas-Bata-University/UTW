using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Parts
{
    public class Database : MonoBehaviour
    {
        public List<GameObject> hulls;
        public List<GameObject> turrets;
        public UnityEvent isDbInitialized;
        
        public Preset SelectedPreset;
    
        private Dictionary<string, string> _moduleHashes;
    
        private const string MainBodySuffix = "MainBody";
        private const string TurretSuffix = "Turret";
        
        
        
        private async void Start()
        {
            await Initialize();
        }

        private async Task Initialize()
        {
            hulls = new List<GameObject>();
            turrets = new List<GameObject>();
            _moduleHashes = new Dictionary<string, string>();

            var files = Directory.GetFiles(Application.streamingAssetsPath, "*.");
        
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var fileContent = await File.ReadAllBytesAsync(file);
                var hash = GenerateHash(fileContent);

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
                    // To ensure we're not overwriting any existing key.
                    _moduleHashes.TryAdd(fileName, hash);

                    if (asset.name.EndsWith(MainBodySuffix)) 
                        hulls.Add(asset);
                    else if (asset.name.EndsWith(TurretSuffix)) 
                        turrets.Add(asset);
                }
            }

            if (isDbInitialized != null && isDbInitialized.GetPersistentEventCount() > 0)
                isDbInitialized.Invoke();
        }

        private string GenerateHash(byte[] fileContent)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(fileContent);
                var hash = new StringBuilder(hashBytes.Length * 2);

                foreach (var b in hashBytes) 
                    hash.Append(b.ToString("X2"));

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
}