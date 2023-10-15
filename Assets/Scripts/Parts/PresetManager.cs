using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PresetManager : NetworkBehaviour {

    [SerializeField]
    private Database assetDatabase;

    private void Start() {
        DontDestroyOnLoad(this);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoadPresetServerRpc(ulong playerId) {
        Debug.Log($"Loading assets for player ID: {playerId}");

        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Presets/", "*.json");
        var presetList = files.Select(Deserialize).ToArray();

        LoadPresetClientRpc(presetList, MultiplayerUtils.ClientRpcParams(playerId));
    }

    //Call this to load all presets for faction to Database.
    [ClientRpc]
    private void LoadPresetClientRpc(Preset[] presetList, ClientRpcParams clientParams = default) {
        if (IsServer || IsHost) return;
        assetDatabase.AddAll(presetList);
    }

    //YIRO-TODO add faction
    [ServerRpc(RequireOwnership = false)]
    public void SavePresetServerRpc(Preset tankPreset, ulong playerId) {
        string json = JsonUtility.ToJson(tankPreset);
        if (!Directory.Exists(Application.streamingAssetsPath + "/Presets/")) Directory.CreateDirectory(Application.streamingAssetsPath + "/Presets/");
        string filePath = Path.Combine(Application.streamingAssetsPath, "Presets", tankPreset.PresetName + ".json");
        File.WriteAllText(filePath, json);

        Debug.Log($"{tankPreset} successfully saved on SERVER by player ID: {playerId}");
        SaveResponceClientRpc(playerId, MultiplayerUtils.ClientRpcParams(playerId));
    }

    [ClientRpc]
    public void SaveResponceClientRpc(ulong playerId, ClientRpcParams client = default) {
        Debug.Log($"Preset successfully saved on SERVER.");
    }

    private static Preset Deserialize(string path) {
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            Preset deserialized = JsonUtility.FromJson<Preset>(json);
            return deserialized;
        } else {
            Debug.LogError("File not found: " + path);
            return null;
        }
    }

}
