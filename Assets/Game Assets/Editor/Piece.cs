using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace StarBattles{
    [System.Serializable]
    public class Piece : MonoBehaviour
    {
        PieceData pieceData;
        Piece[] joinedObjects;

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

        public Piece(EditorPiece ep)
        {
            pieceData = new PieceData(ep);
            //joinedObjects = new Piece[mountPoints.Length];

        }
        public Piece(PieceData dp)
        {
            pieceData = dp;
            //joinedObjects = new Piece[mountPoints.Length];

        }

        //	void Update(){
        //		gameObject.transform.Rotate(new Vector3(0,0,0.1f));
        //		hideJoinPoints ();
        //		showJoinPoints();
        //	}

        public void movePiece(Vector3 offset)
        {
            transform.position += offset;
        }
        public bool isConnected(Piece to)
        {
            return isConnected(to, new List<Piece>());
        }

        bool isConnected(Piece to, List<Piece> moved)
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
            return countJoinedOjects(new List<Piece>());
        }

        int countJoinedOjects(List<Piece> moved)
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
            offsetGroup(offset, new List<Piece>());
        }

        void offsetGroup(Vector3 offset, List<Piece> moved)
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