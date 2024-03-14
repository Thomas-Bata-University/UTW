using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleManager : NetworkBehaviour
{
    public GameObject playerPrefab;
    private GameObject actualTank;
    private Transform actualPos;

    private void Awake()
    {
        PresetDropdown.OnPresetChange += SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby += Destroy;
        LobbyManager.onSpawnpointChanged += SetPosition;
    }

    private void SpawnTank(NetworkConnection conn, Preset preset)
    {
        if (!IsOwner) return;
        SpawnTankServerRpc(conn, preset, actualPos, actualTank);
    }

    [ServerRpc]
    private void SpawnTankServerRpc(NetworkConnection conn, Preset preset, Transform actualPos, GameObject actualTank)
    {
        if (actualPos is not null && actualTank is null)
        {
            actualTank = Instantiate(playerPrefab, actualPos.position, Quaternion.identity);
            Spawn(actualTank, conn, gameObject.scene);

            SetSpawnedTank(conn, actualTank);

            Debug.Log($"Tank spawned for client ID: {conn.ClientId} preset: {preset} at spawnpoint: {actualPos.gameObject.name} ({actualPos.position})");
            LogResponse(conn, $"Tank spawned for client ID: {conn.ClientId} preset: {preset} at spawnpoint: {actualPos.gameObject.name} ({actualPos.position})");
        }
        else
        {
            Debug.Log($"You must select spawnpoint to spawn tank");
            LogResponse(conn, "You must select spawnpoint to spawn tank");
        }
    }

    private void SetPosition(Transform newPos, bool isLocked)
    {
        if (isLocked)
            return;

        actualPos = newPos;

        if (actualTank is null)
            return;

        actualTank.transform.position = newPos.position;
    }

    private void Destroy(NetworkConnection conn)
    {
        if (Owner != conn) return;
        Despawn(gameObject);
    }

    [TargetRpc]
    public void LogResponse(NetworkConnection conn, string text)
    {
        Debug.Log(text);
    }

    [TargetRpc]
    public void SetSpawnedTank(NetworkConnection conn, GameObject spawnedTank)
    {
        actualTank = spawnedTank;
    }

    [ServerRpc]
    private void DespawnTank(NetworkObject tankToDespawn)
    {
        tankToDespawn.Despawn();
    }

    private void OnDestroy()
    {
        Debug.Log("Destroying VM");

        PresetDropdown.OnPresetChange -= SpawnTank;
        UTW.SceneManager.OnClientDisconnectLobby -= Destroy;

        if (actualTank is not null)
            DespawnTank(actualTank.GetComponent<NetworkObject>());
    }
}
