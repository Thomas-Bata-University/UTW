using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public string gameScene;

        public void Ready()
        {
            SceneManager.LoadScene(gameScene);
        }

        public void Back()
        {
            Application.Quit();
        }
    }
}