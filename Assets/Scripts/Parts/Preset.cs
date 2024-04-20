using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class Preset {

    public string presetName = "Default";
    [Obsolete] public string hull;
    [Obsolete] public string turret;
    public int faction = 0;
    public int color = 0;
    public string tankName = "Default-tank";
    public MainPart mainPart = CreateDefaultPart();

    public Preset() {
        //Do not delete, used for NETWORKING
    }

    public Preset(string presetName, string hull, string turret) {
        this.presetName = presetName;
        this.hull = hull;
        this.turret = turret;
        this.faction = 0;
    }

    public Preset(string presetName, string hull, string turret, int color, string tankName) {
        this.presetName = presetName;
        this.hull = hull;
        this.turret = turret;
        this.faction = 0;
        this.color = color;
        this.tankName = tankName;
    }

    public override string ToString() {
        return $"Preset name: {presetName} " +
            $"| Hull: {hull} " +
            $"| Turret: {turret} " +
            $"| Faction: {faction}";
    }

    [Serializable]
    public class TankData {
        public int key;
        public string prefabName;
        public string partName;
        public TankPositions tankPosition;
        public Vector3 partPosition;

        public TankData() {
        }

        public TankData(int key, string prefabName, string partName, TankPositions tankPosition, Vector3 partPosition) {
            this.key = key;
            this.prefabName = prefabName;
            this.partName = partName;
            this.tankPosition = tankPosition;
            this.partPosition = partPosition;
        }
    }

    [Serializable]
    public class MainPart {
        public TankData mainData;
        public TankPart[] parts;

        public MainPart() {
        }

        public MainPart(TankData mainData, TankPart[] parts) {
            this.mainData = mainData;
            this.parts = parts;
        }
    }

    [Serializable]
    public class TankPart {

        public TankData partData;

        public TankPart() {
        }

        public TankPart(TankData partData) {
            this.partData = partData;
        }
    }

    [Obsolete("Just for testing purpose.")]
    private static MainPart CreateDefaultPart() {
        TankPart[] parts = new TankPart[] {
            new TankPart(new TankData(1, "cannon", "cannon_1", TankPositions.GUNNER, Vector3.zero)),
            new TankPart(new TankData(2, "cannon", "cannon_2", TankPositions.GUNNER, Vector3.zero))
        };

        return new MainPart(new TankData(0, "hull", "main", TankPositions.DRIVER, Vector3.zero), parts);
    }

}
