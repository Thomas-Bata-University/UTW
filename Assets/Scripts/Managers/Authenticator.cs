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

    public struct Host1UsernameBroadcast : IBroadcast
    {
        public string Password;
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

        public TextMeshPro input;

        [SerializeField] private string userNameInput = "HelloWorld";

        public override void InitializeOnce(NetworkManager networkManager)
        {
            base.InitializeOnce(networkManager);

            input = GameObject.Find("UsernameInputText").GetComponent<TextMeshPro>();

            //Listen for connection state change as client.
            base.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            //Listen for broadcast from client. Be sure to set requireAuthentication to false.
            base.NetworkManager.ServerManager.RegisterBroadcast<UsernameBroadcast>(OnUsernameBroadcast, false);
            //Listen to response from server.
            base.NetworkManager.ClientManager.RegisterBroadcast<ResponseBroadcast>(OnResponseBroadcast);
        }

        /// <summary>
        /// Called when a connection state changes for the local client.
        /// </summary>
        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
        {
            /* If anything but the started state then exit early.
             * Only try to authenticate on started state. The server
             * doesn't have to send an authentication request before client
             * can authenticate, that is entirely optional and up to you. In this
             * example the client tries to authenticate soon as they connect. */
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
            /* If client is already authenticated this could be an attack. Connections
             * are removed when a client disconnects so there is no reason they should
             * already be considered authenticated. */
            if (conn.Authenticated)
            {
                conn.Disconnect(true);
                return;
            }

            GameManager.Instance.CreateOrSelectPlayer(pb.Username);

            bool correctPassword = (pb.Username == string.Empty);
            SendAuthenticationResponse(conn, correctPassword);
            /* Invoke result. This is handled internally to complete the connection or kick client.
             * It's important to call this after sending the broadcast so that the broadcast
             * makes it out to the client before the kick. */
            OnAuthenticationResult?.Invoke(conn, correctPassword);
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
            /* Tell client if they authenticated or not. This is
             * entirely optional but does demonstrate that you can send
             * broadcasts to client on pass or fail. */
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