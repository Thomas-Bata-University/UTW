using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using FishNet.Transporting;

using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class SpawnTank : NetworkBehaviour
{
    public GameObject player;
    [SerializeField]
    private VehicleManager vehicleManager;
    private SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject place;
        string selected = "Spawn" + PlayerPrefs.GetString("Spawn");
        place = GameObject.Find(selected);  //máme spawnpoint, mùžeme na jeho .transform.position umístit tank
                                            //VehicleManager.Instance.SpawnVehicle(InstanceFinder.ClientManager.Connection, place.transform);
      
        if (LocalConnection.IsLocalClient)
        {
            GameObject camera = Instantiate(player, place.transform.position, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
  /*      bool spawned = false;
        bool scene = false;
        if (spawned == false)
        {
            int countLoaded = UnityEngine.SceneManagement.SceneManager.sceneCount;
 
            for (int i = 0; i < countLoaded; i++)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name == GameSceneUtils.GAME_SCENE)
                {
                    scene = true;
                }
            }
                if (scene == true)
            {

                spawned = true;
            }
        }
        else
        {
            scene = true;
        }*/
    }
}