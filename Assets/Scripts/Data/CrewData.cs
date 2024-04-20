using FishNet.Connection;
using FishNet.Object;

public class CrewData {

    public NetworkConnection conn;
    public TankPositions tankPosition;
    public bool empty;
    public bool swapRequest;
    public NetworkObject tankPart;
    public int tankPartChild;

    public CrewData() {

    }

    public CrewData(TankPositions tankPosition) {
        this.tankPosition = tankPosition;
        empty = true;
    }

}
