using FishNet.Connection;
using FishNet.Object;

public class CrewData
{
    public NetworkConnection conn;
    public TankPositions tankPosition;
    public bool empty;
    public bool swapRequest;
    public NetworkObject tankPart;
    public int childIndex;

    public CrewData()
    {

    }

    public CrewData(TankPositions tankPosition, NetworkObject tankPart, int childIndex)
    {
        this.tankPosition = tankPosition;
        this.tankPart = tankPart;
        this.childIndex = childIndex;
        empty = true;
    }
}
