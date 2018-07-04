using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*****************************************************//
//Created by Daniel Mapes at Full Sail University      //
//Copyright(c) 2018 Lucas Moskun                       //
//*****************************************************//

// A convienience class
public class Srt
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;

    public Srt()
    {
        Clear();
    }

    public Srt(Srt s)
    {
        Set(s);
    }

    public Srt(Vector3 p, Quaternion r, Vector3 s)
    {
        Set(p, r, s);
    }
    public Srt(Transform t)
    {
        Set(t);
    }

    public void Clear()
    {
        localPosition = Vector3.zero;
        localRotation = Quaternion.identity;
        localScale = Vector3.one;
    }

    public void Set(Srt s)
    {
        localPosition = s.localPosition;
        localRotation = s.localRotation;
        localScale = s.localScale;
    }

    public void Set(Vector3 pos, Quaternion rot, Vector3 s)
    {
        localPosition = pos;
        localRotation = rot;
        localScale = s;
    }

    public void Set(Transform t)
    {
        localPosition = t.localPosition;
        localRotation = t.localRotation;
        localScale = t.localScale;
    }

    public void Print(string tag)
    {
        Debug.Log(tag + ": " + localPosition + " " + localRotation + " " + localScale);
    }

    // Invert a scale vector.
    static public Vector3 ScaleInverse(Vector3 scale)
    {
        Vector3 invscale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
        return invscale;
    }

    public Srt Inverse()
    {
        Srt retval = new Srt(this);
        retval.Invert();
        return retval;
    }

    public void Invert()
    {
        localScale = ScaleInverse(localScale);
        localRotation = Quaternion.Inverse(localRotation);
        localPosition = Vector3.Scale(localRotation * -localPosition, localScale);
    }

    static public Srt operator *(Srt pnt, Srt child)
    {
        return new Srt(
            (pnt.localRotation * Vector3.Scale(pnt.localScale, child.localPosition)) + pnt.localPosition,
            pnt.localRotation * child.localRotation,
            Vector3.Scale(pnt.localScale, child.localScale)
            );
    }

    static public Srt operator *(Srt pnt, Transform child)
    {
        return new Srt(
            (pnt.localRotation * Vector3.Scale(pnt.localScale, child.localPosition)) + pnt.localPosition,
            pnt.localRotation * child.localRotation,
            Vector3.Scale(pnt.localScale, child.localScale)
            );
    }

    static public Srt operator *(Transform pnt, Srt child)
    {
        return new Srt(
            (pnt.localRotation * Vector3.Scale(pnt.localScale, child.localPosition)) + pnt.localPosition,
            pnt.localRotation * child.localRotation,
            Vector3.Scale(pnt.localScale, child.localScale)
            );
    }

    public void RotateAboutSiblingPoint(Vector3 pivot_pt, Quaternion dquat)
    {
        // If we created an Srt as a pivot frame converting frame to be a child would only change 
        // the frames position. 
        // Srt frame_pivot = ( pivot_pt, identity, 1f )
        // Srt frame_child = ( frame.localPosition - pivot_pt, frame.localRotation, frame.localScale )
        //
        // We then rotate frame_pivot by the users dquat.
        // frame_pivot = ( pivot_pt, dquat, 1f ) 
        // 
        // Converting frame_child back to world through frame_pivot yeilds.
        localPosition = (dquat * (localPosition - pivot_pt)) + pivot_pt;
        localRotation = dquat * localRotation;
    }

    public Vector3 TransformPoint(Vector3 child_pt)
    {
        Vector3 position = Vector3.Scale(child_pt, localScale);
        position = localRotation * position;
        position += localPosition;
        return position;
    }

    public Vector3 InverseTransformPoint(Vector3 sibling_pt)
    {
        Vector3 position = sibling_pt - localPosition;
        position = Quaternion.Inverse(localRotation) * position;
        position = Vector3.Scale(position, ScaleInverse(localScale));
        return position;
    }
}
