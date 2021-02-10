using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    private bool tap, isDragging, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region Standalone Input

        if(Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTouch = Input.mousePosition;
            isDragging = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
            isDragging = false;
        }

        #endregion

        #region Mobile Input

        if(Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.touches[0].position;
                isDragging = true;
            }
            if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTouch = swipeDelta = Vector2.zero;
                isDragging = false;
            }
        }

        #endregion

        //Calculate Distance
        if(isDragging)
        {
            if(Input.touches.Length > 0)
            {
                swipeDelta = Input.touches[0].position -  startTouch;
            }
            if(Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }


        //Check for deadzone bounds
        if(swipeDelta.magnitude > 10)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if(Mathf.Abs(x) > Mathf.Abs(y)) //Check if the swipe is on X axis
            {
                if (x < 0)  //Left Swipe
                    swipeLeft = true;
                else        //Right Swipe
                    swipeRight = true;
            }
            else
            {
                if (y < 0)  //Down Swipe
                    swipeDown = true;
                else        //Up Swipe
                    swipeUp = true;
            }
        }
    }
}
