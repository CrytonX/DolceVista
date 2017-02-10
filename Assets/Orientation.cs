using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientation : MonoBehaviour {

	public Canvas portraitCanvas;
	public Canvas landscapeCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	void Update() {
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
			if (portraitCanvas.gameObject.activeSelf) {
				landscapeCanvas.gameObject.SetActive (true);
				portraitCanvas.gameObject.SetActive (false);
			}
		}
		else if (Input.deviceOrientation == DeviceOrientation.Portrait) {
			if (landscapeCanvas.gameObject.activeSelf) {
				landscapeCanvas.gameObject.SetActive(false);
				portraitCanvas.gameObject.SetActive(true);
			}
		}
	}

}
