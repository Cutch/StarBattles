using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarBattles
{
    public class DB
    {
        internal struct ShipBean
        {
            internal string name;
            internal double health;
            internal double weight;
            internal double energyCapacity;
            internal double energyGeneration;
            internal double energyCost;
            internal double damage;
            internal double speed;
            internal double range;
            internal bool fireable;
            internal int classificationId;
            internal string prefabPath;
            internal string spritePath;
            internal double reloadTime;
            internal Vector3[] mountPoints;
            internal ShipBean(string name, double health, double weight,
                double energyCapacity, double energyGeneration, double energyCost,
                double damage, double speed, double range, double reloadTime, bool fireable,
                int classificationId, string prefabPath, string spritePath, Vector3[] mountPoints)
            {
                this.name = name;
                this.health = health;
                this.weight = weight;
                this.energyCapacity = energyCapacity;
                this.energyGeneration = energyGeneration;
                this.energyCost = energyCost;
                this.damage = damage;
                this.speed = speed;
                this.range = range;
                this.reloadTime = reloadTime;
                this.fireable = fireable;
                this.classificationId = classificationId;
                this.prefabPath = prefabPath;
                this.spritePath = spritePath;
                this.mountPoints = mountPoints;
            }
        }
        static List<ShipBean> db = new List<ShipBean>();
        static DB()
        {
            db.Add(new ShipBean("Std. Bridge", 1500, 5, 200, 0, 0, 0, 0, 0, 0.0, false, 1, "PrefabPieces/ShipItems/Bridge", "", new Vector3[2] { new Vector3(0, -1, 180), new Vector3(0, 1, 0) }));
            db.Add(new ShipBean("Battery (Small)", 300, 2, 300, 0, 0, 0, 0, 0, 0.0, false, 2, "PrefabPieces/ShipItems/Battery", "", new Vector3[4] { new Vector3(1, 0, 90), new Vector3(0, 1, 0), new Vector3(-1, 0, 270), new Vector3(0, -1, 180) }));
            db.Add(new ShipBean("Battery (Large)", 1500, 6, 1000, 0, 0, 0, 0, 0, 0.0, false, 2, "PrefabPieces/ShipItems/LargeBattery", "", new Vector3[10] { new Vector3(1, 0, 90), new Vector3(1, 0.66f, 90), new Vector3(1, -0.66f, 90), new Vector3(-1, 0, 270), new Vector3(-1, 0.66f, 270), new Vector3(-1, -0.66f, 270), new Vector3(0.5f, -1, 180), new Vector3(-0.5f, -1, 180), new Vector3(0.5f, 1, 0), new Vector3(-0.5f, 1, 0) }));
            db.Add(new ShipBean("Solar Panel", 300, 2, 0, 20, 0, 0, 0, 0, 0.0, false, 2, "PrefabPieces/ShipItems/Solar", "", new Vector3[4] { new Vector3(1, 0, 90), new Vector3(0, 1, 0), new Vector3(-1, 0, 270), new Vector3(0, -1, 180) }));
            db.Add(new ShipBean("Rectangle Truss", 900, 3, 0, 0, 0, 0, 0, 0, 0.0, false, 3, "PrefabPieces/ShipItems/RectangleTruss", "", new Vector3[8] { new Vector3(1, 0, 90), new Vector3(1, 0.66f, 90), new Vector3(1, -0.66f, 90), new Vector3(-1, 0, 270), new Vector3(-1, 0.66f, 270), new Vector3(-1, -0.66f, 270), new Vector3(0, -1, 180), new Vector3(0, 1, 0) }));
            db.Add(new ShipBean("Square Truss", 300, 1, 0, 0, 0, 0, 0, 0, 0.0, false, 3, "PrefabPieces/ShipItems/SquareTruss", "", new Vector3[4] { new Vector3(1, 0, 90), new Vector3(0, 1, 0), new Vector3(-1, 0, 270), new Vector3(0, -1, 180) }));
            db.Add(new ShipBean("Triangle Truss", 150, 0.5, 0, 0, 0, 0, 0, 0, 0.0, false, 3, "PrefabPieces/ShipItems/TriangleTruss", "", new Vector3[3] { new Vector3(1, 0, 90), new Vector3(0, 0, 315), new Vector3(0, -1, 180) }));
            db.Add(new ShipBean("Armour", 1300, 5, 0, 0, 0, 0, 0, 0, 0.0, false, 3, "PrefabPieces/ShipItems/Armour", "", new Vector3[4] { new Vector3(1, 0, 90), new Vector3(0, 1, 0), new Vector3(-1, 0, 270), new Vector3(0, -1, 180) }));
            db.Add(new ShipBean("Std. Engine", 500, 3, 0, 0, 20, 0, 100, 0, 0.0, true, 4, "PrefabPieces/ShipItems/Engine", "Images/EngineSprite_1", new Vector3[1] { new Vector3(0.05f, 1, 0) }));
            db.Add(new ShipBean("Std. Rotor Jets", 300, 0.5, 0, 0, 10, 0, 30, 0, 0.0, true, 5, "PrefabPieces/ShipItems/RotorJets", "Images/rotorJets_1", new Vector3[1] { new Vector3(0, 1, 0) }));
            db.Add(new ShipBean("Slvr. Rotor Jets", 400, 0.75, 0, 0, 15, 0, 60, 0, 0.0, true, 5, "PrefabPieces/ShipItems/RotorJetsSilver", "Images/rotorJetsP_1", new Vector3[1] { new Vector3(0, 1, 0) }));
            db.Add(new ShipBean("Railgun", 200, 3, 0, 0, 5, 0, 15, 35, 0.7, true, 6, "PrefabPieces/ShipItems/Gun", "", new Vector3[3] { new Vector3(0, -1, 180), new Vector3(-1, -0.6f, 270), new Vector3(-1, -0.6f, 90) }));
            db.Add(new ShipBean("Laser", 200, 4, 0, 0, 20, 0, 80, 20, 0.0, true, 6, "PrefabPieces/ShipItems/Laser", "", new Vector3[3] { new Vector3(0, -1, 180), new Vector3(-1, -0.6f, 270), new Vector3(-1, -0.6f, 90) }));
            db.Add(new ShipBean("Rocket Launcher", 500, 4, 0, 0, 50, 0, 120, 100, 1.5, true, 6, "PrefabPieces/ShipItems/RocketLauncher", "", new Vector3[3] { new Vector3(0, -1, 180), new Vector3(-1, -0.1f, 270), new Vector3(-1, -0.1f, 90) }));
        }
        static internal ShipBean getObjectById(int id)
        {
            return db[id];
        }

    }
}