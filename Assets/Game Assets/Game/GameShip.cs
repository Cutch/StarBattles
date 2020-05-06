﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace StarBattles
{
    public class GameShip : MonoBehaviour
    {
        List<GameObject> GameShipPieces = new List<GameObject>();
        Dictionary<string, Action[]> keyWatchDown = new Dictionary<string, Action[]>();
        Dictionary<string, Action[]> keyWatchUp = new Dictionary<string, Action[]>();
        Dictionary<string, bool> keyDown = new Dictionary<string, bool>();
        double energyCapacity = 0;
        double energyGeneration = 0;
        double currentEnergy = 0;
        Vector2 positionAccel = Vector2.zero;
        Vector3 rotationAccel = Vector3.zero;
        internal Vector2 positionSpeed = Vector2.zero;
        internal Vector3 rotationSpeed = Vector3.zero;
        float maxPositionSpeed = 0;
        float maxRotationSpeed = 0;
        Boolean isMyShip = false;
        double shipWeight;
        GameShipPiece bridgePiece;
        RectTransform energyPanel;
        RectTransform energyPanelBackground;
        void Update()
        {
            // Get Energy
            float frameSeconds = Time.deltaTime;
            energyChange(energyGeneration * frameSeconds);
            // Check for inputs
            bool keyContains = false;
            foreach (KeyValuePair<string, Action[]> entry in keyWatchDown)
            {
                if (Input.GetKeyUp(entry.Key))
                {
                    foreach (Action a in keyWatchUp[entry.Key])
                        a();
                    keyDown.Remove(entry.Key);
                    keyContains = false;
                }
                else if (keyContains = keyDown.ContainsKey(entry.Key))
                {
                    foreach (Action a in entry.Value)
                        a();
                }
                if (!keyContains && Input.GetKeyDown(entry.Key))
                {
                    keyDown.Add(entry.Key, true);
                    foreach (Action a in entry.Value)
                        a();
                }
            }
            //Debug.Log(frameSeconds);
            //Debug.Log(rotationSpeed.z);
            //Debug.Log(this.positionSpeed * frameSeconds);
            //Debug.Log(this.rotationSpeed.z * frameSeconds);
            //Debug.Log(this.rotationSpeed * frameSeconds);
            // Calculate and change position
            if (this.positionAccel.magnitude > 0)
            {
                this.positionSpeed += this.positionAccel * frameSeconds;
                if (this.positionSpeed.magnitude > maxPositionSpeed)
                    this.positionSpeed *= maxPositionSpeed / this.positionSpeed.magnitude;
            }
            if (Math.Abs(this.rotationAccel.z) > 0)
            {
                this.rotationSpeed += this.rotationAccel * frameSeconds;
                if (Math.Abs(this.rotationSpeed.z) > maxRotationSpeed)
                    this.rotationSpeed.z *= maxRotationSpeed / Math.Abs(this.rotationSpeed.z);
            }
            this.gameObject.transform.position += (Vector3)this.positionSpeed * frameSeconds;
            this.gameObject.transform.Rotate(this.rotationSpeed * frameSeconds);
            if (isMyShip)
                Camera.main.transform.position = this.gameObject.transform.position;
            positionAccel = Vector3.zero;
            rotationAccel = Vector3.zero;
            maxPositionSpeed = 0;
            maxRotationSpeed = 0;
        }
        public GameShip LoadShipByName(string shipName, Boolean isMyShip, Vector2 position)
        {
            Ship s = SaveLoadShip.Load(shipName);
            return this.LoadShip(s, isMyShip, position);
        }
        public GameShip LoadShip(Ship s, Boolean isMyShip, Vector2 position)
        {
            this.isMyShip = isMyShip;
            if (isMyShip)
            {
                energyPanel = GameObject.Find("EnergyLevel").GetComponent<RectTransform>();
                energyPanelBackground = GameObject.Find("EnergyBackground").GetComponent<RectTransform>();
            }
            List<GameShipPiece> GameShipPiecesConvert = new List<GameShipPiece>();
            GameShipPieces.Clear();
            GameObject lp = gameObject;
            Vector2 centerScreen = position;
            Dictionary<int, GameShipPiece> spLookup = new Dictionary<int, GameShipPiece>();
            var worldToPixels = ((Screen.height / 2.0f) / Camera.main.orthographicSize);
            float halfSize = Camera.main.orthographicSize;
            foreach (PieceData pd in s.piecesData)
            {
                GameObject go = Resources.Load(DB.getObjectById(pd.shipObjectId).prefabPath, typeof(GameObject)) as GameObject;
                Sprite objectSprite = go.GetComponent<Image>().sprite;

                pd.location.y = (pd.location.y / pd.size.y) * (objectSprite.bounds.size.y);
                pd.location.x = (pd.location.x / pd.size.x) * (objectSprite.bounds.size.x);

                GameObject GameShipPiece = Instantiate(Resources.Load("PrefabPieces/ShipSprites/ShipPiece", typeof(GameObject)) as GameObject, position + pd.location, Quaternion.Euler(pd.rotation));
                GameShipPiece.transform.SetParent(this.gameObject.transform);
                GameShipPiece sp = GameShipPiece.GetComponent<GameShipPiece>();
                sp.loadFromPieceData(this, pd);
                sp.setSpriteImage(objectSprite);
                if (sp.classificationId == 1)
                    bridgePiece = sp;
                GameShipPieces.Add(GameShipPiece.gameObject);
                spLookup.Add(sp.getSaveId(), sp);
                if (isMyShip)
                    GameShipPiece.layer = LayerMask.NameToLayer("MyShip");
                else
                    GameShipPiece.layer = LayerMask.NameToLayer("EnemyShips");
            }

            foreach (GameShipPiece sp in spLookup.Values)
            {
                GameShipPiece[] temp = new GameShipPiece[sp.getSavedJoinedIds().Length];
                for (int i = 0; i < sp.getSavedJoinedIds().Length; i++)
                {
                    int id = sp.getSavedJoinedIds()[i];
                    if (id != 0)
                        temp[i] = spLookup[id];
                }
                sp.setJoinedObjects(temp);
                if (isMyShip && sp.fireable)
                {
                    char[] keys = sp.getFireKeys();
                    foreach (char c in keys)
                    {
                        if (keyWatchDown.ContainsKey(c.ToString()))
                        {
                            Action[] oldArray = keyWatchDown[c.ToString()];
                            Action[] newArray = new Action[oldArray.Length + 1];
                            for (int i = 0; i < oldArray.Length; i++)
                                newArray[i] = oldArray[i];
                            newArray[oldArray.Length] = (() => sp.fire());
                            keyWatchDown[c.ToString()] = newArray;

                            oldArray = keyWatchUp[c.ToString()];
                            newArray = new Action[oldArray.Length + 1];
                            for (int i = 0; i < oldArray.Length; i++)
                                newArray[i] = oldArray[i];
                            newArray[oldArray.Length] = (() => sp.stopFire());
                            keyWatchUp[c.ToString()] = newArray;
                        }
                        else
                        {
                            keyWatchDown.Add(c.ToString(), new Action[] { (() => sp.fire()) });
                            keyWatchUp.Add(c.ToString(), new Action[] { (() => sp.stopFire()) });
                        }
                    }
                }
            }

            foreach (GameShipPiece sp in spLookup.Values)
            {
                //sp.updateJoinPointsFromLoad();
            }
            recalculateWeight();
            updateLevels();
            return this;
        }
        public void splitShip(GameShipPiece[] toSplit)
        {
            if (toSplit.Length == 0) return;
            GameObject ship = Instantiate(Resources.Load("PrefabPieces/ShipSprites/Ship", typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity);
            GameShip newShip = ship.GetComponent<GameShip>();

            foreach (GameShipPiece j in toSplit)
            {
                newShip.GameShipPieces.Add(j.gameObject);
                j.transform.SetParent(ship.transform);
                GameShipPieces.Remove(j.gameObject);
            }
            newShip.updateLevels();
            this.updateLevels();
        }
        public void destroyPiece(GameObject obj)
        {
            if (obj == bridgePiece.gameObject) // SHip destroyed
            {
                foreach (GameObject o in GameShipPieces)
                    Destroy(o);
            }
            else
            {
                GameShipPiece toRemove = obj.GetComponent<GameShipPiece>();
                GameShipPiece[] oldJoined = toRemove.getJoinedObjects();
                toRemove.removeSelfFromJoinedPiece();
                GameShipPieces.Remove(obj);
                foreach (GameShipPiece j in oldJoined)
                {
                    foreach (GameShipPiece k in oldJoined)
                    {
                        if (j != null && k != null && j != k
                            && GameShipPieces.Contains(j.gameObject)
                            && GameShipPieces.Contains(k.gameObject)
                            && !k.isConnected(j))
                        {
                            splitShip(j.getConnected());
                        }
                    }
                }
                Destroy(obj);
                recalculateWeight();

            }
        }
        public void updateLevels()
        {
            energyCapacity = 0;
            energyGeneration = 0;
            foreach (GameObject g in GameShipPieces)
            {
                GameShipPiece s = g.GetComponent<GameShipPiece>();
                energyCapacity += s.energyCapacity;
                energyCapacity += s.energyGeneration;
                energyGeneration += s.energyGeneration;
            }
            energyChange(0);
        }
        public void engineFire(GameShipPiece s)
        {
            double speed = s.speed / 10;
            positionAccel = new Vector2((float)(speed * -Math.Sin(this.transform.eulerAngles.z * Math.PI / 180)), (float)(speed * Math.Cos(this.transform.eulerAngles.z * Math.PI / 180)));
            //Debug.Log("Called : "+positionSpeed);
            maxPositionSpeed = (float)(speed * 2);
        }
        public void gunFire(GameShipPiece s)
        {

        }
        public void rotorFire(GameShipPiece s)
        {
            double speed = s.speed * 4;
            float angleFromCenter = (float)(Math.Atan2(s.transform.localPosition.y, s.transform.localPosition.x));
            //Vector2.Angle(Vector2.zero, (Vector2)s.transform.localPosition);
            float distanceFromCenter = Vector3.Distance(Vector3.zero, s.transform.localPosition);
            float anglePower = (float)Math.Cos(angleFromCenter + (s.transform.localEulerAngles.z * Math.PI / 180));
            float distancePower = distanceFromCenter / 2;
            //Debug.Log(distanceFromCenter);
            rotationAccel = new Vector3(0, 0, (float)(anglePower * speed * distancePower));
            //Debug.Log("Called : " + rotationSpeed);
            maxRotationSpeed = (float)(speed * 2);
            //Debug.Log(entry.Key);
        }
        public void laserFire(GameShipPiece s)
        {

        }
        public void rocketFire(GameShipPiece s)
        {

        }
        public void recalculateWeight()
        {
            double weight = 0;
            foreach (GameObject g in GameShipPieces)
                weight += g.GetComponent<GameShipPiece>().weight;
            shipWeight = weight;
        }
        public float getSpeed()
        {
            return positionSpeed.magnitude;
        }
        public float getDirection()
        {
            return (float)Math.Atan2(positionSpeed.y, positionSpeed.x);
        }
        public float getWeight()
        {
            return (float)this.shipWeight;
        }
        internal bool energyChange(double energy)
        {
            //if(energy < 0)
            //    Debug.Log(energy + " "+ currentEnergy);
            //if (energy > 0)
            //    Debug.Log(energy + " " + currentEnergy + " "+energyCapacity);
            if (energy < 0 && currentEnergy - energy < 0)
                return false;
            currentEnergy += energy;
            if (currentEnergy > energyCapacity)
                currentEnergy = energyCapacity;
            if (isMyShip)
            {
                //Debug.Log(energyPanelBackground.rect.height * (1 - (currentEnergy / energyCapacity)));
                energyPanel.offsetMax = new Vector2(0, -(float)(energyPanelBackground.rect.height * (1 - (currentEnergy / energyCapacity))));
            }
            return true;
        }
    }
}