using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
namespace StarBattles{
    public class JoinPoint : MonoBehaviour, IPointerClickHandler
    {
        public static JoinPoint otherSelectedJoin;
        bool joined = false;
        int otherJoinId;
        JoinPoint otherJoin;
        Image image;
        int id;
        Color selectedColour = new Color(0, 255, 0, 255);
        Color unselectedColour = new Color(0, 255, 255, 255);
        Color joinColour = new Color(0, 0, 155, 255);
        Color removeColour = new Color(255, 0, 0, 255);
        EditorPiece parentItem;

        public bool isJoined()
        {
            return joined;
        }
        public int getId()
        {
            return id;
        }
        public int getOtherJoinId()
        {
            return otherJoinId;
        }

        void Start()
        {
            parentItem = GetComponentInParent<EditorPiece>();
            image = GetComponent<Image>();
            //Assign Color
            if (image != null)
            {
                if (joined)
                    image.color = joinColour;
                else
                    image.color = unselectedColour;
            }
        }

        public void setId(int _id)
        {
            id = _id;
        }

        public void setJoined(bool _joined, JoinPoint other = null)
        {
            joined = _joined;
            if (joined)
            {
                otherJoinId = other.id;
                otherJoin = other;
            }
            else
            {
                otherJoinId = 0;
                otherJoin = null;
            }
            if (image != null)
            {
                if (joined)
                    image.color = joinColour;
                else
                    image.color = unselectedColour;
            }
        }

        void selectedThis()
        {
            print(joined);
            if (joined)
            {
                image.color = removeColour;
                otherSelectedJoin = this;
                hideAllJoinPointsExcept();
            }
            else
            {
                if (otherSelectedJoin)
                    otherSelectedJoin.unselectThis();
                otherSelectedJoin = this;
                image.color = selectedColour;
                showAllJoinPoints();
                hideAllJoinedPoints();
                hideAllConnectedJoinPoints(this.getId());
            }
        }

        public void show()
        {
            this.gameObject.SetActive(true);
            //rend = this.GetComponent<Renderer>();
            //rend.enabled = true;
        }

        public void hide()
        {
            this.gameObject.SetActive(false);
            //rend = GetComponent<Renderer>();
            //rend.enabled = false;
        }

        public void unselectThis()
        {
            if (image != null)
            {
                if (joined)
                    image.color = joinColour;
                else
                    image.color = unselectedColour;
            }
            otherSelectedJoin = null;

        }

        void unJoin()
        {
            otherJoin.parentItem.unsetJoinedObject(otherJoin.id);
            parentItem.unsetJoinedObject(id);
            otherJoin.setJoined(false);
            setJoined(false);
            otherSelectedJoin = null;
            image.color = unselectedColour;
        }

        void join(JoinPoint other, bool andMove = true)
        {
            if (!other.parentItem.isConnected(parentItem))
            { // Todo: Self circular join
                if (andMove)
                    other.parentItem.moveToJoinPoint(other.id, id, parentItem);
                other.parentItem.setJoinedObject(other.id, parentItem);
                parentItem.setJoinedObject(id, otherSelectedJoin.parentItem);
                setJoined(true, other);
                other.setJoined(true, this);
                otherSelectedJoin = null;
            }
        }
        static void showAllJoinPoints()
        {
            foreach (EditorPiece p in EditorPiece.FindObjectsOfType<EditorPiece>())
            {
                p.showJoinPoints();
            }
        }
        void hideAllJoinPointsExcept()
        {
            foreach (EditorPiece p in EditorPiece.FindObjectsOfType<EditorPiece>())
            {
                if (!p.Equals(parentItem))
                    p.hideJoinPoints();
            }
        }

        void hideAllConnectedJoinPoints(int except = 0)
        {
            foreach (EditorPiece p in EditorPiece.FindObjectsOfType<EditorPiece>())
            {
                if (p.isConnected(parentItem))
                    p.hideJoinPoints(except);
            }
        }
        static void showAllJoinedPoints()
        {
            foreach (EditorPiece p in EditorPiece.FindObjectsOfType<EditorPiece>())
            {
                p.showJoinedPoints();
            }
        }

        static void hideAllJoinedPoints()
        {
            foreach (EditorPiece p in EditorPiece.FindObjectsOfType<EditorPiece>())
            {
                p.hideJoinedPoints();
            }
        }

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            if (otherSelectedJoin == null)
            { // Select this
                selectedThis();
            }
            else if (otherSelectedJoin == this)
            { // What to do when this clicked twice
                if (joined)
                {
                    //otherSelectedJoin = null;
                    // Remove link
                    unJoin();
                    unselectThis();
                    parentItem.showJoinPoints();
                }
                else
                {
                    unselectThis();
                    parentItem.showJoinPoints();
                }
            }
            else
            { // Other join is clicked
                join(otherSelectedJoin);
                showAllJoinPoints();
            }
        }

        #endregion
    }
}