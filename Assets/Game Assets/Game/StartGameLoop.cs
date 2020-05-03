using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
namespace StarBattles
{
    public class StartGameLoop : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        public void LoadEditor()
        {
            Debug.Log("Editor Game");
            SceneManager.LoadScene("EditorScene");
        }

        public void LoadGame()
        {
            Debug.Log("Main Game");
            if (SaveLoadShip.LoadList().Length > 0)
            {
                Debug.Log("Create a Ship First");
            }
            SceneManager.LoadScene("MainGame");
            //	Application.LoadLevel(1);
        }
    }
}