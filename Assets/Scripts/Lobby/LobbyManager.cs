using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public string gameScene;
         
        public void Ready()
        {


            int x= 0; int y=0;
            SceneManager.LoadScene(gameScene);
        }

        public void Back()
        {
            Application.Quit();
        }
    }
}