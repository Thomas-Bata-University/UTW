using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Database : MonoBehaviour
{
    public List<GameObject> hulls;
    public List<GameObject> turrets;
    public List<GameObject> canons;
    public List<GameObject> suspensions;
    public List<GameObject> maps;
    public UnityEvent isDbInitialized;
    
    public Preset SelectedPreset = null;

    private string[] names = { "hull", "turret", "canon", "suspension", "map", };



    private List<GameObject>[] arrayOfLists;



    void Start()
    {
        InitializeArrayOfLists();
        //hulls = Resources.LoadAll<GameObject>("Hulls");
        //turrets = Resources.LoadAll<GameObject>("Turrets");
        Initialize();
    }
    void Initialize()
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
                for (int i = 0; i < arrayOfLists.Length; i++)
                {
                        if (asset.name.EndsWith(names[i], StringComparison.OrdinalIgnoreCase))
                        {
                            arrayOfLists[i].Add(asset);
                        }
                }
            }

            
            isDbInitialized.Invoke();
        }
    }

    void InitializeArrayOfLists()
    {
        arrayOfLists = new List<GameObject>[]
        {
            hulls,
            turrets,
            canons,
            suspensions,
            maps
        };
    }

}
