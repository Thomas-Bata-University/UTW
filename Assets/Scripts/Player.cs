using System;
using Factions;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using JetBrains.Annotations;
using Managers;
using UnityEngine;

public sealed class Player : NetworkBehaviour
{
    public static Player Instance { get; private set; }

    [SyncVar] public string username;

    [SyncVar] public Guid FactionId;

    [SyncVar] public bool isReady;

    [SyncVar] [CanBeNull] private Faction _faction;


    public override void OnStartServer()
    {
        base.OnStartServer();

        GameManager.Instance.players.Add(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.Instance.players.Remove(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner) return;

        Instance = this;

        /*UIManager.Instance.Initialize();

            UIManager.Instance.Show<LobbyView>();*/
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ServerSetIsReady(!isReady);        
            RetrieveAssetsFromServer();
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void ServerSetIsReady(bool value)
    {
        isReady = value;
    }
    

    private void RetrieveAssetsFromServer()
    {
        _faction = FactionsManager.Instance.GetFactionById(FactionId);
    }
}