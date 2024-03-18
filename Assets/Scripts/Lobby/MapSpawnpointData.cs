using FishNet.Connection;
using UnityEngine;

public class MapSpawnpointData {

    public SpawnpointState spawnpointState = SpawnpointState.EMPTY;
    public VehicleManager vehicleManager;
    public GameObject spawnpoint;
    public Transform position;
    public bool locked;

    public MapSpawnpointData() {

    }

    public MapSpawnpointData(GameObject spawnpoint, Transform position) {
        this.spawnpoint = spawnpoint;
        this.position = position;
        this.locked = false;
    }

}
