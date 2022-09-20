using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace StarBattles
{
    public class StartGameLoop : MonoBehaviour
    {
        public void LoadEditor()
        {
            Debug.Log("Editor Game");
            Scenes.Load("EditorScene");
        }

        public void LoadGame()
        {
            Debug.Log("Main Game");
            if (SaveLoadShip.LoadList().Length > 0)
            {
                Debug.Log("Create a Ship First");
            }
            Scenes.Load("MainGame");
            //	Application.LoadLevel(1);
        }
    }
}