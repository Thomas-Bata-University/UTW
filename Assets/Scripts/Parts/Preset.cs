using System;
using Unity.Netcode;

[Serializable]
public class Preset : INetworkSerializable {

    public string presetName;
    public string hull;
    public string turret;
    public string faction;

    public Preset() {
        //Do not delete, used for NETWORKING
    }

    public Preset(string presetName, string hull, string turret) {
        this.presetName = presetName;
        this.hull = hull;
        this.turret = turret;
        this.faction = "faction";
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref presetName);
        serializer.SerializeValue(ref hull);
        serializer.SerializeValue(ref turret);
        serializer.SerializeValue(ref faction);
    }

    public override string ToString() {
        return $"Preset name: {presetName}" +
            $" hull: {hull}" +
            $" turret: {turret}" +
            $"faction: {faction}";
    }

}
