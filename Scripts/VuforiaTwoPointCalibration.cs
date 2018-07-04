/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

//*************************************************************************************************//
// This script has been modified for calibrating a room scale model in AR world frame              //
// place a copy of this script on two vuforia trackable images                                     //
//Copyright(c) 2018 Lucas Moskun                                                                   //
//*************************************************************************************************//

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// 
/// Changes made to this file could be overwritten when upgrading the Vuforia version. 
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
/// 

public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PROTECTED_MEMBER_VARIABLES

    static public bool point1Found = false;
    static public bool point2Found = false;
    public int pointID;
    bool trackingFound = false;
    bool worldPlaced = false;

    GameObject point1;
    GameObject point2;
    PlaceScaledModel placeModel;

    static public Vector3 point1Pos;
    static public Vector3 point2Pos;
    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        point1 = GameObject.FindGameObjectWithTag("point1");
        point2 = GameObject.FindGameObjectWithTag("point2");
        placeModel = GameObject.FindGameObjectWithTag("TestSpawn").GetComponent<PlaceScaledModel>();
    }

    //void Update()  //For only using 1 point! (recommended to wrap in timer for distinct calibration period)
    //{
    //    if(trackingFound)
    //    {
    //        if (pointID == 1/* && !point1Found*/)
    //        {
    //            //point1Found = true;
    //            point1.transform.position = transform.position;
    //            placeModel.UpdateWorld();
    //        }
    //    }
    //}

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        trackingFound = true;
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;

        //if (pointID == 1 && !worldPlaced) //for only using 1 point!
        //{
        //    worldPlaced = true;
        //    placeModel.SetModelRotation();
        //    placeModel.PlaceWorld();
        //}

        //once both points have been found, place world
        if (pointID == 1 && !point1Found)
        {
            point1Found = true;
            point1.transform.position = transform.position;
            point1Pos = transform.position;
        }
        if(pointID == 2 && !point2Found)
        {
            point2Found = true;
            point2.transform.position = transform.position;
            point2Pos = transform.position;
        }
        if(point1Found && point2Found)
        {
            placeModel.SetModelRotation();
            placeModel.PlaceWorld(point1Pos, point2Pos);
        }

    }

    protected virtual void OnTrackingLost()
    {
        trackingFound = false;
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }
    #endregion // PROTECTED_METHODS
}
