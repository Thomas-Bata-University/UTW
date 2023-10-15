using Unity.Netcode;
using UnityEngine;

public class MultiplayerUtils : MonoBehaviour {

    /// <summary>
    /// Settings for sending message to specific player.
    /// </summary>
    /// <param name="id">ID of the player to whom the message will be sent.</param>
    /// <returns></returns>
    public static ClientRpcParams ClientRpcParams(ulong id) {
        return new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { id }
            }
        };
    }

}
