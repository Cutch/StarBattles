using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
namespace StarBattles
{
    public class EditorDropPanel : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        List<GameObject> editorPieces = new List<GameObject>();
        EditorPiece selectedLinkPiece = null;
        EditorPiece selectedEditorPiece = null;
        Boolean acceptInput = false;
        GameObject linkButton;
        GameObject editorInnerScroll;
        GameObject linkerInnerScroll;
        GameObject objectInput;
        GameObject keyInput;
        GameObject editorView;
        GameObject instructionText;
        GameObject savePanel;
        GameObject loadPanel;
        GameObject saveButton;
        GameObject scrapButton;
        void Update()
        {
            if (acceptInput)
            {
                if (Input.GetKey(KeyCode.Escape) && selectedLinkPiece != null) // Escape
                {
                    Dropdown dd = keyInput.GetComponent<Dropdown>();
                    dd.options[dd.value].text = "Empty Key Slot";
                    selectedLinkPiece.setFireKey(dd.value, (char)0);
                    dd.RefreshShownValue();
                    selectedLinkPiece.setLinkColour();
                }
                if (Input.inputString != "" && selectedLinkPiece != null)
                {
                    char c = Input.inputString.ToCharArray()[0];
                    Dropdown dd = keyInput.GetComponent<Dropdown>();
                    Debug.Log(dd.value);
                    dd.options[dd.value].text = KeyNames.charToName(c);
                    dd.RefreshShownValue();
                    selectedLinkPiece.setFireKey(dd.value, c);
                    selectedLinkPiece.setLinkColour();
                }
            }
        }
        void Start()
        {
            scrapButton = GameObject.Find("ScrapButton");
            scrapButton.SetActive(false);
            saveButton = GameObject.Find("SaveBoxSaveButton");
            loadPanel = GameObject.Find("LoadPanel");
            loadPanel.SetActive(false);
            savePanel = GameObject.Find("SavePanel");
            savePanel.SetActive(false);
            instructionText = GameObject.Find("InstructionText");
            editorView = GameObject.Find("EditorView");
            objectInput = GameObject.Find("ObjectInput");
            keyInput = GameObject.Find("KeyInput");
            keyInput.SetActive(false);
            linkButton = GameObject.Find("LinkButton");
            editorInnerScroll = GameObject.Find("EditorInnerScroll");
            editorInnerScroll.SetActive(true);
            linkerInnerScroll = GameObject.Find("LinkerInnerScroll");
            linkerInnerScroll.SetActive(false);
        }
        #region IDropHandler implementation
        public void OnDrop(PointerEventData eventData) 
        {

            //print ("drag");
            //print (EditorDragDrop.itemBeingDragged.transform.position);
            if (EditorDragDrop.itemBeingDragged != null)
            {
                GameObject copy = Instantiate(EditorDragDrop.itemBeingDragged.toDrop as GameObject, EditorDragDrop.itemBeingDragged.overlayObject.transform.position, Quaternion.identity) as GameObject;
                //print (copy.transform.position);
                //copy.transform.SetParent (EditorDragDrop.itemBeingDragged.transform.parent);
                copy.transform.SetParent(transform);
                editorPieces.Add(copy);
                //print (EditorDragDrop.itemBeingDragged.transform.position);
            }
            //else {

            //}
            UpdateStats();
        }
        public void validateSave()
        {
            string text = GameObject.Find("SaveInputField/Text").GetComponent<Text>().text;
            if (text != "")
            {
                List<EditorPiece> editorPiecesConvert = new List<EditorPiece>();
                editorPieces.ForEach(delegate (GameObject go)
                {
                    editorPiecesConvert.Add(go.GetComponent<EditorPiece>());
                });
                Debug.Log(editorPiecesConvert.Count);
                SaveLoadShip.Save(new Ship(text, editorPiecesConvert));
                savePanel.SetActive(false);
            }
        }
        public void cancelSave()
        {
            savePanel.SetActive(false);
        }
        public void SaveEditorShip()
        {
            savePanel.SetActive(true);
            bool linksAreSet = true;
            bool areConnected = true;
            bool noObjects = editorPieces.Count == 0;
            if (!noObjects)
            {
                editorPieces.ForEach(delegate (GameObject go)
                {
                    EditorPiece ep = go.GetComponent<EditorPiece>();
                    if (ep.fireable && ep.getFireKey()[0] == 0)
                        linksAreSet = false;
                });
                EditorPiece ep0 = editorPieces[0].GetComponent<EditorPiece>();
                editorPieces.ForEach(delegate (GameObject go)
                {
                    EditorPiece ep = go.GetComponent<EditorPiece>();
                    if (!ep.isConnected(ep0))
                        areConnected = false;
                });
            }
            string issuesString = "";
            saveButton.SetActive(true);
            if (noObjects)
            {
                issuesString += "There are no objects to save\n";
                saveButton.SetActive(false);
            }
            else
            {
                if (!linksAreSet)
                    issuesString += "Not all objects are linked to keys\n";
                if (!areConnected)
                    issuesString += "Not all objects are connected\n";
            }
            GameObject.Find("SaveIssues").GetComponent<Text>().text = issuesString;
        }
        public void validateLoad()
        {
            if (selectedLine != null)
            {
                string shipName = selectedLine.gameObject.GetComponentInChildren<Text>().text;
                loadPanel.SetActive(false);
                loadShip(shipName);
            }
        }
        public void cancelLoad()
        {
            loadPanel.SetActive(false);
        }
        void loadShip(string shipName)
        {
            List<EditorPiece> editorPiecesConvert = new List<EditorPiece>();
            editorPieces.Clear();
            clearPanel();
            GameObject lp = editorView;
            RectTransform view = lp.GetComponent<RectTransform>();
            Vector2 centerScreen = new Vector2(view.rect.width / 2, view.rect.height / 2);
            Ship s = SaveLoadShip.Load(shipName);
            Dictionary<int, EditorPiece> epLookup = new Dictionary<int, EditorPiece>();
            foreach (PieceData pd in s.piecesData)
            {
                GameObject go = Instantiate(Resources.Load(DB.getObjectById(pd.shipObjectId).prefabPath, typeof(GameObject)) as GameObject, pd.location + centerScreen, Quaternion.Euler(pd.rotation));
                editorPieces.Add(go);
                EditorPiece ep = go.GetComponent<EditorPiece>();
                ep.loadFromPieceData(pd);
                epLookup.Add(ep.getSaveId(), ep);
                go.transform.SetParent(transform);
            }

            foreach (EditorPiece ep in epLookup.Values)
            {
                EditorPiece[] temp = new EditorPiece[ep.getSavedJoinedIds().Length];
                for (int i = 0; i < ep.getSavedJoinedIds().Length; i++)
                {
                    int id = ep.getSavedJoinedIds()[i];
                    if (id != 0)
                        temp[i] = epLookup[id];
                }
                ep.setJoinedObjects(temp);
            }

            foreach (EditorPiece ep in epLookup.Values)
            {
                ep.updateJoinPointsFromLoad();
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
            loadPanel.SetActive(true);
            string[] shipList = SaveLoadShip.LoadList();
            GameObject loadList = GameObject.Find("LoadBoxList");
            GameObject loadBoxLoadButton = GameObject.Find("LoadBoxLoadButton");
            loadBoxLoadButton.SetActive(false);
            foreach (RectTransform txt in loadList.GetComponentsInChildren<RectTransform>())
            {
                if (txt.gameObject.name == "LoadListLine(Clone)")
                    Destroy(txt.gameObject);
            }
            GameObject firstObj = null;
            foreach (string shipName in shipList)
            {
                GameObject go = Instantiate(Resources.Load("PrefabPieces/Editor/LoadListLine", typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity);
                go.GetComponentInChildren<Text>().text = shipName;
                go.GetComponent<RectTransform>().SetParent(loadList.GetComponent<RectTransform>());
                if(firstObj == null)
                    firstObj = go;
            }
            firstObj.GetComponent<EditorLoadLineSelect>().select();
            if (shipList.Length > 0)
                loadBoxLoadButton.SetActive(true);
        }
        void clearPanel()
        {
            GameObject lp = editorView;
            foreach (EditorPiece go in lp.GetComponentsInChildren<EditorPiece>())
            {
                Destroy(go.gameObject);
            }
        }
        void UpdateStats()
        {
            double energyCapacity = 0;
            double energyCost = 0;
            double energyGeneration = 0;
            double weight = 0;
            double damage = 0;
            double health = 0;
            double speed = 0;
            GameObject lp = editorView;
            foreach (EditorPiece ep in lp.GetComponentsInChildren<EditorPiece>())
            {
                //Debug.Log(ep);
                //EditorPiece ep = go.GetComponent<EditorPiece>();
                energyCapacity += ep.energyCapacity;
                energyCost += ep.energyCost;
                energyGeneration += ep.energyGeneration;
                weight += ep.weight;
                damage += ep.damage;
                health += ep.health;
                speed += ep.speed;
            }
            GameObject.Find("WeightStats").GetComponent<Text>().text = string.Format("Weight {0:0}t", weight);
            GameObject.Find("HealthStats").GetComponent<Text>().text = string.Format("Health {0:0,000}", health);
            GameObject.Find("EnergyStoreStats").GetComponent<Text>().text = string.Format("Storage {0:0}j", energyCapacity);
            GameObject.Find("EnergyStats").GetComponent<Text>().text = string.Format("Energy {0:0}j/s", energyGeneration);
            GameObject.Find("SpeedStats").GetComponent<Text>().text = string.Format("Speed {0:0}km/s", speed);
            GameObject.Find("DamageStats").GetComponent<Text>().text = string.Format("Damage {0:0}d/s", damage);
        }

        public void toggleCreateLinkLinkMode()
        {
            if (acceptInput)
            {
                linkButton.GetComponentInChildren<Text>().text = "Link Mode";
                editorInnerScroll.SetActive(true);
                linkerInnerScroll.SetActive(false);
                acceptInput = false;
                editorPieces.ForEach(delegate (GameObject go)
                {
                    EditorPiece ep = go.GetComponent<EditorPiece>();
                    ep.unsetLinkColour();
                });
            }
            else
            {
                linkButton.GetComponentInChildren<Text>().text = "Create Mode";
                editorInnerScroll.SetActive(false);
                linkerInnerScroll.SetActive(true);
                acceptInput = true;
                editorPieces.ForEach(delegate (GameObject go)
                {
                    EditorPiece ep = go.GetComponent<EditorPiece>();
                    ep.setLinkColour();
                });
                scrapButton.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            BroadcastMessage("hideAndUnselectJoinPoints");
            objectInput.GetComponent<Text>().text = "None";
            Dropdown dd = keyInput.GetComponent<Dropdown>();
            foreach (var k in dd.options)
            {
                k.text = "Empty Key Slot";
            }
            dd.value = 0;
            dd.RefreshShownValue();
            instructionText.GetComponent<Text>().text = "Select an object";
            keyInput.SetActive(false);
            selectedEditorPiece = null;
            selectedLinkPiece = null;
            scrapButton.SetActive(false);
        }
        public void editorPieceClicked(EditorPiece ep)
        {
            if (acceptInput)
            {
                selectedLinkPiece = ep;
                objectInput.GetComponent<Text>().text = ep.name;
                List<string> l = new List<string>();
                foreach (char c in selectedLinkPiece.getFireKey())
                {
                    l.Add(KeyNames.charToName(c));
                }
                Dropdown dd = keyInput.GetComponent<Dropdown>();
                for (int i = 0; i < dd.options.Count; i++)
                {
                    if (i < l.Count)
                        dd.options[i].text = l[i];
                    else
                        dd.options[i].text = "Empty Key Slot";
                }
                dd.value = 0;
                dd.RefreshShownValue();
                //dd.options[dd.value].text = String.Join(", ", l.ToArray());
                if (selectedLinkPiece.fireable)
                    instructionText.GetComponent<Text>().text = "Press any key";
                else
                    instructionText.GetComponent<Text>().text = "This object can't be activated";
                keyInput.SetActive(true);
            }
            else
            {
                selectedEditorPiece = ep;
                scrapButton.SetActive(true);
            }
        }
        #endregion
        public void scrapObject()
        {
            editorPieces.Remove(selectedEditorPiece.gameObject);
            Destroy(selectedEditorPiece.gameObject);
        }
        public void testShip()
        {
            Scenes.Load("MainGame", new Dictionary<string, object>(){
                {"ship", new Ship("Test Ship", editorPieces.Select(x => x.GetComponent<EditorPiece>()))},
                {"test", true}
                });
        }
        public void rotateShip()
        {
            HashSet<EditorPiece> components = new HashSet<EditorPiece>();
            foreach(GameObject piece in editorPieces)
            {
                EditorPiece ep = piece.GetComponent<EditorPiece>();
                if (components.Contains(ep)) continue;
                List<EditorPiece> eps = ep.getConnected();
                components.UnionWith(eps);
                Vector3 point = new Vector3();
                foreach(EditorPiece i in eps)
                {
                    point += i.transform.position;
                }
                point /= eps.Count;
                foreach (EditorPiece i in eps)
                {
                    i.transform.RotateAround(point, new Vector3(0, 0, 1), 90);
                }
            }
        }
    }
}