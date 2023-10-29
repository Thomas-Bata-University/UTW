using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class HullAssembly : NetworkBehaviour {
    public GameObject hullPrefab;
    public GameObject turretPrefab;
    private GameObject _assetDb;

    public Preset SelectedPreset;

    public GameObject _turretInst;
    public GameObject _hullInst;

    private void Start() {
        _assetDb = GameObject.Find("AssetDatabase");
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        SelectedPreset = dbComponent.SelectedPreset;
        Debug.Log("Client has selected preset: " + SelectedPreset.presetName);

        AssemblyServerRpc(SelectedPreset.hull, SelectedPreset.turret);
    }


    [ServerRpc(RequireOwnership = false)]
    private void AssemblyServerRpc(string hull, string turret, NetworkConnection netCon = null) {
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        hullPrefab = dbComponent.hulls.Find(x => x.name == hull);
        turretPrefab = dbComponent.turrets.Find(x => x.name == turret);

        GameObject hullInst = Instantiate(hullPrefab, transform.position, Quaternion.identity);
        Spawn(hullInst, netCon);
        hullInst.transform.SetParent(transform);

        Transform turretGO = hullInst.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        Transform mainbody = hullInst.transform.Find("MainBody");

        GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
        Spawn(turretInst, netCon);
        turretInst.transform.SetParent(hullInst.transform);
    }

}
