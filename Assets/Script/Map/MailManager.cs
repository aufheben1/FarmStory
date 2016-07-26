using UnityEngine;
using System.Collections;

public class MailManager : MonoBehaviour {
	public UILabel mailCountLabel;
	public HeartManager heartManager;

	private const int MAX_MAIL = 99;

	private int mailCount;

	void Start () {
		if (!PlayerPrefs.HasKey ("Mail")){
			mailCount = 0; 
			PlayerPrefs.SetInt ("Mail", mailCount);
			PlayerPrefs.Save ();
		}
		else{
			mailCount = PlayerPrefs.GetInt ("Mail");
		}
		mailCountLabel.text = mailCount.ToString ();
	}

	bool IsEmpty(){
		if (mailCount <= 0)
			return true;
		else
			return false;
	}

	bool IsFull(){
		if (mailCount >= MAX_MAIL)
			return true;
		else
			return false;
	}

	void OnMailButtonClick2(){
		heartManager.UseHeart ();
	}
	void OnMailButtonClick(){
		if (IsEmpty()) return;
		if (heartManager.AddHeartByMail ()){
			PlayerPrefs.SetInt("Mail", --mailCount);
			PlayerPrefs.Save ();
			mailCountLabel.text = mailCount.ToString();
		}

	}
}
