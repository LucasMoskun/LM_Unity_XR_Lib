using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//***************************************************************************************//
//uses vector from eye position through contoller position to grab objects at a distance //
//***************************************************************************************//

[RequireComponent(typeof(SteamVR_TrackedController))]
public class HandEyeRaySelector : HandRaySelector
{
    public float rayDistance = 200;
    public bool drawRay = false;
    float eyeToHandDist;
    float hitPtToEyeDist;
    float distRatio;

    Vector3 grab = Vector3.zero;

    protected override void Start()
    {
        base.Start();
    }

    Vector3 EyePos()
    {
        return transform.parent.TransformPoint(UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye));
    }

    public override void GrabFocus(bool val)
    {
        if(val)
        {
            //initialize eye hand ratio
            eyeToHandDist = (controller.transform.position - EyePos()).magnitude;
            hitPtToEyeDist = (hit.point - EyePos()).magnitude;
            distRatio = hitPtToEyeDist / eyeToHandDist;

        }
        focusGrabbed = val;
    }

    protected override void SetCursor()
    {
        if (focusGrabbed)
        {
            //set cursor grab location to hand eye vector
            Vector3 handEyeVec = controller.transform.position - EyePos();
            float updateHitToEyeDist = handEyeVec.magnitude * distRatio;
            cursor.localRotation = controller.transform.rotation;
            cursor.localPosition = EyePos() + handEyeVec.normalized * updateHitToEyeDist;
        }
        else if (obj != null)
        {
            cursor.Set(hit.point, controller.transform.rotation, Vector3.one);

        }
        else
        {
            cursor.Set(controller.transform.position, controller.transform.rotation, Vector3.one);
        }

    }

    // Update is called once per frame
    protected override void Update()
    {
        SetCursor();

        if (!focusGrabbed)
        {
            Vector3 eyeHandDist = controller.transform.position - EyePos();
            bool hitfound = Physics.Raycast(EyePos(), eyeHandDist, out hit, rayDistance);
            //Debug.DrawRay(eye.position, eyeHandDist, Color.red, 60);
            //laserLine.enabled = false;

            if (hitfound)
            {
                Vodget tempObj = hit.collider.gameObject.GetComponent<Vodget>();
                if (tempObj)
                {
                    //reset cached obj focus, initial interaction logic
                    if (tempObj != obj)
                    {
                        if (obj != null)
                        {
                            obj.Focus(this, false);
                        }
                        obj = tempObj;
                        obj.Focus(this, true);

                    }
                }
            } else
            {
                laserLine.enabled = false;
                if (obj != null)
                {
                    obj.Focus(this, false);
                }
                obj = null;
            }
        }

        if (obj != null)
        {
            obj.FocusUpdate(this);
        }

        DrawLaser();
    }

    protected override void DrawLaser()
    {
        if (drawRay)
        {
            laserLine.enabled = true;
            laserLine.SetPosition(0, EyePos());
            laserLine.SetPosition(1, cursor.localPosition);
        }
    }
}
