using UnityEngine;
using System.Collections;

public class OptionButtonOpen : MonoBehaviour {
	public GameObject targetPanel;
	public GameObject[] button;
	public GameObject pauseWindow;

	private AnimationCurve presetCurve;
		/*
		= new AnimationCurve(
		new Keyframe(0f,0f,1f,1f),
		new Keyframe(1f,1f,1f,1f)
		);
	*/
	void Start(){
		presetCurve = button[1].GetComponent<TweenPosition>().animationCurve;
	}

	IEnumerator OnCloseButtonClick(){
		TweenPosition.Begin (button[0], 0.4f, new Vector3(0,75,0));
		TweenRotation.Begin(button[0], 0.4f, Quaternion.Euler(0,0,0));
		for (int i = 1; i < 6; i++) {
			TweenPosition.Begin (button[i], 0.4f, new Vector3(0,0,0));
			button[i].GetComponent<TweenPosition>().animationCurve= presetCurve;
		}

		yield return new WaitForSeconds (0.4f);

		int[] targetY;
		float[] duration;

		if (Application.loadedLevel == 2){
			targetY = new int[]{515, 430, 325, 220, 115, 0};
			duration = new float[]{0.4f, 1.5f, 1.13f, 0.765f, 0.4f, 0};
		}
		else{
			targetY = new int[]{0, 115, 220, 325, 430, 0};
			duration = new float[]{0, 0.4f, 0.765f, 1.13f, 1.5f, 0};
		}

		for (int i = 1; i < 6; i++) {
			TweenPosition.Begin (button [i], duration [i], new Vector3 (0, targetY [i], 0));
			button[i].GetComponent<TweenPosition>().animationCurve= presetCurve;
		}
		TweenPosition.Begin(button[0], duration[0], new Vector3(0, targetY[0], 0));
		button [0].GetComponent<TweenPosition> ().from = new Vector3 (0, 75, 0);
		TweenRotation.Begin (button [0], duration [0], Quaternion.Euler (0, 0, -180));

		targetPanel.SetActive (true);
		gameObject.SetActive (false);
	}

	void OnSoundButtonClick(){
		SetOption ("Sound");
	}

	void OnEffectButtonClick(){
		SetOption ("Effect");
	}

	void OnVibrationButtonClick(){
		SetOption ("Vibration");
	}

	void OnExitButtonClick(){
		if (Application.loadedLevelName == "Map")
			Application.Quit ();
		else{
			if (pauseWindow.transform.parent.gameObject.activeSelf) return;
			GameObject.FindWithTag("GameController").SendMessage("PauseGame", SendMessageOptions.DontRequireReceiver);
			pauseWindow.SetActive(true);
			pauseWindow.transform.parent.gameObject.SetActive(true);
		}

	}
	
	void SetOption(string code){
		if (!PlayerPrefs.HasKey (code))
			PlayerPrefs.SetInt (code, 0);
		else{
			PlayerPrefs.SetInt(code, (PlayerPrefs.GetInt (code) + 1) % 2);
		}
		PlayerPrefs.Save ();
		if (code == "Sound" ){
			if (PlayerPrefs.GetInt (code) == 0)
				GameObject.FindWithTag("SoundSource").GetComponent<AudioSource>().mute = true;
			else
				GameObject.FindWithTag("SoundSource").GetComponent<AudioSource>().mute = false;
		}

		gameObject.BroadcastMessage ("SetStopsign", SendMessageOptions.DontRequireReceiver);
		//GameObject.FindWithTag ("GameController").SendMessage ("SetOption", SendMessageOptions.DontRequireReceiver);
	}

}
