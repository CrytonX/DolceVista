using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Intro : MonoBehaviour {

    public GameObject splashImage;
    public GameObject parent;
    public Sprite newSprite;
    public Color newColor;

    public GameObject demo;
    public GameObject retail;

    public float timeLeft = 3f;
    // Use this for initialization
    void Start () {
        Screen.orientation = ScreenOrientation.Portrait;
        parent.SetActive(false);
	}
	
	public void UpdateButton()
    {
        splashImage.SetActive(false);
        parent.SetActive(true);
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            UpdateButton();
        }
    }

    public void selectBook(string book)
    {
        if (book == "Demo")
            demo.SetActive(true);
        else if (book == "Full")
            retail.SetActive(true);
    }
}
