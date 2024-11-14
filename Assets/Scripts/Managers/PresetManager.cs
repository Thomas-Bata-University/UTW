using System;
using System.Collections.Generic;
using System.IO;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PresetManager : NetworkBehaviour
{
    [SerializeField]
    private Database assetDatabase;

    private void Start()
    {
        assetDatabase = FindObjectOfType<Database>();

        if (IsServer)
        {
            TestPreset(Preset.CreateDefaultPreset());
        }
    }

    public override void OnStartClient()
    {
        if (LocalConnection.IsLocalClient)
        {
            Debug.Log($"Client ID: {LocalConnection.ClientId} connected... Loading asset.");
            LoadPreset(LocalConnection);
        }
        else
        {
            Debug.LogWarning($"Client ID: {Owner.ClientId} is owner of this object... Cannot load asset.");
        }
    }

    /// <summary>
    /// Load preset for client.
    /// </summary>
    /// <param name="networkConnection"></param>
    [ServerRpc(RequireOwnership = false)]
    public void LoadPreset(NetworkConnection networkConnection)
    {
        Debug.Log($"Loading assets for player ID: {networkConnection.ClientId}");

        var player = GameManager.Instance.GetPlayerByConnection(networkConnection.ClientId);
        if (player.Faction is null) return;

        LoadPresetOnClient(networkConnection, player.Faction.Presets);
    }

    // Call this to load all presets for faction to Database.
    [ObserversRpc]
    private void LoadPresetOnClient(NetworkConnection networkConnection, List<Preset> presetList)
    {
        if (!networkConnection.IsLocalClient) return;
        assetDatabase.AddAllPresets(presetList);
    }

    [Obsolete("Generate testing preset on server start")]
    public void TestPreset(Preset preset)
    {
        string json = JsonUtility.ToJson(preset);
        if (!Directory.Exists(Application.streamingAssetsPath + "/Presets/"))
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Presets/");
        string filePath = Path.Combine(Application.streamingAssetsPath, "Presets", preset.presetName + ".json");
        File.WriteAllText(filePath, json);
    }

    //YIRO-TODO add faction
    [ServerRpc(RequireOwnership = false)]
    public void SavePreset(NetworkConnection networkConnection, Preset tankPreset)
    {
        string json = JsonUtility.ToJson(tankPreset);
        if (!Directory.Exists(Application.streamingAssetsPath + "/Presets/"))
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Presets/");
        string filePath = Path.Combine(Application.streamingAssetsPath, "Presets", tankPreset.presetName + ".json");
        File.WriteAllText(filePath, json);

        Debug.Log($"{tankPreset} successfully saved on SERVER by player ID: {networkConnection.ClientId}");
        SavePresetResponse(networkConnection);
    }

    [TargetRpc]
    public void SavePresetResponse(NetworkConnection networkConnection = default)
    {
        Debug.Log($"Preset successfully saved on SERVER.");
    }

    private static Preset Deserialize(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Preset deserialized = JsonUtility.FromJson<Preset>(json);
            return deserialized;
        }
        else
        {
            Debug.LogError("File not found: " + path);
            return null;
        }
    }
}