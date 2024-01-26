using System;
using Factions;
using JetBrains.Annotations;

[Serializable]
public class PlayerData
{
    public string PlayerName;
    public ulong ClientId;
    public string Preset;
    public int FactionId = 0;

    [CanBeNull] public Faction Faction { get; set; }

    public PlayerData(string playerName, ulong clientId, string preset)
    {
        PlayerName = playerName;
        ClientId = clientId;
        Preset = preset;
    }
}