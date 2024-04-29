using FishNet;
using FishNet.Transporting.Tugboat;
using UnityEngine;

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
        var tugboat = GetTugboat();

        if (tugboat == null)
        {
            Debug.LogError("Couldn't find Tugboat component!");
            return;
        }

        tugboat.SetClientAddress("127.0.0.1");

        tugboat.StartConnection(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}