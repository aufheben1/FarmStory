using UnityEngine;
using System.Collections;

public class FailWindow : MonoBehaviour {
	public UILabel stageNum;
	public GameObject jolly_fail;

	void Start () {
		stageNum.text = "Stage  " + (Application.loadedLevel - 2);
		StartCoroutine (JollyInit ());
	}

	IEnumerator JollyInit(){
		yield return new WaitForSeconds (0.5f);
		Instantiate (jolly_fail);
	}

	void SaveStageNumberOnMap(bool setWindow){
		PlayerPrefs.SetInt ("DepartButton", Application.loadedLevel - 2);
		PlayerPrefs.SetInt ("DestinationButton", Application.loadedLevel - 2);
		PlayerPrefs.SetInt ("WindowSetOn", (setWindow ? 1 : 0));
		PlayerPrefs.Save();
		Application.LoadLevel (2);
	}

	void OnConfirmButtonClick(){
		SaveStageNumberOnMap (true);
	}

	void OnCloseButtonClick(){
		SaveStageNumberOnMap (false);
	}
}
