using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class VehicleManager : NetworkBehaviour
{
    public static VehicleManager Instance;
    [SerializeField] private GameObject hullPrefab;
    [SerializeField] private GameObject turretPrefab;
    private Database assetDatabase;
    public GameObject testPrefab;


    private Dictionary<NetworkConnection, Preset> playerPresets = new Dictionary<NetworkConnection, Preset>();
    private Dictionary<NetworkConnection, GameObject> playerHulls = new Dictionary<NetworkConnection, GameObject>();
    private Dictionary<NetworkConnection, GameObject> playerTurrets = new Dictionary<NetworkConnection, GameObject>();

    // Start is called before the first frame update

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        assetDatabase = FindObjectOfType<Database>();
        
    }

    private void Update()
    {
//        if (Input.GetKeyDown(KeyCode.F3))
  //      {
    //        SpawnVehicle(InstanceFinder.ClientManager.Connection, assetDatabase.SelectedPreset, this.transform);
      //  }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnVehicle(NetworkConnection networkConnection, Preset preset, Transform position)
    {
        GameObject hullInst = Instantiate(hullPrefab, position.position, Quaternion.identity);
        Spawn(hullInst, networkConnection);
        hullInst.transform.SetParent(transform);

        Transform turretGO = hullInst.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        Transform mainbody = hullInst.transform.Find("MainBody");

        GameObject turretInst = Instantiate(turretPrefab, turretMount, Quaternion.identity, mainbody);
        Spawn(turretInst, networkConnection);
        turretInst.transform.SetParent(hullInst.transform);
        Debug.Log("Succesfully spawned a tank.");
    }

    [TargetRpc]
    void RpcUpdatePosition(NetworkConnection networkConnection, Transform newPosition)
    {
        if (playerHulls[networkConnection] != null && playerTurrets[networkConnection] != null)
        {
            playerHulls[networkConnection].transform.position = newPosition.position;

            Transform turretM = playerHulls[networkConnection].transform.Find("TurretMount");
            if (turretM != null)
            {
                Vector3 turretMountV = turretM.position;

                Transform mainBody = playerHulls[networkConnection].transform.Find("MainBody");
                if (mainBody != null)
                {
                    playerTurrets[networkConnection].transform.position = turretMountV;
                    playerTurrets[networkConnection].transform.rotation = mainBody.rotation;
                    playerTurrets[networkConnection].transform.SetParent(playerHulls[networkConnection].transform);
                }
            }
        }
    }
}
