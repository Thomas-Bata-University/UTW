using FishNet.Connection;
using UnityEngine;

public class MapSpawnpointData {

    public SpawnpointState spawnpointState = SpawnpointState.EMPTY;
    public VehicleManager vehicleManager;
    public GameObject spawnpoint;
    public Transform transform;
    public bool locked;

    public MapSpawnpointData() {

    }

    public MapSpawnpointData(GameObject spawnpoint, Transform position) {
        this.spawnpoint = spawnpoint;
        this.transform = position;
        this.locked = false;
    }

}
