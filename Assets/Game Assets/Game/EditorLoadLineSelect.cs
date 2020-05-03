using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
namespace StarBattles
{
    public class EditorLoadLineSelect : MonoBehaviour, IPointerClickHandler
    {
        static Color selected = new Color(1, 1, 1, 0.66f);
        static Color unselected = new Color(1, 1, 1, 0.33f);
        public void OnPointerClick(PointerEventData eventData)
        {
            select();
            EditorDropPanel.loadLineSelect(this);
        }
        public void select()
        {
            gameObject.GetComponent<Image>().color = selected;
        }
        public void unselect()
        {
            gameObject.GetComponent<Image>().color = unselected;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}