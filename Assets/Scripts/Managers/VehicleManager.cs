using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class VehicleManager : NetworkBehaviour {

    private void Awake() {
        PresetDropdown.OnPresetChange += SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby += Destroy;
    }

    public void SpawnTank(NetworkConnection conn, Preset preset) {
        if (!IsOwner) return;
        SpawnTankServerRpc(conn, preset);
    }

    [ServerRpc]
    public void SpawnTankServerRpc(NetworkConnection conn, Preset preset) {
        if (FindObjectOfType<LobbyManager>().CanSpawnTank(conn)) {
            //TODO spawn tank
            Debug.Log($"Tank spawned for client ID: {conn.ClientId} preset: {preset}");
            LogResponse(conn, $"Tank spawned for client ID: {conn.ClientId} preset: {preset}");
        } else {
            Debug.Log($"Tank spawned for client ID: {conn.ClientId} preset: {preset}");
            LogResponse(conn, "You must select spawnpoint to spawn tank");
        }
    }

    private void Destroy(NetworkConnection conn) {
        if (Owner != conn) return;
        Despawn(gameObject);
    }

    [TargetRpc]
    public void LogResponse(NetworkConnection conn, string text) {
        Debug.Log(text);
    }

    private void OnDestroy() {
        PresetDropdown.OnPresetChange -= SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby -= Destroy;
    }

}
