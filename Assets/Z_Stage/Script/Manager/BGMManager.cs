using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	void Start(){
		if (!PlayerPrefs.HasKey ("Sound") || PlayerPrefs.GetInt ("Sound") == 1)
			GetComponent<AudioSource>().mute = false;
	}

	public void SetMute(bool token){
		GetComponent<AudioSource>().mute = token;
	}

	public void PauseBGM(){
		GetComponent<AudioSource>().Pause ();
	}

	public void ResumeBGM(){
		GetComponent<AudioSource>().Play ();
	}

	public void PlayBGM(AudioClip source){
		GetComponent<AudioSource>().clip = source;
		GetComponent<AudioSource>().Play ();
	}
}
