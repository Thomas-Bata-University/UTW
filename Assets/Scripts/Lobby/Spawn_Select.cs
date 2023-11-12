using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spawn_Select : MonoBehaviour
{
    public int players;
    int faction;
    public Material mat_selected;
    public Material mat_populated1;
    public Material mat_empty;
    public Transform sphere;
    // Start is called before the first frame update
    void Start()
    {
        sphere = transform.Find("Sphere");
        Debug.Log(sphere.name);
            if (players < 1)
            {
                players = 0;
                sphere.GetComponent<Renderer>().material = mat_empty;
            }
            else
            {
                sphere.GetComponent<Renderer>().material = mat_populated1;
            }
        }
    
    private void OnMouseDown()
    {
        Debug.Log(name);
        string selected = PlayerPrefs.GetString("Spawn");
        if (selected!=name)
        {
            players++;
            sphere.GetComponent<Renderer>().material = mat_selected;

            GameObject leaving = GameObject.Find(selected);
            if (leaving != null)
            {
                Spawn_Select druhyskript = leaving.GetComponent<Spawn_Select>();
                druhyskript.players--;
            PlayerPrefs.SetString("Spawn", name);
            if(druhyskript.players < 1)
            {

                druhyskript.players = 0;
                druhyskript.sphere.GetComponent<Renderer>().material = mat_empty;
            }
            else
            {
                druhyskript.sphere.GetComponent <Renderer>().material = mat_populated1;
            }
          }

        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
