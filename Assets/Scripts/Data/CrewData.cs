using FishNet.Connection;
using UnityEngine;

public class CrewData {

    public NetworkConnection conn;
    public TankPositions tankPosition;
    public bool empty;

    public CrewData() {

    }

    public CrewData(TankPositions tankPosition) {
        this.tankPosition = tankPosition;
        empty = true;
    }

}
