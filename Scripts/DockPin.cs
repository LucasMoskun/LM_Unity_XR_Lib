using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************//
// Attatch to gameObject you wish dock as an assembly object                     // 
// this script allows for docking to collider with dockPinTarget attatched to it //
// once docked the item can be rotated and inspected                             //
// gameObject also needs Assembler attatched                                     //
//Copyright(c) 2018 Lucas Moskun                                                 //
//*******************************************************************************//


public class DockPin : MonoBehaviour {

    Srt controllerSrt = new Srt();
    Transform parentSave = null;
    Vector3 initialGrab = Vector3.zero;
    DockPinTarget pinTarget = null;
    public float breakDistance = 0.25f;

    //axis locks
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pinTarget != null)
            return;

        pinTarget = other.gameObject.GetComponent<DockPinTarget>();
        if(pinTarget != null)
        {
            Snap(pinTarget.transform);
            initialGrab = transform.InverseTransformPoint(controllerSrt.localPosition);
        }
    }

    //snap object to location
    void Snap(Transform _pinTransform)
    {
        parentSave = transform;
        transform.parent = _pinTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        RotatePin();
    }

    //rotate on point
    void RotatePin()
    {
        Quaternion localRot = transform.localRotation;
        Vector3 currentPos = transform.InverseTransformPoint(controllerSrt.localPosition);
        Vector3 interactionPoint = initialGrab;

        //public variables can be turned on to lock certain axis rotation
        if(lockX)
        {
            currentPos.x = 0;
            interactionPoint.x = 0;
        }
        if(lockY)
        {
            currentPos.y = 0;
            interactionPoint.y = 0;
        }
        if(lockZ)
        {
            currentPos.z = 0;
            interactionPoint.z = 0;
        }

        Quaternion deltaRot = Quaternion.FromToRotation(interactionPoint, currentPos);
        localRot *= deltaRot;
        transform.localRotation = localRot;
    }

    //break off from dock location
    bool TestBreakSnap()
    {
        bool breakSnap = false;
        Vector3 currentPos = transform.InverseTransformPoint(controllerSrt.localPosition);
        float distance = transform.TransformVector(initialGrab - currentPos).magnitude;
        if(distance > breakDistance)
        {
            breakSnap = true;
        }

        return breakSnap;
    }

    void Unsnap()
    {
        transform.parent = parentSave;
        pinTarget = null;
        //controllerSrt = null;
    }

    public bool IsDocked
    {
       get { return pinTarget != null; }
    }

    public void Grab(Vector3 _position)
    {
        initialGrab = transform.InverseTransformPoint(_position);
    }

    public void UpdateDocked(Srt _controllerSrt)
    {
        controllerSrt.Set(_controllerSrt);
        if(pinTarget != null)
        {
            if(TestBreakSnap())
            {
                Unsnap();
            }
            else
            {
                RotatePin();
            }
        }
    }
}
