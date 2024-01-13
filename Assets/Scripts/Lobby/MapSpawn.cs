//using Den.Tools.SceneEdit;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSpawn : MonoBehaviour
{
   
    public GameObject spawnPoint;
    string mapname;

    public bool spawnpoints = true;
    // Start is called before the first frame update
    void Start()
    {
        mapname = "Greenmap";   //zde m�sto Greenmap budeme na��tat jm�no mapy, kterou chceme zobrazit
        Instantiate(Resources.Load("maps/" + mapname), transform.position, Quaternion.identity);
        if (spawnpoints == true)
        {
            GameObject place;
            GameObject spwn_inst;
            Boolean done = false;
            int i = 1;
            while (!done)
            {
                place = GameObject.Find("Spawn" + i.ToString());
                if (place != null)
                {
                    spwn_inst = Instantiate(spawnPoint, place.transform.position, Quaternion.identity);
                    spwn_inst.name = i.ToString();
                    i++;
                }
                else
                {
                    done = true;
                }

            }


        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
