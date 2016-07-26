using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
	public UISprite flyingCrop;
	public UISprite symbol;
	public UISprite digit1;
	public UISprite digit2;
	private GameObject missionPanel;
    private int xPos = 0;
    private int nTop = 90;
    private int cTop = 120;
    private float scaleSize;

	public void Initialize(int type, bool plus, bool cri, bool top, int count, GameObject panel, float multScale){
        scaleSize = 1 / multScale;
		missionPanel = panel;
        string code;
        if (!plus)
        {
            code = "Minus_";
        }
        else if (cri)
        {
            code = "Critical_";
        }
        else
        {
            code = "Normal_";
        }

        if (top)
        {
            nTop = 60;
            cTop = 40;
        }



		flyingCrop.spriteName = PipeTools.cropNameMapper [type];
		flyingCrop.MakePixelPerfect ();

		if (plus) {
			symbol.spriteName = "Plus";
		}
		else{
			symbol.spriteName = "Minus";
		}

		if (cri) {
            /*
			flyingCrop.color = Color.red;
			crop.color = Color.red;
			symbol.color = Color.red;
			digit1.color = Color.red;
			digit2.color = Color.red;
             */
		}

		if (count < 10) {
			digit2.gameObject.SetActive(false);
			digit1.spriteName = code + count.ToString();
		}
		else{
			digit1.spriteName = code + (count / 10).ToString();
			digit2.spriteName = code + (count % 10).ToString();
            xPos = -15;
		}

		if (!plus)
			StartCoroutine (AnimateGhostMinus());
		else if (!cri)
			StartCoroutine (AnimateGhostPlus());
		else
			StartCoroutine (AnimateGhostCritical());

	}

	IEnumerator AnimateGhostCritical(){
		flyingCrop.gameObject.transform.parent = missionPanel.transform;
        flyingCrop.MakePixelPerfect();

		flyingCrop.alpha = 1;
		float flyingSpeed = Random.Range (0.2f, 0.5f);
		TweenPosition.Begin (flyingCrop.gameObject, flyingSpeed, Vector3.zero);
		TweenRotation.Begin (flyingCrop.gameObject, flyingSpeed, Quaternion.Euler (0, 0, -30));
		Destroy (flyingCrop.gameObject, flyingSpeed);
        //gameObject.transform.localScale = Vector3.one * 2.5f;
        gameObject.transform.localScale = Vector3.zero;
        TweenScale.Begin(gameObject, 0.2f, Vector3.one * 2.5f * scaleSize).method = UITweener.Method.BounceIn;
        TweenPosition.Begin(gameObject, 0.2f, new Vector3(xPos, cTop * scaleSize, 0)).method = UITweener.Method.EaseIn;

        AlphaAll(0.2f, 1);

        yield return new WaitForSeconds(0.8f);
        AlphaAll(0.2f, 0);
        //TweenPosition.Begin(gameObject, 0.2f, new Vector3(500, 90, 0));

        Destroy(gameObject, 0.2f);
	}

	IEnumerator AnimateGhostPlus(){
		flyingCrop.gameObject.transform.parent = missionPanel.transform;
        flyingCrop.MakePixelPerfect();

		flyingCrop.alpha = 1;
		float flyingSpeed = Random.Range (0.2f, 0.5f);
		TweenPosition.Begin (flyingCrop.gameObject, flyingSpeed, Vector3.right * xPos);
		TweenRotation.Begin (flyingCrop.gameObject, flyingSpeed, Quaternion.Euler (0, 0, -30));
		Destroy (flyingCrop.gameObject, flyingSpeed);
        
        gameObject.transform.localScale = Vector3.zero;
        TweenScale.Begin(gameObject, 0.3f, Vector3.one * 1.3f * scaleSize).method = UITweener.Method.BounceIn;
		TweenPosition.Begin(gameObject, 0.3f, new Vector3(xPos, nTop * scaleSize, 0)).method = UITweener.Method.EaseIn;

		AlphaAll (0.3f, 1);

		yield return new WaitForSeconds (0.8f);
		AlphaAll (0.2f, 0);
        //TweenPosition.Begin(gameObject, 0.2f, new Vector3(500, 90, 0));

		Destroy (gameObject, 0.2f);
	}

	IEnumerator AnimateGhostMinus(){
		flyingCrop.alpha = 0;

		gameObject.transform.localPosition = new Vector3 (xPos, nTop * scaleSize, 0);
        gameObject.transform.localScale = Vector3.zero;
        TweenScale.Begin(gameObject, 0.3f, Vector3.one * 1.3f * scaleSize).method = UITweener.Method.BounceIn;
        AlphaAll(0.3f, 1);

        yield return new WaitForSeconds(0.5f);

        AlphaAll(0.2f, 0);
        TweenPosition.Begin(gameObject, 0.2f, Vector3.right * xPos).method = UITweener.Method.EaseIn;
        Destroy(gameObject,0.2f);
	}

	void AlphaAll(float duration, float alpha){
		TweenAlpha.Begin (symbol.gameObject, duration, alpha);
		TweenAlpha.Begin (digit1.gameObject, duration, alpha);
		if (digit2.gameObject.activeSelf) {
			TweenAlpha.Begin (digit2.gameObject, duration, alpha);
		}
	}
}
