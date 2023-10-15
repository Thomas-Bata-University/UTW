using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public void HostGameDialog() {
        SceneManager.LoadScene(GameSceneUtils.LOBBY_SCENE);
    }
    public void GarageDialog() {
        SceneManager.LoadScene(GameSceneUtils.GARAGE_SCENE);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Disconnect() {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(GameSceneUtils.MAIN_MENU_SCENE);
    }

}
