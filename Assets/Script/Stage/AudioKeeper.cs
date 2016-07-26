using UnityEngine;
using System.Collections;

public class AudioKeeper : MonoBehaviour {

	public GameObject effectManager;

	[HideInInspector]
	public AudioClip mainBGM;
	[HideInInspector]
	public AudioClip waterBGM;
	public AudioClip successBGM;
	public AudioClip failBGM;

	public AudioClip successConnect;
	public AudioClip failConnect;

	[HideInInspector]
	public AudioClip readyGo;
	[HideInInspector]
	public AudioClip tileClicked;
	[HideInInspector]
	public AudioClip[] crash;
	[HideInInspector]
	public AudioClip blocked;
	[HideInInspector]
	public AudioClip waterStart;
	[HideInInspector]
	public AudioClip waterMult;
	[HideInInspector]
	public AudioClip waterIn;
	[HideInInspector]
	public AudioClip waterEnd;
	[HideInInspector]
	public AudioClip fail;
	[HideInInspector]
	public AudioClip waterExplosion;

	public AudioClip hammer1;

	public AudioClip bandFire;
	public AudioClip bandUsed;

	public AudioClip starEffect;

	// Use this for initialization
	void Start () {


		/*Load BGM*/
		mainBGM = (AudioClip) Resources.Load ("Sound/BGM/MainBGM1",typeof(AudioClip));

		if (PlayerPrefs.HasKey ("Sound") && PlayerPrefs.GetInt ("Sound") == 0) {
			GetComponent<AudioSource>().mute = true;
		}
		GetComponent<AudioSource>().clip = mainBGM;
		GetComponent<AudioSource>().Play ();
		waterBGM = 	(AudioClip) Resources.Load ("Sound/BGM/WaterBGM",typeof(AudioClip));
		successBGM = (AudioClip) Resources.Load ("Sound/BGM/Success",typeof(AudioClip));;
		failBGM = (AudioClip) Resources.Load ("Sound/BGM/Fail",typeof(AudioClip));;


		/*Load Effect Sound*/
		readyGo = (AudioClip) Resources.Load ("Sound/Effect/ReadyGo",typeof(AudioClip));
		tileClicked = (AudioClip) Resources.Load ("Sound/Effect/EmptyTile",typeof(AudioClip));
		blocked = (AudioClip) Resources.Load ("Sound/Effect/Blocked",typeof(AudioClip));
		waterStart = (AudioClip) Resources.Load ("Sound/Effect/WaterStart",typeof(AudioClip));
		waterMult = (AudioClip) Resources.Load ("Sound/Effect/WaterMult",typeof(AudioClip));
		waterIn = (AudioClip) Resources.Load ("Sound/Effect/WaterIn",typeof(AudioClip));
		waterEnd = (AudioClip) Resources.Load ("Sound/Effect/WaterEnd",typeof(AudioClip));
		fail = (AudioClip) Resources.Load ("Sound/Effect/Fail",typeof(AudioClip));
		waterExplosion = (AudioClip) Resources.Load ("Sound/Effect/WaterEx",typeof(AudioClip));
		bandFire = (AudioClip) Resources.Load ("Sound/Effect/BandFire",typeof(AudioClip));
		bandUsed = (AudioClip) Resources.Load ("Sound/Effect/BandUsed",typeof(AudioClip));
		hammer1 = (AudioClip) Resources.Load ("Sound/Effect/AllSquare1",typeof(AudioClip));
		successConnect = (AudioClip) Resources.Load ("Sound/Effect/SuccessConnect",typeof(AudioClip));
		failConnect = (AudioClip) Resources.Load ("Sound/Effect/FailConnect",typeof(AudioClip));


		crash = new AudioClip[10];
		for (int i = 0 ; i < 10 ; i++){
			crash[i] = (AudioClip) Resources.Load ("Sound/Effect/Crash/Crash" + i,typeof(AudioClip));
		}
		//starEffect = (AudioClip) Resources.Load ("Sound/Effect/StarEffect",typeof(AudioClip));
		starEffect = (AudioClip) Resources.Load ("Sound/Effect/WaterEnd",typeof(AudioClip));

	}

	public void PauseBGM(){
		GetComponent<AudioSource>().Pause ();
	}

	public void ResumeBGM(){
		GetComponent<AudioSource>().Play ();
	}


	public void PlayBGM(string name){
		AudioClip temp = (AudioClip) this.GetType ().GetField (name).GetValue (this);
		GetComponent<AudioSource>().clip = temp;
		GetComponent<AudioSource>().Play ();
	}

	public void PlayBGM(AudioClip temp){
		GetComponent<AudioSource>().clip = temp;
		GetComponent<AudioSource>().Play ();
	}

	public void PlayEffectsound(string name){
		if (PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0) return;
		AudioClip temp = (AudioClip)this.GetType ().GetField (name).GetValue (this);
		effectManager.GetComponent<AudioSource>().PlayOneShot (temp);
	}

	public void PlayEffectsound(string name, float time){
		if (PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0) return;
		AudioClip temp = (AudioClip)this.GetType ().GetField (name).GetValue (this);
		StartCoroutine(PlayEffectsoundDelay(temp, time));
	}

	IEnumerator PlayEffectsoundDelay(AudioClip clip, float time){
		yield return new WaitForSeconds (time);
		effectManager.GetComponent<AudioSource>().PlayOneShot (clip);
	}

	public void PlayEffectsound(AudioClip temp){
		if (PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0) return;
		effectManager.GetComponent<AudioSource>().PlayOneShot (temp);
	}



}
