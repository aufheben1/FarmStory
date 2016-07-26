


using UnityEngine;
using System.Collections;

public class myButtonScale : MonoBehaviour {
	public Transform tweenTarget;
	public Vector3 hover = new Vector3(1f, 1f, 1f);
	public Vector3 pressed = new Vector3(0.8f, 0.8f, 0.8f);
	public float duration = 0.2f;


	bool isPressed = false;
	bool isTweening = false;
	bool tweenReserve = false;
	
	void Start ()
	{
		if (tweenTarget == null) tweenTarget = transform;
	}
	
	void OnMouseDown(){
		BeginTween (true);
	}

	void OnMouseUp(){
		BeginTween (false);
	}

	void BeginTween(bool press){
		if (isPressed == press)	return;
		if (isTweening)	{
			tweenReserve = true;
			return;
		}
		isPressed = press;
		isTweening = true;
		iTween.ScaleTo(tweenTarget.gameObject, iTween.Hash("time", duration, "scale", press? pressed : hover, "oncomplete", "TweenOver"));

	}

	void TweenOver(){
		if (tweenReserve) {
			isPressed = !isPressed;
			tweenReserve = false;
			iTween.ScaleTo(tweenTarget.gameObject, iTween.Hash("time", duration, "scale", isPressed? pressed : hover, "oncomplete", "TweenOver"));
		}
		else{
			isTweening = false;
		}
	}

	void RemoveTween(){
		Destroy (GetComponent<iTween> ());
	}

	void OnPress (bool isPressed)
	{



	}
	
	void OnHover (bool isOver)
	{

	}
}
