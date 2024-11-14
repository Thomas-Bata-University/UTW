using System.Collections;
using System.Collections.Generic;
using ChobiAssets.PTM;
using Factions;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class RoundSystem : NetworkBehaviour
{
    public GameObject onGameEnd;
    public Animator winningScreenAnimator;

    private Coroutine _gameEnding;
    private Dictionary<Faction, int> _factions = new();


    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Called OnStartClient");
    }

    public void Awake()
    {
        if (InstanceFinder.IsServer)
        {
            UTW.SceneManager.OnClientJoinLobby += OnClientJoinLobby;
        }
    }

    private void OnClientJoinLobby(NetworkConnection conn)
    {
        if (InstanceFinder.IsServer)
        {
            Faction clientFaction = GetClientFaction(conn);

            if (_factions.ContainsKey(clientFaction))
            {
                _factions[clientFaction]++;
            }
            else
            {
                _factions[clientFaction] = 1;
            }

            UTW.SceneManager.OnClientJoinLobby -= OnClientJoinLobby;
        }
    }

    public void PlayerDied()
    {
        Faction clientFaction = GetClientFaction(NetworkManager.ClientManager.LocalConnection);
        if (InstanceFinder.IsServer)
        {
            if (_factions.ContainsKey(clientFaction))
            {
                _factions[clientFaction]--;
                if (_factions[clientFaction] == 0)
                {
                    _factions.Remove(clientFaction);
                }

                if (_factions.Count == 1)
                {
                    _gameEnding = StartCoroutine(WaitBeforeEndRound());
                }
            }
        }
    }

    private Faction GetClientFaction(NetworkConnection conn)
    {
        //return conn.clientFaction;
    }

    [ObserversRpc]
    void GameEndsClientRpc()
    {
        winningScreenAnimator
        onGameEnd.SetActive(true);
    }

    private IEnumerator WaitBeforeEndRound()
    {
        yield return new WaitForSeconds(7);
        
    }
}