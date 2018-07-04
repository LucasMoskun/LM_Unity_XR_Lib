using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**********************************************//
//Used in conjuction with vuforia               //
//draws ray from touch point to select objects  //
//in AR world                                   //
//Copyright(c) 2018 Lucas Moskun                //
//**********************************************// 

public class AndroidRaySelector : Selector
{
    bool focusGrabbed = false;
    public override void GrabFocus(bool val)
    {
        if (val)
        {
            // Convert hit point to child of cursor
            grabpt = touchPos;
        }
        focusGrabbed = val;
    }

    LineRenderer laserLine;
    float laserW = .01f;
    float laserL = 5f;
    Vodget obj = null;
    RaycastHit hit;
    Vector3 grabpt = Vector3.zero;
    Vector3 touchPos = Vector3.zero;

    //hand tracking

    // Use this for initialization
    void Start()
    {

        ///initialiaze laser
        laserLine = GetComponent<LineRenderer>();
        Vector3[] initLaserPos = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLine.startWidth = laserW;
        laserLine.SetPositions(initLaserPos);
        laserLine.endWidth = .008f;
        laserLine.enabled = false;
    }


    protected override void SetCursor()
    {
        if (focusGrabbed)
        {
            // Move original hit point in Cursor frame back to world frame.
            cursor.Set(touchPos, transform.rotation, Vector3.one);
        }
        else
        {
            if (obj == null)
            {
                cursor.Set(transform.position, transform.rotation, Vector3.one);
            }
            else
            {
                cursor.Set(touchPos, transform.rotation, Vector3.one);
            }
        }
    }

    float magnitude;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (obj && !focusGrabbed)
            {

                SetCursor();
                obj.Button(this, ButtonType.Trigger, true);
            }
            else if (obj)
            {
                //cast ray from touch point
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + ray.direction * magnitude;
            }
        }
        else
        {
            if (obj)
            {
                //disable if nothing hit
                touchPos = Vector3.zero;
                SetCursor();
                obj.Button(this, ButtonType.Trigger, false);
                obj.Focus(this, false);
                obj = null;
            }
        }


        if (!focusGrabbed)
        {
            //disable laser so only draws if hitfound
            laserLine.enabled = false;
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    touchPos = hit.point;
                    magnitude = (touchPos - Camera.main.ScreenToWorldPoint(Input.mousePosition)).magnitude;
                    //on update

                    Debug.Log(hit.collider.gameObject.tag);
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
        Vector3 laserPos = touchPos;
        laserLine.SetPosition(0, laserPos);
        laserLine.SetPosition(1, cursor.localPosition);
    }
}
