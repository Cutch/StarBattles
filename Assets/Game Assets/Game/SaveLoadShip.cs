using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
namespace StarBattles
{
    class SaveLoadShip
    {
        static void addSurogate(BinaryFormatter bf)
        {
            SurrogateSelector ss = new SurrogateSelector();

            Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
            Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate();
            ss.AddSurrogate(typeof(Vector3),
                            new StreamingContext(StreamingContextStates.All),
                            v3ss);
            ss.AddSurrogate(typeof(Vector2),
                            new StreamingContext(StreamingContextStates.All),
                            v2ss);

            // 5. Have the formatter use our surrogate selector
            bf.SurrogateSelector = ss;
        }
        public static List<Ship> savedShips = new List<Ship>();
        public static void Save(Ship ship)
        {
            //LoadList();
            savedShips.Clear();
            Ship foundShip = savedShips.Find(s => ship.name == s.name);
            if (foundShip != null)
            {
                Debug.Log("Overwrite");
                SaveLoadShip.savedShips.Remove(foundShip);
            }
            SaveLoadShip.savedShips.Add(ship);
            BinaryFormatter bf = new BinaryFormatter();
            addSurogate(bf);
            FileStream file = File.Create(Application.persistentDataPath + "/savedShips.shp");
            bf.Serialize(file, SaveLoadShip.savedShips);
            file.Close();
            Debug.Log("Saved Ship List");
        }

        public static Ship Load(string name)
        {
            if (File.Exists(Application.persistentDataPath + "/savedShips.shp"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                addSurogate(bf);
                FileStream file = File.Open(Application.persistentDataPath + "/savedShips.shp", FileMode.Open);
                SaveLoadShip.savedShips = (List<Ship>)bf.Deserialize(file);
                file.Close();
                Ship foundShip = savedShips.Find(s => name == s.name);
                return foundShip;
            }
            return null;
        }
        public static string[] LoadList()
        {
            List<string> shipNames = new List<string>();
            if (File.Exists(Application.persistentDataPath + "/savedShips.shp"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                addSurogate(bf);
                FileStream file = File.Open(Application.persistentDataPath + "/savedShips.shp", FileMode.Open);
                SaveLoadShip.savedShips = (List<Ship>)bf.Deserialize(file);
                file.Close();
                foreach (Ship s in savedShips)
                {
                    shipNames.Add(s.name);
                }
            }
            return shipNames.ToArray();
        }
    }
}