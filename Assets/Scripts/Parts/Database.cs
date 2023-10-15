using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Database : MonoBehaviour {

    public List<GameObject> hulls;
    public List<GameObject> turrets;
    public List<Preset> presetList;

    public Preset SelectedPreset = null;

    void Start() {
        DontDestroyOnLoad(this);
        Initialize();
    }

    void Initialize() {
        string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.");
        foreach (string file in files) {
            var loadedBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, file));

            if (loadedBundle == null) {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var assets = loadedBundle.LoadAllAssets<GameObject>();
            foreach (var asset in assets) {
                if (asset.name.EndsWith("Hull")) hulls.Add(asset); //YIRO-TODO return MainBody
                if (asset.name.EndsWith("Turret")) turrets.Add(asset);
            }
        }
    }

    public void AddAll(Preset[] presetList) {
        foreach (var preset in presetList) {
            this.presetList.Add(preset);
            Debug.Log($"Loaded preset: {preset}");
        }
    }

}
