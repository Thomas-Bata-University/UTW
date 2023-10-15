using ChobiAssets.PTM;
<<<<<<< HEAD
using FishNet;
using FishNet.Connection;
using FishNet.Object;
=======
using Unity.Netcode;
using UnityEngine;
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)

public class HullAssembly : NetworkBehaviour {
    public GameObject hullPrefab;
    public GameObject turretPrefab;
    private GameObject _assetDb;

    public Preset SelectedPreset;

    public GameObject _turretInst;
    public GameObject _hullInst;

    /*
     void Start()
    {
        assetDb = GameObject.Find("AssetDatabase");
        Database dbComponent = (Database)assetDb.GetComponent(typeof(Database));

        selectedPreset = dbComponent.SelectedPreset;
        Debug.Log("Client has selected preset: " + selectedPreset.presetName);

        AssemblyServerRpc(NetworkManager.Singleton.LocalClientId, selectedPreset.hull, selectedPreset.turret);
    }
*/
<<<<<<< HEAD
    private void Start()
    {
=======
    public override void OnNetworkSpawn() {
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        _assetDb = GameObject.Find("AssetDatabase");
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        SelectedPreset = dbComponent.SelectedPreset;
        Debug.Log("Client has selected preset: " + SelectedPreset.PresetName);

<<<<<<< HEAD
        AssemblyServerRpc(SelectedPreset.hull, SelectedPreset.turret);
    }


    [ServerRpc(RequireOwnership = false)]
    private void AssemblyServerRpc(string hull, string turret, NetworkConnection netCon = null)
    {
=======
        AssemblyServerRpc(NetworkManager.Singleton.LocalClientId, SelectedPreset.Hull, SelectedPreset.Turret);
    }


    [ServerRpc]
    private void AssemblyServerRpc(ulong netID, string hull, string turret) {
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        hullPrefab = dbComponent.hulls.Find(x => x.name == hull);
        turretPrefab = dbComponent.turrets.Find(x => x.name == turret);

        GameObject hullInst = Instantiate(hullPrefab, transform.position, Quaternion.identity);
<<<<<<< HEAD
        //hullInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID,  true);
        Spawn(hullInst, netCon);
=======
        hullInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID, true);
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        hullInst.transform.SetParent(transform);

        Transform turretGO = hullInst.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        Transform mainbody = hullInst.transform.Find("MainBody");

        GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
<<<<<<< HEAD
        //turretInst.GetComponent<NetworkObject>().Spawn(netID,  true);
        Spawn(turretInst, netCon);
=======
        turretInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID, true);
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        turretInst.transform.SetParent(hullInst.transform);
/*
        AssemblyClientRPC(hullInst.GetComponent<NetworkObject>().NetworkObjectId,
            turretInst.GetComponent<NetworkObject>().NetworkObjectId, hullInst.name);
            */

    }

<<<<<<< HEAD

    /*
    [ObserversRpc]
    private void AssemblyClientRPC(ulong hullID, ulong turretID, string parentObjectName)
    {
=======
    [ClientRpc]
    private void AssemblyClientRPC(ulong hullID, ulong turretID, string parentObjectName) {
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)

        NetworkObject hullObj = NetworkManager.SpawnManager.SpawnedObjects[hullID];
        NetworkObject turretObj = NetworkManager.SpawnManager.SpawnedObjects[turretID];
        _turretInst = turretObj.gameObject;
        _hullInst = hullObj.gameObject;

        _turretInst.transform.GetComponentInChildren<Cannon_Fire_CS>().OnSpawnRPC();
        _turretInst.transform.GetComponentInChildren<Cannon_Vertical_CS>().OnSpawnRPC();
        _turretInst.transform.GetComponentInChildren<Turret_Horizontal_CS>().OnSpawnRPC();

        _hullInst.transform.GetComponentInChildren<Aiming_Control_CS>().OnSpawnRPC();
    }
<<<<<<< HEAD
    */
}
=======
}
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
