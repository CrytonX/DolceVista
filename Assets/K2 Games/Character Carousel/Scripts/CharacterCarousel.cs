using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterCarousel : MonoBehaviour
{
    #region Variables
    [HideInInspector]
    public new Transform transform;

	public List<CarouselElement> prefabElementList = new List<CarouselElement>();

	[HideInInspector]
	private List<CarouselElement> elementList = new List<CarouselElement>();

	public UnityEngine.UI.Button.ButtonClickedEvent clickHandler;
    //public CarouselElement prefab;

    private int screensToSpawn;

    /// <summary>
    /// The currently selected screen object
    /// </summary>
    public CarouselElement currentScreen
    {
        get
        {
            return elementList[currentScreenIndex];
        }
    }

    /// <summary>
    /// The index of the currently selected screen
    /// </summary>
    public int currentScreenIndex
    {
        get
        {
            return (int)((currentPosition - 1) / offsetBetweenElements);
        }
    }

	public bool startAtCurrentPosition = false;
    //[HideInInspector]
    public float currentPosition;//used to scroll left and right
    public float offsetBetweenElements = 0.5f;//how much space to place between elements. This is relative to the elements width
    public float edgeWidth = 25;//this helps offset the ends of the animation to look like book pages

    public float brakingTime = 0.5f;//this allows for inertia. I.E as you swipe quickly it takes a second or so to come to a stop

    public float minSpeedThreshold = 0.01f, snappingPercentage = 0.025f;
    public float jumpingTime = 0.2f,//this is the speed to move towards screens clicked directly.
        jumpingAccelerationTime = 0.05f;//this is how much time to spend accelerating

    float lastDelta, startingDelta;
    float currentTime;
    float jumpingRatio, targetJumpingRatio, currentJumpingAccelerationTime;//this helps to jump faster if you click 2 screens away rather than just the next one. The target just helps with acceleration
    bool snapped, ignoreClicks;
    float jumpIndex = -1;//which view, if any, to jump to
    #endregion

    #region Methods
    void Start()
    {
        transform = base.transform;

		screensToSpawn = prefabElementList.Count;

        if(screensToSpawn < 0)
        {
            enabled = false;
            return;
        }

		for (int i = 0; i < screensToSpawn; i++) {
			GameObject temp = Instantiate<GameObject> (prefabElementList [i].gameObject);
			temp.transform.SetParent (transform);
			temp.transform.localScale = Vector3.one;
			temp.name = "Element " + i;
			elementList.Add (temp.GetComponent<CarouselElement> ());
			elementList [i].Initialise (this, i);
			Button b = temp.GetComponent<Button>();
			if (b != null) {
				b.onClick = clickHandler;
			}
		}

		if (startAtCurrentPosition) {
			JumpToView((int)(currentPosition + (currentPosition * offsetBetweenElements)));
		} else {
			JumpToView((screensToSpawn + 1) / 2);//start by viewing the middle element
		}
	}

    void Update()
    {
        #region Jumping to Screen
        if(jumpIndex > -1)//if jumping to an object
        {
            if(jumpingRatio < targetJumpingRatio)
            {
                currentJumpingAccelerationTime += Time.deltaTime / jumpingAccelerationTime;//acceleration time, normalised to be between 0 and 1

                jumpingRatio = targetJumpingRatio * currentJumpingAccelerationTime;//apply the acceleration
            }

            bool movingForward;//helps to prevent very quick snap backs by instantly snapping when we go over or under the value we want

            if(jumpIndex > currentPosition)
            {
                currentPosition += offsetBetweenElements * (Time.deltaTime / jumpingTime) * jumpingRatio;
                movingForward = true;
            }
            else
            {
                currentPosition -= offsetBetweenElements * (Time.deltaTime / jumpingTime) * jumpingRatio;
                movingForward = false;
            }

            if((movingForward && currentPosition > jumpIndex) || (!movingForward && currentPosition < jumpIndex))//if close to the view we need to snap to
            {
                currentPosition = jumpIndex;
                jumpIndex = -1;//stop jumping
            }
        }
        #endregion

        #region Snapping
        else if(!snapped)//if snapping or jumping to view an element
            if(lastDelta < minSpeedThreshold && lastDelta > -minSpeedThreshold)//if going slowly
            {
                float mod = currentPosition % offsetBetweenElements;

                if(mod != 0)
                {
                    if(mod < offsetBetweenElements / 2)//if we need to snap back
                        currentPosition -= minSpeedThreshold;//then keep moving
                    else
                        currentPosition += minSpeedThreshold;

                    mod = currentPosition % offsetBetweenElements;

                    if(mod < snappingPercentage)//if finished
                    {
                        currentPosition -= mod;//snap
                        snapped = true;
                    }
                    else if(mod > offsetBetweenElements - snappingPercentage)//if finished
                    {
                        currentPosition += offsetBetweenElements - mod;//snap
                        snapped = true;
                    }
                }
                else
                    snapped = true;
            }
            #endregion

            #region Scrolling
            else if(lastDelta > 0.001 || lastDelta < -0.001)//if we just finished a swipe, then apply momentum
            {
                currentTime += Time.deltaTime / brakingTime;

                currentPosition += lastDelta;//keep moving with momentum. This is purposely ignoring deltaTime!

                lastDelta = startingDelta * (1 - currentTime);//slow the momentum as more time passes. This includes the delta

                if(currentTime > 1)
                    lastDelta = 0;//hard stop
            }
        #endregion

        #region Max and Min Caps
        if(currentPosition > (screensToSpawn + 1) * offsetBetweenElements)//if on the last screen
            currentPosition = (screensToSpawn + 1) * offsetBetweenElements;//cap at max
        else if(currentPosition < 1)
            currentPosition = 1;//cap at the min
        #endregion

        List<int> elementIndexes = new List<int>();

        for(int i = 0; i < elementList.Count; i++)
            if(elementList[i] != null)
            {
                elementList[i].UpdatePosition(currentPosition - i * offsetBetweenElements, edgeWidth);

                #region Depth Sorting
                float current = currentPosition - i * offsetBetweenElements;

                if(current > 1)//if going off to the right
                    elementIndexes.Add(i);//then we want a low draw order priority by adding to the top
                else
                    elementIndexes.Insert(0, i);//if we are on the left, then add to the front of the queue
                #endregion
            }

        #region Sort for Drawing Order
        for(int i = 0; i < elementIndexes.Count; i++)
            elementList[elementIndexes[i]].transform.SetSiblingIndex(i);//this ensures the middle element, the one with the smallest z, will always draw first
        #endregion
    }

    public void OnDragStart()
    {
        jumpIndex = -1;//stop any jumps for other user input
        lastDelta = 0;
        ignoreClicks = true;
    }

    public void OnDragEnd(float delta)
    {
        ignoreClicks = false;
        startingDelta = delta;
        lastDelta = delta;
        currentTime = 0;
        snapped = false;
        jumpIndex = -1;
    }

    /// <summary>
    /// Called whenever you click on a card, jumps the carousel to viewing it
    /// </summary>
    /// <param name="index"></param>
    public void JumpToView(int index)
    {
        if(!ignoreClicks)
        {
            jumpIndex = 1 + (index * offsetBetweenElements);
            currentTime = 0;
            jumpingRatio = 0;
            currentJumpingAccelerationTime = 0;
            targetJumpingRatio = Mathf.Abs(currentPosition - jumpIndex) / offsetBetweenElements;//basically how many screens are in the way to see the one they one, and thus how fast to move between screens
        }
    }
    #endregion
}
