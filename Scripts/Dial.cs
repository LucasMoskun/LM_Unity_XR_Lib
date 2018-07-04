using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//event that allows for dial value to be broadcated
[System.Serializable]
public class MyFloatEvent : UnityEvent<float>
{
}

//*****************************************************//
// An interactable object that can be rotated          //
// the value (angle) of the roation can be broadcasted //
//Copyright(c) 2018 Lucas Moskun                       //
//*****************************************************//

public class Dial : Vodget {
    public MyFloatEvent m_MyEvent;

    bool grabbing = false;
    Vector3 interactionPoint = Vector3.zero;

    public override void Focus(Selector selector, bool state)
    {
        //highlight, pulse, etc
    }

    public override void Button(Selector selector, Selector.ButtonType button, bool state)
    {
        if(button == Selector.ButtonType.Trigger)
        {
            if(state == true)
            {
                interactionPoint = transform.InverseTransformPoint(selector.Cursor.localPosition);
                interactionPoint.z = 0f;

                grabbing = true;
                selector.GrabFocus(true);
            }
            else
            {
                grabbing = false;
                selector.GrabFocus(false);
            }
        }
    }

    public override void FocusUpdate(Selector selector)
    {
        if(grabbing)
        {
            //rotate dial
            Quaternion localRot = transform.localRotation;
            float dialVal = 0f;

            Vector3 currentPos = transform.InverseTransformPoint(selector.Cursor.localPosition);
            currentPos.z = 0f;
            Quaternion deltaRot = Quaternion.FromToRotation(interactionPoint, currentPos);

            localRot *= deltaRot;
            transform.localRotation = localRot;

            /////////////////////////////

            //calculate and broadcast angle
            Vector3 controlNorm = transform.localRotation * Vector3.up;
            controlNorm.Normalize();
            float angle = Vector3.Dot(controlNorm, Vector3.up);
            angle = Mathf.Acos(angle);
            angle *= Mathf.Rad2Deg;

            if(Vector3.Dot(controlNorm, Vector3.right) > 0f)
            {
                angle = 360f - angle;
            }

            dialVal = angle;

            m_MyEvent.Invoke(dialVal);
            //Debug.Log("Dial Value " + dialVal);
        }
    }
}
