using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeLaserSpawn : MonoBehaviour
{
    //***********************************************************//
    //spawns gaze laser prefab as child of camera eye            //
    // (attatch this script to camera eye)                       //
    //***********************************************************//

    void Start()
    {
        //
        GameObject laser = Instantiate(Resources.Load("***Filepath***") as GameObject);
        laser.transform.parent = transform;
        laser.transform.localPosition = new Vector3(-.25f, 0, 0);
        laser.transform.localRotation = Quaternion.identity;
    }
}
