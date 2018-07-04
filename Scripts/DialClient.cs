using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialClient : MonoBehaviour
{
    //********************************************************************//
    // attatch this to gameobject that you wish to be manipulated by dial //
    // drag gameobject onto the dial event to listen for updates          //
    //Copyright(c) 2018 Lucas Moskun                                      //
    //********************************************************************//

    Vector3 worldEuler = Vector3.zero;

    private void Start()
    {
        worldEuler = transform.eulerAngles;
    }

    //rotates object according to dial position
    public void DialChanged(float angle)
    {
        worldEuler.x = angle;
        transform.eulerAngles = worldEuler;
    }
}
