using UnityEngine;
using Vuforia;
using System.Collections;

public class TrackingFound : MonoBehaviour, ITrackableEventHandler
{

    private TrackableBehaviour mTrackableBehaviour;

    public GameObject ui;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(
                                       TrackableBehaviour.Status previousStatus,
                                       TrackableBehaviour.Status newStatus)
    {
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

    public void OnTrackingFound()
    {
        ui.SetActive(true);
    }

    private void OnTrackingLost()
    {
        ui.SetActive(false);
    }
}