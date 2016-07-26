using UnityEngine;
using System;
using System.Collections;

public class HeartManager : MonoBehaviour {
	public UILabel heartCountLabel;
	public UILabel timeCountLabel;

	private const int MAX_HEARTS = 5;
	private static TimeSpan newHeartInterval = new TimeSpan(0,3,0);

	private int heartCount;
	private DateTime lastTimeHeartDecreased;
	private TimeSpan timeToAddHeart;

	void Start () {
		if (!PlayerPrefs.HasKey ("Heart")) {
			heartCount = MAX_HEARTS;
			PlayerPrefs.SetInt ("Heart", heartCount);
			PlayerPrefs.Save();
		}
		else{
			heartCount = PlayerPrefs.GetInt ("Heart");
			heartCountLabel.text = heartCount.ToString();
		}

		GetDelayedHeartCount ();

		if (PlayerPrefs.HasKey ("AddHeart") && PlayerPrefs.GetInt ("AddHeart") == 1) {
			AddHeartByMail();
			PlayerPrefs.SetInt ("AddHeart", 0);
			PlayerPrefs.Save ();
		}
		//DateTime a = new DateTime(
	}

	void Update () {
		if (IsFull()) return;
		timeToAddHeart -= TimeSpan.FromSeconds (Time.deltaTime);
		//timeToAddHeart = timeToAddHeart.Subtract(Time.deltaTime);
		if (timeToAddHeart < new TimeSpan(0,0,0))
			AddHeart ();
		else
			//timeCountLabel.text = timeToAddHeart.Minutes.ToString() + ":" + timeToAddHeart.Seconds.ToString();
			timeCountLabel.text = new DateTime().Add (timeToAddHeart).ToString ("mm:ss");
				
	}

	void SetHeartLabel(){
		heartCountLabel.text = heartCount.ToString();
		if (IsFull())
			timeCountLabel.text = "full";
	}

	public void FullHeart(){
		heartCount = 5;
		PlayerPrefs.SetInt ("Heart", heartCount);
		PlayerPrefs.Save();
		SetHeartLabel();
		GameObject.FindWithTag ("GameController").SendMessage ("RequireWindowClose", SendMessageOptions.DontRequireReceiver);
	}

	public void AddHeart (){
		if (IsFull ())	return;
		lastTimeHeartDecreased = System.DateTime.Now;
		timeToAddHeart = newHeartInterval;
		PlayerPrefs.SetString ("HeartTime", lastTimeHeartDecreased.ToString ());
		PlayerPrefs.SetInt ("Heart", ++heartCount);
		PlayerPrefs.Save();
		SetHeartLabel();
	}

	public bool AddHeartByMail(){
		if (IsFull()) return false;
		PlayerPrefs.SetInt ("Heart", ++heartCount);
		PlayerPrefs.Save();
		SetHeartLabel();
		return true;
	}

	public bool UseHeart(){
		if (IsEmpty())
			return false;
		if (IsFull ()){
			timeToAddHeart = newHeartInterval;
			lastTimeHeartDecreased = System.DateTime.Now;
			PlayerPrefs.SetString ("HeartTime", lastTimeHeartDecreased.ToString ());
		}
		PlayerPrefs.SetInt ("Heart", --heartCount);
		PlayerPrefs.Save();
		SetHeartLabel();
		return true;
	}

	public bool IsEmpty(){
		if (heartCount <= 0)
			return true;
		else
			return false;
	}

	private bool IsFull(){
		if (heartCount >= MAX_HEARTS)
			return true;
		else
			return false;
	}	

	void OnApplicationFocus(bool focusStatus) {
		if (focusStatus)
			GetDelayedHeartCount ();
	}

	void GetDelayedHeartCount (){
		if (!IsFull()) {
			lastTimeHeartDecreased = DateTime.Parse (PlayerPrefs.GetString("HeartTime"));
			while (true){
				if (IsFull())
					break;
				lastTimeHeartDecreased = lastTimeHeartDecreased.Add (newHeartInterval);
				if (lastTimeHeartDecreased <= System.DateTime.Now)
					heartCount++;
				else
					break;
			}
			PlayerPrefs.SetInt ("Heart", heartCount);
			PlayerPrefs.Save();
			if (!IsFull ())
				timeToAddHeart = lastTimeHeartDecreased.Subtract(System.DateTime.Now);
		}
		
		SetHeartLabel();
	}

}
