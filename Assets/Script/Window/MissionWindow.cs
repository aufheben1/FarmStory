using UnityEngine;
using System.Collections;

public class MissionWindow : MonoBehaviour {
	public HeartManager heartManager;
	public GameObject[] star;
	public GameObject[] mission;
    public UILabel stageLabel;
    public UILabel limitLabel;
	private int stageNum;
	
	public void WindowInitialize(int stageNum){
		stageLabel.text = "Stage  " + stageNum.ToString ();
		this.stageNum = stageNum;
		SetStageInfo ();
	}
	
	void SetStageInfo(){
		StartCoroutine (cSetStageInfo ());
	}

	void HideStageInfo(){
		for (int i = 0; i < 3; i++) {
			mission [i].SetActive (false);
			star[i].SetActive(false);
		}
		mission [3].SetActive (false);
	}

	IEnumerator cSetStageInfo(){
        limitLabel.text = "";
		yield return new WaitForSeconds (0.5f);
		//별 개수 표시
		int starCount = 0;
		if (PlayerPrefs.HasKey (stageNum.ToString () + "star"))
			starCount = PlayerPrefs.GetInt (stageNum.ToString () + "star");
		for (int i = 0; i < starCount; i++)
			star [i].SetActive (true);

        
		StageInformation info = GetComponent<InfoTable> ().stageInfo [stageNum];

        if (info.limitType == StageInformation.LimitType.pipe)
        {
            limitLabel.text = "Limited Pipe";
        }
        else
        {
            limitLabel.text = "TimeAttack";
        }

        Vector2[] missionPosition = {
                                     new Vector2(0,-120), 
                                     new Vector2(-150,-120), new Vector2(150,-120),
                                     new Vector2(0, -80), new Vector2(-150,-200), new Vector2 (150,-200),
                                     new Vector2(-150,-80), new Vector2(150,-80), new Vector2(-150,-200), new Vector2 (150,-200)
                                    };
        int missionCount = 0;
        foreach (StageInfo.Mission stageMission in info.mission)
        {
            mission[stageMission.cropType].SetActive(true);
            mission[stageMission.cropType].transform.localPosition = missionPosition[PipeTools.GetIndex(info.mission.Length, missionCount++)];
            mission[stageMission.cropType].GetComponentInChildren<UILabel>().text = stageMission.cropCount.ToString();
        }
	}

	bool buttonClicking = false;

	IEnumerator OnStartButtonClick(){
		if (buttonClicking) yield break;
		buttonClicking = true;
		if (heartManager.IsEmpty ()) {
			Debug.Log ("하트가 모자랍니다!");
			buttonClicking = false;
			yield break;
		}
		yield return new WaitForSeconds (0.2f);
		heartManager.UseHeart ();
		Application.LoadLevel (Application.loadedLevel + stageNum);
		buttonClicking = false;
	}
}
