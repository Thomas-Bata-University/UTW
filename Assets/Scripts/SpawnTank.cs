using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject place;
        string selected = "Spawn" + PlayerPrefs.GetString("Spawn");
        place = GameObject.Find(selected);  //m�me spawnpoint, m��eme na jeho .transform.position um�stit tank

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
