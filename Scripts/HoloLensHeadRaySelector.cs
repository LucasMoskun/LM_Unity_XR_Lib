using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

//******************************************************//
//Gaze cast object grabbing for hololens                //
//Use pinch to grab selected object                     //
//Copyright(c) 2018 Lucas Moskun                        //
//******************************************************//

public class HoloLensHeadRaySelector : Selector
{
    bool focusGrabbed = false;
    public override void GrabFocus(bool val)
    {
        if (val)
        {
            // Convert hit point to child of cursor
            grabpt = transform.InverseTransformPoint(hit.point);
        }
        focusGrabbed = val;
    }

    LineRenderer laserLine;
    float laserW = .01f;
    float laserL = 5f;
    Vodget obj = null;
    RaycastHit hit;
    Vector3 grabpt = Vector3.zero;

    //hand tracking
    List<uint> trackHands = new List<uint>();
    GestureRecognizer gestureRecognize;

    // Use this for initialization
    void Start()
    {
        //AR interaction source
        InteractionManager.InteractionSourceDetected += InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionSourceLost;
        gestureRecognize = new GestureRecognizer();
        gestureRecognize.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognize.Tapped += TapRecognized;
        gestureRecognize.StartCapturingGestures();

        ///initialiaze laser
        laserLine = GetComponent<LineRenderer>();
        Vector3[] initLaserPos = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLine.startWidth = laserW;
        laserLine.SetPositions(initLaserPos);
        laserLine.endWidth = .008f;
        laserLine.enabled = false;
    }

    //Detetect hand events
    void InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
    {
        //Debug.Log("interaction");
        //keep track of hand gestures
        if (args.state.source.kind != InteractionSourceKind.Hand)
        {
            return;
        }
        trackHands.Add(args.state.source.id);
    }

    void InteractionSourceLost(InteractionSourceLostEventArgs args)
    {
        if (args.state.source.kind != InteractionSourceKind.Hand)
        {
            return;
        }

        if (trackHands.Contains(args.state.source.id))
        {
            trackHands.Remove(args.state.source.id);
        }

    }

    void TapRecognized(TappedEventArgs args)
    {
        //Debug.Log("Tap");
        if (obj && !focusGrabbed)
        {
            SetCursor();
            obj.Button(this, ButtonType.Trigger, true);
        }
        else
        {
            SetCursor();
            obj.Button(this, ButtonType.Trigger, false);
            obj = null;
        }
    }

    void TriggerDown()
    {
        if (obj)
        {
            SetCursor();
        }
    }

    void TriggerUp(object sender)
    {
        if (obj)
        {
            SetCursor();
            obj = null;
        }
    }


    protected override void SetCursor()
    {
        if (focusGrabbed)
        {
            // Move original hit point in Cursor frame back to world frame.
            cursor.Set(transform.TransformPoint(grabpt), transform.rotation, Vector3.one);
        }
        else
        {
            if (obj == null)
            {
                cursor.Set(transform.position, transform.rotation, Vector3.one);
            }
            else
            {
                cursor.Set(hit.point, transform.rotation, Vector3.one);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!focusGrabbed)
        {
            //cast ray in front of this object
            bool hitFound = Physics.Raycast(transform.position, transform.forward, out hit, 200);
            //Debug.DrawRay(transform.position, forward * 200, Color.red, 30);

            //disable laser so only draws if hitfound
            laserLine.enabled = false;

            if (hitFound)
            {
                Vodget tobj = hit.collider.gameObject.GetComponent<Vodget>();
                if (tobj)
                {
                    //reset cached obj focus
                    if (tobj != obj)
                    {
                        SetCursor();
                        if (obj != null)
                        {
                            obj.Focus(this, false);
                        }
                        obj = tobj;
                        obj.Focus(this, true);
                    }
                }
            }
            else
            {
                //reset cached variables if no hit
                laserLine.enabled = false;
                if (obj != null)
                {
                    SetCursor();
                    obj.Focus(this, false);
                }
                obj = null;
            }
        }

        if (obj != null)
        {
            SetCursor();
            obj.FocusUpdate(this);
            DrawLaser();
        }
    }

    void DrawLaser()
    {
        laserLine.enabled = true;
        Vector3 laserPos = transform.position;
        laserLine.SetPosition(0, laserPos);
        laserLine.SetPosition(1, cursor.localPosition);
    }
}
