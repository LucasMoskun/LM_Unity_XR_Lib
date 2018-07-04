using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//****************************************************************************//
//Used to place a virtual model into an alterante world frame                 //
//****************************************************************************//

public class PlaceScaledModel : MonoBehaviour {
    public GameObject model;

    GameObject point1;
    GameObject point2;
    GameObject modelPt1;
    GameObject modelPt2;

    Quaternion modelRotation;
    Srt modelPointSrt;

    bool modelSet = false;

	void Start () {
        //2 points in alternate world need to be tagged "point 1" and "point 2"       
        //2 corresponding points need to be tagged on the moddel "modelPt1" "modelpt2"
        point1 = GameObject.FindGameObjectWithTag("point1");
        point2 = GameObject.FindGameObjectWithTag("point2");
        modelPt1 = GameObject.FindGameObjectWithTag("ModelPt1");
        modelPt2 = GameObject.FindGameObjectWithTag("ModelPt2");
    }

    //call once to caluclate model rotation
    public void SetModelRotation()
    {
        modelRotation = Quaternion.LookRotation(modelPt2.transform.position - modelPt1.transform.position, Vector3.up);
    }

    //call once to place the model
    public void PlaceWorld(Vector3 _point1, Vector3 _point2)
    {
        Srt roomPointSrt = new Srt(_point1, Quaternion.LookRotation(_point2 - _point1, Vector3.up), Vector3.one);
        modelPointSrt = new Srt(modelPt1.transform.position, modelRotation, Vector3.one);
        Srt finalModelSrt = new Srt();
        finalModelSrt = roomPointSrt * modelPointSrt.Inverse();
        model.transform.position = finalModelSrt.localPosition;
        model.transform.rotation = finalModelSrt.localRotation;
    }

    //can be called from update for continuous calibration
    public void UpdateWorld()
    {
        Srt roomPointSrt = new Srt(point1.transform.position, point1.transform.rotation, Vector3.one);
        Srt finalModelSrt = new Srt();
        finalModelSrt = roomPointSrt * modelPointSrt.Inverse();
        model.transform.position = finalModelSrt.localPosition;
        model.transform.rotation = finalModelSrt.localRotation;
    }
}
