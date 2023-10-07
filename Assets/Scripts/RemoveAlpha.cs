using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAlpha : MonoBehaviour
{
    [SerializeField]
    Image[] maps;

    private void Start()
    {
        for (int i1 = 0; i1 < maps.Length; i1++)
        {
            maps[i1].alphaHitTestMinimumThreshold = 1f;
        }
    }
}