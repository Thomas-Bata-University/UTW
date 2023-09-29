using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string lobbyScene;
    public string garageScene;

    public void HostGameDialog()
    {
        SceneManager.LoadScene(lobbyScene);
    }
    public void GarageDialog()
    {
        SceneManager.LoadScene(garageScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
