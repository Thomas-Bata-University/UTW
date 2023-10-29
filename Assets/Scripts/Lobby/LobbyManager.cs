using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class LobbyManager : MonoBehaviour {

        public void Ready() {
            SceneManager.LoadScene(GameSceneUtils.GAME_SCENE);
        }

        public void Back() {
            Application.Quit();
        }
    }

}