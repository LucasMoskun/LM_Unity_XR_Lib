using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**********************************************************************//
// Interactable object that can be grabbed and docked onto docking pins //
// for assembly functionality                                           //
//**********************************************************************//

public class Assembler : Grabber {
    public DockPin pin;

    public override void Button(Selector selector, Selector.ButtonType button, bool state)
    {
        base.Button(selector, button, state);
        pin.Grab(selector.Cursor.localPosition);
    }

    public override void FocusUpdate(Selector selector)
    {
        if(grabbing)
        {
            if(pin.IsDocked)
            {
                //run the docking pins update
                pin.UpdateDocked(selector.Cursor);
            }
            else
            {
                Srt objsrt = selector.Cursor * child_offset;
                transform.position = objsrt.localPosition;
                transform.rotation = objsrt.localRotation;
            }
        }
        
    }
}
