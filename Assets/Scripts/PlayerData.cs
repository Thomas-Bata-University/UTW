using System;
using Factions;
using JetBrains.Annotations;

[Serializable]
public class PlayerData
{
    public string PlayerName;

    public int ClientConnectionId;

    public string Preset;
    public int FactionId = 0;

    [CanBeNull]
    public Faction Faction { get; set; }

    [NonSerialized]
    public string sceneName;

    public PlayerData()
    {
    }

    public PlayerData(string playerName, string preset)
    {
        PlayerName = playerName;
        ClientConnectionId = -2;
        Preset = preset;
    }
}