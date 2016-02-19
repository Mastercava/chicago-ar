/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class ImageTargetTrackableEventHandler : MonoBehaviour,
                                            ITrackableEventHandler
{
    #region PUBLIC_MEMBER_VARIABLES
    public bool isBeingTracked;
    #endregion PUBLIC_MEMBER_VARIABLES
    
    #region PRIVATE_MEMBER_VARIABLES
    private TrackableBehaviour mTrackableBehaviour;
    #endregion // PRIVATE_MEMBER_VARIABLES

	public TrackableBehaviour.Status currentStatus;

    #region PUBLIC_METODS
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
		currentStatus = newStatus;

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS
    private void OnTrackingFound()
    {
        isBeingTracked = true;
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {	
			//if(ImageTargetUIEventHandler.ExtendedTrackingIsEnabled)
			component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
             //if(ImageTargetUIEventHandler.ExtendedTrackingIsEnabled)
             component.enabled = true;
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
		if(currentStatus == TrackableBehaviour.Status.TRACKED) {
			MarkerManager.GetInstance ().MarkerFound(gameObject);
		}
    }


    private void OnTrackingLost()
    {
        isBeingTracked = false;
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Disable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;
        }
		
		MarkerManager.GetInstance ().MarkerLost(gameObject);
    }

    #endregion // PRIVATE_METHODS
}
