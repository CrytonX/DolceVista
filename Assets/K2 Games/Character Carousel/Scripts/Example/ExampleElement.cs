using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This is where you would add your own data!
/// </summary>
public class ExampleElement : CarouselElement
{
    public Text text;
    public Image characterImage;

    public Sprite[] characterImages;//this is just a quick example of using an image for each character. You probably want to move and read this from a manager somewhere else

    //void Start()//don't implement this unless you include the following lines as well
    //{
    //    transform = base.transform as RectTransform;
    //    transition.SetState(TransitionalObjects.BaseTransition.TransitionState.TransitionIn);//show this transition is waiting since we will control it manually
    //}

    /// <summary>
    /// Set your data here. DO NOT implement start. If you have to you must copy what is in the parent class
    /// </summary>
    /// <param name="carousel"></param>
    /// <param name="index"></param>
    public override void Initialise(CharacterCarousel carousel, int index)
    {
        base.Initialise(carousel, index);

		if (text) {
			text.text = "Character " + (index + 1);
		}
        if(index < characterImages.Length)//check if you added more screens but not more images
            characterImage.sprite = characterImages[index];//an example to show chanaging individual character images
    }
}
