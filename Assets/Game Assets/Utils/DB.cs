using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace StarBattles{
    public class DB
    {
        [Serializable]
        public struct ShipBean
        {
            public string name;
            public double health;
            public double weight;
            public double energyCapacity;
            public double energyGeneration;
            public double energyCost;
            public double damage;
            public double speed;
            public double range;
            public bool fireable;
            public int classificationId;
            public string prefabPath;
            public string spritePath;
            public double reloadTime;
            public Vector3[] mountPoints;
            public ShipBean(string name, double health, double weight,
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
        
        [Serializable]
        private class ShipBeans
        {
            public ShipBean[] shipBeans;
        }
        static ShipBeans sb = new ShipBeans();
        static DB()
        {
            string path = "Assets/Game Assets/Data/ShipPieces.json";
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            sb = JsonUtility.FromJson<ShipBeans>(reader.ReadToEnd());
            reader.Close();
        }
        static public ShipBean getObjectById(int id)
        {
            return sb.shipBeans[id];
        }


    }
}