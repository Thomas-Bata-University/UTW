using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoundSystem : NetworkBehaviour
{
    public Transform[] spawnPoints;
    public GameObject playerPrefab;
    public GameObject OnGameEnd;

    public int playersAlive;
    

    private List<Transform> remainingSpawnPoints;
    //private List<ulong> loadingClients = new List<ulong>();
/*
    public override void OnNetworkSpawn()
    {
        
        //if(IsServer)
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Called OnStartClient");
        SpawnPlayer();
    }

    /*
    [ServerRpc(RequireOwnership = false)]
        void ClientIsReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            //if (!loadingClients.Contains(serverRpcParams.Receive.SenderClientId)) { return; }

            SpawnPlayer(serverRpcParams.Receive.SenderClientId);
            //loadingClients.Remove(serverRpcParams.Receive.SenderClientId);
        }
    */
    /*
    [ServerRpc(RequireOwnership = false)]
    private void ClientIsReadyServerRpc()
    {
        Debug.Log("Called ClientIsReadyServerRpc");
        SpawnPlayer(LocalConnection);
    }
*/
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection netCon = null)
    {
        var spawnPointIndex = Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[spawnPointIndex];
        remainingSpawnPoints.RemoveAt(spawnPointIndex);
        var playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        //playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,  true);
        Spawn(playerInstance, netCon);
        Debug.Log("Player has been spawned");

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
    [ObserversRpc]
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
