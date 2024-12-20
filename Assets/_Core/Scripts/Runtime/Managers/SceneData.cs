using FishNet.Connection;
using System.Collections.Generic;

public class SceneData {

    public int handle;
    public string sceneName;
    public string lobbyName;
    public int playerCount;
    public LobbyState lobbyState = LobbyState.OPEN;
    public NetworkConnection lobbyOwner;
    public List<NetworkConnection> clients = new List<NetworkConnection>();
    public LobbyManager lobbyManager;

    public SceneData() {

    }

    public SceneData(int handle, string sceneName, string lobbyName, NetworkConnection conn) {
        this.handle = handle;
        this.sceneName = sceneName;
        this.lobbyName = lobbyName;
        this.playerCount = 0;
        this.lobbyOwner = conn;
    }

    public override string ToString() {
        return $"Handle: {handle} | Scene name: {sceneName} | Player count: {playerCount}";
    }

}
