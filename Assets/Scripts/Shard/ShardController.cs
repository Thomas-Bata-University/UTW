using FishNet;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShardController : MonoBehaviour {

    [SerializeField]
    private UTW.SceneManager sceneManager;

    [Header("UI")]
    public Button buttonPrefab;
    public GridLayoutGroup layoutGroup;
    public TextMeshProUGUI statusText;

    public void CreateLobby() {
        UTW.SceneManager.Instance.CreateLobby(InstanceFinder.ClientManager.Connection);
    }

    public void ConnectToLobby(int handle) {
        UTW.SceneManager.Instance.ConnectToLobby(InstanceFinder.ClientManager.Connection, handle);
    }

    public void RefreshLobby() {
        if (InstanceFinder.IsServer) return;

        ChangeStatusText("Refreshing...");
        ClearButtons();
        UTW.SceneManager.Instance.GetLobbyData(InstanceFinder.ClientManager.Connection);
    }

    public void CreateLobbyButtons(Dictionary<int, SceneData> lobbyData) {
        foreach (var sceneData in lobbyData) {
            CreateButton(sceneData.Value);
        }

        if (lobbyData.Count > 0) {
            ChangeStatusText("");
        } else {
            ChangeStatusText("No Lobby found.");
        }
    }

    private void ClearButtons() {
        for (int i = 0; i < layoutGroup.transform.childCount; i++) {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }
    }

    private void CreateButton(SceneData sceneData) {
        Button button = Instantiate(buttonPrefab, layoutGroup.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{sceneData.sceneName} - players: {sceneData.playerCount}";
        button.onClick.AddListener(() => ConnectToLobby(sceneData.handle));
    }

    private void ChangeStatusText(string text) {
        statusText.text = text;
    }

    public void ExitGame() {
        Application.Quit();
    }

}
