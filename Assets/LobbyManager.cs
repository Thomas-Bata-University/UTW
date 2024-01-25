using FishNet;
using FishNet.Connection;
using FishNet.Managing.Object;
using FishNet.Object;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class LobbyManager : NetworkBehaviour
{

    public GameObject playerPrefab;
    public Button SpawnButton;
    public int LobbyManagerId;
    Transform playerPosition;
    Preset playerPreset;
    // public delegate void ClientJoinLobby(NetworkConnection networkConnection);
    // public event ClientJoinLobby OnClientJoinLobby;
    public delegate void SpawnPointChange(NetworkConnection networkConnection, Preset preset, Transform position);

    //  
    SceneManager sceneManager = new();
    public event SpawnPointChange OnSpawnPointChange;
    NetworkConnection pripojeni;

    //    private Dictionary<int,LobbyData> LobbyDataList = new Dictionary<int,LobbyData>();
    // Start is called before the first frame update
    void Start()
    {
        sceneManager.OnClientJoinLobby += NewClient;
        SpawnMap();
    }
    void NewClient(NetworkConnection networkConnection)
    {
        pripojeni = networkConnection;
        GameObject tank = Instantiate(playerPrefab);
        InstanceFinder.ServerManager.Spawn(tank, networkConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateTank()
    {
        OnSpawnPointChange.Invoke(pripojeni, playerPreset, playerPosition);
    }

    void UpdateSpawn(Transform position)
    {
        playerPosition = position;
        UpdateTank();
    }

    void SpawnMap()
    {
        Vector3 pozice = new Vector3(0, 0, 0);
        string mapname = "Greenmap";   //zde místo Greenmap budeme naèítat jméno mapy, kterou chceme zobrazit
        Instantiate(Resources.Load("maps/" + mapname), pozice, Quaternion.identity);
        GameObject place;
        Boolean done = false;
        int i = 1;
        while (!done)
        {
            place = GameObject.Find("Spawn" + i.ToString());
            if (place != null)
            {
                Button button = Instantiate(SpawnButton, place.transform);
                button.onClick.AddListener(() => UpdateSpawn(place.transform));
                button.name = i.ToString();
                i++;
            }
            else
            {
                done = true;
            }
        }
    }

    /*
        public void SetPos(FishNet.Connection.NetworkConnection networkConnection, Transform position)
        {
            bool exists = LobbyDataList.Any(x => x.Key == networkConnection.ClientId);
            if (!exists)
            {
                LobbyData data = new LobbyData(position);
                LobbyDataList.Add(networkConnection.ClientId,data);
            }
        }
        public void Disconnect(FishNet.Connection.NetworkConnection networkConnection)
        {
            LobbyDataList.Remove(networkConnection.ClientId);
        }
        public void SpawnCam()
        {
            var connections = InstanceFinder.SceneManager.SceneConnections.First(x=> x.Key.handle == handle).Value;
            foreach(var connection in connections)
            {
                Vector3 pozice = LobbyDataList.First(x => x.Key == connection.ClientId).Value.spawnpos.position;

                GameObject hullInst = Instantiate(playerCam, pozice, Quaternion.identity);
                Spawn(hullInst, connection);
                hullInst.transform.SetParent(transform);
            }
        }

    }

    public class LobbyData
    {
        public Transform spawnpos;

        public LobbyData(Transform spawnpos)
        {
            this.spawnpos = spawnpos;
        }
    }
    */

}