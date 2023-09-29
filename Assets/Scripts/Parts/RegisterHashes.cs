using Unity.Netcode;
using UnityEngine;

public class RegisterHashes : MonoBehaviour
{
    private GameObject assetDb;
    public Database dbComponent;
    public NetworkManager nManager;

    public void GetAssetDb()
    {
        {
            nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

            assetDb = GameObject.Find("AssetDatabase");
            dbComponent = (Database)assetDb.GetComponent(typeof(Database));

            foreach(var hull in dbComponent.hulls)
            {
                var np = new NetworkPrefab();
                np.Prefab = hull;
                nManager.NetworkConfig.Prefabs.Add(np);
                Debug.Log("Loaded prefab: " + hull.name);
            }

            foreach (var turret in dbComponent.turrets)
            {
                var np = new NetworkPrefab();
                np.Prefab = turret;
                nManager.NetworkConfig.Prefabs.Add(np);
                Debug.Log("Loaded prefab: " + turret.name);
            }
        }
    }
}
