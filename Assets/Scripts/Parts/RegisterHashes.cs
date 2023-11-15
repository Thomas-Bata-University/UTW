using FishNet.Managing;
using UnityEngine;

public class RegisterHashes : MonoBehaviour
{
    private GameObject _assetDb;
    public Database dbComponent;
    public NetworkManager nManager;

    public void GetAssetDb()
    {
        {
            nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

            _assetDb = GameObject.Find("AssetDatabase");
            
            dbComponent = _assetDb.GetComponent(typeof(Database)) as Database;
            
            /*
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
            */
        }
    }
}
