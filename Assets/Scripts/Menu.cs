using FishNet;
using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private Tugboat GetTugboat()
    {
        if (InstanceFinder.NetworkManager.TryGetComponent(out Tugboat t))
            return t;
        else
            return null;
    }

    private const string sAddress = "10.5.8.71";
    private const ushort sPort = 7770;
    public void JoinOfficialShard()
    {
        if (!HasUsername())
            return;

        var tugboat = GetTugboat();

        if (tugboat == null)
        {
            Debug.LogError("Couldn't find Tugboat component!");
            return;
        }

        tugboat.SetClientAddress(sAddress);
        tugboat.SetPort(sPort);

        tugboat.StartConnection(false);
    }

    public void DirectConnect()
    {
        if (!HasUsername())
            return;

        var tugboat = GetTugboat();

        if (tugboat == null)
        {
            Debug.LogError("Couldn't find Tugboat component!");
            return;
        }

        tugboat.SetClientAddress("127.0.0.1");

        tugboat.StartConnection(false);
    }

    private bool HasUsername()
    {
        if ((GameObject.Find("UsernameInputText").GetComponent<TMP_Text>().text.Length - 1) == 0)
        {
            Debug.Log("Please provide a username before joining a server");
            return false;
        }

        return true;
    }

    public void SwitchToMainMenu()
    {
        SceneManager.LoadScene(GameSceneUtils.MAIN_MENU_SCENE);
    }

    public void SwitchToHelpScene()
    {
        SceneManager.LoadScene(GameSceneUtils.HELP_SCENE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}