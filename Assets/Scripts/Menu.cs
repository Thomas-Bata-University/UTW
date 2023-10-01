using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject lobbyPanel;
    public GameObject clientPanel;
    public GameObject hostPanel;
    public InputField ip;


    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        /*
        lobbyPanel.SetActive(false);
        hostPanel.SetActive(false);
        clientPanel.SetActive(false);
        */
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved= false;
        var password = System.Text.Encoding.ASCII.GetString(request.Payload);
        if (password == "kombajn") response.Approved = true;

        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        response.Position = Vector3.zero;
        response.Rotation = Quaternion.identity;
        response.Reason = "Wrong password";
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        /*
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        hostPanel.SetActive(true);
        */
        
        SceneManager.LoadScene("ShardScene");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void Join()
    {
        /*
        if (ip.text.Length > 0)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(); = ip.text;
        }
        else
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1");
        }
        */
        
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("kombajn");

        NetworkManager.Singleton.StartClient();
        /*
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        clientPanel.SetActive(true);
        */
        
        SceneManager.LoadScene("ShardScene");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartGame()
    {
        LobbyPanelClientRpc();
    }
 

    [ClientRpc]
    void LobbyPanelClientRpc()
    {
        //lobbyPanel.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Tank");
        Debug.Log("Found: " + players.Length);
        GameObject player = null;
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkObject>().IsLocalPlayer) player = players[i];
        }
        if (player != null)
        {
            HullAssembly ha = (HullAssembly)player.GetComponent(typeof(HullAssembly));
        }
        else Debug.Log("Failed to run AssemblyServerRPC!");
    }
}
