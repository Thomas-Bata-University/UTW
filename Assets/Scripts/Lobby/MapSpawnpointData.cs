using FishNet.Connection;
using UnityEngine;

public class MapSpawnpointData {

    public NetworkConnection conn;
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
