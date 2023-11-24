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

    public PlayerData Data { get; set; }

    //TODO Pick from input, then spawn player! on spawn player data will be either
    //TODO created or retrieved from GameManager
    //TODO see [OnStartServer]
    public string PlayerName { get; set; }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Data = GameManager.Instance.CreateOrSelectPlayer(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.Instance.RemovePlayer(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner) return;

        Instance = this;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RetrieveAssetsFromServer();
        }
    }

    private void RetrieveAssetsFromServer()
    {
        Data.Faction = FactionsManager.Instance.GetFactionById(Data.FactionId);
    }
}