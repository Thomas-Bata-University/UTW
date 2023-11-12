using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapSpawn : MonoBehaviour
{
   
    public GameObject spawnPoint;
    string mapname;
    // Start is called before the first frame update
    void Start()
    {
        mapname = "Greenmap";   //zde místo Greenmap budeme posílat jméno mapy, kterou chceme zobrazit
        Instantiate(Resources.Load("maps/"+mapname), transform.position, Quaternion.identity); ;
        GameObject place;
        GameObject spwn_inst;
        Boolean done = false;
        int i = 1;
        while (!done)
        {
            place = GameObject.Find("Spawn" + i.ToString());
            if(place != null)
            {
                spwn_inst = Instantiate(spawnPoint,place.transform.position, Quaternion.identity);
                spwn_inst.name = i.ToString();
                i++;
            }
            else
            {
                done = true;
            }
 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
