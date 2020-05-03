using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace StarBattles
{
    public class ShipPiece : MonoBehaviour
    {
        string name;
        internal double health;
        internal double origHealth;
        internal double weight;
        internal double energyCapacity;
        internal double energyGeneration;
        internal double energyCost;
        internal double damage;
        internal double speed;
        internal double beamDistance;
        internal bool destroyed;
        double currentHealth;
        string prefabPath;
        string animSpritePath;
        internal bool fireable;
        internal int classificationId;
        static int objectId = 1;
        int[] savedJoinedPointIds;
        int[] savedJoinedPieceIds;
        int saveId;
        int thisId;
        GameShip parentShip;
        GameObject joinPrefab;
        Vector3[] mountPoints;
        JoinPoint[] mountPointObjects;
        RectTransform objectRectTransform;
        Action fireFunc;
        ShipPiece[] joinedObjects;
        Sprite defaultImage;
        SpriteRenderer spriteRenderer;
        private char[] fireKeys;
        float firePerSecond;
        float lastTime = -1;
        float lastFiredTime = -1;

        private Animator animator;

        public char[] getFireKeys()
        {
            return fireKeys;
        }
        public int getSaveId()
        {
            return saveId;
        }
        public void setSaveId(int id)
        {
            saveId = id;
        }
        public int getId()
        {
            return thisId;
        }
        public void setId(int id)
        {
            thisId = id;
            objectId = Mathf.Max(objectId, thisId + 1);
        }
        public ShipPiece(GameShip parentShip, PieceData dp)
        {
            loadFromPieceData(parentShip, dp);
        }
        public void loadFromPieceData(GameShip parentShip, PieceData dp)
        {
            DB.ShipBean sb = DB.getObjectById(dp.shipObjectId);
            this.beamDistance = sb.range;
            this.parentShip = parentShip;
            this.name = sb.name;
            this.health = sb.health;
            this.currentHealth = this.health;
            this.weight = sb.weight;
            this.energyCapacity = sb.energyCapacity;
            this.energyGeneration = sb.energyGeneration;
            this.energyCost = sb.energyCost;
            this.damage = sb.damage;
            this.speed = sb.speed;
            this.name = sb.name;
            this.classificationId = sb.classificationId;
            this.fireable = sb.fireable;
            this.fireKeys = dp.fireKeys;
            this.setId(dp.objectId);
            this.animSpritePath = sb.spritePath;
            this.mountPoints = sb.mountPoints;
            this.mountPointObjects = new JoinPoint[mountPoints.Length];
            this.joinedObjects = new ShipPiece[mountPoints.Length];
            this.prefabPath = sb.prefabPath;
            this.savedJoinedPieceIds = dp.joinedPieceids;
            this.savedJoinedPointIds = dp.joinedPointIds;
            this.saveId = dp.saveId;
            this.firePerSecond = (float)sb.reloadTime;
            this.destroyed = false;
            //createJoinPoints();
            if (fireable)
            {
                setFireFunc();
                animator = GetComponent<Animator>();
                //Resources.
                //GetComponent<Image>().sprite.texture.
                Debug.Log((animSpritePath));
                Debug.Log(Resources.Load(animSpritePath));
                animator.runtimeAnimatorController = Resources.Load(animSpritePath) as RuntimeAnimatorController;
                animator.enabled = false;
            }
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void ApplyDamage(double dmg)
        {
            this.currentHealth -= dmg;
            this.currentHealth = Math.Round(currentHealth);
            this.adjustHealthBar();
            if (this.currentHealth <= 0)
            {
                parentShip.recalculateWeight();
                this.destroyed = true;
                parentShip.destroyPiece(gameObject);

            }
        }
        public void adjustHealthBar()
        {
            SpriteRenderer s = this.gameObject.transform.Find("HealthBar").GetComponent<SpriteRenderer>();
            s.size = new Vector2((float)(currentHealth / health), 0.2f);
        }
        public void setSpriteImage(Sprite objectSprite)
        {
            spriteRenderer.sprite = objectSprite;
            defaultImage = objectSprite;
        }
        void setFireFunc()
        {
            switch (this.name)
            {
                case "Std. Engine":
                    fireFunc = (() => this.engineFire());
                    break;
                case "Railgun":
                    fireFunc = (() => this.gunFire());
                    break;
                case "Laser":
                    fireFunc = (() => this.laserFire());
                    break;
                case "Rocket Launcher":
                    fireFunc = (() => parentShip.rocketFire(this));
                    break;
                case "Std. Rotor Jets":
                    fireFunc = (() => parentShip.rotorFire(this));
                    break;
                case "Slvr. Rotor Jets":
                    fireFunc = (() => parentShip.rotorFire(this));
                    break;
            }
        }
        GameObject fireObject = null;
        LineRenderer lr = null;
        Vector3 laserDirection;
        GameObject laserHitEmitter;
        public void laserFire()
        {
            if (!parentShip.energyChange(-energyCost * Time.deltaTime))
            {
                stopLaserFire();
                return;
            }
            parentShip.laserFire(this);
            Vector3 top = this.transform.position;
            double rad = this.transform.eulerAngles.z * Math.PI / 180;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            top.x += spriteRenderer.bounds.extents.x * -sin;
            top.y += spriteRenderer.bounds.extents.y * cos;

            if (fireObject == null)
            {
                fireObject = Instantiate(Resources.Load("PrefabPieces/Objects/LaserLine", typeof(GameObject)) as GameObject, top, Quaternion.identity);
                laserHitEmitter = Instantiate(Resources.Load("PrefabPieces/Objects/LaserHit", typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity);
                laserHitEmitter.SetActive(false);
                lr = fireObject.GetComponent<LineRenderer>();
                fireObject.transform.SetParent(this.transform);
                laserDirection = new Vector2(-sin, cos);
                RaycastHit2D rh = Physics2D.Raycast(top, new Vector2(-sin, cos), (float)beamDistance, LayerMask.GetMask("EnemyShips"));
                lr.SetPosition(1, laserDirection * (float)((rh.collider == null || beamDistance < rh.distance) ? beamDistance : rh.distance));
                if (rh.collider != null)
                {
                    rh.collider.SendMessage("ApplyDamage", Time.deltaTime * this.damage);
                    laserHitEmitter.transform.SetPositionAndRotation(rh.point, Quaternion.Euler(0, 0, (float)(Math.Atan2(-rh.normal.y, -rh.normal.x) * 180 / Math.PI + 90)));
                    laserHitEmitter.SetActive(true);
                }
            }
            else
            {
                RaycastHit2D rh = Physics2D.Raycast(top, new Vector2(-sin, cos), (float)beamDistance, LayerMask.GetMask("EnemyShips"));
                lr.SetPosition(1, laserDirection * (float)((rh.collider == null || beamDistance < rh.distance) ? beamDistance : rh.distance));
                if (rh.collider != null)
                {
                    rh.collider.SendMessage("ApplyDamage", Time.deltaTime * this.damage);
                    laserHitEmitter.transform.SetPositionAndRotation(rh.point, Quaternion.Euler(0, 0, (float)(Math.Atan2(-rh.normal.y, -rh.normal.x) * 180 / Math.PI + 90)));
                    laserHitEmitter.SetActive(true);
                }
                else
                    laserHitEmitter.SetActive(false);
            }
        }
        public void stopLaserFire()
        {
            if (fireObject)
            {
                Destroy(laserHitEmitter);
                Destroy(fireObject);
            }
            fireObject = null;
            lr = null;
        }
        public void engineFire()
        {
            if (!parentShip.energyChange(-energyCost * Time.deltaTime))
            {
                stopEngineFire();
                return;
            }
            animator.enabled = true;
            parentShip.engineFire(this);
        }
        public void stopEngineFire()
        {
            animator.enabled = false;
            spriteRenderer.sprite = defaultImage;
        }
        public void gunFire()
        {
            lastTime = Time.time;
            if (lastTime > lastFiredTime)
            {
                if (!parentShip.energyChange(-energyCost))
                    return;
                parentShip.gunFire(this);
                Vector3 top = this.transform.position;
                double rad = this.transform.eulerAngles.z * Math.PI / 180;
                float cos = (float)Math.Cos(rad);
                float sin = (float)Math.Sin(rad);
                top.x += spriteRenderer.bounds.extents.x * -sin;
                top.y += spriteRenderer.bounds.extents.y * cos;
                DamageObject dmgo = Instantiate(Resources.Load("PrefabPieces/Objects/Bullet",
                    typeof(GameObject)) as GameObject, top, Quaternion.Euler(0, 0, (float)(Math.Atan2(sin, cos) * 180 / Math.PI))).GetComponent<DamageObject>();
                dmgo.damage = damage * firePerSecond;
                dmgo.positionSpeed = new Vector2(-sin, cos) * 40 + parentShip.positionSpeed;
                lastFiredTime = (lastTime + firePerSecond);
            }
        }
        public void fire()
        {
            fireFunc();
        }
        public void stopFire()
        {
            stopLaserFire();
            stopEngineFire();
        }

        public Vector2 getFinalPosition()
        {
            return (Vector2)(transform.position);
        }
        public Vector3 getFinalRotation()
        {
            return (Vector3)(gameObject.transform.eulerAngles);
        }

        static int joinIdToIndex(int joinId)
        {
            return (joinId % 100);
        }

        int indexToJoinId(int index)
        {
            return thisId * 100 + index;
        }

        Vector2 joinIndexToOffset(int index)
        {
            return joinIndexToOffsetWithAngle(index, getFinalRotation().z);
        }
        Vector2 joinIndexToOffsetWithAngle(int index, float angle)
        {
            if (objectRectTransform == null)
                objectRectTransform = gameObject.GetComponent<RectTransform>();
            return (Vector2)(Quaternion.AngleAxis(((angle) + 360) % 360, Vector3.forward) * Vector2.Scale((Vector2)mountPoints[index], (Vector2)objectRectTransform.rect.size / 2));
        }
        double joinIndexToAngle(int index)
        {
            return mountPoints[index].z;
        }
        Vector2 joinIndexToPosition(int index)
        {
            return joinIndexToOffset(index) + (Vector2)getFinalPosition();
        }

        void Start()
        {
            thisId = objectId++;
        }

        public void createJoinPoints()
        {
            for (int i = 0; i < mountPoints.Length; i++)
            {
                if (mountPointObjects[i] == null)
                {
                    mountPointObjects[i] = (JoinPoint)((GameObject)Instantiate(joinPrefab, joinIndexToPosition(i), Quaternion.identity))
                                                        .GetComponent<JoinPoint>();
                    mountPointObjects[i].gameObject.transform.SetParent(transform);
                    mountPointObjects[i].setId(indexToJoinId(i));
                    //if (joinedObjects[i] != null)
                    //{
                    //    mountPointObjects[i].setJoined(true, joinedObjects[i]);
                    //}
                    mountPointObjects[i].hide();
                }
            }
        }

        public void updateJoinPointsFromLoad()
        {
            for (int i = 0; i < mountPoints.Length; i++)
            {
                if (savedJoinedPointIds[i] != 0)
                {
                    if (joinedObjects[i] != null)
                    {
                        mountPointObjects[i].setJoined(true, joinedObjects[i].getJoinPointById(savedJoinedPointIds[i]));
                    }
                }
            }
        }
        //	void Update(){
        //		gameObject.transform.Rotate(new Vector3(0,0,0.1f));
        //		hideJoinPoints ();
        //		showJoinPoints();
        //	}

        public void showJoinPoints()
        {
            for (int i = 0; i < mountPoints.Length; i++)
            {
                mountPointObjects[i].show();
            }
        }

        public void hideJoinPoints()
        {
            hideJoinPoints(0);
        }
        public void hideAndUnselectJoinPoints()
        {
            hideJoinPoints(0);
            for (int i = 0; i < mountPointObjects.Length; i++)
            {
                mountPointObjects[i].unselectThis();
            }
        }
        public void hideJoinPoints(int except)
        {
            for (int i = 0; i < mountPointObjects.Length; i++)
            {
                if (mountPointObjects[i].getId() != except)
                    mountPointObjects[i].hide();
            }
        }



        public void showJoinedPoints()
        {
            for (int i = 0; i < mountPointObjects.Length; i++)
            {
                if (mountPointObjects[i].isJoined())
                    mountPointObjects[i].show();
                //Destroy (mountPointObjects [i].gameObject);
            }
        }
        public void hideJoinedPoints()
        {
            for (int i = 0; i < mountPointObjects.Length; i++)
            {
                if (mountPointObjects[i].isJoined())
                    mountPointObjects[i].hide();
                //Destroy (mountPointObjects [i].gameObject);
            }
        }

        public void setJoinedObject(int joinId, ShipPiece piece)
        {
            int id = joinIdToIndex(joinId);
            joinedObjects[id] = piece;
        }

        public void unsetJoinedObject(int joinId)
        {
            int id = joinIdToIndex(joinId);
            joinedObjects[id] = null;
        }

        public void setJoinedObjects(ShipPiece[] g)
        {
            joinedObjects = g;
        }
        public ShipPiece[] getJoinedObjects()
        {
            return joinedObjects;
        }
        public int[] getSavedJoinedIds()
        {
            return savedJoinedPieceIds;
        }
        public JoinPoint[] getMountPointObjects()
        {
            return mountPointObjects;
        }
        public JoinPoint getJoinPointById(int id)
        {
            return mountPointObjects[joinIdToIndex(id)];
        }


        public void movePiece(Vector3 offset)
        {
            transform.position += offset;
        }

        public void removeSelfFromJoinedPiece()
        {
            for (int i = 0; i < joinedObjects.Length; i++)
            {
                if (joinedObjects[i] != null)
                    joinedObjects[i].removeJoinedPiece(this);
            }
        }
        public void removeJoinedPiece(ShipPiece s)
        {
            for (int i = 0; i < joinedObjects.Length; i++)
            {
                if (joinedObjects[i] == s)
                    joinedObjects[i] = null;
            }
        }
        public ShipPiece[] getConnected()
        {
            List<ShipPiece> connects = new List<ShipPiece>();
            getConnected(connects);
            return connects.ToArray();
        }
        void getConnected(List<ShipPiece> moved)
        {
            if (!moved.Contains(this))
            {
                moved.Add(this);
                for (int i = 0; i < joinedObjects.Length; i++)
                {
                    if (joinedObjects[i] != null)
                        joinedObjects[i].getConnected(moved);
                }
            }
        }
        public bool isConnected(ShipPiece to)
        {
            return isConnected(to, new List<ShipPiece>());
        }
        bool isConnected(ShipPiece to, List<ShipPiece> moved)
        {
            if (!moved.Contains(this))
            {
                moved.Add(this);
                if (to == this)
                    return true;
                for (int i = 0; i < joinedObjects.Length; i++)
                {
                    if (joinedObjects[i] != null)
                        if (joinedObjects[i].isConnected(to, moved))
                            return true;
                }
            }
            return false;
        }
        public int countJoinedOjects()
        {
            return countJoinedOjects(new List<ShipPiece>());
        }

        int countJoinedOjects(List<ShipPiece> moved)
        {
            int count = 0;
            if (!moved.Contains(this))
            {
                moved.Add(this);
                count++;
                for (int i = 0; i < joinedObjects.Length; i++)
                {
                    if (joinedObjects[i] != null)
                        count += joinedObjects[i].countJoinedOjects(moved);
                }
            }
            return count;
        }

        public void offsetGroup(Vector3 offset)
        {
            offsetGroup(offset, new List<ShipPiece>());
        }

        void offsetGroup(Vector3 offset, List<ShipPiece> moved)
        {
            if (!moved.Contains(this))
            {
                this.movePiece(offset);
                moved.Add(this);
                for (int i = 0; i < joinedObjects.Length; i++)
                {
                    if (joinedObjects[i] != null)
                        joinedObjects[i].offsetGroup(offset, moved);
                }
            }
        }
    }
}