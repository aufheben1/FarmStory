using UnityEngine;
using System.Collections;

public class TileButton : MonoBehaviour {

	public GameObject tweenTarget;
	public Color pressed = new Color(0.8f, 0.8f, 0.8f); 
	public float duration = 0.2f;

	public GameObject target;
	public string functionName;
	
	protected Color mColor;
	protected bool mStarted = false;
	protected bool mHighlighted = false;

	public Color defaultColor
	{
		get
		{
			mStart();
			return mColor;
		}
		set
		{
			mStart();
			mColor = value;
		}
	}

	void mStart ()
	{
		if (!mStarted)
		{
			Init();
			mStarted = true;
		}
	}

	void OnDisable ()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenColor tc = tweenTarget.GetComponent<TweenColor>();
			
			if (tc != null)
			{
				tc.color = mColor;
				tc.enabled = false;
			}
		}
	}
	
	protected void Init ()
	{
		if (tweenTarget == null) tweenTarget = transform.Find ("Tile").GetComponent<UISprite>().gameObject;
		UIWidget widget = tweenTarget.GetComponent<UIWidget>();
		
		if (widget != null)
		{
			mColor = widget.color;
		}
		else
		{
			Renderer ren = tweenTarget.GetComponent<Renderer>();
			
			if (ren != null)
			{
				mColor = ren.material.color;
			}
			else
			{
				Light lt = tweenTarget.GetComponent<Light>();
				
				if (lt != null)
				{
					mColor = lt.color;
				}
				else
				{
					Debug.LogWarning(NGUITools.GetHierarchy(gameObject) + " has nothing for UIButtonColor to color", this);
					enabled = false;
				}
			}
		}
		//OnEnable();
	}

	public virtual void OnPress (bool isPressed)
	{
		if (StageMainController.paused || StageMainController.processing) return;
		if (enabled && (GetComponent<Tile>().tileType == "em" || GetComponent<Tile>().tileType == "p"))
		{
			if (!mStarted) mStart();
			target.SendMessage ("OnTileDown", GetComponent<Tile>().vec, SendMessageOptions.DontRequireReceiver);
			TweenColor.Begin(tweenTarget, duration, isPressed ? new Color(mColor.r * 0.6f, mColor.g * 0.6f, mColor.b * 0.6f, 1) : mColor);
		}
	}

	void OnClick () { 
		if (StageMainController.paused || StageMainController.processing) return;
		if (string.IsNullOrEmpty(functionName)) return;
		if (target == null) target = gameObject;
		target.SendMessage(functionName, GetComponent<Tile>(), SendMessageOptions.DontRequireReceiver);
	}

}
