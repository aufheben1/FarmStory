using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour {
	public AudioClip buttonEffectSound;

	public GameObject missionWindow;
	public GameObject purchaseWindow;
	public HeartManager heartManager;
	[HideInInspector]
	public bool isWindowOn = false;
	public bool initializing = true;

	void InitFinished(){
		initializing = false;
	}

	void RequireWindowOpen(int stageNum){
		if (initializing) return;
		if (isWindowOn) return;
		isWindowOn = true;
		if (!PlayerPrefs.HasKey ("Effect") || PlayerPrefs.GetInt ("Effect") == 1) {
			GetComponent<AudioSource>().PlayOneShot (buttonEffectSound);
		}
		if (!heartManager.IsEmpty ()) {
			missionWindow.SetActive (true);
			missionWindow.BroadcastMessage ("WindowInitialize", stageNum, SendMessageOptions.DontRequireReceiver);
		}
		else{
			purchaseWindow.SetActive (true);
		}

		//missionWindow.GetComponent<MissionWindow> ().WindowInitialize (stageNum);
		//missionWindow.SendMessage ("WindowInitialize", stageNum, SendMessageOptions.DontRequireReceiver);
	}

    void OnBackButtonClick(){
        if (!isWindowOn){
            Application.LoadLevel(1);
            return;
        }
        RequireWindowClose();
    }

	void RequireWindowClose(){
		if (missionWindow.activeSelf){
			missionWindow.BroadcastMessage ("HideStageInfo", SendMessageOptions.DontRequireReceiver);
			missionWindow.SetActive (false);
		}
		else if (purchaseWindow.activeSelf){
			purchaseWindow.SetActive (false);
		}
		isWindowOn = false;
	}
}
