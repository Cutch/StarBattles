using UnityEngine;
using System.Collections;
namespace StarBattles
{
    public class EditorGUI : MonoBehaviour
    {
        public Texture2D icon;

        void OnGUI()
        {
            // Make a background box
            //GUI.Box(new Rect(400,10,100,90), "Loader Menu");

            //if (GUI.Button (new Rect (510,10, 100, 50), icon)) {
            //	print ("you clicked the icon");
            //}

            //// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
            //if(GUI.Button(new Rect(410,40,80,20), "Level 1")) {
            //	Application.LoadLevel(1);
            //}

            //// Make the second button.
            //if(GUI.Button(new Rect(410,70,80,20), "Level 2")) {
            //	Application.LoadLevel(2);
            //}
        }
    }
}