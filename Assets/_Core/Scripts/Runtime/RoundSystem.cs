using System;
using System.Collections;
using System.Collections.Generic;
using Factions;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoundSystem : NetworkBehaviour
{
    
    public GameObject deathScreen;
    public GameObject winningScreen;
    
    private Coroutine _gameEnding;
    private Coroutine _playerDisconnecting;
    public Dictionary<int, int> _playerParties = new();
    
    public TMP_Text _waitForEndText;
    public TMP_Text _waitForWinText;
    [SerializeField] private float remainingTime;

    public void Awake()
    {
        if (InstanceFinder.IsServer)
        {
            UTW.SceneManager.OnClientJoinLobby += OnClientJoinLobby;
        }
    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60F);
        int seconds = Mathf.FloorToInt(remainingTime - minutes * 60);
        _waitForWinText.text = "Game will end in:" + string.Format("{0:0}:{1:00}", minutes, seconds);
        _waitForEndText.text = "You will be returned to Main manu in:" + string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    #region Connections

    [ServerRpc(RequireOwnership = false)]
    public void OnClientDisconnectFromLobby(NetworkConnection conn)
    {
        OnClientDisconnectLobby(conn);
    }
    public void OnClientDisconnectLobby(NetworkConnection conn)
    {
        if (InstanceFinder.IsServer)
        {
            Faction clientFaction = GameManager.Instance.GetFactionByConnection(conn);
            if (_playerParties.ContainsKey(clientFaction.Id))
            {
                _playerParties[clientFaction.Id]--;
                Debug.Log("Player parties count: " + _playerParties.Count);
                if (_playerParties[clientFaction.Id] == 0)
                {
                    _playerParties.Remove(clientFaction.Id);
                    Debug.Log("Player party removed from playerParties");
                }
            }
            else
            {
                Debug.Log("Player not found in playerParties");
            }
        }
    }

    private void OnClientJoinLobby(NetworkConnection conn)
    { 
        if (InstanceFinder.IsServer)
        {
            Faction clientFaction = GameManager.Instance.GetFactionByConnection(conn);
            if (_playerParties.ContainsKey(clientFaction.Id))
            {
                _playerParties[clientFaction.Id]++;
                Debug.Log("Player found in playerParties");
            }
            else
            {
                _playerParties[clientFaction.Id] = 1;
                Debug.Log("Player not found in playerParties and making new party");
            }
        }
    }

    #endregion

    #region TankDestroyed

    public void OnTankDestroyed(NetworkConnection conn)
    {
        Faction clientFaction = GameManager.Instance.GetFactionByConnection(conn);
        if (InstanceFinder.IsServer)
        {
            if (_playerParties.ContainsKey(clientFaction.Id))
            {
                _playerParties[clientFaction.Id]--;
                ShowEndingScreen(conn);
                if (_playerParties[clientFaction.Id] == 0)
                {
                    _playerParties.Remove(clientFaction.Id);
                    Debug.Log("Player party removed from playerParties");
                }
                if (_playerParties.Count == 1)
                {
                    Debug.Log("Game ending");
                    GameEndsClientRpc();
                }
                else
                {
                    Debug.Log("Player cannot be removed from playerParties and or player party cannot be removed from playerParties");
                }
            }
        }
    }
    
    public void OnTankDestroyed(NetworkConnection[] conns)
    {
        foreach (var conn in conns)
        {
            OnTankDestroyed(conn);
        }
    }

    #endregion
    

    #region DisconnectDeadPlayer
    
    [TargetRpc]
    void ShowEndingScreen(NetworkConnection conn)
    {
        StartCoroutine(WaitBeforeDisconnectPlayer(conn));
    }

    private IEnumerator WaitBeforeDisconnectPlayer(NetworkConnection conn)
    {
        deathScreen.SetActive(true);
        yield return new WaitForSeconds(5);
        UTW.SceneManager.Instance.Disconnect(conn);
    }
    
    #endregion

    #region DisconnectWinners

    [ObserversRpc]
    void GameEndsClientRpc()
    {
        StartCoroutine(WaitBeforeEndRound());
    }
    private IEnumerator WaitBeforeEndRound()
    {
        winningScreen.SetActive(true);
        yield return new WaitForSeconds(5);
        UTW.SceneManager.Instance.Disconnect(LocalConnection);
    }

    #endregion
    
    public void OnDestroy()
    {
        if (InstanceFinder.IsServer)
        {
            UTW.SceneManager.OnClientJoinLobby -= OnClientJoinLobby;
        }
    }
}