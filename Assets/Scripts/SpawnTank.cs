using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTank : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        GameObject place;
        string selected = "Spawn" + PlayerPrefs.GetString("Spawn");
        place = GameObject.Find(selected);  //máme spawnpoint, mùžeme na jeho .transform.position umístit tank
        Instantiate(player, place.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
