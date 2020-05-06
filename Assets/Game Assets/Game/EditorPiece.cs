using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace StarBattles
{
    public class EditorPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public string name;
        public double health;
        public double weight;
        public double energyCapacity;
        public double energyGeneration;
        public double energyCost;
        public double damage;
        public double speed;
        public string prefabPath;
        public string animSpritePath;
        public bool fireable;
        public int classificationId;
        public int shipObjectId;
        static int objectId = 1;
        int[] savedJoinedPointIds;
        int[] savedJoinedPieceIds;
        int saveId;
        int thisId;
        public GameObject joinPrefab;
        public Vector3[] mountPoints;
        JoinPoint[] mountPointObjects;
        public static EditorPiece itemBeingDragged;
        Vector3 startPosition;
        Transform startParent;
        EditorPiece[] joinedObjects;
        bool didDrag = false;
        RectTransform objectRectTransform;
        private bool onTheMove = false;
        private Vector3 onTheMovePos;
        private Vector3 onTheMoveAngle;
        static Color issueColour = Color.red;
        static Color goodColour = Color.green;
        static Color noColour = Color.white;
        internal List<char> fireKeys;
        private bool linkMode = false;

        public char[] getFireKey()
        {
            return fireKeys.FindAll((x => x != (char)0)).ToArray();
        }
        public void setFireKey(int i, char key)
        {
            if (fireable)
            {
                while (fireKeys.Count <= i)
                    fireKeys.Add((char)0);
                fireKeys[i] = key;
            }
        }
        public void addFireKey(char key)
        {
            if (fireable)
            {
                fireKeys.Add(key);
            }
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
        public EditorPiece(PieceData dp)
        {
            loadFromPieceData(dp);
        }
        public void loadFromPieceData(PieceData dp)
        {
            DB.ShipBean sb = DB.getObjectById(dp.shipObjectId);
            this.shipObjectId = dp.shipObjectId;
            this.name = sb.name;
            this.health = sb.health;
            this.weight = sb.weight;
            this.energyCapacity = sb.energyCapacity;
            this.energyGeneration = sb.energyGeneration;
            this.energyCost = sb.energyCost;
            this.damage = sb.damage;
            this.speed = sb.speed;
            this.animSpritePath = sb.spritePath;
            this.classificationId = sb.classificationId;
            this.fireable = sb.fireable;
            this.fireKeys = new List<char>(dp.fireKeys);
            this.setId(dp.objectId);
            this.mountPoints = sb.mountPoints;
            this.mountPointObjects = new JoinPoint[mountPoints.Length];
            this.joinedObjects = new EditorPiece[mountPoints.Length];
            this.prefabPath = sb.prefabPath;
            this.savedJoinedPieceIds = dp.joinedPieceids;
            this.savedJoinedPointIds = dp.joinedPointIds;
            this.saveId = dp.saveId;
            createJoinPoints();
        }

        public Vector2 getFinalPosition()
        {
            return (Vector2)(onTheMove ? onTheMovePos : transform.position);
        }
        public Vector3 getFinalRotation()
        {
            return (Vector3)(onTheMove ? onTheMoveAngle : gameObject.transform.eulerAngles);
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
            // (angle - mountPoints [index].z)
            //Vector2 vec = (Vector2)(Quaternion.AngleAxis(((angle)+360)%360,Vector3.forward) * ((Vector3)(Vector2)mountPoints [index]));
            //Vector2 vec = (Vector2)mountPoints [index];
            //return Vector2.Scale (vec, (Vector2)objectRectTransform.rect.size / 2);
            //print (angle);
            return (Vector2)(Quaternion.AngleAxis(((angle) + 360) % 360, Vector3.forward) * Vector2.Scale((Vector2)mountPoints[index], (Vector2)objectRectTransform.rect.size / 2));
        }
        double joinIndexToAngle(int index)
        {
            //Vector2 vec1 = (Vector2)mountPoints [index];
            //Vector2 vec2 = new Vector2(mountPoints [index].z, mountPoints [index].w);
            //return Vector2.Angle (vec1, vec2);
            return mountPoints[index].z;
        }
        //	Vector2 joinIndexToAngleVector(int index){
        //		Vector2 vec2 = new Vector2(mountPoints [index].z, mountPoints [index].w);
        //		return vec2;
        //	}
        Vector2 joinIndexToPosition(int index)
        {
            return joinIndexToOffset(index) + (Vector2)getFinalPosition();
        }

        void Start()
        {
            thisId = objectId++;
            if (joinedObjects == null)
            {
                mountPointObjects = new JoinPoint[mountPoints.Length];
                joinedObjects = new EditorPiece[mountPoints.Length];
                fireKeys = new List<char>();
            }
            createJoinPoints();
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

        public void setJoinedObject(int joinId, EditorPiece piece)
        {
            int id = joinIdToIndex(joinId);
            joinedObjects[id] = piece;
        }

        public void unsetJoinedObject(int joinId)
        {
            int id = joinIdToIndex(joinId);
            joinedObjects[id] = null;
        }

        public void setJoinedObjects(EditorPiece[] g)
        {
            joinedObjects = g;
        }
        public EditorPiece[] getJoinedObjects()
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
            //Debug.Log("Got Id : " + id + " becomes... " + joinIdToIndex(id) + " size : " + mountPointObjects.Length);
            return mountPointObjects[joinIdToIndex(id)];
        }


        public void movePiece(Vector3 offset)
        {
            transform.position += offset;
        }
        IEnumerator Wait(float seconds, System.Action a)
        {
            print('1');
            yield return new WaitForSeconds(seconds);
            print('2');
            a();
        }
        public void moveToJoinPoint(int myJoinId, int toJoinId, EditorPiece to)
        {
            print("===============================================");
            double currentRotation = (double)gameObject.transform.eulerAngles.z;
            moveToJoinPoint(myJoinId, toJoinId, to, null);
        }
        public void moveToJoinPoint(int myJoinId, int toJoinId, EditorPiece to, List<EditorPiece> moved)
        {
            if (moved == null)
                moved = new List<EditorPiece>();
            if (!moved.Contains(this))
            {
                int myIndex = joinIdToIndex(myJoinId);
                int toIndex = joinIdToIndex(toJoinId);
                double ang = joinIndexToAngle(myIndex);
                double angTo = to.joinIndexToAngle(toIndex);
                double currentRotation = (double)getFinalRotation().z;
                double currentToRotation = (double)to.getFinalRotation().z;
                print("--------------------------");
                print(currentRotation);
                print("myJoinId=" + myJoinId);
                print("toJoinId=" + toJoinId);
                //		print (myIndex);
                //		print (toIndex);
                //		print (ang);
                //		print (angTo);
                //		print (currentRotation);
                //		print (currentToRotation);
                float newAngle = (float)((((ang) - (angTo - currentToRotation) + 180) + 1080) % 360);
                print("=" + myIndex + " " + newAngle + " ");
                //print (newAngle);
                onTheMovePos = (Vector3)(to.joinIndexToPosition(toIndex) - joinIndexToOffsetWithAngle(myIndex, newAngle));
                onTheMoveAngle = new Vector3(0, 0, newAngle);
                onTheMove = true;

                iTween.MoveTo(gameObject, iTween.Hash("position", onTheMovePos, "time", 1, "oncomplete", "clearOnTheMoveValues", "oncompletetarget", gameObject));
                iTween.RotateTo(gameObject, onTheMoveAngle, 1);

                moved.Add(this);
                moved.Add(to);
                //StartCoroutine(Wait(0.01f, delegate() {
                for (int i = 0; i < joinedObjects.Length; i++)
                {
                    if (mountPointObjects[i] != null && mountPointObjects[i].isJoined())
                    {
                        joinedObjects[i].moveToJoinPoint(mountPointObjects[i].getOtherJoinId(), mountPointObjects[i].getId(), this, moved);
                    }
                }
                //}));
            }
        }
        public void clearOnTheMoveValues()
        {
            onTheMove = false;
        }
        //	void updateJoinedPos(EditorPiece joined){
        //		List<EditorPiece> moved = new List<EditorPiece> ();
        //		moved.Add (this);
        //		moved.Add (joined);
        //		for (int i = 0; i < joinedObjects.Length; i++) {
        //			if(joinedObjects[i] != null)
        //				joinedObjects[i].updateJoinedPos(indexToJoinId(i), moved);
        //		}
        //	}
        //
        //	void updateJoinedPos(int joinId, List<EditorPiece> moved){
        //		if(!moved.Contains(this)){
        //			moved.Add(this);
        //			for (int i = 0; i < joinedObjects.Length; i++) {
        //				if(joinedObjects[i] != null){
        //					moveToJoinPoint(joinId, indexToJoinId(i), joinedObjects[i]);
        //					joinedObjects[i].updateJoinedPos(indexToJoinId(i), moved);
        //				}
        //			}
        //		}
        //	}

        public bool isConnected(EditorPiece to)
        {
            return isConnected(to, new HashSet<EditorPiece>());
        }

        bool isConnected(EditorPiece to, HashSet<EditorPiece> moved)
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
        public List<EditorPiece> getConnected()
        {
            HashSet<EditorPiece> components = new HashSet<EditorPiece>();
            this.getConnected(components);
            return components.ToList();
        }

        void getConnected(HashSet<EditorPiece> components)
        {
            if (!components.Contains(this))
            {
                components.Add(this);
                for (int i = 0; i < joinedObjects.Length; i++)
                {
                    if (joinedObjects[i] != null)
                        joinedObjects[i].getConnected(components);
                }
            }
        }
        public void setLinkColour()
        {
            hideJoinPoints();
            if (fireable)
            {
                char[] keys = this.getFireKey();
                if (keys.Length > 0 && keys[0] != 0)
                    this.gameObject.GetComponent<Image>().color = goodColour;
                else
                    this.gameObject.GetComponent<Image>().color = issueColour;
            }
            linkMode = true;
        }
        public void unsetLinkColour()
        {
            if (fireable)
            {
                this.gameObject.GetComponent<Image>().color = noColour;
            }
            linkMode = false;
        }
        public int countJoinedOjects()
        {
            return countJoinedOjects(new List<EditorPiece>());
        }

        int countJoinedOjects(List<EditorPiece> moved)
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
            offsetGroup(offset, new List<EditorPiece>());
        }

        void offsetGroup(Vector3 offset, List<EditorPiece> moved)
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

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            if (didDrag == false)
            {
                if (linkMode)
                    GameObject.Find("EditorView").GetComponent<EditorDropPanel>().editorPieceClicked(this);
                else
                {
                    GameObject.Find("EditorView").GetComponent<EditorDropPanel>().editorPieceClicked(this);
                    showJoinPoints();
                }
            }
            didDrag = false;
        }

        #endregion

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData)
        {
            itemBeingDragged = this;
            startPosition = transform.position;
            startParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            Debug.Log(this.countJoinedOjects());

        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData)
        {
            didDrag = true;
            Vector3 offset = (Vector3)eventData.position - transform.position;
            offsetGroup(offset);
            this.gameObject.SetActive(true);
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData)
        {
            itemBeingDragged = null;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (transform.parent != startParent)
            {
                transform.position = startPosition;
            }
        }

        #endregion
    }
}