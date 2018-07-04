using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**************************************//
// attatch to gameObject to allow for   //
// user/controller interaction          //
//**************************************//

public class Grabber : Vodget {

    protected Srt child_offset = new Srt();
    protected bool grabbing = false;

    public override void Focus(Selector selector, bool state)
    {
        // Highlight, pulse the haptic or something... 
    }

    // Button is called by selectors on vodgets that have focus. 
    // Note: It is common for vodgets to call the selectors GrabFocus(true) 
    public override void Button(Selector selector, Selector.ButtonType button, bool state)
    {
        if (button == Selector.ButtonType.Trigger)
        {
            if (state == true)
            {
                Srt objsrt = new Srt(transform.position, transform.rotation, transform.lossyScale);
                child_offset = selector.Cursor.Inverse() * objsrt;
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
        if ( grabbing )
        {
            Srt objsrt = selector.Cursor * child_offset;
            transform.position = objsrt.localPosition;
            transform.rotation = objsrt.localRotation;
            //transform.lossyScale = objsrt.localScale;
        }
    }
}
