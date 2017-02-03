//Copyright 2017, Austin Ford, All rights reserved.
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour {

	public void loadMainLevel(string scene)
    {
        if (scene == "VR" || scene == "VR2")
        Screen.orientation = ScreenOrientation.LandscapeLeft;
   
        SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);
    }
}
