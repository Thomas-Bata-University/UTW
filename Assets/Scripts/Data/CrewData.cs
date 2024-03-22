using FishNet.Connection;

public class CrewData {

    public NetworkConnection conn;
    public TankPositions tankPosition;
    public bool empty;
    public bool swapRequest;

    public CrewData() {

    }

    public CrewData(TankPositions tankPosition) {
        this.tankPosition = tankPosition;
        empty = true;
    }

}
