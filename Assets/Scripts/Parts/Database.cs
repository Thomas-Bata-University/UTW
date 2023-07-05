using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Database : MonoBehaviour
{
    public List<GameObject> hulls;
    public List<GameObject> turrets;
    public UnityEvent isDbInitialized;

    public Preset SelectedPreset = null;


    void Start()
    {
        //hulls = Resources.LoadAll<GameObject>("Hulls");
        //turrets = Resources.LoadAll<GameObject>("Turrets");
        Initialize();
    }
     void Initialize()
    {
        string[] files = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.");
        foreach (string file in files)
        {
            var loadedBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, file));

            if (loadedBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var assets = loadedBundle.LoadAllAssets<GameObject>();
            foreach (var asset in assets)
            {
                if (asset.name.EndsWith("Hull")) hulls.Add(asset);
                if (asset.name.EndsWith("Turret")) turrets.Add(asset);
            }
        }
        isDbInitialized.Invoke();
    }
}
