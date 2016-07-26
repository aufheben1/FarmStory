using UnityEngine;
using System.Collections;

public class EffectSoundManager : MonoBehaviour {
	
	public void PlayEffectSound(AudioClip source){
		if (PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0) return;
		GetComponent<AudioSource>().PlayOneShot (source);
	}

	public void PlayEffectSound(AudioClip source, float delay){
		if (PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0) return;
		StartCoroutine (PlayEffectSoundDelay (source, delay));
	}

	IEnumerator PlayEffectSoundDelay(AudioClip source, float delay){
		yield return new WaitForSeconds (delay);
		GetComponent<AudioSource>().PlayOneShot (source);
	}
	
	public void RandomEffectSound(AudioClip[] clipList){
		if (PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0) return;
		int i = Random.Range (0, clipList.Length);
		GetComponent<AudioSource>().PlayOneShot (clipList [i]);
	}
}
