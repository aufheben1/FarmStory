using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuccessWindow : MonoBehaviour {
	public UILabel stageNum;
    public UILabel score;
	public GameObject allSquare;
    public GameObject[] mission;
	public GameObject[] star;
    public GameObject[] jollyStar;

	private int starCount;

	// Use this for initialization
	void Start () {
		stageNum.text = "Stage  " + (Application.loadedLevel - 2).ToString ();
		PlayerPrefs.SetInt ("AddHeart", 1);
		PlayerPrefs.Save ();
	}

    public void Initialize(int starCount, Dictionary<int, int> cropCount, int score)
    {
        StartCoroutine(ShowStar(starCount));

        this.score.text = score.ToString();
        int totalNumberOfCropType = cropCount.Count;
        
        Vector2[] cropPosition = {
                                     new Vector2(121,-100), 
                                     new Vector2(121,-30), new Vector2(121,-170),
                                     new Vector2(121, -30), new Vector2(11,-170), new Vector2 (261,-170),
                                     new Vector2(11,-30), new Vector2(261,-30), new Vector2(11,-170), new Vector2 (261,-170)
                                 };

        int count = 0;
        foreach (KeyValuePair<int, int> pair in cropCount)
        {
            mission[pair.Key].SetActive(true);
            mission[pair.Key].GetComponentInChildren<UILabel>().text = pair.Value.ToString();
            mission[pair.Key].transform.localPosition = cropPosition[PipeTools.GetIndex(totalNumberOfCropType, count++)];
        }
    }

	IEnumerator ShowStar(int starCount){
		this.starCount = starCount;
        GameObject gameController = GameObject.FindWithTag("GameController");
		
        Instantiate(jollyStar[starCount - 1]);
		
		for (int i = 0; i < starCount; i++) { 
			gameController.SendMessage("StarEffectSound", SendMessageOptions.DontRequireReceiver);
			star[i].SetActive(true);
			yield return new WaitForSeconds (0.3f);
		}
	}

	/*
	IEnumerator ShowScore(Vector3 score){
		yield return new WaitForSeconds (0.5f);
		for (int i = 0; i < 3; i++) {
			yield return new WaitForSeconds (0.3f);
			textLabel[i].gameObject.SetActive(true);
			scoreLabel[i].gameObject.SetActive(true);
			scoreLabel[i].text = score[i].ToString();
		}
		slash.gameObject.SetActive (true);
		if (GameObject.FindWithTag ("GameController").GetComponent<GameController> ().CheckAllSquare ()) {
			allSquare.SetActive (true);
			GameObject.Find ("AudioKeeper").GetComponent<AudioKeeper>().PlayEffectsound("hammer1",0.9f);
		}

	}
	*/
	void SaveStageNumberOnMap(bool setWindow){
		PlayerPrefs.SetInt ("DepartButton", Application.loadedLevel - (setWindow ? 2 : 1));
		PlayerPrefs.SetInt ("DestinationButton", Application.loadedLevel - 1);
		PlayerPrefs.SetInt ("WindowSetOn", (setWindow ? 1 : 0));
		PlayerPrefs.Save();
		Application.LoadLevel (2);
	}


	void OnConfirmButtonClick(){
		SaveStageNumberOnMap (true);
		/*다음 스테이지로 이동
		 * 현재는 바로 신으로 이동하지만
		 * 맵으로 가서 맵에서 이동하는 것을 보여줄 필요가 있음 */
		//Application.LoadLevel (Application.loadedLevel + 1); 
	}

	void OnCloseButtonClick(){
		SaveStageNumberOnMap (false);
		//Application.LoadLevel (2);
	}

	void OnScreenshotButtonClick(){
		/* 스크린샷 찍는 기능 추가 필요 */
		//GameObject.FindWithTag ("GameController").SendMessage ("TakeScreenShot", SendMessageOptions.DontRequireReceiver);
		Debug.Log ("스크린샷 찍기!! 아직 구현은 안됨...");
	}
}
