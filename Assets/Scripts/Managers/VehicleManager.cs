using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class VehicleManager : NetworkBehaviour {

    private void Awake() {
        PresetDropdown.OnPresetChange += SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby += Destroy;
    }

    [ServerRpc]
    public void SpawnTank(NetworkConnection conn, Preset preset) {
        if (FindObjectOfType<LobbyManager>().CanSpawnTank(conn)) {
            Debug.Log($"Tank spawned for client ID: {conn.ClientId} preset: {preset}");
        } else {
            SpawnTankResponse(conn, "You must select spawnpoint to spawn tank");
        }
    }

    [TargetRpc]
    private void SpawnTankResponse(NetworkConnection conn, string reason) {
        Debug.Log(reason);
    }

    private void Destroy(NetworkConnection conn) {
        if (Owner != conn) return;
        Despawn(gameObject);
    }

    private void OnDestroy() {
        PresetDropdown.OnPresetChange -= SpawnTank;
    }

}
