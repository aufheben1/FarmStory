using UnityEngine;
using System.Collections;

public class myButtonMessage : MonoBehaviour {
	/*
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}

	public GameObject target;
	public string functionName;
	public Trigger trigger = Trigger.OnClick;
	public bool includeChildren = false;

	void OnMouseUpAsButton(){
	}

	// Use this for initialization
	void Start () {
		if (target == null)	target = gameObject;
	}

	void SendMessage(){

	}
	// Update is called once per frame
	void Update () {
	
	}*/

	IEnumerator GoNext(){
		PlayerPrefs.SetInt ("DepartButton", PlayerPrefs.HasKey ("MaxStage") ? PlayerPrefs.GetInt ("MaxStage") : 1);
		PlayerPrefs.SetInt ("DestinationButton", PlayerPrefs.HasKey ("MaxStage") ? PlayerPrefs.GetInt ("MaxStage") : 1);
		PlayerPrefs.SetInt ("WindowSetOn", 0);
		PlayerPrefs.Save ();

		if (!(PlayerPrefs.HasKey ("Effect") && PlayerPrefs.GetInt ("Effect") == 0)) {
			AudioClip bSound = (AudioClip)Resources.Load ("Sound/Effect/Bbok1", typeof(AudioClip));
			GetComponent<AudioSource> ().PlayOneShot (bSound);
			//GameObject.FindWithTag ("MainCamera").GetComponent<AudioSource> ().PlayOneShot (bSound);
		}
		yield return new WaitForSeconds(0.2f);

		Application.LoadLevel (Application.loadedLevel + 1);
	}

	void OnMouseUpAsButton(){
		StartCoroutine (GoNext ());
	}
}
