using FishNet.Connection;
using System;

public readonly struct ChatMessage
{
    public readonly NetworkConnection Connection;

    public readonly DateTime Timestamp;
    public readonly string Sender;
    public readonly string Content;

    public ChatMessage(NetworkConnection connection, DateTime timestamp, string sender, string content)
    {
        Connection = connection;
        Timestamp = timestamp;
        Sender = sender;
        Content = content;
    }
}
