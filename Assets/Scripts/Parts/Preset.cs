using System;
using Enum;
using UnityEngine;

[Serializable]
public class Preset {

    public string presetName = "Default";
    [Obsolete] public string hull;
    [Obsolete] public string turret;
    public int faction = 0;
    public int color = 0;
    public string tankName = "Default-tank";
    public MainPart mainPart;

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

    [Serializable]
    public class TankData {

        public int key;
        public string databaseKey;
        public string partName;
        public TankPositions tankPosition;
        public Vector3 partPosition;
        public PartType partType;

        public TankData() {
        }

        public TankData(int key, string databaseKey, string partName, TankPositions tankPosition, Vector3 partPosition,
            PartType partType) {
            this.key = key;
            this.databaseKey = databaseKey;
            this.partName = partName;
            this.tankPosition = tankPosition;
            this.partPosition = partPosition;
            this.partType = partType;
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

    [Obsolete("Default testing preset")]
    public static Preset CreateDefaultPreset() {
        TankPart[] parts = {
            new TankPart(new TankData(1, "CromwellTurret", "turret", TankPositions.GUNNER,
                Vector3.zero, PartType.TURRET)),
            new TankPart(new TankData(2, "visor", "visor", TankPositions.OBSERVER,
                new Vector3(0, -0.091f, 0.11f), PartType.OTHER))
        };

        Preset preset = new Preset();
        preset.presetName = "Default";
        preset.faction = 1;
        preset.color = 0;
        preset.tankName = "Default-tank";
        preset.mainPart = new MainPart(new TankData(0, "CromwellHull", "main", TankPositions.DRIVER,
            new Vector3(0, 1.1f, 0), PartType.HULL), parts);

        return preset;
    }

}