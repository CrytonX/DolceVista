using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsTracker : MonoBehaviour {

    public GoogleAnalyticsV4 googleAnalytics;

	// Use this for initialization
	void Start () {
        Debug.Log(Application.loadedLevelName);
        googleAnalytics.LogScreen(Application.loadedLevelName);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
