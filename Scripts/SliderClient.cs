using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**********************************************************************//
// attatch this to gameobject that you wish to be manipulated by slider //
// drag gameobject onto the slider event to listen for updates          //
//**********************************************************************//

public class SliderClient : MonoBehaviour {

    Vector3 worldObjInitPos;

    private void Start()
    {
        worldObjInitPos = transform.position;

    }

    //moves object according to slider position
    public void SliderChanged ( float sliderVal ) {
        //move world object
        float newWorldYPos = worldObjInitPos.y + sliderVal;
        Vector3 worldPos = transform.position;
        worldPos.y = newWorldYPos;
        transform.position = worldPos;
    }
}
