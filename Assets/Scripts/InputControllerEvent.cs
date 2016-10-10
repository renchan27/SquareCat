﻿using UnityEngine;
using System.Collections;

public enum TouchInputType {
	Tap,
	Hold,
	Swipe
}
	
public struct TouchInput {
	public Vector2 position;
	public TouchInputType inType;
	public Vector2 swipeDistance;
}
	
public class InputControllerEvent : MonoBehaviour {

	//Delegate and event
	public delegate void TouchInputDelegate(TouchInput touchInput);
	public static event TouchInputDelegate onTouchInput = delegate {};

	//Constants
	private const int kMaxNumTouches = 2;
	private const float kMaxDistanceTap = 1f;
	private const float kTimeforHold = 0.7f; //arbitrary

	private float _holdCounter = 0f;
	private Vector2 _startPosition = new Vector2();

	void Update(){

		//Touch [] touchList;
		Touch touch;
		TouchInput touchInput = new TouchInput ();

		mousePosition (touchInput);

		if (Input.touchCount != 0) {

			for (int i = 0; i < Input.touchCount; i++) {

				touch = Input.GetTouch(i);
				Vector2 touchPosition = Camera.main.ScreenToWorldPoint (touch.position);

				switch (touch.phase) {

				case TouchPhase.Began:
					_startPosition = touchPosition;
					break;

				case TouchPhase.Moved:
					touchInput.swipeDistance = touchPosition - (Vector2)Camera.main.ScreenToWorldPoint(_startPosition) ;

					if (touchInput.swipeDistance.magnitude < kMaxDistanceTap) {
						if (_holdCounter > kTimeforHold) {
							touchInput.inType = TouchInputType.Hold;
						} else {
							touchInput.inType = TouchInputType.Tap;
						}
					} else {
						touchInput.inType = TouchInputType.Swipe;
					}
					break;

				case TouchPhase.Stationary:
					_holdCounter += Time.deltaTime;
					if (_holdCounter > kTimeforHold) {
						touchInput.inType = TouchInputType.Hold;
					} else {
						touchInput.inType = TouchInputType.Tap;
					}
					break;
				
				case TouchPhase.Ended:
					_holdCounter = 0f;
					break;
				}

				touchInput.position = touchPosition;
				onTouchInput (touchInput);
			}
		}
	}

	void mousePosition(TouchInput touchInput){

		touchInput.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown (0)) {
			_startPosition = (Vector2)Input.mousePosition;
		}

		if (Input.GetMouseButton (0)) {
			_holdCounter += Time.deltaTime;

			//Need this because Hold needs to be active before movement is over
			if ((_startPosition - (Vector2)Input.mousePosition).magnitude < kMaxDistanceTap) {
				if (_holdCounter > kTimeforHold) {
					touchInput.inType = TouchInputType.Hold;
					onTouchInput (touchInput);
				}
			}
		}
	
		if(Input.GetMouseButtonUp(0)){
			touchInput.swipeDistance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(_startPosition);

			if (touchInput.swipeDistance.magnitude < kMaxDistanceTap) {
				if (_holdCounter > kTimeforHold) {
					touchInput.inType = TouchInputType.Hold;
				} else {
					touchInput.inType = TouchInputType.Tap;
				}
			} else {
				touchInput.inType = TouchInputType.Swipe;
				Debug.Log (touchInput.inType);
			}
			onTouchInput (touchInput);
		}
	}
}
