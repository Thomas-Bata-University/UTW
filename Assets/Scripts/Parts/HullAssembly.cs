using UnityEngine;
using ChobiAssets.PTM;
using Unity.Netcode;

public class HullAssembly : NetworkBehaviour
{
    public GameObject hullPrefab;
    public GameObject turretPrefab;
    private GameObject assetDb;

    public Preset selectedPreset;

    public GameObject _turretInst;
    public GameObject _hullInst;

    void Start()
    {
        assetDb = GameObject.Find("AssetDatabase");
        Database dbComponent = (Database)assetDb.GetComponent(typeof(Database));

        selectedPreset = dbComponent.SelectedPreset;
        Debug.Log("Client has selected preset: " + selectedPreset.presetName);

        AssemblyServerRpc(NetworkManager.Singleton.LocalClientId, selectedPreset.hull, selectedPreset.turret);
    }

    [ServerRpc]
    public void AssemblyServerRpc(ulong netID, string hull, string turret)
    {
        Database dbComponent = (Database)assetDb.GetComponent(typeof(Database));

        hullPrefab = dbComponent.hulls.Find(x => x.name == hull);
        turretPrefab = dbComponent.turrets.Find(x => x.name == turret);

        GameObject hullInst = Instantiate(hullPrefab, transform.position, Quaternion.identity, transform);

        hullInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID,  true);

        Transform turretGO = hullInst.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        Transform mainbody = hullInst.transform.Find("MainBody");

        GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
        turretInst.GetComponent<NetworkObject>().SpawnWithOwnership(netID,  true);

        AssemblyClientRPC(hullInst.GetComponent<NetworkObject>().NetworkObjectId, turretInst.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ClientRpc]
    void AssemblyClientRPC(ulong hullID, ulong turretID)
    {
        NetworkObject hullObj = NetworkManager.SpawnManager.SpawnedObjects[hullID];
        NetworkObject turretObj = NetworkManager.SpawnManager.SpawnedObjects[turretID];

        _turretInst = turretObj.gameObject;
        _hullInst = hullObj.gameObject;
        Transform mainbody = hullObj.transform.Find("MainBody");
        _turretInst.transform.SetParent(mainbody);


        _turretInst.transform.GetComponentInChildren<Cannon_Fire_CS>().OnSpawnRPC();
        _turretInst.transform.GetComponentInChildren<Cannon_Vertical_CS>().OnSpawnRPC();
        _turretInst.transform.GetComponentInChildren<Turret_Horizontal_CS>().OnSpawnRPC();

        _hullInst.transform.GetComponentInChildren<Aiming_Control_CS>().OnSpawnRPC();
    }
}
 