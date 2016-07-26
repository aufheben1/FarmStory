using UnityEngine;
using System.Collections;

public class PauseWindow : MonoBehaviour {
	public UILabel stageNum;
	public GameObject[] mission;


	private GameObject gameManager;

	void Start () {
		stageNum.text = "Stage  " + (Application.loadedLevel - 2);
        gameManager = GameObject.FindWithTag("GameController");
        /*
		gameManager = GameObject.FindWithTag ("GameController");
		char mType = gameManager.GetComponent<GameController> ().stageInfo.missionType;
		int mNum =  gameManager.GetComponent<GameController> ().stageInfo.missionNumber;

		switch (mType) {
		case 'p' : 
			mission[0].SetActive(true);
			mission[0].GetComponentInChildren<UILabel>().text = mNum.ToString();
			break;
		case 'c' : 
			mission[1].SetActive(true);
			mission[1].GetComponentInChildren<UILabel>().text = mNum.ToString();
			break;
		case 's' :
			mission[2].SetActive(true);
			mission[2].transform.Find ("Label_Text").GetComponent<UILabel>().text = mNum.ToString();
			break;
		case 'a' : 
			mission[3].SetActive(true);
			break;
		default : break;
		}
         */
	}

	void SaveStageNumberOnMap(bool setWindow){
		PlayerPrefs.SetInt ("DepartButton", Application.loadedLevel - 2);
		PlayerPrefs.SetInt ("DestinationButton", Application.loadedLevel - 2);
		PlayerPrefs.SetInt ("WindowSetOn", (setWindow ? 1 : 0));
		PlayerPrefs.Save();
		Application.LoadLevel (2);
	}

	void OnResumeButtonClick(){
		gameManager.SendMessage ("PauseWindowClose", SendMessageOptions.DontRequireReceiver);
		gameObject.SetActive (false);
	}

    void OnCloseButtonClick()
    {
        OnResumeButtonClick();
    }

	void OnReplayButtonClick(){
		SaveStageNumberOnMap (true);
	}

	void OnExitButtonClick(){
		SaveStageNumberOnMap (false);
	}

}
