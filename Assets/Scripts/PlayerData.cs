using System;
using Factions;
using JetBrains.Annotations;

public record PlayerData
{
    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set; }
    public string Preset { get; private set; }
    
    public Guid FactionId { get; set; }
    
    [CanBeNull] public Faction Faction { get; set; }

    public PlayerData(string playerName, ulong clientId, string preset)
    {
        PlayerName = playerName;
        ClientId = clientId;
        Preset = preset;
    }
}
