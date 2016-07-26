using UnityEngine;
using System.Collections;

public class TitlePage : MonoBehaviour {
	
	void Start () {
		if (!(PlayerPrefs.HasKey ("Sound") && PlayerPrefs.GetInt ("Sound") == 0)) 
			audio.mute = false;
	}
}
