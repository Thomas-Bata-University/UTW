using System;
using Factions;
using JetBrains.Annotations;

[Serializable]
public class PlayerData
{
    public string PlayerName;

    public int ClientConnection;

    public string Preset;
    public int FactionId = 0;

    [CanBeNull]
    public Faction Faction { get; set; }

    [NonSerialized]
    public string sceneName;

    public PlayerData()
    {
    }

    public PlayerData(string playerName, int clientConnection, string preset)
    {
        PlayerName = playerName;
        ClientConnection = clientConnection;
        Preset = preset;
    }
}