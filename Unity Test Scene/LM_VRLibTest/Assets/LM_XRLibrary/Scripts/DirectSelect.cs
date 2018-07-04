using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*************************************************************************//
//Allows for direct collision object grabbing and manipulation             //
//interactable gameObject needs to have vodget/grabber/assembler attatched //
//Copyright(c) 2018 Lucas Moskun                                           //
//*************************************************************************//

[RequireComponent(typeof(SteamVR_TrackedController))]
public class DirectSelect : Selector
{
    SteamVR_TrackedController controller;
    Vodget obj = null;
    bool focusGrabbed = false;
    Vector3 grabPt = Vector3.zero;
    List<Vodget> activeVogets = new List<Vodget>();

    public override void GrabFocus(bool val)
    {

        focusGrabbed = val;
    }

    protected override void SetCursor()
    {
         cursor.Set(controller.transform.position, controller.transform.rotation, Vector3.one);
    }


    // Use this for initialization
    void Start()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.TriggerClicked += TriggerDown;
        controller.TriggerUnclicked += TriggerUp;

    }

    // Update is called once per frame
    void Update()
    {
        if (activeVogets.Contains(obj))
        {
            SetCursor();
            obj.FocusUpdate(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SetCursor();
        obj = other.gameObject.GetComponent<Vodget>();
        obj.Focus(this, true);
        if(!activeVogets.Contains(obj))
        {
            activeVogets.Add(obj);
        }
    }

    // ! may want to use depending on collision edge case (keeps only one object selected at once)
    //private void OnTriggerExit(Collider other) 
    //{
    //    if (activeVogets.Contains(obj))
    //    {
    //        activeVogets.Remove(obj);
    //        obj.Focus(this, false);
    //    }
    //}

    void TriggerDown(object sender, ClickedEventArgs e)
    {
        if (obj)
        {
            SetCursor();
            obj.Button(this, ButtonType.Trigger, true);
        }
    }

    void TriggerUp(object sender, ClickedEventArgs e)
    {
        if (obj)
        {
            SetCursor();
            obj.Focus(this, false);
            obj.Button(this, ButtonType.Trigger, false);

            if (activeVogets.Contains(obj))
            {
                activeVogets.Remove(obj);
                obj.Focus(this, false);
            }
        }
    }
}
