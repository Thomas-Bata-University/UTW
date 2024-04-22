using FishNet.Transporting.Tugboat;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject networkManager;
    [SerializeField] private GameObject presetManager;
    private Tugboat _tugboat;

    private void Start()
    {
        if (networkManager.TryGetComponent(out Tugboat t))
        {
            _tugboat = t;
        }
        else
        {
            Debug.LogError("Couldn't find Tugboat component!");
        }
    }

    private const string sAddress = "10.5.8.71";
    private const ushort sPort = 7770;
    public void JoinOfficialShard()
    {
        _tugboat.SetClientAddress(sAddress);
        _tugboat.SetPort(sPort);

        _tugboat.StartConnection(false);
    }

    public void DirectConnect()
    {
        _tugboat.SetClientAddress("127.0.0.1");

        _tugboat.StartConnection(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
