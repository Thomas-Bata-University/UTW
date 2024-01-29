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
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;
//using UnityEngine.UIElements;

public class LobbyManager : NetworkBehaviour
{

    public GameObject V_Man;
    public Button SpawnButton;
    public int LobbyManagerId;
    public GameObject Canvas;
    public GameObject mapa;
    private Database assetDatabase;

    //  public delegate void ClientJoinLobby(NetworkConnection networkConnection);
    //   public static event ClientJoinLobby OnClientJoinLobby;
    public delegate void SpawnPointChange(NetworkConnection networkConnection, Transform position);
    public event SpawnPointChange OnSpawnPointChange;
    public delegate void PresetChange(NetworkConnection networkConnection, Preset preset);
    public event PresetChange OnPresetChange;
    
    
    //    private Dictionary<int,LobbyData> LobbyDataList = new Dictionary<int,LobbyData>();
    // Start is called before the first frame update
    void Start()
    {
        assetDatabase = FindObjectOfType<Database>();
        if (InstanceFinder.IsServer)
        {
        Canvas = GameObject.Find("SpawnCanvas");
        SpawnMap();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdateSpawn(NetworkConnection networkConnection, Transform position)
    {
        Debug.Log("UpdateSpawn");
        PlayerPrefs.SetFloat("coord_x", position.position.x);
        PlayerPrefs.SetFloat("coord_y", position.position.y -100);
        PlayerPrefs.SetFloat("coord_z", position.position.z);

        //       VehicleManager.Instance.RpcUpdatePosition(networkConnection, position);
        VehicleManager.Instance.SpawnVehicle(networkConnection, assetDatabase.SelectedPreset, position);

        OnSpawnPointChange.Invoke(networkConnection,position);
    }
    void PresetChanged(Preset preset)
    {
        Vector3 pozice = new Vector3(PlayerPrefs.GetFloat("coord_x"), PlayerPrefs.GetFloat("coord_y"), PlayerPrefs.GetFloat("coord_z"));
        GameObject emptyGO = new GameObject();
        Transform pos = emptyGO.transform;
        pos.position = pozice;
        Destroy(emptyGO);
        UpdatePreset(InstanceFinder.ClientManager.Connection, preset, pos);
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdatePreset(NetworkConnection networkConnection, Preset preset, Transform position)
    {
        VehicleManager.Instance.SpawnVehicle(networkConnection, preset, position);
        OnPresetChange.Invoke(networkConnection, preset);
    }
    void SpawnMap()
    {
        Vector3 pozice = new Vector3(0, 0, 0);
        var mapobject = Instantiate(mapa, pozice, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(mapobject);
        GameObject place;
        Boolean done = false;
        int i = 1;
        while (!done)
        {
            place = GameObject.Find("Spawn" + i.ToString());
            if (place != null)
            {
                Button button = Instantiate(SpawnButton, Canvas.transform);
                button.transform.position = new Vector3(place.transform.position.x, place.transform.position.y+10, place.transform.position.z);
                button.onClick.AddListener(() => UpdateSpawn(InstanceFinder.ClientManager.Connection, place.transform));
                InstanceFinder.ServerManager.Spawn(button.gameObject);
                i++;
            }
            else
            {
                done = true;
            }
        }
    }


    private void Awake()
    {
        if (InstanceFinder.IsServer)
        {
            PlayerPrefs.SetFloat("coord_x", 0);
            PlayerPrefs.SetFloat("coord_y", -100);
            PlayerPrefs.SetFloat("coord_z", 0);

            UTW.SceneManager.OnClientJoinLobby += ClientJoin;
        }
    }

    private void ClientJoin(NetworkConnection conn)
    {
        Scene scene = GetComponent<NetworkObject>().gameObject.scene;
        if (conn.Scenes.First().handle == scene.handle)
        {
            GameObject tank = Instantiate(V_Man);
            InstanceFinder.ServerManager.Spawn(tank, conn);
            PresetDropdown.OnPresetChanged += PresetChanged;
            Debug.Log($"TEST FROM LOBBY MANAGER - CLIENT: {conn.ClientId} JOINED LOBBY {scene.name}");
        }
    }

    private void OnDestroy()
    {
        if (InstanceFinder.IsServer)
        {
            UTW.SceneManager.OnClientJoinLobby -= ClientJoin;
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