using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Chat : NetworkBehaviour
{
    [SyncObject]
    private readonly SyncList<ChatMessage> _messages = new();

    public IReadOnlyList<ChatMessage> Messages
    {
        get => _messages;
    }

    [Server]
    public void PostMessage(NetworkConnection conn, DateTime timestamp, string sender, string content)
    {
        _messages.Add(new ChatMessage(conn, timestamp, sender, content));

        Debug.Log($"{sender} sent a message: {content} ({timestamp:T})");
    }

    [Server]
    public void DeleteMessage(int messageIndex)
    {
        _messages.RemoveAt(messageIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerPostMessage(NetworkConnection conn, DateTime timestamp, string username, string content)
    {
        PostMessage(conn, timestamp, username, content);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerDeleteMessage(int messageIndex)
    {
        DeleteMessage(messageIndex);
    }
}
