using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************//
// An interactable object that can be slid along an axis //
// the distance along defined axis can be broadcasted    //
//*******************************************************//

public class Slider : Vodget {
    public MyFloatEvent m_MyEvent;

    bool grabbing = false;
    Vector3 interactionPoint = Vector3.zero;
    float minY = 0.7379591f;
    float maxY = 3.49f;

    Vector3 worldObjInitPos = Vector3.zero;

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
                //cache inital interaction point
                interactionPoint = transform.InverseTransformPoint(selector.Cursor.localPosition);
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
            //update position and output value
            Vector3 currentPos = transform.localPosition;
            float sliderVal = 0f;
            float currY = transform.InverseTransformPoint(selector.Cursor.localPosition).y;
            currentPos.y += currY - interactionPoint.y;
            currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);
            transform.localPosition = currentPos;
            sliderVal = ((currentPos.y - minY) / (maxY - minY) * (1 - 0) + 0);

            //invoke value change event
            m_MyEvent.Invoke(sliderVal);

           // Debug.Log("Slider Value : " +  sliderVal);
        }
    }
}
