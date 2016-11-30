using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SwitchMode : MonoBehaviour {

    public GameObject VRMode;
    public GameObject ThreeSixty;



    public GvrViewer gvr;

    public Image buttonSprite;

	// Use this for initialization
	void Start () {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        gvr.VRModeEnabled = false;
	}

    public void Switch()
    {
        gvr.VRModeEnabled = !gvr.VRModeEnabled;
        if (gvr.VRModeEnabled)
        {
            VRMode.SetActive(true);
            ThreeSixty.SetActive(false);
        }
        else
        {
            VRMode.SetActive(false);
            ThreeSixty.SetActive(true);
        }
    }

    public void SwitchToAR()
    {
        SceneManager.LoadScene("Book");
    }
}
