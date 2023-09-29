using UnityEngine;
using ChobiAssets.PTM;
using Den.Tools;
using Unity.Netcode;

public class HullAssembly : NetworkBehaviour
{
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
    public override void OnNetworkSpawn()
    {
        _assetDb = GameObject.Find("AssetDatabase");
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        SelectedPreset = dbComponent.SelectedPreset;
        Debug.Log("Client has selected preset: " + SelectedPreset.presetName);

        AssemblyServerRpc(NetworkManager.Singleton.LocalClientId, SelectedPreset.hull, SelectedPreset.turret);
    }

    
    [ServerRpc]
    private void AssemblyServerRpc(ulong netID, string hull, string turret)
    {
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        hullPrefab = dbComponent.hulls.Find(x => x.name == hull);
        turretPrefab = dbComponent.turrets.Find(x => x.name == turret);

        GameObject hullInst = Instantiate(hullPrefab, transform.position, Quaternion.identity);
        hullInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID,  true);
        hullInst.transform.SetParent(transform);

        Transform turretGO = hullInst.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        Transform mainbody = hullInst.transform.Find("MainBody");

        GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
        turretInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID,  true);
        turretInst.transform.SetParent(hullInst.transform);

        AssemblyClientRPC(hullInst.GetComponent<NetworkObject>().NetworkObjectId,
            turretInst.GetComponent<NetworkObject>().NetworkObjectId, hullInst.name);
    }

    [ClientRpc]
    private void AssemblyClientRPC(ulong hullID, ulong turretID, string parentObjectName)
    {

        NetworkObject hullObj = NetworkManager.SpawnManager.SpawnedObjects[hullID];
        NetworkObject turretObj = NetworkManager.SpawnManager.SpawnedObjects[turretID];
        _turretInst = turretObj.gameObject;
        _hullInst = hullObj.gameObject;

        _turretInst.transform.GetComponentInChildren<Cannon_Fire_CS>().OnSpawnRPC();
        _turretInst.transform.GetComponentInChildren<Cannon_Vertical_CS>().OnSpawnRPC();
        _turretInst.transform.GetComponentInChildren<Turret_Horizontal_CS>().OnSpawnRPC();

        _hullInst.transform.GetComponentInChildren<Aiming_Control_CS>().OnSpawnRPC();
    }
}
 