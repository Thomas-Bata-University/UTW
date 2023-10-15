<<<<<<< HEAD
using FishNet.Transporting.Tugboat;
=======
using Unity.Netcode;
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

<<<<<<< HEAD
<<<<<<< HEAD
public class Menu : MonoBehaviour
{
    //public GameObject menuPanel;
    //public GameObject lobbyPanel;
    //public GameObject clientPanel;
    //public GameObject hostPanel;
    //public InputField ip;
    
    [SerializeField] private GameObject networkManager;
    private Tugboat _tugboat;
    
    private void Start()
    {
        //NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
=======
public class Menu : MonoBehaviour {
=======
public class Menu : NetworkBehaviour {
>>>>>>> 2f2b2f3 (Server client connection check)
    public GameObject menuPanel;
    public GameObject lobbyPanel;
    public GameObject clientPanel;
    public GameObject hostPanel;
    public InputField ip;

    [SerializeField]
    private PresetManager presetManager;

    private void Start() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
<<<<<<< HEAD
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
=======
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisonnected;
>>>>>>> 2f2b2f3 (Server client connection check)
        /*
        lobbyPanel.SetActive(false);
        hostPanel.SetActive(false);
        clientPanel.SetActive(false);
        */

        if (networkManager.TryGetComponent(out Tugboat t))
        {
            _tugboat = t;
        }
        else
        {
            Debug.LogError("Couldn't find Tugboat component!");
        }
    }

<<<<<<< HEAD
    /*
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved= false;
=======
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        response.Approved = false;
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        var password = System.Text.Encoding.ASCII.GetString(request.Payload);
        if (password == "kombajn") response.Approved = true;

        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        response.Position = Vector3.zero;
        response.Rotation = Quaternion.identity;
        response.Reason = "Wrong password";
    }
<<<<<<< HEAD
*/
    
    public void Host()
    {
        _tugboat.StartConnection(true);
        _tugboat.StartConnection(false);

        //NetworkManager.Singleton.StartHost();
        SceneManager.LoadScene("ShardScene");
=======

    private void ClientConnected(ulong playerId) {
        Debug.Log($"Playerd connected to the server with ID: {playerId}");
        if (IsServer) return;
        presetManager.LoadPresetServerRpc(playerId);
    }

    private void ClientDisonnected(ulong playerId) {
        Debug.Log($"Playerd disconnected from the server with ID: {playerId}");
    }

    public void Host() {
        NetworkManager.Singleton.StartHost();
        /*
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        hostPanel.SetActive(true);
        */
        SceneManager.LoadScene(GameSceneUtils.SHARD_SCENE);
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Join() {
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
<<<<<<< HEAD
        
        //NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("kombajn");
=======

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("kombajn");
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)

        //NetworkManager.Singleton.StartClient();
        /*
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        clientPanel.SetActive(true);
        */
<<<<<<< HEAD
<<<<<<< HEAD
        
        
        _tugboat.StartConnection(false);
        SceneManager.LoadScene("ShardScene");
    }
/*
    public void MainMenu()
    {
       // SceneManager.LoadScene("MainMenuScene");
=======

=======
>>>>>>> 2f2b2f3 (Server client connection check)
        SceneManager.LoadScene(GameSceneUtils.SHARD_SCENE);
    }
    public void MainMenu() {
        SceneManager.LoadScene(GameSceneUtils.MAIN_MENU_SCENE);
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
    }

    public void StartGame() {
        LobbyPanelClientRpc();
    }


<<<<<<< HEAD
    [ObserversRpc]
    void LobbyPanelClientRpc()
    {
=======
    [ClientRpc]
    void LobbyPanelClientRpc() {
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        //lobbyPanel.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Tank");
        Debug.Log("Found: " + players.Length);
        GameObject player = null;
<<<<<<< HEAD
        for(int i = 0; i < players.Length; i++)
        {
            //if (players[i].GetComponent<NetworkObject>().IsLocalPlayer) player = players[i];
=======
        for (int i = 0; i < players.Length; i++) {
            if (players[i].GetComponent<NetworkObject>().IsLocalPlayer) player = players[i];
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
        }
        if (player != null) {
            HullAssembly ha = (HullAssembly)player.GetComponent(typeof(HullAssembly));
        } else Debug.Log("Failed to run AssemblyServerRPC!");
    }
<<<<<<< HEAD
    */
=======

>>>>>>> 2f2b2f3 (Server client connection check)
}
