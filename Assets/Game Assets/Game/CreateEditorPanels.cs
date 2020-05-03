using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarBattles
{
    public class CreateEditorPanels : MonoBehaviour
    {
        public GameObject itemSlot;
        string[] classifications = new string[] {
        "Control Modules",
        "Energy Modules",
        "Structural",
        "Propulsion",
        "Rotator",
        "Weapons"};
        // Use this for initialization
        void Start()
        {
            //GameObject a = (GameObject)Instantiate(itemSlot);
            //a.transform.SetParent(this.transform, false);
            GameObject ei = Resources.Load("PrefabPieces/Editor/EditorItem", typeof(GameObject)) as GameObject;
            GameObject cr = Resources.Load("PrefabPieces/Editor/ClassificationRow", typeof(GameObject)) as GameObject;
            List<GameObject> gos = new List<GameObject>();
            foreach (GameObject go in Resources.LoadAll("PrefabPieces/ShipItems", typeof(GameObject)))
            {
                gos.Add(go);
            }
            //var guids2 = AssetDatabase.FindAssets("", new string[] { "Assets/Game Assets/PrefabPieces/ShipItems" });
            //List<GameObject> gos = new List<GameObject>();

            /*     foreach (var guid in guids2)
                 {
                     GameObject go = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject)) as GameObject;
                     gos.Add(go);
                 }*/
            this.transform.position = new Vector2(0, 0);
            gos.Sort(new ClassificationCompare());
            int lastClassifier = -1;
            foreach (var go in gos)
            {
                EditorPiece ep = go.GetComponent<EditorPiece>();
                if (lastClassifier != ep.classificationId - 1)
                {
                    lastClassifier = ep.classificationId - 1;
                    GameObject b = (GameObject)Instantiate(cr);
                    b.transform.SetParent(this.transform, false);
                    b.transform.Find("ClassText").GetComponent<Text>().text = classifications[lastClassifier];
                }
                GameObject a = (GameObject)Instantiate(ei);
                a.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = go.GetComponent<UnityEngine.UI.Image>().sprite;
                a.transform.Find("Image").GetComponent<EditorDragDrop>().toDrop = go;
                a.transform.Find("Panel").Find("WeightText").GetComponent<Text>().text = (ep.weight % 1 != ep.weight) ? ep.weight.ToString() + "t" : string.Format("{0:0.0}t", ep.weight);
                string s = "";
                if (ep.name.Length > 0)
                    s += ep.name + "\n";
                if (ep.health != 0)
                    s += string.Format("Health {0:0}\n", ep.health);
                if (ep.energyCapacity != 0)
                    s += string.Format("Energy Storage {0:0}j\n", ep.energyCapacity);
                if (ep.energyGeneration != 0)
                    s += string.Format("Energy {0:0}j/s\n", ep.energyGeneration);
                if (ep.energyCost != 0)
                    s += string.Format("Energy Use {0:0}j/s\n", ep.energyCost);
                if (ep.speed != 0)
                    s += string.Format("Speed {0:0}km/s\n", ep.speed);
                if (ep.damage != 0)
                    s += string.Format("Damage {0:0}d/s\n", ep.damage);
                a.transform.Find("Panel").Find("InfoText").GetComponent<Text>().text = s;
                a.transform.SetParent(this.transform, false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public class ClassificationCompare : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            EditorPiece epx = x.GetComponent<EditorPiece>();
            EditorPiece epy = y.GetComponent<EditorPiece>();
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                {
                    int cmp = epx.classificationId.CompareTo(epy.classificationId);
                    if (cmp != 0)
                        return cmp;
                    else
                        return epx.name.CompareTo(epy.name);
                }
            }
        }
    }
}