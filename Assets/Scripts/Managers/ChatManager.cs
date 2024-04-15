using FishNet.Object;
using System;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    private Chat _chat;
    private string _messageContent = "";

    private string username = "";

    private Vector2 scrollPosition;

    private void Start()
    {
        if (LocalConnection == Owner)
        {
            _chat = FindAnyObjectByType<Chat>(FindObjectsInactive.Include);

            SetUsername(PlayerPrefs.GetString("username"));
        }
    }

    private void OnGUI()
    {
        if (!IsOwner) return;

        float width = Screen.width * 0.250f;
        float height = Screen.height * 0.270f;
        float xPos = Screen.width - width;
        float yPos = (Screen.height - height) / 2.6f;

        GUILayout.BeginArea(new Rect(xPos, yPos, width, height));

        GUILayout.BeginHorizontal();

        _messageContent = GUILayout.TextField(_messageContent);

        if (GUILayout.Button("Send"))
        {
            _chat.ServerPostMessage(LocalConnection, DateTime.Now, username, _messageContent);

            // Clearing the textbox
            _messageContent = "";
        }

        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(width), GUILayout.Height(height - 50));

        for (int i = 0; i < _chat.Messages.Count; i++)
        {
            ChatMessage message = _chat.Messages[i];

            GUILayout.BeginHorizontal();

            GUILayout.Label($"{message.Sender} ({message.Timestamp:T})\n{message.Content}");

            // Do this when we have SteamIDs
            //if (message.Sender.Equals(username) && GUILayout.Button("Delete"))
            //    _chat.ServerDeleteMessage(i);

            if (message.Connection == LocalConnection && GUILayout.Button("Delete"))
                _chat.ServerDeleteMessage(i);

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void SetUsername(string newUsername)
    {
        username = newUsername;
    }
}
