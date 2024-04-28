using FishNet;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShardController : MonoBehaviour
{

    [SerializeField]
    private UTW.SceneManager sceneManager;

    [Header("UI")]
    public Button buttonPrefab;
    public GridLayoutGroup layoutGroup;
    public TextMeshProUGUI statusText;

    private void Start()
    {
        ChangeStatusText("Refreshing...");
        StartCoroutine(LateStart());
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
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{sceneData.sceneName} - players: {sceneData.playerCount}";
        button.onClick.AddListener(() => ConnectToLobby(sceneData.handle));
    }

    private void ChangeStatusText(string text)
    {
        statusText.text = text;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
