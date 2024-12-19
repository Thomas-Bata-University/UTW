using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class LobbyController : MonoBehaviour
{
    public GameObject lobbyManagerPrefab;
    public GameObject menuPanel;
    public GameObject chatPrefab;
    public GameObject chatManagerPrefab;

    [Header("Hide after spawnpoint lock")]
    public GameObject presetDropdown;
    public GameObject readyButton;

    [Header("Swap")]
    public GameObject swapRequestPanel;
    public Image progressBar;
    public float countdownTime = 15f;
    private Coroutine swapCoroutine;

    [SerializeField] private GameObject startButton;
    private UTW.SceneManager sceneManager;
    private NetworkConnection conn;

    private void Awake()
    {
        if (InstanceFinder.IsServer) return;

        InstanceFinder.SceneManager.OnLoadEnd += SceneLoadEnd;
        sceneManager = UTW.SceneManager.Instance;
        conn = InstanceFinder.ClientManager.Connection;
        startButton.SetActive(false);
        swapRequestPanel.SetActive(false);
        HideObjects(true);
    }

    private void SceneLoadEnd(SceneLoadEndEventArgs args)
    {
        sceneManager.InitializeLobbyManagers(lobbyManagerPrefab, chatPrefab, conn);
        sceneManager.InitializeChatManager(chatManagerPrefab, conn);
        sceneManager.Connected(conn);
    }

    public void SetSwapData(VehicleManager vehicleManager, NetworkConnection requestConn, int key, int oldKey)
    {
        swapRequestPanel.SetActive(true);
        swapRequestPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Request from {GameManager.Instance.GetPlayerByConnection(requestConn.ClientId).PlayerName}";
        swapRequestPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Swap from {vehicleManager._tankCrew[key].tankPosition} to {vehicleManager._tankCrew[oldKey].tankPosition}?";
        swapCoroutine = StartCoroutine(StartCountdown(vehicleManager, requestConn, key, oldKey));
        OnClick(2).RemoveAllListeners();
        OnClick(3).RemoveAllListeners();
        OnClick(2).AddListener(() => Swap(vehicleManager, requestConn, key, oldKey, true));
        OnClick(3).AddListener(() => Swap(vehicleManager, requestConn, key, oldKey, false));
    }

    private ButtonClickedEvent OnClick(int childIndex)
    {
        return swapRequestPanel.transform.GetChild(childIndex).GetComponent<Button>().onClick;
    }

    private void Swap(VehicleManager vehicleManager, NetworkConnection requestConn, int key, int oldKey, bool swap)
    {
        StopCoroutine(swapCoroutine);
        vehicleManager.SwapRequestResponse(requestConn, key, oldKey, swap);
        swapRequestPanel.SetActive(false);
    }

    private IEnumerator StartCountdown(VehicleManager vehicleManager, NetworkConnection requestConn, int key, int oldKey)
    {
        float currentTime = countdownTime;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            progressBar.fillAmount = currentTime / countdownTime;
            yield return null;
        }
        Swap(vehicleManager, requestConn, key, oldKey, false);
    }

    public void SpawnpointReady()
    {
        HideObjects(true);
        FindObjectOfType<LobbyManager>().SetSpawnpointReady();
    }

    public void LeaveSpawnpoint()
    {
        HideObjects(true);
        FindObjectOfType<LobbyManager>().LeaveSpawnpoint(conn);
    }

    public void DisconnectFromLobby()
    {
        FindObjectOfType<LobbyManager>().LeaveSpawnpoint(conn);
        FindObjectOfType<RoundSystem>().OnClientDisconnectFromLobby(conn);
        sceneManager.Disconnect(conn);
    }

    public void ActivateStartButton()
    {
        startButton.SetActive(true);
    }

    public void StartGame()
    {
        sceneManager.StartGame(conn);
    }

    public void HideObjects(bool active)
    {
        presetDropdown.SetActive(!active);
        readyButton.SetActive(!active);
    }

    private void OnDestroy()
    {
        if (InstanceFinder.IsServer) return;
        InstanceFinder.SceneManager.OnLoadEnd -= SceneLoadEnd;
    }
}