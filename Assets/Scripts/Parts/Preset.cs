using System;
using Unity.Netcode;

[Serializable]
public class Preset : INetworkSerializable {

    private string presetName;
    public string PresetName { get { return presetName; } set { presetName = value; } }

    private string hull;
    public string Hull { get { return hull; } set { hull = value; } }

    private string turret;
    public string Turret { get { return turret; } set { turret = value; } }

    private string faction;
    public string Faction { get { return faction; } set { faction = value; } }

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
