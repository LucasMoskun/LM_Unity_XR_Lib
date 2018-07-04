using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**********************************************************//
//Draws ray forward from controller                         //
//allowing any object with a vodget/grabber/assembler       //
// script attatched to be interacted with from a distance   //
//**********************************************************//

[RequireComponent(typeof(SteamVR_TrackedController))]
public class HandRaySelector : Selector
{

    protected SteamVR_TrackedController controller;

    protected bool focusGrabbed = false;
    public override void GrabFocus(bool val)
    {
        if (val)
        {
            // Convert hit point to child of cursor
            grabpt = controller.transform.InverseTransformPoint(hit.point);
        }
        focusGrabbed = val;
    }

    protected LineRenderer laserLine;
    protected float laserW = .01f;
    protected float laserL = 5f;
    protected Vodget obj = null;
    protected RaycastHit hit;
    protected Vector3 grabpt = Vector3.zero;

    // Use this for initialization
    protected virtual void Start()
    {
        controller = GetComponent<SteamVR_TrackedController>();

        controller.TriggerClicked += TriggerDown;
        controller.TriggerUnclicked += TriggerUp;
        ///initialiaze laser
        laserLine = GetComponent<LineRenderer>();
        Vector3[] initLaserPos = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLine.startWidth = laserW;
        laserLine.SetPositions(initLaserPos);
        laserLine.endWidth = .008f;
        laserLine.enabled = false;
    }

    protected void TriggerDown(object sender, ClickedEventArgs e)
    {
        if (obj)
        {
            SetCursor();
            obj.Button(this, ButtonType.Trigger, true);
        }
    }

    protected void TriggerUp(object sender, ClickedEventArgs e)
    {
        if (obj)
        {
            SetCursor();
            obj.Button(this, ButtonType.Trigger, false);
            obj = null;
        }
    }


    protected override void SetCursor()
    {
        if (focusGrabbed)
        {
            // Move original hit point in Cursor frame back to world frame.
            cursor.Set(controller.transform.TransformPoint(grabpt), controller.transform.rotation, Vector3.one);
        }
        else
        {
            if (obj == null)
            {
                cursor.Set(controller.transform.position, controller.transform.rotation, Vector3.one);
            }
            else
            {
                cursor.Set(hit.point, controller.transform.rotation, Vector3.one);
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
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

    protected virtual void DrawLaser()
    {
        laserLine.enabled = true;
        Vector3 laserPos = transform.position;
        laserLine.SetPosition(0, laserPos);
        laserLine.SetPosition(1, cursor.localPosition);
    }
}

