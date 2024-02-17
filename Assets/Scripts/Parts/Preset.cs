using System;

[Serializable]
public class Preset {

    public string presetName;
    public string hull;
    public string turret;
    public int faction;

    public Preset() {
        //Do not delete, used for NETWORKING
    }

    public Preset(string presetName, string hull, string turret) {
        this.presetName = presetName;
        this.hull = hull;
        this.turret = turret;
        this.faction = 0;
    }

    public override string ToString() {
        return $"Preset name: {presetName} " +
            $"| Hull: {hull} " +
            $"| Turret: {turret} " +
            $"| Faction: {faction}";
    }

}
