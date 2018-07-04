using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LM_XR
{
    //********************************************//
    // GazeCast detetects an interactable object  //
    // and draws a laserline to it while focused  //
    //********************************************//

    public class GazeCast : MonoBehaviour
    {
        LineRenderer laserLine;
        float laserW = .1f;
        float laserL = 5f;
        Interactable obj;

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

        // Update is called once per frame
        void Update()
        {
            //cast ray in front of this object
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            bool hitFound = Physics.Raycast(transform.position, forward, out hit, 200);
            //Debug.DrawRay(transform.position, forward * 200, Color.red, 30);

            //disable laser so only draws if hitfound
            laserLine.enabled = false;

            //reset cached obj focus
            if(obj)
            {
                obj.SetCollectionFocus(false);
                obj.SetLocalFocus(false);
            }

            if(hitFound)
            {
                obj = hit.collider.gameObject.GetComponent<Interactable>().ReturnInteractable();
                if(obj)
                {
                    //draw laser
                    laserLine.enabled = true;
                    Vector3 laserPos = transform.position;
                    laserLine.SetPosition(0, laserPos);
                    laserLine.SetPosition(1, hit.point);

                    //setFocus
                    obj.SetCollectionFocus(true);
                    obj.SetLocalFocus(true);
                }
            }
            else
            {
                //reset cached variables if no hit
                laserLine.enabled = false;
                obj = null;
            }
        }
    }
}
