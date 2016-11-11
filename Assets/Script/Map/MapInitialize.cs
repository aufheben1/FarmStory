using UnityEngine;
using System.Collections;

public class MapInitialize : MonoBehaviour {
	public int maxNumberOfStage;
	public int[] numberOfStageForChapter;

	public InfoTable info;
	public GameObject enableButton;
	public GameObject disableButton;
	public GameObject landmark;
	public GameObject ringRing;
	public GameObject buttonLight;
	public GameObject[] sectionClear;
	public GameObject[] sectionUnClear;
    public Warehouse warehouse;

	void Start () {
        //맵 위의 버튼 및 오브젝트 초기화
		InitializeButton ();

        //사운드 옵션에 따라 배경음악 키고 끄기
		if (!PlayerPrefs.HasKey ("Sound") || PlayerPrefs.GetInt ("Sound") == 1) {
			GameObject.FindWithTag("SoundSource").GetComponent<AudioSource>().mute = false;
		}
	}

	void InitializeButton (){
		//현재까지 열린 스테이지와 섹션 초기화
		int maxSection;
		int maxStage;
		if (!PlayerPrefs.HasKey("MaxSection"))
		{
			PlayerPrefs.SetInt("MaxSection", 0);
			PlayerPrefs.Save();
		}
		maxSection = PlayerPrefs.GetInt("MaxSection");
		if (!PlayerPrefs.HasKey("MaxStage"))
		{
			PlayerPrefs.SetInt("MaxStage", 1);
			PlayerPrefs.Save();
		}
		maxStage = PlayerPrefs.GetInt("MaxStage");
		bool newSection = false;
		int stageNum = 1;
		int sectionNum = 0;
		int accumeNum = numberOfStageForChapter[0];
		
		while (true)	{
			if (stageNum > maxNumberOfStage) break;
			if (stageNum > maxStage && stageNum > accumeNum) break;
			if (stageNum > maxStage)    //회색버튼 추가 (섹션 내 비활성 스테이지)
				MakeDisableButton(stageNum);
			else {  
				if (stageNum > accumeNum) {     //
					if (sectionNum >= maxSection)	{
						int sumCrop = 0;
						int missionCrop;
						foreach (string cropName in PipeTools.cropNameMapper) {
							if (PlayerPrefs.HasKey(cropName))
								sumCrop += PlayerPrefs.GetInt(cropName);
						}
						missionCrop = PipeTools.goalCropPerSection[maxSection];
						
						if (sumCrop >= missionCrop) {
							//섹션 통과
							PlayerPrefs.SetInt("MaxSection", maxSection + 1);
                            foreach (string cropName in PipeTools.cropNameMapper){
                                PlayerPrefs.SetInt(cropName, 0);
                            }
                            PlayerPrefs.Save();
							newSection = true;
                            break;
						}
						else {
							MakeDisableButton(stageNum);
							break;
						}
					}
					else	{	
						//이미 열린 적 있는 색션
						sectionClear[sectionNum++].SetActive(true);
						accumeNum += numberOfStageForChapter[sectionNum];
					}
				}
				//일반 버튼 넣기
				MakeEnableButton(stageNum);
			}
			stageNum++;
		}
		
		//섹션 오브젝트를 초기화
		sectionClear[sectionNum++].SetActive(true);
		for (int i = sectionNum  ; i < numberOfStageForChapter.Length ; i++)
			sectionUnClear[i].SetActive(true);

		//맵 패널의 위치 설정
		GameObject mapPanel = GameObject.Find ("Map");
		int depart = Mathf.Min (PlayerPrefs.GetInt ("DepartButton"), maxStage);
		int destination = Mathf.Min (PlayerPrefs.GetInt ("DestinationButton"), maxStage);
		float currentResolution = GetComponent<MapTraveler> ().currentResolution;

		if (newSection) {
			depart = stageNum - 1;
			destination = stageNum;
            accumeNum += numberOfStageForChapter[sectionNum + 1];
			landmark.transform.parent = GameObject.Find("Level " + depart.ToString()).transform;
			landmark.transform.localPosition = Vector3.zero;
			Vector2 buttonPos = GameObject.Find("Level " + depart.ToString()).transform.localPosition;

            mapPanel.transform.localPosition = GetComponent<MapTraveler>().ClampPosition(-buttonPos.x, -buttonPos.y);
            //mapPanel.transform.localPosition = -new Vector3(
				//Mathf.Clamp (buttonPos.x, -(1920 - 960 * currentResolution), (1920 - 960 * currentResolution)),
				//Mathf.Clamp (buttonPos.y, -(3115 - 540 * currentResolution), (3115 - 540 * currentResolution)),
				//0);

			StartCoroutine(NewSection(stageNum, accumeNum, sectionNum));
			return;
		}
		else	{
			landmark.transform.parent = GameObject.Find("Level " + maxStage.ToString()).transform;
			landmark.transform.localPosition = Vector3.zero;
			Vector2 buttonPos = GameObject.Find("Level " + depart.ToString()).transform.localPosition;

            mapPanel.transform.localPosition = GetComponent<MapTraveler>().ClampPosition(-buttonPos.x, -buttonPos.y);

            //mapPanel.transform.localPosition = -new Vector3(
//				Mathf.Clamp (buttonPos.x, -(1920 - 960 * currentResolution), (1920 - 960 * currentResolution)),
				//Mathf.Clamp (buttonPos.y, -(3115 - 540 * currentResolution), (3115 - 540 * currentResolution)),
				//0);
		}
			
		if (depart != destination) {
			StartCoroutine(SlideMap(mapPanel, destination, maxStage));
            return;
		}
		else if (PlayerPrefs.GetInt("WindowSetOn") == 1)
		{
			gameObject.SendMessage("InitFinished", SendMessageOptions.DontRequireReceiver);
			gameObject.SendMessage("RequireWindowOpen", depart, SendMessageOptions.DontRequireReceiver);
			ringRing.SetActive(true);
			ringRing.transform.parent = GameObject.Find("Level " + maxStage.ToString()).transform;
			ringRing.transform.localPosition = Vector2.zero;
		}
		else
		{
			ringRing.SetActive(true);
			ringRing.transform.parent = GameObject.Find("Level " + maxStage.ToString()).transform;
			ringRing.transform.localPosition = Vector2.zero;
		}
		gameObject.SendMessage("InitFinished", SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator SlideMap(GameObject panel, int destination, int maxStage){
		GameObject desButton = GameObject.Find ("Level " + destination.ToString ());
		float currentResolution = GetComponent<MapTraveler> ().currentResolution;
        Vector3 destinationPosition = GetComponent<MapTraveler>().ClampPosition(-desButton.transform.localPosition.x, -desButton.transform.localPosition.y);
            //new Vector3 (
			//Mathf.Clamp (-desButton.transform.localPosition.x, -(1920 - 960 * currentResolution) ,(1920 - 960 * currentResolution)),
			//Mathf.Clamp (-desButton.transform.localPosition.y, -(3115 - 540 * currentResolution),(3115 - 540 * currentResolution)),
			//0);

		float waitingTime = 1.5f;

		TweenPosition.Begin (panel, waitingTime, destinationPosition);
		if(destination == maxStage){
			landmark.transform.parent = GameObject.Find ("Level " + (destination-1).ToString()).transform;
			landmark.transform.localPosition = Vector3.zero;
			landmark.transform.parent = desButton.transform;
			TweenPosition.Begin (landmark, waitingTime, Vector3.zero);
		}

		yield return new WaitForSeconds(waitingTime);

		ringRing.SetActive(true);
		ringRing.transform.parent = GameObject.Find("Level " + maxStage.ToString()).transform;
		ringRing.transform.localPosition = Vector2.zero;

        yield return new WaitForSeconds(1);
		gameObject.SendMessage("InitFinished", SendMessageOptions.DontRequireReceiver);
		gameObject.SendMessage("RequireWindowOpen", destination, SendMessageOptions.DontRequireReceiver);
	}

    IEnumerator NewSection(int currentStage, int accumeNum, int sectionNum)
    {
        sectionClear[sectionNum].SetActive(true);
        sectionUnClear[sectionNum].SetActive(false);
        yield return new WaitForSeconds(0.5f);
        warehouse.Initialize();
        int section = PlayerPrefs.GetInt("Section");
        for (int buttonNum = currentStage; buttonNum < accumeNum; buttonNum++)
        {
            MakeDisableButton(buttonNum);
        }

        GameObject panel = GameObject.Find("Map");
        GameObject desButton = GameObject.Find("Level " + currentStage.ToString());
        float currentResolution = GetComponent<MapTraveler>().currentResolution;


        Vector3 destinationPosition = GetComponent<MapTraveler>().ClampPosition(-desButton.transform.localPosition.x, -desButton.transform.localPosition.y);
        //new Vector3(
            //Mathf.Clamp(-desButton.transform.localPosition.x, -(1920 - 960 * currentResolution), (1920 - 960 * currentResolution)),
            //Mathf.Clamp(-desButton.transform.localPosition.y, -(3115 - 540 * currentResolution), (3115 - 540 * currentResolution)),
            //0);

        float waitingTime = 1.5f;

        TweenPosition.Begin(panel, waitingTime, destinationPosition);
        landmark.transform.parent = GameObject.Find("Level " + (currentStage- 1).ToString()).transform;
        landmark.transform.localPosition = Vector3.zero;
        landmark.transform.parent = desButton.transform;
        TweenPosition.Begin(landmark, waitingTime, Vector3.zero);

        yield return new WaitForSeconds(waitingTime);

        GameObject stageButtonPanel = GameObject.Find("Level " + currentStage.ToString());
        buttonLight.SetActive(true);
        buttonLight.transform.parent = GameObject.Find("Level " + currentStage.ToString()).transform;
        buttonLight.transform.localPosition = Vector2.zero;
        TweenAlpha.Begin(buttonLight, 0.5f, 1).from = 0.4f;
        yield return new WaitForSeconds(0.5f);
        TweenAlpha.Begin(buttonLight, 1.5f, 0);
        stageButtonPanel.GetComponentInChildren<UIImageButton>().gameObject.SetActive(false);
        MakeEnableButton(currentStage);

        
        yield return new WaitForSeconds(0.75f);
        ringRing.SetActive(true);
        ringRing.transform.parent = GameObject.Find("Level " + currentStage.ToString()).transform;
        ringRing.transform.localPosition = Vector2.zero;
        yield return new WaitForSeconds(0.75f);

        gameObject.SendMessage("InitFinished", SendMessageOptions.DontRequireReceiver);
        gameObject.SendMessage("RequireWindowOpen", currentStage, SendMessageOptions.DontRequireReceiver);
    }
	
	void MakeDisableButton(int stageNum)
	{
		GameObject stageButtonPanel = GameObject.Find("Level " + stageNum.ToString());
		GameObject stageButton = NGUITools.AddChild(stageButtonPanel, disableButton);
		stageButton.transform.localScale = Vector2.one * 0.7f;
		UIImageButton tempButton = stageButton.GetComponent<UIImageButton>();
		tempButton.normalSprite = "im_map_button_disabled_" + (stageNum % 4 + 1).ToString();
		tempButton.hoverSprite = "im_map_button_disabled_" + (stageNum % 4 + 1).ToString();
		tempButton.pressedSprite= "im_map_button_disabled_" + (stageNum % 4 + 1).ToString();
		tempButton.disabledSprite= "im_map_button_disabled_" + (stageNum % 4 + 1).ToString();
	}
		
	void MakeEnableButton(int stageNum)
	{
		GameObject stageButtonPanel = GameObject.Find("Level " + stageNum.ToString());
		GameObject stageButton = NGUITools.AddChild(stageButtonPanel, enableButton);
		stageButton.transform.localScale = Vector2.one * 0.7f;
		UIImageButton tempButton = stageButton.GetComponent<UIImageButton>();

		tempButton.GetComponentInChildren<UISprite>().spriteName = "im_map_button_" + (stageNum % 4 + 1).ToString();
		tempButton.normalSprite = "im_map_button_" + (stageNum % 4 + 1).ToString();
		tempButton.hoverSprite = "im_map_button_" + (stageNum % 4 + 1).ToString();
		tempButton.pressedSprite = "im_map_button_" + (stageNum % 4 + 1).ToString();
		tempButton.disabledSprite = "im_map_button_" + (stageNum % 4 + 1).ToString();
		tempButton.GetComponentInChildren<UILabel>().text = stageNum.ToString();
        if (PlayerPrefs.HasKey(stageNum + "star"))
            for (int i = 0; i < PlayerPrefs.GetInt(stageNum + "star"); i++)
                stageButton.transform.Find("Sprite_star" + i.ToString()).gameObject.SetActive(true);
	}
}
