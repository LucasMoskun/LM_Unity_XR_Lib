using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**********************************************************************************//
// Allows user to scale and rotate the world around them with controller grips      //
// Attatch to Camera rig and place two steam vr tracked controllers in public slots //
//Copyright(c) 2018 Lucas Moskun                                                    //
//**********************************************************************************//

public class WorldGrabber : MonoBehaviour
{
    public SteamVR_TrackedController leftControl;
    public SteamVR_TrackedController rightControl;
    public bool dolly;

    Srt worldOffset = new Srt();
    Srt worldMoved = new Srt();
    Srt controllerSrt = new Srt();

    float initialScale = 1;
    Vector3 bimanualInitialVec = Vector3.zero;
    Vector3 initialUp = Vector3.zero;

    bool leftGrabbed = false;
    bool rightGrabbed = false;

    // Use this for initialization
    void Start()
    {
        //controller events
        leftControl.Gripped += LeftGripDown;
        leftControl.Ungripped += LeftGripUp;
        rightControl.Gripped += RightGripDown;
        rightControl.Ungripped += RightGripUp;
    }

    void SetControllerSrt()
    {
        if (leftGrabbed && !rightGrabbed)
        {
            controllerSrt.Set(leftControl.transform.localPosition, leftControl.transform.localRotation, Vector3.one);
        }
        else if (!leftGrabbed && rightGrabbed)
        {
            controllerSrt.Set(rightControl.transform.localPosition, rightControl.transform.localRotation, Vector3.one);
        }
        else
        {
            //both controllers gripped
            //update average location and length
            Vector3 leftPos = leftControl.transform.localPosition;
            Vector3 rightPos = rightControl.transform.localPosition;
            Vector3 averagePos = (leftPos + rightPos) / 2;
            Vector3 lengthBetween = rightPos - leftPos;

            Quaternion newrotation = Quaternion.FromToRotation(bimanualInitialVec, lengthBetween);

            float newMagnitude = lengthBetween.magnitude / initialScale;
            Vector3 newScale = new Vector3(newMagnitude,newMagnitude,newMagnitude);

            controllerSrt.Set(averagePos, newrotation, newScale);
        }
    }

    void SetInitialVector()
    {
        bimanualInitialVec = rightControl.transform.localPosition - leftControl.transform.localPosition;
    }

    void SetInitialScale()
    {
        initialScale = (rightControl.transform.localPosition - leftControl.transform.localPosition).magnitude;
    }

    void SetInitialUp()
    {
        initialUp = transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        SetControllerSrt();

        //convert controller SRT values to the the camera rig
        if (leftGrabbed || rightGrabbed)
        {
         
            Srt worldCameraFrame = controllerSrt * worldOffset;

            if (dolly)
            {
                Quaternion fixWonkyUp = Quaternion.FromToRotation(worldCameraFrame.localRotation * Vector3.up, Vector3.up);
                worldCameraFrame.localRotation = fixWonkyUp * worldCameraFrame.localRotation;
                worldCameraFrame.localPosition = fixWonkyUp * (worldCameraFrame.localPosition - controllerSrt.localPosition) + controllerSrt.localPosition;
            }

            Srt cameraRigSrt = new Srt(transform.localPosition, transform.localRotation, transform.localScale);
            worldMoved = cameraRigSrt * worldCameraFrame;
            cameraRigSrt = worldMoved.Inverse() * cameraRigSrt;

            transform.localPosition = cameraRigSrt.localPosition;
            transform.localRotation = cameraRigSrt.localRotation;
            transform.localScale = cameraRigSrt.localScale;

        }
    }

    //move functions
    void CalculateWorldOffest()
    { 
        Srt cameraSrt = new Srt(transform.position, transform.rotation, transform.localScale);
        worldOffset = controllerSrt.Inverse() * cameraSrt.Inverse();
    }

    //grip events
    void LeftGripDown(object sender, ClickedEventArgs e)
    {
        if(rightGrabbed)
        {
            SetInitialVector();
            SetInitialScale();
        }
        else
        {
            SetInitialUp();
        }
        leftGrabbed = true;
        SetControllerSrt();
        CalculateWorldOffest();
    }
    void LeftGripUp(object sender, ClickedEventArgs e)
    {
        leftGrabbed = false;
        SetControllerSrt();
        CalculateWorldOffest();
    }
    void RightGripDown(object sender, ClickedEventArgs e)
    {
        if(leftGrabbed)
        {
            SetInitialVector();
            SetInitialScale();
        }
        else
        {
            SetInitialUp();
        }
        rightGrabbed = true;
        SetControllerSrt();
        CalculateWorldOffest();
    }
    void RightGripUp(object sender, ClickedEventArgs e)
    {
        rightGrabbed = false;
        SetControllerSrt();
        CalculateWorldOffest();
    }
}
