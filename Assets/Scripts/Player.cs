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
            RetrieveAssetsFromServer();
            RetrievePlayerDataFromServer();
        }
    }

    private void RetrieveAssetsFromServer()
    {
        Data.Faction = FactionsManager.Instance.GetFactionById(Data.FactionId);
    }

    private void RetrievePlayerDataFromServer()
    {
      // TODO Data = GameManager.Instance.GetPlayerDataByName(PlayerName);
    }
}