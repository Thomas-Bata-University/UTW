using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class RoundSystem : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject playerPrefab;
    public GameObject OnGameEnd;

    public int playersAlive;
    

    private List<Transform> remainingSpawnPoints;
    private List<ulong> loadingClients = new List<ulong>();
/*
    public override void OnNetworkSpawn()
    {
        
        if(IsServer)
        {
            foreach(NetworkClient networkClient in NetworkManager.Singleton.ConnectedClientsList)
            {
                loadingClients.Add(networkClient.ClientId);
            }
            remainingSpawnPoints = new List<Transform>(spawnPoints);
        }
        if(IsClient)
        {
            ClientIsReadyServerRpc();
        }
        
    }
    */
    private void Start()
    {
        ClientIsReadyServerRpc();
    }

[ServerRpc(RequireOwnership = false)]
    void ClientIsReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //if (!loadingClients.Contains(serverRpcParams.Receive.SenderClientId)) { return; }

        SpawnPlayer(serverRpcParams.Receive.SenderClientId);
        //loadingClients.Remove(serverRpcParams.Receive.SenderClientId);
    }

    void SpawnPlayer(ulong clientId)
    {
        var spawnPointIndex = Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[spawnPointIndex];
        //remainingSpawnPoints.RemoveAt(spawnPointIndex);
        var playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,  true);

        playersAlive++;
    }

    public void PlayerDied()
    {
        playersAlive--;
        if(playersAlive < 2)
        {
            GameEndsClientRpc();
            Debug.Log("Game ends!");

            StartCoroutine("WaitBeforeEndRound");
        }
    }
    [ClientRpc]
    void GameEndsClientRpc()
    {
        OnGameEnd.SetActive(true);
    }
/*
    IEnumerator WaitBeforeEndRound()
    {
        yield return new WaitForSeconds(7);
        ServerGameNetPortal.Instance.EndRound();
    }
    */
}
