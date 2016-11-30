using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour {

	public void loadMainLevel(string scene)
    {
        if (scene == "VR")
        Screen.orientation = ScreenOrientation.LandscapeLeft;
   
        SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);
    }
}
