using System;
using System.Collections.Generic;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public struct UserBroadcast : IBroadcast
{
    public string Username;
    public List<string> Hashes;
}

public struct ResponseBroadcast : IBroadcast
{
    public bool Passed;
    public string Message;
}

public class Authenticator : HostAuthenticator
{
    /// <summary>
    /// Called when authenticator has concluded a result for a connection. Boolean is true if authentication passed, false if failed.
    /// Server listens for this event automatically.
    /// </summary>
    public override event Action<NetworkConnection, bool> OnAuthenticationResult;

    [SerializeField] private string userNameInput = "HelloWorld";

    public override void InitializeOnce(NetworkManager networkManager)
    {
        base.InitializeOnce(networkManager);

        NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        NetworkManager.ServerManager.RegisterBroadcast<UserBroadcast>(OnUserBroadcast, false);
        NetworkManager.ClientManager.RegisterBroadcast<ResponseBroadcast>(OnResponseBroadcast);
    }

    /// <summary>
    /// Called when a connection state changes for the local client.
    /// </summary>
    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState != LocalConnectionState.Started)
            return;

        //Authentication was sent as host, no need to authenticate normally.
        if (AuthenticateAsHost())
            return;

        var input = GameObject.Find("UsernameInputText").GetComponent<TMP_Text>().text;

        UserBroadcast ub = new UserBroadcast()
        {
            Username = input,
            Hashes = Database.hashes
        };

        NetworkManager.ClientManager.Broadcast(ub);
    }

    /// <summary>
    /// Received on server when a client sends the password broadcast message.
    /// </summary>
    /// <param name="conn">Connection sending broadcast.</param>
    /// <param name="ub"></param>
    private void OnUserBroadcast(NetworkConnection conn, UserBroadcast ub)
    {
        var player = GameManager.Instance.CreateOrSelectPlayer(ub.Username);

        if (IsAlreadyConnected(player))
        {
            Debug.Log($"Player {player.PlayerName} is already connected!");

            SendAuthenticationResponse(conn, false, "Player with this name is already connected!");
            OnAuthenticationResult?.Invoke(conn, false);

            return;
        }

        Debug.Log($"Checking hashes for {player.PlayerName}");

        if (!UserHasRequiredHashes(ub.Hashes))
        {
            Debug.Log($"Player {player.PlayerName} has different AssetDB!");

            SendAuthenticationResponse(conn, false, "You have a different AssetDB!");
            OnAuthenticationResult?.Invoke(conn, false);

            return;
        }

        Debug.Log($"Hash check successful for {player.PlayerName}");

        player.ClientConnectionId = conn.ClientId;
        GameManager.Instance.UpdateDictionary(player.PlayerName);

        Debug.Log($"Player {player.PlayerName} authenticated!");

        SendAuthenticationResponse(conn, true, "Authentication complete.");
        OnAuthenticationResult?.Invoke(conn, true);
    }

    private bool IsAlreadyConnected(PlayerData player)
    {
        if (GameManager.Instance.GetPlayerByName(player.PlayerName).ClientConnectionId == -2)
            return false;

        return true;
    }

    private bool UserHasRequiredHashes(List<string> hashes)
    {
        HashSet<string> userHashes = new HashSet<string>(hashes);
        HashSet<string> serverHashes = new HashSet<string>(Database.hashes);

        return userHashes.IsSupersetOf(serverHashes);
    }

    /// <summary>
    /// Received on client after server sends an authentication response.
    /// </summary>
    /// <param name="rb"></param>
    private void OnResponseBroadcast(ResponseBroadcast rb)
    {
        string result = rb.Message;
        NetworkManager.Log(result);
    }

    /// <summary>
    /// Sends an authentication result to a connection.
    /// </summary>
    private void SendAuthenticationResponse(NetworkConnection conn, bool authenticated, string message)
    {
        ResponseBroadcast rb = new ResponseBroadcast()
        {
            Passed = authenticated,
            Message = message
        };

        NetworkManager.ServerManager.Broadcast(conn, rb, false);
    }

    /// <summary>
    /// Called after handling a host authentication result.
    /// </summary>
    /// <param name="conn">Connection authenticating.</param>
    /// <param name="authenticated">True if authentication passed.</param>
    protected override void OnHostAuthenticationResult(NetworkConnection conn, bool authenticated)
    {
        SendAuthenticationResponse(conn, authenticated, string.Empty);
        OnAuthenticationResult?.Invoke(conn, authenticated);
    }
}