using System;
using Factions;
using JetBrains.Annotations;

[Serializable]
public class PlayerData
{
    public string PlayerName;
    public int FactionId;
    public int ClientConnectionId;
    [CanBeNull] public Faction Faction { get; set; }
    [NonSerialized] public string SceneName;

    public PlayerData()
    {
    }

    public PlayerData(string playerName, int factionId)
    {
        PlayerName = playerName;
        FactionId = factionId;
        ClientConnectionId = ConnectionCodes.OFFLINE_CODE;
    }
}