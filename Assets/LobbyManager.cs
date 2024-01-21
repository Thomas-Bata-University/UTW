using FishNet;
using FishNet.Connection;
using FishNet.Managing.Object;
using FishNet.Object;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyManager : NetworkBehaviour
{

    public GameObject playerCam;
    
    public int LobbyManagerId ;
    public int handle;

    private Dictionary<int,LobbyData> LobbyDataList = new Dictionary<int,LobbyData>();
    // Start is called before the first frame update
    void Start()
    {

       
        //InstanceManager.LobbyManager... registrace eventu
    }


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