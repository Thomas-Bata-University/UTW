using System;
using FishNet;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Example.Authenticating;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

namespace Managers
{
    public struct UsernameBroadcast : IBroadcast
    {
        public string Username;
    }

    public class Authenticator : HostAuthenticator
    {
        #region Public.

        /// <summary>
        /// Called when authenticator has concluded a result for a connection. Boolean is true if authentication passed, false if failed.
        /// Server listens for this event automatically.
        /// </summary>
        public override event Action<NetworkConnection, bool> OnAuthenticationResult;

        #endregion

        TMP_Text input;


        [SerializeField] private string userNameInput = "HelloWorld";

        public override void InitializeOnce(NetworkManager networkManager)
        {
            base.InitializeOnce(networkManager);

            var inputGO = GameObject.Find("UsernameInputText");
            input = inputGO.GetComponent<TMP_Text>();

            base.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            base.NetworkManager.ServerManager.RegisterBroadcast<UsernameBroadcast>(OnUsernameBroadcast, false);
            base.NetworkManager.ClientManager.RegisterBroadcast<ResponseBroadcast>(OnResponseBroadcast);
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

            UsernameBroadcast pb = new UsernameBroadcast()
            {
                Username = input.text
            };

            base.NetworkManager.ClientManager.Broadcast(pb);
        }


        /// <summary>
        /// Received on server when a client sends the password broadcast message.
        /// </summary>
        /// <param name="conn">Connection sending broadcast.</param>
        /// <param name="pb"></param>
        private void OnUsernameBroadcast(NetworkConnection conn, UsernameBroadcast pb)
        {
            var player = GameManager.Instance.CreateOrSelectPlayer(pb.Username);
            player.ClientConnection = conn.ClientId;
            Debug.Log($"Player {player.PlayerName} authenticated!");

            SendAuthenticationResponse(conn, true);

            OnAuthenticationResult?.Invoke(conn, true);
        }

        /// <summary>
        /// Received on client after server sends an authentication response.
        /// </summary>
        /// <param name="rb"></param>
        private void OnResponseBroadcast(ResponseBroadcast rb)
        {
            string result = (rb.Passed) ? "Authentication complete." : "Authenitcation failed.";
            NetworkManager.Log(result);
        }

        /// <summary>
        /// Sends an authentication result to a connection.
        /// </summary>
        private void SendAuthenticationResponse(NetworkConnection conn, bool authenticated)
        {
            ResponseBroadcast rb = new ResponseBroadcast()
            {
                Passed = authenticated
            };
            base.NetworkManager.ServerManager.Broadcast(conn, rb, false);
        }

        /// <summary>
        /// Called after handling a host authentication result.
        /// </summary>
        /// <param name="conn">Connection authenticating.</param>
        /// <param name="authenticated">True if authentication passed.</param>
        protected override void OnHostAuthenticationResult(NetworkConnection conn, bool authenticated)
        {
            SendAuthenticationResponse(conn, authenticated);
            OnAuthenticationResult?.Invoke(conn, authenticated);
        }
    }
}