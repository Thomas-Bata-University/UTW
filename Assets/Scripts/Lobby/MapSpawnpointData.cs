using FishNet.Connection;
using UnityEngine;
using UnityEngine.UI;

public class MapSpawnpointData {

    public NetworkConnection conn;
    public GameObject button;
    public Transform position;
    public bool locked;

    public MapSpawnpointData() {

    }

    public MapSpawnpointData(GameObject button, Transform position) {
        this.button = button;
        this.position = position;
        this.locked = false;
    }

}
