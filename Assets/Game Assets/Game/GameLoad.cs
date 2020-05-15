using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace StarBattles
{
    public class GameLoad : MonoBehaviour
    {
        List<GameShip> ships = new List<GameShip>();
        GameShip myShip = null;
        void Start()
        {
            if (Scenes.getParam("test") != null)
            {
                loadShip((Ship)Scenes.getParam("ship"), Vector2.zero, true);
            }
            else
            {
                loadShip(SaveLoadShip.LoadList()[0], Vector2.zero, true);
                loadShip(SaveLoadShip.LoadList()[0], new Vector2(0, 15), false);
            }
        }
        public void validateLoad()
        {
            if (selectedLine != null)
            {
                string shipName = selectedLine.gameObject.GetComponentInChildren<Text>().text;
                //loadPanel.SetActive(false);
                loadShip(shipName, Vector3.zero, true);
            }
        }
        public void cancelLoad()
        {
            //loadPanel.SetActive(false);
        }
        void loadShip(Ship shipData, Vector2 startPos, bool isMyShip = true)
        {
            if (isMyShip)
                if (myShip != null)
                    ships.Remove(myShip);
            GameObject ship = Instantiate(Resources.Load("PrefabPieces/ShipSprites/Ship", typeof(GameObject)) as GameObject, (Vector3)startPos, Quaternion.identity);
            GameShip newShip = ship.GetComponent<GameShip>().LoadShip(shipData, isMyShip, startPos);
            if (isMyShip)
            {
                myShip = newShip;
            }
        }
        void loadShip(string shipName, Vector2 startPos, bool isMyShip = true)
        {
            if (isMyShip)
                if (myShip != null)
                    ships.Remove(myShip);
            GameObject ship = Instantiate(Resources.Load("PrefabPieces/ShipSprites/Ship", typeof(GameObject)) as GameObject, (Vector3)startPos, Quaternion.identity);
            GameShip newShip = ship.GetComponent<GameShip>().LoadShipByName(shipName, isMyShip, startPos);
            if (isMyShip)
            {
                myShip = newShip;
            }
        }
        static EditorLoadLineSelect selectedLine = null;
        public static void loadLineSelect(EditorLoadLineSelect sel)
        {
            if (selectedLine != null)
                selectedLine.unselect();
            selectedLine = sel;
        }
        public void LoadEditorShip()
        {
            //loadPanel.SetActive(true);
            string[] shipList = SaveLoadShip.LoadList();
            GameObject loadList = GameObject.Find("LoadBoxList");
            foreach (RectTransform txt in loadList.GetComponentsInChildren<RectTransform>())
            {
                if (txt.gameObject.name == "LoadListLine(Clone)")
                    Destroy(txt.gameObject);
            }
            foreach (string shipName in shipList)
            {
                GameObject go = Instantiate(Resources.Load("PrefabPieces/Editor/LoadListLine", typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity);
                go.GetComponentInChildren<Text>().text = shipName;
                go.GetComponent<RectTransform>().SetParent(loadList.GetComponent<RectTransform>());
            }
        }
        /*void UpdateStats()
        {
            double energyCapacity = 0;
            double energyCost = 0;
            double energyGeneration = 0;
            double weight = 0;
            double damage = 0;
            double health = 0;
            double speed = 0;
            GameObject lp = editorView;
            foreach (GameShipPiece sp in lp.GetComponentsInChildren<GameShipPiece>())
            {
                //Debug.Log(sp);
                //GameShipPiece sp = go.GetComponent<GameShipPiece>();
                energyCapacity += sp.energyCapacity;
                energyCost += sp.energyCost;
                energyGeneration += sp.energyGeneration;
                weight += sp.weight;
                damage += sp.damage;
                health += sp.health;
                speed += sp.speed;
            }
            GameObject.Find("WeightStats").GetComponent<Text>().text = string.Format("Weight {0:0}t", weight);
            GameObject.Find("HealthStats").GetComponent<Text>().text = string.Format("Health {0:0,000}", health);
            GameObject.Find("EnergyStoreStats").GetComponent<Text>().text = string.Format("Storage {0:0}j", energyCapacity);
            GameObject.Find("EnergyStats").GetComponent<Text>().text = string.Format("Energy {0:0}j/s", energyGeneration);
            GameObject.Find("SpeedStats").GetComponent<Text>().text = string.Format("Speed {0:0}km/s", speed);
            GameObject.Find("DamageStats").GetComponent<Text>().text = string.Format("Damage {0:0}d/s", damage);
        }*/
    }
}