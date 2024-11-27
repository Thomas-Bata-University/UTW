using FishNet.Object;
using UnityEngine;

public class SetCamera : NetworkBehaviour
{
    public GameObject cameraPivot;

    private void Start()
    {
        //if (!NetworkManager.Singleton.IsClient) return;
        if (transform.parent.gameObject.GetComponent<NetworkObject>().IsOwner)
        {
            cameraPivot.SetActive(true);
        }
    }
}
