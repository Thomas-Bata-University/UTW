using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string _ConnectScene;
    public string _GarageScene;

    public void HostGameDialog()
    {
        SceneManager.LoadScene(_ConnectScene);
    }
    public void GarageDialog()
    {
        SceneManager.LoadScene(_GarageScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
