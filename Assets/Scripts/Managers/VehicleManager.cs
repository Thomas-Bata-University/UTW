using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : NetworkBehaviour
{
    public GameObject hullPrefab;
    public GameObject turretPrefab;
    private GameObject _assetDb;

    public Preset SelectedPreset;
    public GameObject _turretInst;
    public GameObject _hullInst;


    public static VehicleManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;

        _assetDb = GameObject.Find("AssetDatabase");
        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));
        //InstanceManager.LobbyManager... registrace eventu
    }

    /* void SpawnVehicle(NetworkConnection networkConnection, Preset preset, Transform position)
     {
         if (!networkConnection.IsLocalClient) return;

         Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

         hullPrefab = dbComponent.hulls.Find(x => x.name == preset.hull);
         turretPrefab = dbComponent.turrets.Find(x => x.name == preset.turret);

         GameObject hullInst = Instantiate(hullPrefab, position.position, Quaternion.identity);
         Spawn(hullInst, networkConnection);
         hullInst.transform.SetParent(transform);

         Transform turretGO = hullInst.transform.Find("TurretMount");
         Vector3 turretMount = turretGO.position;
         Transform mainbody = hullInst.transform.Find("MainBody");

         GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
         Spawn(turretInst, networkConnection);
         turretInst.transform.SetParent(hullInst.transform);

     }
    */

    public void SpawnVehicle(NetworkConnection networkConnection, Transform position)
    {
        if (!networkConnection.IsLocalClient) return;

        Database dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        hullPrefab = dbComponent.hulls.Find(x => x.name == SelectedPreset.hull);
        turretPrefab = dbComponent.turrets.Find(x => x.name == SelectedPreset.turret);

        GameObject hullInst = Instantiate(hullPrefab, position.position, Quaternion.identity);
        Spawn(hullInst, networkConnection);
        hullInst.transform.SetParent(transform);

        Transform turretGO = hullInst.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        Transform mainbody = hullInst.transform.Find("MainBody");

        GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
        Spawn(turretInst, networkConnection);
        turretInst.transform.SetParent(hullInst.transform);

    }

}
