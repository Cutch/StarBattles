using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
namespace StarBattles
{
    public class EditorDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameObject toDrop;
        public static EditorDragDrop itemBeingDragged;
        public GameObject overlayObject;
        Vector3 startPosition;
        Transform startParent;

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData)
        {
            itemBeingDragged = this;
            overlayObject = Instantiate(this.gameObject);
            overlayObject.transform.SetParent(this.transform.GetComponentInParent<Transform>());
            startPosition = this.transform.position;
            startParent = this.transform.parent;
            overlayObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            overlayObject.AddComponent<Canvas>();
            overlayObject.GetComponent<Canvas>().overrideSorting = true;
            overlayObject.GetComponent<Canvas>().sortingOrder = 6;
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData)
        {
            overlayObject.transform.position = eventData.position;
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(overlayObject);
            itemBeingDragged = null;
            //if (transform.parent == startParent)
            //{
            //    transform.position = startPosition;
            //}
        }

        #endregion

    }
}