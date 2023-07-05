public struct PlayerData
{
    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set; }
    public string Preset { get; private set; }

    public PlayerData(string playerName, ulong clientId, string preset)
    {
        PlayerName = playerName;
        ClientId = clientId;
        Preset = preset;
    }
}
