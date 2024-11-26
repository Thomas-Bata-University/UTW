using FishNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShardController : MonoBehaviour
{
    [Header("UI")] public Button buttonPrefab;
    public GridLayoutGroup layoutGroup;
    public TextMeshProUGUI statusText;

    [SerializeField] private PopupWindow popup;

    private void Start()
    {
        if (InstanceFinder.NetworkManager.IsClient) {
            var player = GameManager.Instance.GetPlayerByConnection(InstanceFinder.ClientManager.Connection.ClientId);
            if (player.Faction == null) ChooseFaction(player);
        }

        ChangeStatusText("Refreshing...");
        StartCoroutine(LateStart());
    }

    private void ChooseFaction(PlayerData player)
    {
        var factions = GameManager.Instance.GetAllFactions();
        var options = factions.Select(faction => faction.Name).ToList();

        popup.Show(
            "Select Faction",
            "Choose your faction to begin:",
            options,
            selectedValue =>
            {
                GameManager.Instance.SetFactionForPlayer(
                    InstanceFinder.ClientManager.Connection,
                    player,
                    factions[options.IndexOf(selectedValue)]
                );
            }
        );
    }

    //TODO add timer for client to prevent lobby creation
    public void CreateLobby()
    {
        UTW.SceneManager.Instance.CreateLobby(InstanceFinder.ClientManager.Connection);
    }

    public void ConnectToLobby(int handle)
    {
        UTW.SceneManager.Instance.ConnectToLobby(InstanceFinder.ClientManager.Connection, handle);
    }

    public IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        RefreshLobby();
    }

    public void RefreshLobby()
    {
        if (InstanceFinder.IsServer) return;

        ChangeStatusText("Refreshing...");
        ClearButtons();
        UTW.SceneManager.Instance.GetLobbyData(InstanceFinder.ClientManager.Connection);
    }

    public void CreateLobbyButtons(Dictionary<int, SceneData> lobbyData)
    {
        var cnt = lobbyData.Count;

        foreach (var sceneData in lobbyData)
        {
            if (sceneData.Value.lobbyState == LobbyState.ONGOING)
            {
                cnt--;
                continue;
            }
                
            CreateButton(sceneData.Value);
        }

        if (cnt > 0)
        {
            ChangeStatusText("");
        }
        else
        {
            ChangeStatusText("No Lobby found.");
        }
    }

    private void ClearButtons()
    {
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }
    }

    private void CreateButton(SceneData sceneData)
    {
        Button button = Instantiate(buttonPrefab, layoutGroup.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{sceneData.lobbyName} - players: {sceneData.playerCount}";
        button.onClick.AddListener(() => ConnectToLobby(sceneData.handle));
    }

    private void ChangeStatusText(string text)
    {
        statusText.text = text;
    }

    private void Disconnect()
    {
        UTW.SceneManager.Instance.DisconnectFromShard(InstanceFinder.ClientManager.Connection);

        SceneManager.LoadScene(GameSceneUtils.MAIN_MENU_SCENE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}