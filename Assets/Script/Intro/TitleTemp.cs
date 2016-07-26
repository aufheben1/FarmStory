using UnityEngine;
using System.Collections;

public class TitleTemp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (!(PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0)) 
			GetComponent<AudioSource>().mute = false;
		StartCoroutine (GoMain ());
	}

	IEnumerator GoMain(){
		yield return new WaitForSeconds(4);
		Application.LoadLevel (Application.loadedLevel + 1);
	}

}
