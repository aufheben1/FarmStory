using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	void Start(){
		if (!PlayerPrefs.HasKey ("Sound") || PlayerPrefs.GetInt ("Sound") == 1)
			audio.mute = false;
	}

	public void SetMute(bool token){
		audio.mute = token;
	}

	public void PauseBGM(){
		audio.Pause ();
	}

	public void ResumeBGM(){
		audio.Play ();
	}

	public void PlayBGM(AudioClip source){
		audio.clip = source;
		audio.Play ();
	}
}
