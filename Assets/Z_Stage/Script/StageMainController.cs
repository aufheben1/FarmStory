using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StageMainController : MonoBehaviour {
	//게임 상태를 저장하기 위한 정적 변수들
	public static bool paused;
	public static bool flowing;
	public static bool flowPaused;
	public static bool processing;
    public static bool windowOpen;

	private enum TouchMode{Default, Hammer, Obstacle};

	private TouchMode touchMode = TouchMode.Default;
	
	public Tile[,] tile;
	public Pipe[] nextPipe;

	private Tile startTile;
	private Tile endTile;
	private GameObject endBubble;
	private Tile waterFlowingTile;

	private UISlider flowSlider;

	private GameObject preview;

	private bool endSliding = false;

	private int sumPipeWeight;
	private float processingTime = 0.4f;
	private float waterFlowDuration;
	private float waterFlowDurationFast;

	private int score = 0;
	private int pipeUsed = 0;
	private int totalPipe = 0;
	private int totalCrosspipe = 0;
	private float flowedTime = 0;

	//스코어바 슬라이딩 관련 정보
	private bool scorebarSliding = false;	
	private float scorebarSlideTimer;
	private float scorebarSlideStartValue = 0;
	private float scorebarSlideDestinationValue = 0;

		private int flowingDir;

	//게임오브젝트와 관련있는 컴포넌트들
	private StageInfo stageInfo;
	private BGMManager bgmManager;
	private EffectSoundManager effectsoundManager;
	private PrefabManager prefabManager;
	public ObjectManager objectManager;
	private SoundManager soundManager;

	private Dictionary<int, int> cropCount = new Dictionary<int, int>();

	//물리 계산을 위한 정보
	private int[,] pipeinfo = {{0, 1, 4, 4},
		{4, 4, 2, 3},
		{0, 1, 2, 3},
		{4, 3, 0, 4},
		{4, 2, 4, 0},
		{3, 4, 1, 4},
		{2, 4, 4, 1}};		//pipeNum에 따른 outDir 값
	
	private int[] nextTileX = {-1, 1, 0, 0};
	private int[] nextTileY = {0, 0, 1, -1};		//방향에 따라 다음 타일을 찾기 위한 정보
	private int[] nextAngle = {2,0,1,3};			//방향에 따라 포인트 화살표의 각도를 정하기 위한 정보
	private int[] boxPosition = {-484, -316, -229, -143, -56};


	void Awake(){
		paused = true;
		flowPaused = false;
		flowing = false;
		processing = false;
        windowOpen = false;
	}

	public void StartStage(){
		StartCoroutine (LoadGame ());
	}

	public void LoadComponent(){
		stageInfo = GetComponent<StageInfo> ();
		bgmManager = GetComponentInChildren<BGMManager>();
		effectsoundManager = GetComponentInChildren<EffectSoundManager>();
		prefabManager = GetComponentInChildren<PrefabManager> ();
		objectManager = GetComponentInChildren<ObjectManager> ();
		soundManager = GetComponentInChildren<SoundManager> ();
	}

	public bool tutorialStage = false;

	IEnumerator LoadGame(){
		//스테이지 의미션 정보 받아와 설정
		SetStageData();

		bgmManager.PlayBGM (soundManager.stageMainBGM);

		//인터페이스 기본 세팅 후 이동
		float routineTime = 2f;
		InitilizeTile ();
		SetMission ();
		MoveInterface (routineTime);

		yield return new WaitForSeconds (routineTime * 0.5f);
		StartCoroutine (ReadyGo ()); //레디 고 
        if (GameObject.FindWithTag("Tutorial") != null)
        {
			tutorialStage = true;
            GameObject.FindWithTag("Tutorial").SendMessage("InitController", SendMessageOptions.DontRequireReceiver);
        }
		yield return new WaitForSeconds (routineTime * 0.5f);

        for (int i = 0; i < 3; i++)
        {
            TweenPosition.Begin(objectManager.scoreStar[i], 0.5f, new Vector3(120f * stageInfo.starCondition[i] / stageInfo.starCondition[2] -70, -40, 0));
        }

		routineTime = 0.4f;
		InitializeSpecialTile (); //인아웃 스타트 엔드 등 재설정

		//파이프 생성
		nextPipe = new Pipe[5];
		for (int i = 0; i < 5; i++) {
			for (int j = 0 ; j < i; j++){
				TweenPosition.Begin (nextPipe[j].gameObject, routineTime, Vector3.up * boxPosition[4 - i + j]).method = UITweener.Method.EaseIn;
			}
			yield return new WaitForSeconds(routineTime);
			nextPipe[i] = GeneratePipe();
		}
		nextPipe [0].transform.localScale = Vector3.one * 1.143f;

		//프리뷰 만들기
		SetPreview ((int)startTile.vec.x + nextTileX [startTile.pipeNum], (int)startTile.vec.y + nextTileY [startTile.pipeNum]);
		ComputeRoad ();		//최초 계산

		Unload ();//다쓴거 언로드 (가능한가?)

        objectManager.missionButtonScale.enabled = true;
        loadFinished = true;

		yield return new WaitForSeconds (1.5f);
		StartCoroutine (StartGame ());	
		/*튜토리얼 시작하기! - 언제 해야할지는 잘..*/
	}

    bool loadFinished = false;
	bool gamestart =false;

    void OnBackButtonClick()
    {
        if (!gamestart)
        {
            return;
        }
        else if (windowOpen)
        {
            objectManager.windowPanel.BroadcastMessage("OnCloseButtonClick", SendMessageOptions.DontRequireReceiver);
            return;
        }
        else
        {
            objectManager.optionButton.SendMessage("OnButtonClick", SendMessageOptions.DontRequireReceiver);
        }

    }


    IEnumerator StartGame()
    {
        if (loadFinished && !gamestart)
        {
			gamestart =true;
			objectManager.missionWindowPanel.GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(0.2f);
            objectManager.missionWindowPanel.transform.parent = objectManager.topRightAnchor.transform;
            TweenScale tempScale = TweenScale.Begin(objectManager.missionWindowPanel, 0.6f, Vector3.one * 0.2f);
            tempScale.method = UITweener.Method.EaseIn;
            TweenPosition tempPos = TweenPosition.Begin(objectManager.missionWindowPanel, 0.6f, new Vector3(-111, -400, 0));
            tempPos.method = UITweener.Method.EaseIn;
            Destroy(objectManager.missionWindowPanel, 0.6f);
            yield return new WaitForSeconds(0.6f);

            int missionCount = 0;
            int[] panelY = { 0, -83, -166, -249 };
            foreach (StageInfo.Mission mission in stageInfo.mission)
            {
                objectManager.mission[mission.cropType].panel.SetActive(true);
                objectManager.mission[mission.cropType].currentLabel.text = (0).ToString();
                objectManager.mission[mission.cropType].goalLabel.text = mission.cropCount.ToString();
                if (mission.cropCount > 100)
                {
                    objectManager.mission[mission.cropType].goalLabel.transform.localScale = Vector2.one * 30;
                }
                else
                {
                    objectManager.mission[mission.cropType].goalLabel.transform.localScale = Vector2.one * 40;
                }
                objectManager.mission[mission.cropType].panel.transform.localPosition = new Vector2(8, panelY[missionCount++]);
            }

            if (tutorialStage)
            {
                GameObject.FindWithTag("Tutorial").SendMessage("SetTutorial", scaleSize, SendMessageOptions.DontRequireReceiver);
                yield break;
            }

            paused = false;

        }
    }

	void SetStageData(){
		//미션에 따른 설정
		if (stageInfo.gameMode == StageInfo.GameMode.timeAttack) {
            timelimit = (float)stageInfo.limitNum;
			waterFlowDuration = 3;
			waterFlowDurationFast = 3;
			objectManager.itemButton[3].SetActive (false);
            objectManager.restrictBox.GetComponentInChildren<UISprite>().spriteName = "Clock";
            objectManager.restrictBox.GetComponentInChildren<UISprite>().MakePixelPerfect();
            objectManager.secondHand.gameObject.SetActive(true);
            objectManager.restrictNumber.pivot = UIWidget.Pivot.BottomLeft;
            objectManager.restrictNumber.transform.localPosition = new Vector2(16, 541);
            objectManager.restrictNumber.transform.localScale = Vector2.one * 50;
            objectManager.restrictNumber.color = new Color(1, 0.871f, 0.09f);
            objectManager.restrictNumber.effectColor = new Color(0, 0, 0, 0);
            objectManager.restrictNumber2.gameObject.SetActive(true);
            objectManager.restrictNumber2.pivot = UIWidget.Pivot.BottomLeft;
            objectManager.restrictNumber2.transform.localPosition = new Vector2(16, 541);
            objectManager.restrictNumber2.transform.localScale = Vector2.one * 50;
		} else{
			waterFlowDuration = 0.2f;
			waterFlowDurationFast = 0.2f;
			objectManager.itemButton[0].SetActive (false);
            objectManager.restrictNumber.text = stageInfo.limitNum.ToString();
		}

		int stageNum = Application.loadedLevel - 2;
		int sectionNum = 0;
		for (int i = 0; i < PipeTools.NumOfStagePerSection.Length; i++) {
			if (stageNum <= PipeTools.NumOfStagePerSection[i]){
				sectionNum = i;
				break;
			}
			else{
				stageNum -= PipeTools.NumOfStagePerSection[i];
			}
		}
		int[] bgNum = {0,0,1,2,3,4,4};
		objectManager.background.mainTexture = (Texture) Resources.Load ("Background/" + bgNum[sectionNum].ToString(), typeof(Texture));

		
		//파이프 가중치 총합 합산
		sumPipeWeight = stageInfo.GetSumPipeWeight ();
	}

	void InitilizeTile(){
		//패널들의 초기위치 및 크기 설정
		objectManager.tilePanel.transform.localPosition = new Vector2 (0, 2500);
		objectManager.nextPanel.transform.localPosition = new Vector2 (111, 2500);
		objectManager.itemPanel.transform.localPosition = new Vector2 (-1500, 0);
		objectManager.scorePanel.transform.localPosition = new Vector2(1500, -160);
		objectManager.missionWindowPanel.transform.localPosition = new Vector2 (0, -1000);
		objectManager.startButtonPanel.transform.localScale = Vector2.zero;
        objectManager.missionPanel.transform.localPosition = new Vector2(1500, -480);
		foreach (GameObject obj in objectManager.itemButton) {
			obj.transform.localScale = Vector2.zero;
		}

		//모든 타일 위치 보정 및 스타트 엔드 타일 스프라이트 교체
		tile = new Tile[10, 8];
		for (int i = 0; i < 10; i++){
			for (int j = 0; j < 8; j++){
				tile[i,j] = objectManager.tilePanel.transform.Find("Tile" + i.ToString() + j.ToString()).GetComponent<Tile>();
				tile[i,j].Initialize(i, j);
			}

			if (tile[i,0].tileType != "none"){
				GameObject bot = NGUITools.AddChild(tile[i,0].gameObject, prefabManager.emptyBottom);
				bot.transform.localScale = new Vector2(126, 145);
			}
			for (int j = 1; j < 8; j++){
				if(tile[i,j].tileType != "none" && tile[i, j-1].tileType == "none"){
					GameObject bot = NGUITools.AddChild(tile[i,j].gameObject, prefabManager.emptyBottom);
					bot.transform.localScale = new Vector2(126, 145);
				}
			}
		}

        //타일 사이즈 조정
        int xCount = 10;
        int yCount = 8;

        for (int i = 0; i < 10; i++)
        {
            if (IsLineBlank(i, true)) xCount--;
        }

        for (int i = 0; i < 8; i++)
        {
            if (IsLineBlank(i, false)) yCount--;
        }

        if (xCount % 2 == 1) xCount++;
        if (yCount % 2 == 1) yCount++;

        float resolution = Screen.width / (float) Screen.height;

        scaleSize = Mathf.Min(10f / xCount, 8f / yCount, 1.7f) * Mathf.Min(resolution / 1.7f , 1f);

        objectManager.tilePanel.transform.localScale = Vector2.one * scaleSize;
	}

    float scaleSize = 1;

    bool IsLineBlank(int lineNum, bool xLine)
    {
        for (int i = 0; i < (xLine ? 8 : 10); i++)
        {
            if (xLine && tile[lineNum, i].tileType != "none")
                return false;
            if (!xLine && tile[i, lineNum].tileType != "none")
                return false;
        }
        return true;
    }

    int inoutCount = 0;

	void InitializeSpecialTile(){
		//타일패널 이동 후 값 설정 및 스프라이트 변경 함수
		GameObject tempObject;
		for (int i = 0; i < 10; i++){
			for (int j = 0; j < 8; j++){
				if (tile[i,j].outVec.Length != 0 && tile[i,j].tileType == "em"){
					//인아웃 화살표 설정
					for (int k = 0; k < tile[i,j].outVec.Length; k++){
						tempObject= NGUITools.AddChild (objectManager.tilePanel, prefabManager.inoutArrow);
						tempObject.transform.localScale = new Vector2(80,55);
						tempObject.transform.localPosition = PipeTools.GetPosition (i,j) + 63 * new Vector2(nextTileX[(int)tile[i,j].outVec[k].x], nextTileY[(int)tile[i,j].outVec[k].x]);
						tempObject.transform.localRotation = Quaternion.Euler (0,0, tile[i,j].outVec[k].x < 2 ? 90 : 0 );
					}
				}
				switch(tile[i,j].tileType){
				case "i" :
                    tile[i,j].outDir[tile[i,j].pipeNum] = 0;
                    tempObject = NGUITools.AddChild(tile[i, j].gameObject, prefabManager.inOut);
                    tempObject.GetComponent<InOutInitialize>().Initialize(PipeTools.ReverseDir(tile[i, j].pipeNum), inoutCount, true);

                    int nX = (int)tile[i, j].outVec[0].x;
                    int nY= (int)tile[i, j].outVec[0].y;

                    tempObject = NGUITools.AddChild(tile[nX, nY].gameObject, prefabManager.inOut);
                    tempObject.GetComponent<InOutInitialize>().Initialize(tile[nX, nY].pipeNum, inoutCount++, false);
					break;
				case "o" :
					tile[i,j].outDir[0] = 0;
					break;
				case "s" :
					//스타트 타일 
					startTile = tile[i,j];
					tempObject = NGUITools.AddChild(objectManager.tilePanel, prefabManager.startTile);
                        
					tempObject.transform.Find ("Pond").GetComponent<UISprite>().spriteName = "Pond_" + startTile.pipeNum;
					tempObject.transform.Find ("Water").GetComponent<UISprite>().spriteName = "Water_" + startTile.pipeNum;
					tempObject.transform.Find ("Stone").localPosition = new Vector2(nextTileX[startTile.pipeNum], nextTileY[startTile.pipeNum]) * 63;
					if (startTile.pipeNum == 3){
						tempObject.transform.Find ("Pond").localScale = new Vector2(134, 135);
						tempObject.transform.Find ("Water").localScale = new Vector2(134, 135);
					}
					tempObject.transform.localPosition = new Vector3(1200, 0, 0);
					tempObject.transform.parent = tile[i,j].transform ;
                    tempObject.transform.localScale = Vector2.one;
					TweenPosition.Begin (tempObject, 0.4f, Vector3.back);
					startTile.outDir[startTile.pipeNum] = startTile.pipeNum;
					break;
				case "e" :
					//엔드 타일
					endTile = tile[i,j];
					tempObject = NGUITools.AddChild(objectManager.tilePanel, prefabManager.endTile);
					tempObject.transform.Find ("Pond").GetComponent<UISprite>().spriteName = "Pond_" + endTile.pipeNum;
					tempObject.transform.Find("Slider").Find ("Water").GetComponent<UISprite>().spriteName = "Water_" + endTile.pipeNum;
				
					if (endTile.pipeNum == 3){
						tempObject.transform.Find ("Pond").localScale = new Vector2(134, 135);
						tempObject.transform.Find("Slider").Find ("Water").localScale = new Vector2(134, 135);
					}
					UISlider tempSprite = tempObject.GetComponentInChildren<UISlider>();
					UISprite tempSprite2 = tempObject.transform.Find("Slider").Find ("Water").GetComponent<UISprite>();
					tempSprite.sliderValue = 0;
					if (endTile.pipeNum < 2){
						tempSprite2.fillDirection = UISprite.FillDirection.Horizontal;
					}
					else{
						tempSprite2.fillDirection = UISprite.FillDirection.Vertical;
					}


					if (endTile.pipeNum % 3 ==0){
						tempSprite2.invert = false;
					}
					else{
						tempSprite2.invert = true;
					}

					tempObject.transform.localPosition = new Vector3(-1200, 0, -1);
					tempObject.transform.parent = tile[i,j].transform;
                    tempObject.transform.localScale = Vector2.one;
					endBubble = endTile.transform.Find ("EndTile(Clone)").Find ("Bubble").gameObject;
					endBubble.SetActive (false);
					tile[i,j].outDir[PipeTools.ReverseDir(tile[i,j].pipeNum)] = 0;
					TweenPosition.Begin (tempObject, 0.4f, Vector3.back);
					break;
				}
				if (tile[i,j].tileType != "none"){
					tile[i,j].gameObject.SendMessage("mStart",SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	IEnumerator ReadyGo (){
		//미션창 중앙 디스플레이 및 이동
		objectManager.missionWindowPanel.GetComponent<Animator> ().enabled = true;
		TweenPosition.Begin (objectManager.missionWindowPanel, 0.6f, Vector3.zero).method = UITweener.Method.EaseIn;
		//TweenScale.Begin (objectManager.missionPanel, 0.6f, Vector3.one * 3).method = UITweener.Method.BounceIn;
		yield return new WaitForSeconds (1.4f);
		//objectManager.missionPanel.transform.parent = objectManager.topRightAnchor.transform;

		effectsoundManager.PlayEffectSound (soundManager.readyGo);
		yield return new WaitForSeconds (1.5f);
		
	}

	void SetMission (){
		//미션창 내부 좌표 배치
        Vector2[] centerMissionPosition = { 
                                     new Vector2(0,-110), 
                                     new Vector2(-180,-110), new Vector2(180,-110),
                                     new Vector2(0, -60), new Vector2(-180,-200), new Vector2 (180,-200),
                                     new Vector2(-180,-60), new Vector2(180,-60), new Vector2(-180,-200), new Vector2 (180,-200)
                                          };
        string gameModeText;
        if (stageInfo.gameMode == StageInfo.GameMode.limitedPipe)
        {
            gameModeText = "by laying " + stageInfo.limitNum.ToString() + " pipes";
        }
        else
        {
            gameModeText = "in time";
        }

        objectManager.missionLabel.text = "Grow the crops \n" + gameModeText;

        int[] windowY = {212, 274, 352, 452};
		int missionCount = 0;
		foreach (StageInfo.Mission mission in stageInfo.mission) {
            objectManager.centerMission[mission.cropType].SetActive(true);
            objectManager.centerMission[mission.cropType].GetComponentInChildren<UILabel>().text = mission.cropCount.ToString();
            objectManager.centerMission[mission.cropType].transform.localPosition = centerMissionPosition[PipeTools.GetIndex(stageInfo.mission.Length, missionCount++)];
		}
        objectManager.missionBackground.transform.localScale = new Vector2(200, windowY[stageInfo.mission.Length - 1]);

 
	}

	void MoveInterface(float time){
		Destroy (objectManager.loading);
		/*여러 패널들에 대한 트윈 설정*/
		TweenPosition.Begin (objectManager.nextPanel, time, new Vector3 (111, 0, 0));
		TweenPosition.Begin (objectManager.tilePanel, time, new Vector3(0,-10,0));
		TweenPosition.Begin (objectManager.itemPanel, time, new Vector3 (111, 0, 0));
		TweenPosition.Begin (objectManager.scorePanel, time, new Vector2 (-111, -160));
        TweenPosition.Begin(objectManager.missionPanel, time, new Vector2(-111, -480));
		//TweenPosition.Begin (objectManager.missionWindowPanel, time, Vector2.zero);

		TweenScale tempTween = TweenScale.Begin (objectManager.startButtonPanel, time * 0.5f, Vector2.one);
		tempTween.delay = time;
		tempTween.method = UITweener.Method.BounceIn;
		Destroy (tempTween, time * 2);

		foreach (GameObject obj in objectManager.itemButton) {
			tempTween = TweenScale.Begin (obj,time * 0.5f, Vector3.one);
			tempTween.delay = time;
			tempTween.method = UITweener.Method.BounceIn;
			Destroy (tempTween, time * 2);
		}

	}

    Vector2 GetBandPosition(float x, float y)
    {
        return new Vector2( -x * 126 - 300, -(y - 2) * 126);
    }

    IEnumerator OnBandageButtonClick()
    {
        if (!flowing || flowPaused || paused) yield break;
        GameObject band = NGUITools.AddChild(waterFlowingTile.gameObject, prefabManager.bandage);
        flowPaused = true;
        band.GetComponent<UISprite>().MakePixelPerfect();
        band.transform.localPosition = GetBandPosition(waterFlowingTile.vec.x, waterFlowingTile.vec.y);
        TweenPosition.Begin(band, 0.2f, Vector3.zero);
        //밴드 소리
        effectsoundManager.PlayEffectSound(soundManager.bandFly);
        yield return new WaitForSeconds(0.2f);
        TweenScale.Begin(band, 0.5f, new Vector3(140, 140)).style = UITweener.Style.Loop;
        yield return new WaitForSeconds(3);
        band.GetComponent<UISprite>().MakePixelPerfect();
        TweenScale.Begin(band, 0.2f, new Vector3(140, 140)).style = UITweener.Style.Loop;
        yield return new WaitForSeconds(2);
        //밴드 떨어지는 소리
        effectsoundManager.PlayEffectSound(soundManager.bandFinish);
        TweenScale.Begin(band, 0.5f, new Vector3(480,480,0));
        TweenAlpha.Begin(band, 0.5f, 0);
        yield return new WaitForSeconds(0.5f);
        objectManager.itemButton[0].SendMessage("ItemUsed", SendMessageOptions.DontRequireReceiver);
        flowPaused = false;
        Vibrate();
        //이동 대기

        //박힌 동안의 효과
        //박힌 동안 대기
        //날아가는 효과
        //아이템 버튼 샌드메세지
    }

	void OnHammerButtonClick(){
		//아이템 버튼 클릭 시 해머모드 설정/해제
		if (paused)	return;
		ChangeTouchMode (TouchMode.Hammer);
	}

	IEnumerator ItemHammer(Tile targetTile){
		processing = true;
		//터치모드 변경
		ChangeTouchMode (TouchMode.Default);

		if ((targetTile.tileType == "p" || targetTile.tileType == "op") && !targetTile.isFlowed) {
			//파이프가 있을 경우 풀 삭제
			targetTile.DestroyPipe (0.5f);
			//targetTile.grass.spriteName = "TextureEmpty";
		}
		else if (targetTile.tileType != "ob"){
			//물이 이미 흘렀거나 장애물 타일이 아닐 경우 실행취소
			processing = false;
			yield break;
		}

		//해머 애니메이션
		GameObject hammer = NGUITools.AddChild (targetTile.gameObject, prefabManager.hammer);
		hammer.transform.localPosition = new Vector2 (-160, 70);
		Destroy (hammer, 0.5f);
		targetTile.tileType = "em";
		targetTile.outDir = new int[]{4,4,4,4};
		//길 계산
		ComputeRoad ();
		objectManager.itemButton [1].SendMessage ("ItemUsed", SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.5f);
		objectManager.hammerHighlight.transform.parent = targetTile.transform;
		objectManager.hammerHighlight.transform.localPosition = Vector2.zero;
		objectManager.hammerHighlight.SetActive(true);
		TweenScale.Begin (objectManager.hammerHighlight, 0.2f, new Vector3 (300, 300, 1)).from = new Vector3(130,130,1);
		TweenAlpha.Begin (objectManager.hammerHighlight, 0.2f, 1).from = 0.5f;
        effectsoundManager.PlayEffectSound(soundManager.hammer);
		StartCoroutine (WaitHide(objectManager.hammerHighlight, 0.3f));

		Destroy(Instantiate(prefabManager.pipeExplosion, PipeTools.GetPosition(targetTile.vec.x, targetTile.vec.y) / 540f * scaleSize, transform.rotation), 0.5f);
		//타일 속성 재설정 및 종료
		targetTile.Initialize ();
		processing = false;

		yield break;
	}

		
	IEnumerator WaitHide(GameObject gObj, float time){
		yield return new WaitForSeconds (time);
		gObj.SetActive (false);
	}

	void OnObstacleButtonClick(){
		//아이템 버튼 클릭 시 터치모드 변경
		if (paused)	return;
		ChangeTouchMode (TouchMode.Obstacle);
	}

	IEnumerator ItemObstacle(Tile targetTile){
		processing = true;
		//터치모드 변경
		ChangeTouchMode (TouchMode.Default);

		if ((targetTile.tileType == "p" || targetTile.tileType == "em") && !targetTile.isFlowed) {
			//타일이 장애물로 변경가능한 타일일 시
			if (targetTile.tileType == "p") {
				//파이프가 있을 경우 파이프 삭제
				targetTile.DestroyPipe();
			}
			//장애물 효과
			GameObject anim = NGUITools.AddChild(targetTile.gameObject, prefabManager.obstacleGen);
			Destroy (anim, 0.58f);
			targetTile.tileType = "ob";
			ComputeRoad();	//길계산
			objectManager.itemButton[2].SendMessage("ItemUsed", SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(0.58f);
            effectsoundManager.PlayEffectSound(soundManager.obstacle);
			//타일 속성 재설정 및 종료
			targetTile.Initialize ();
		}
		processing = false;
	}

	IEnumerator OnResetButtonClick(){
		//리셋 버튼 클릭시 바로 동작
		if (processing || paused) yield break;

		int numToReset = (stageInfo.limitNum - pipeUsed >= 5 ? 5 : stageInfo.limitNum - pipeUsed);
		processing = true;
		objectManager.resetHighlight.SetActive (true);
		TweenAlpha.Begin (objectManager.resetHighlight, 3, 0).from = 1;

		for (int i = 0; i < numToReset; i++) {
			Destroy(nextPipe[i].gameObject);
		}

		for (int i = 0; i < 5 ; i++){
			for (int j = 0; j < i ; j++){
				if (nextPipe[j])
					TweenPosition.Begin (nextPipe[j].gameObject, 0.4f, Vector3.up * boxPosition[4 - i + j]).method = UITweener.Method.EaseIn;
			}
            effectsoundManager.PlayEffectSound(soundManager.pipeCount);
			yield return new WaitForSeconds(0.4f);
			if (i < numToReset)
				nextPipe[i] = GeneratePipe();
		}
		nextPipe [0].transform.localScale= Vector3.one * 1.143f;

		objectManager.resetHighlight.SetActive (false);
		ComputeRoad ();
		processing = false;
		objectManager.itemButton [3].SendMessage ("ItemUsed", SendMessageOptions.DontRequireReceiver);
	}
    bool touchable = true;

    IEnumerator touchModeChange()
    {
        yield return new WaitForSeconds(0.5f);
        touchable = true;
    }

	void ChangeTouchMode(TouchMode mode){
        if (!touchable) return;
        touchable = false;
        StartCoroutine(touchModeChange());
		//터치모드 변경
		switch (touchMode){
		//현재 터치모드에 따라 아이템 버튼의 색상 변경
		case TouchMode.Hammer : 
			objectManager.hammerCover.SetActive(false);
			break;
		case TouchMode.Obstacle :
			objectManager.obstacleCover.SetActive(false);
			break;
		}

		if (touchMode == mode) {
			//기본모드로 변경
			touchMode = TouchMode.Default;
			return;
		}

		switch (mode) {
		//변경될 터치모드에 따라 버튼 색상 변경
		case TouchMode.Hammer :
            objectManager.hammerCover.SetActive(true);
			break;
		case TouchMode.Obstacle :
			objectManager.obstacleCover.SetActive(true);
			break;
		}
		touchMode = mode;
	}

	void OnTileDown(Vector2 vec){
		if (touchMode == TouchMode.Default)
			SetPreview ((int)vec.x, (int)vec.y);
	}

	void OnTileClick(Tile clickedTile){
		//타일 클릭시 모드에 따른 동작 설정

		if (clickedTile.tileType != "s" && clickedTile.tileType != "e") {
			switch(touchMode){
			case TouchMode.Default : 
				StartCoroutine (DefaultTouch(clickedTile));
				break;
			case TouchMode.Hammer :
				StartCoroutine (ItemHammer(clickedTile));
				break;
			case TouchMode.Obstacle :
				StartCoroutine (ItemObstacle(clickedTile));
				break;
			}
		}
	}

    void ResumeGame()
    {
        paused = false;
    }

	IEnumerator DefaultTouch(Tile targetTile){
		//기본모드로 타일을 클릭했을 시의 동작

		//파이프 이동 불가 조건
		if ((targetTile.tileType != "p" && targetTile.tileType != "em") || targetTile.isFlowed) {
            effectsoundManager.PlayEffectSound(soundManager.block);
            /*블락 효과음 실행*/
			yield break;
		}

		//파이프 이동 루틴 시작
		processing = true;
		pipeUsed++;

		//기존 프리뷰 삭제
		if (preview != null)
			Destroy (preview);

        AddScore(100, targetTile);
		//파이프가 이미 있을 시
		if (targetTile.tileType == "p") {
            Destroy(Instantiate(prefabManager.pipeExplosion, PipeTools.GetPosition(targetTile.vec.x, targetTile.vec.y) / 540f * scaleSize, transform.rotation), 0.5f);
			targetTile.DestroyPipe();
			effectsoundManager.RandomEffectSound(soundManager.crash);
			/*
			 * Instantiate(explosion);
			 * */
		} else{
			effectsoundManager.RandomEffectSound(soundManager.touch);
		}

		//파이프의 논리적 정보 교체 및 타일에 박아두기
		targetTile.SetPipe (nextPipe [0]);
		targetTile.tileType = "p";
		targetTile.InitConnection ();

		//프리뷰 생성 및 이동
		if (nextPipe[1] && !tutorialStage){
			preview = NGUITools.AddChild (objectManager.nextPanel, prefabManager.preview[nextPipe[1].pipeNum]);
			preview.transform.localPosition = Vector3.up * boxPosition [0];
			preview.transform.parent = targetTile.transform;
            preview.transform.localScale = Vector3.one * 126;
			TweenPosition.Begin (preview, processingTime * 0.5f, Vector3.zero).method = UITweener.Method.EaseIn;
		}

		/*이거 뭐지?*/
		//int leftNext = (stageInfo.restrictType == 'p' && stageInfo.restrictNumber - pipeUsed < 5) ? stageInfo.restrictNumber - pipeUsed  : 5;

		//넥스트 파이프 논리적/물리적 이동
		for (int i = 1; i < 5; i++) {
			if (nextPipe[i]){
				TweenPosition.Begin (nextPipe[i].gameObject, processingTime * 0.5f, Vector3.up * boxPosition[i - 1]).method = UITweener.Method.EaseIn;
			}
			nextPipe[i - 1] = nextPipe[i];
		}
        if (nextPipe[0])
			nextPipe [0].transform.localScale = Vector3.one * 1.143f;

		//타일 쪼그라드는 효과
		TweenScale.Begin (targetTile.gameObject, processingTime * 0.7f, Vector3.one * 0.8f).from = Vector3.one * 1.4f;

		//논리적 교체 후의 계산과 효과
		ComputeRoad();

		if (stageInfo.gameMode == StageInfo.GameMode.limitedPipe) {
			//개수제한일 시, 남은 개수 변경
			StartCoroutine (UpdatePipeRestriction());
			
			if (stageInfo.limitNum == pipeUsed && !flowing){
				//물 흐르기 시작
                StartCoroutine(StartWaterFlow());
                yield return new WaitForSeconds(processingTime * 0.7f);
                TweenScale.Begin(targetTile.gameObject, processingTime * 0.3f, Vector3.one);
				yield break;
			}
		}

        yield return new WaitForSeconds(processingTime * 0.5f);
        if (stageInfo.gameMode == StageInfo.GameMode.timeAttack || stageInfo.limitNum - pipeUsed >= 5)
            nextPipe[4] = GeneratePipe();
        else
            nextPipe[4] = null;
		yield return new WaitForSeconds (processingTime * 0.2f);
		TweenScale.Begin (targetTile.gameObject, processingTime * 0.3f, Vector3.one);
		yield return new WaitForSeconds (processingTime * 0.3f);
		

		processing = false;
	}

    private int pipeCountMax = 0;
    

	void ComputeRoad(){

		cropCount.Clear ();		//작물 획득 개수 초기화
		TweenAlpha.Begin (objectManager.startButton, 0.4f, 0);
		objectManager.startButtonHighlighter.SetActive(false);


		Tile compTile = startTile;
		int dir = compTile.pipeNum;
        int pipeCount = 0;
		//스타트 타일 플레시
		GameObject flash = NGUITools.AddChild (compTile.gameObject, prefabManager.pipeFlash [7]);
		flash.transform.localScale = Vector3.one * 126;
		Destroy (flash, 0.3f);

		Vector3 compVec = compTile.GetNextTile (dir);

		while (true) {
			//기본 범위 밖으로 벗어났을 시
			if (OutOfBound(compVec)) {
				SetPoint((int)compVec.x, (int)compVec.y, dir, true);
				break;
			}

			compTile = tile [(int)compVec.x, (int)compVec.y];
			dir = (int)compVec.z;

			//해당 타일에 들어갈 수 없을 경우
			if (!compTile.IsEnterable(dir)){
				if (compTile.tileType == "em")
					SetPoint((int)compVec.x, (int)compVec.y, dir, false);
				else
					SetPoint((int)compVec.x, (int)compVec.y, dir, true);
				break;
			}
			//엔드에 도달했을 경우
			else if (compTile.tileType == "e"){
				objectManager.pointError.SetActive(false);
				objectManager.pointArrow.SetActive(false);
				flash = NGUITools.AddChild (compTile.gameObject, prefabManager.pipeFlash [7]);
				flash.transform.localScale = Vector3.one * 126;
				Destroy (flash, 0.3f);
				//미션 충족 시 버튼 이미지 변경
                if (IsMissionSatisfied())
                {
                    TweenAlpha.Begin(objectManager.startButton, 0.4f, 1);
                    objectManager.startButtonHighlighter.SetActive(true);
                }
                if (tutorialStage)
                {
                    GameObject.FindWithTag("Tutorial").SendMessage("StartNextRoutine", SendMessageOptions.DontRequireReceiver);
                }
				break;
			}

			//플래시 효과 적용 및 연결여부 설정
			if (compTile.tileType == "i" || compTile.tileType == "o"){
				flash = NGUITools.AddChild (compTile.gameObject, prefabManager.pipeFlash [7]);
				flash.transform.localScale = Vector3.one * 126;
				Destroy (flash, 0.3f);
			}
			else if (compTile.pipeNum == 2){
				flash = NGUITools.AddChild (compTile.gameObject, prefabManager.pipeFlash [(dir < 2 ? 0 : 1)]);
				if (compTile.connected){
					compTile.crossConnected = true;
				}
				compTile.pipeOnTile.connected[(dir < 2 ? 1 : 0)] = true;
			}
			else{
				flash = NGUITools.AddChild (compTile.gameObject, prefabManager.pipeFlash [compTile.pipeNum]);
				compTile.pipeOnTile.connected[0] = true;
			}
			compTile.connected = true;

			//작물 더하기
			if (compTile.tileType == "p") {
				if (compTile.crossConnected)
					AddCropCount (compTile.cropType, 10);
				else
					AddCropCount(compTile);
			}

            //파이프 개수 더하기
            if (compTile.tileType == "p" || compTile.tileType == "op")
            {
                pipeCount++;
            }

			flash.transform.localScale = Vector3.one * 126;
			Destroy (flash, 0.3f);

			compVec = compTile.GetNextTile (dir);
			dir = (int) compVec.z;
		}

		int[] cropFlyCount = new int[]{0,0,0,0};
        int numberOfCritical = 0;
		objectManager.missionJollyFace.spriteName = "Mission_Face_1";
		//연결 변화여부에 따른 효과
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 8; j++){
				int con = tile[i,j].SetConnection();
				if (con != 0){
					if (con != 1 && !objectManager.missionJolly.activeSelf){
						StartCoroutine(MissionJolly());
					}
                    if (con>= 3) 
                        numberOfCritical++;
					GameObject tempObj = NGUITools.AddChild(tile[i,j].gameObject, prefabManager.ghost);
                    tempObj.transform.localScale = Vector2.one / scaleSize;
					tempObj.GetComponent<Ghost>().Initialize (tile[i,j].cropType, (con == 1 ? false : true), (con >= 3 ? true : false), (j ==7 ? true : false),(con == 4 ? 10 : tile[i,j].cropNum), objectManager.missionPanel, scaleSize);
					cropFlyCount[tile[i,j].cropType]++;
					if (con >= 3){
						objectManager.missionJollyFace.spriteName = "Mission_Face_@";
					}
				}
			}
		}

        //파이프 최대개수 5개이상 증가시 효과
        if (pipeCount > pipeCountMax)
        {
            if (numberOfCritical >= 5){
                GameObject tempO = NGUITools.AddChild(objectManager.textPanel, prefabManager.gorgeous);
                tempO.GetComponent<UISprite>().spriteName = "Gorgeous_2";
                StartCoroutine(TextPopUp(tempO));
            }
            else if (numberOfCritical >= 3)
            {
                GameObject tempO = NGUITools.AddChild(objectManager.textPanel, prefabManager.gorgeous);
                tempO.GetComponent<UISprite>().spriteName = "Gorgeous_1";
                StartCoroutine(TextPopUp(tempO));
            }
            else if (numberOfCritical >= 1)
            {
                GameObject tempO = NGUITools.AddChild(objectManager.textPanel, prefabManager.gorgeous);
                tempO.GetComponent<UISprite>().spriteName = "Gorgeous_0";
                StartCoroutine(TextPopUp(tempO));
            }
             
            pipeCountMax = pipeCount;
        }

		for (int i = 0; i < 4; i++)
        {
			if (cropFlyCount[i] > 0){
                for (int j = 0; j < stageInfo.mission.Length; j++)
                {
                    if (stageInfo.mission[j].cropType == i)
                        StartCoroutine(MissionCounterAnimation(i, j));
                }
				
			}
		}
	}

	IEnumerator MissionJolly(){
		objectManager.missionJolly.SetActive (true);
		yield return new WaitForSeconds (1.33f);
		objectManager.missionJolly.SetActive (false);
	}

	IEnumerator MissionCounterAnimation(int cropType, int missionNum){
		UILabel targetLabel = objectManager.mission[cropType].currentLabel;
		Vector3 currentScale = targetLabel.transform.localScale;
		int count = (cropCount.ContainsKey(cropType) ? cropCount [cropType] : 0);

		yield return new WaitForSeconds (0.3f);

		TweenScale.Begin (targetLabel.gameObject, 0.2f, currentScale * 1.5f);

		yield return new WaitForSeconds (0.2f);

		targetLabel.text = count.ToString ();
		if (cropCount.ContainsKey (cropType) && count >= stageInfo.mission [missionNum].cropCount) {
			targetLabel.color = new Color(1, 0.87f, 0.09f);
		}
		else{
			targetLabel.color = Color.white;
		}

		TweenScale.Begin(targetLabel.gameObject, 0.2f, Vector3.one * (count < 100 ? 70 : 50));
	}

	bool IsMissionSatisfied(){
		//미션 완료 여부 판단
		foreach (StageInfo.Mission mission in stageInfo.mission) {
            if (!cropCount.ContainsKey(mission.cropType)) return false;
			if (cropCount[mission.cropType] < mission.cropCount)
				return false;
		}
		return true;
	}

    bool timeMult = false;

    void TimeMultiplier()
    {
        timeMult = !timeMult;
		objectManager.startCover.SetActive (timeMult);
    }

    void OnStartButtonClick()
    {
		if (paused) return;
        if (stageInfo.gameMode == StageInfo.GameMode.limitedPipe)
        {
            StartCoroutine(StartWaterFlow());
        }
        else
        {
            TimeMultiplier();
        }
    }

	IEnumerator StartWaterFlow(){
        if (flowing) yield break;
        flowing = true;
        flowPaused = true;
        effectsoundManager.PlayEffectSound(soundManager.waterFlowStart);
        Vibrate();
		objectManager.startButtonHighlighter.SetActive (false);
        if (stageInfo.gameMode == StageInfo.GameMode.limitedPipe)
        {
            objectManager.pointArrow.SetActive(false);
            objectManager.pointError.SetActive(false);
            Destroy(preview);
            objectManager.timebar.gameObject.SendMessage("StartSliding", 2, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(2f);
        }
        else
        {
			yield return new WaitForSeconds(1);
            bgmManager.PlayBGM(soundManager.waterFlowBGM);
        }

		flowingDir = startTile.pipeNum;
		int fX = (int)startTile.vec.x;
		int fY = (int)startTile.vec.y;

        Destroy(GameObject.FindWithTag("Stone"));
        for (int i = 0; i < 5; i++)
        {
            GameObject stone = NGUITools.AddChild(startTile.gameObject, prefabManager.stoneFly);
            stone.GetComponent<UISprite>().MakePixelPerfect();
            stone.transform.localPosition = new Vector2(nextTileX[flowingDir], nextTileY[flowingDir]) * 63;
            stone.SendMessage("mStart", SendMessageOptions.DontRequireReceiver);
        }

		waterFlowingTile = tile [fX, fY];
        flowPaused = false;
		SetNextFlow();
        
		/*
		 * 배경음악 및, 효과음 넣기
		 * 스타트 타일 애니매이션
		 * 진동
		 * 애니메이션 시간 대기 -> 이건 ienumerator로 할 필요 있음
		 */
	}

	void SetNextFlow(){
		flowedTime = 0;

		Vector3 NextTileVector = waterFlowingTile.GetNextTile (flowingDir);

		//
		if (OutOfBound (NextTileVector)) {
			/*실패시의 동작*/
			StartCoroutine(Fail(false));
			Debug.Log ("실패!");
			flowPaused = true;
			return;
		}

		waterFlowingTile = tile [(int)NextTileVector.x, (int)NextTileVector.y];
		flowingDir = (int)NextTileVector.z;

		if (!waterFlowingTile.IsEnterable (flowingDir)) {
			/*실패시의 동작*/
			StartCoroutine(Fail(false));
			Debug.Log ("실패!");
			flowPaused = true;
			return;
		}
		else if (waterFlowingTile.tileType == "e") {
            effectsoundManager.PlayEffectSound(soundManager.waterEnd);
			StartCoroutine(ReachedEndTile ());
			flowPaused = true;
			return;
		}
        else if (waterFlowingTile.tileType == "i")
        {
            effectsoundManager.PlayEffectSound(soundManager.waterIn);
        }

		if (waterFlowingTile.tileType == "op") {
			AddScore (1000, waterFlowingTile);
		}
		else if (waterFlowingTile.isFlowed){
			AddScore (6000, waterFlowingTile);
		}
		else if (waterFlowingTile.tileType == "p"){
			AddScore (200 * waterFlowingTile.cropNum * (waterFlowingTile.cropLevel + 1), waterFlowingTile);
		}

        if (waterFlowingTile.tileType == "op" || waterFlowingTile.tileType == "p")
        {
            GameObject tempObj = NGUITools.AddChild(waterFlowingTile.gameObject, prefabManager.flash);
            tempObj.transform.localScale = Vector2.one * 157;
            Destroy(tempObj, 8);
        }
			
		waterFlowingTile.isFlowed = true;
		flowSlider = waterFlowingTile.GetSlider (flowingDir);
		/*
		if (waterFlowingTile.tileType == "p" || waterFlowingTile.tileType == "op") {
			AddCropCount(cropNameMapper[waterFlowingTile.cropType], waterFlowingTile.cropNum);
		}
		*/
	}

	IEnumerator ReachedEndTile(){
		flowedTime = 0;
		flowSlider = endTile.transform.Find("EndTile(Clone)").Find ("Slider").GetComponent<UISlider> ();
		endBubble.SetActive (true);
		endSliding = true;

		yield return new WaitForSeconds (waterFlowDuration);

		foreach (StageInfo.Mission mission in stageInfo.mission) {
			int missionCount = mission.cropCount;
			int harvestedCropCount = (cropCount.ContainsKey(mission.cropType) ? cropCount[mission.cropType] : 0);
			Debug.Log ("미션 : " + PipeTools.cropNameMapper[mission.cropType] + " " + missionCount + " 개 수확");
			Debug.Log ("결과 : " + harvestedCropCount + " 개 획득!");
			if (harvestedCropCount < missionCount){
				/*실패시 동작*/
				StartCoroutine(Fail(true));
				Debug.Log ("실패");
				yield break;
			}
		}
        Debug.Log("성공!");
        StartCoroutine(Success());
	}

	IEnumerator Success(){
		yield return new WaitForSeconds (1);
        bgmManager.PlayBGM(soundManager.successBGM);
        TweenAlpha.Begin(objectManager.whiteSkin, 0.5f, 0.8f);
        UISprite imgText = NGUITools.AddChild(objectManager.textPanel, prefabManager.gorgeous).GetComponent<UISprite>();
        imgText.spriteName= "GoodJob";
        imgText.MakePixelPerfect();
        
		//남은 파이프 개수 * 300만큼 점수 추가
		if (stageInfo.gameMode == StageInfo.GameMode.limitedPipe) {
			for (int i = 0; i < stageInfo.limitNum - pipeUsed;){
				AddScore(300);
				/*0번 파이프에 폭발 효과*/
				effectsoundManager.PlayEffectSound(soundManager.pipeCount);
				if (nextPipe[0] != null) Destroy (nextPipe[0].gameObject);
				pipeUsed++;
				//넥스트 파이프 논리적/물리적 이동
				for (int j = 1; j < 5; j++) {
					if (nextPipe[j])
						TweenPosition.Begin (nextPipe[j].gameObject, 0.2f, Vector3.up * boxPosition[j - 1]).method = UITweener.Method.EaseIn;
					nextPipe[j - 1] = nextPipe[j];
				}
				if(nextPipe[0])
					nextPipe [0].transform.localScale = Vector3.one * 1.143f;
							
				objectManager.restrictNumber.text = (stageInfo.limitNum - pipeUsed).ToString();
				yield return new WaitForSeconds (0.15f);
				
				if (stageInfo.limitNum - pipeUsed >= 5)
					nextPipe [4] = GeneratePipe ();
				else
					nextPipe[4] = null;
			}
			AddScore (300 * (stageInfo.limitNum - pipeUsed));
		}

		yield return new WaitForSeconds (0.3f);

		bool isAllSquare = IsAllsquare();
		//올스퀘어 체크한 후 점수 추가
		if (isAllSquare) {
			Debug.Log("올스퀘어 성공! " + (score * 2).ToString() + "점 추가 획득!");
			AddScore (score * 2);
            yield return new WaitForSeconds(1);
		}

        SaveStageData();

        
		objectManager.graySkin.SetActive(true);
		objectManager.successWindow.SetActive (true);
        paused = true;
        windowOpen = true;
		if (isAllSquare) {
			objectManager.allsquareStamp.SetActive (true);
		}

        for (int i = 0; i < 3; i++)
        {
            if (score >= stageInfo.starCondition[2 - i])
            {
                yield return new WaitForSeconds(0.5f);
                objectManager.successWindow.GetComponent<SuccessWindow>().Initialize(3 - i, cropCount, score);
                //objectManager.successWindow.SendMessage("ShowStar", 3 - i, SendMessageOptions.DontRequireReceiver);
                ChangeMax((Application.loadedLevel - 2).ToString() + "star", 3 -i);
                break;
            }
        }
		
	}

	IEnumerator Fail(bool connectedToEnd){
        flowPaused = true;
		paused = true;
        if (stageInfo.gameMode == StageInfo.GameMode.timeAttack)
        {
            objectManager.pointArrow.SetActive(false);
            objectManager.pointError.SetActive(false);
        }

        bgmManager.PauseBGM();
        if (!connectedToEnd)
        {
            //연결이 안되 실패 시, 물 터지는 효과
            effectsoundManager.PlayEffectSound(soundManager.waterExplosion);
            effectsoundManager.PlayEffectSound(soundManager.auch);
            GameObject waterEx = (GameObject)Instantiate(prefabManager.waterExplosion);
            waterEx.transform.position = PipeTools.BUIGetPosition(waterFlowingTile.vec.x - 0.5f * nextTileX[flowingDir], waterFlowingTile.vec.y - 0.5f * nextTileY[flowingDir]) * scaleSize;
            Destroy(waterEx, 3);
            yield return new WaitForSeconds(3);
            bgmManager.PauseBGM();
            effectsoundManager.PlayEffectSound(soundManager.failCon);
            SetPoint((int)waterFlowingTile.vec.x, (int)waterFlowingTile.vec.y, flowingDir, true);
            objectManager.pointError.transform.localScale *= 1.2f;
        }
        else
        {
            yield return new WaitForSeconds(1);
            bgmManager.PauseBGM();
            TweenAlpha.Begin(objectManager.whiteSkin, 0.5f, 0.8f);
            effectsoundManager.PlayEffectSound(soundManager.failCon);
            UISprite notEnoughText = NGUITools.AddChild(objectManager.textPanel, prefabManager.gorgeous).GetComponent<UISprite>();
            notEnoughText.spriteName = "NotEnough";
            notEnoughText.MakePixelPerfect();
            StartCoroutine(Twinkle(notEnoughText.gameObject, 3, 0.5f));

            yield return new WaitForSeconds(1);
        }
        
		yield return new WaitForSeconds (2);
        bgmManager.PlayBGM(soundManager.failBGM);
		objectManager.graySkin.SetActive (true);
		objectManager.failWindow.SetActive (true);
        windowOpen = true;
        paused = true;
	}

    IEnumerator Twinkle(GameObject target, int times, float delay)
    {
        for (int i = 0; i < times; i++)
        {
            yield return new WaitForSeconds(delay);
            target.SetActive(false);
            yield return new WaitForSeconds(delay);
            target.SetActive(true);
        }
    }

    void ChangeMax(string key, int value)
    {
        if (PlayerPrefs.HasKey(key))
        {
            if (PlayerPrefs.GetInt(key) < value)
            {
                PlayerPrefs.SetInt(key, value);
            }
        }
        else
        {
            PlayerPrefs.SetInt(key, value);
        }
        PlayerPrefs.Save();
    }

    void SumPref(string key, int value)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + value);
        }
        else
        {
            PlayerPrefs.SetInt(key, value);
        }
        PlayerPrefs.Save();
    }

    IEnumerator TextPopUp(GameObject obj)
    {
        obj.transform.localPosition = new Vector3(0, 240, 0);
        UISprite targetSprite = obj.GetComponent<UISprite>();
        targetSprite.MakePixelPerfect();
        Vector3 maxSize = obj.transform.localScale;
        obj.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.3f);
        TweenPosition.Begin(obj, 0.2f,new Vector3(0,360, 0));
        TweenAlpha.Begin(obj, 0.3f, 1).from = 0;
        obj.transform.localScale = maxSize;
        //TweenScale.Begin(obj, 0.2f, maxSize);
        yield return new WaitForSeconds(1f);
        TweenAlpha.Begin(obj, 0.2f, 0);
        yield return new WaitForSeconds(0.2f);
        Destroy(obj);
    }

    void SaveStageData()
    {
        if (IsInLatestSection())
        {
            //작물 획득정보 저장
            foreach (KeyValuePair<int, int> node in cropCount)
            {
                string cropName = PipeTools.cropNameMapper[node.Key];
                SumPref(cropName, node.Value);
            }
        }
        
        //최종 스테이지 저장
        ChangeMax("MaxStage", Application.loadedLevel - 1);
        //스테이지 최고점수 저장
        ChangeMax((Application.loadedLevel - 2).ToString() + "score", score);
    }

    void PauseWindowOpen()
    {
        if (paused) return;
        if (windowOpen) return;
        windowOpen = true;
        paused = true;
        objectManager.pauseWindow.SetActive(true);
    }

    void PauseWindowClose()
    {
        paused = false;
        windowOpen = false;
    }

    bool IsInLatestSection()
    {
        int currentStage = Application.loadedLevel - 2;
        if (!PlayerPrefs.HasKey("MaxSection"))
        {
            PlayerPrefs.SetInt("MaxSection", 0);
            PlayerPrefs.Save();
        }
        int maxSection = PlayerPrefs.GetInt("MaxSection");

        for (int i = 0; i < maxSection; i++)
        {
            currentStage -=  PipeTools.NumOfStagePerSection[i];
            if (currentStage < 0)
                return false;
        }
        return true;
    }

	void Update () {
		ScoreBarSlide ();
		if (paused)	return;
        Timer();
		EndSlide ();
		WaterFlow ();
	}

    private float passedTime = 0;
    private float timelimit;
    private bool resNumberColorChange1 = false;
    private bool resNumberColorChange2= false;

    void Timer()
    {
        if (stageInfo.gameMode != StageInfo.GameMode.timeAttack) return;
        if (flowing) return;

        passedTime += Time.deltaTime * (timeMult ? 10 : 1);
        float timeLeft = timelimit - passedTime;
        float timePercentage = 1 - (passedTime / timelimit);
        Vector2 numberPosition = new Vector2(16, 43 + 498 * timePercentage);
        objectManager.timebar.sliderValue = timePercentage;
        objectManager.restrictNumber.text = timeLeft.ToString("n0") + "s";
        objectManager.restrictNumber2.text = timeLeft.ToString("n0") + "s";
        objectManager.restrictNumber.transform.localPosition = numberPosition;
        objectManager.restrictNumber2.transform.localPosition = numberPosition;

        objectManager.restrictNumber.alpha = (timeLeft - 10) / (timelimit - 20);

        if (passedTime >= timelimit)
        {
            StartCoroutine(StartWaterFlow());
        }
    }

	void EndSlide(){
		if (!endSliding) return;
		flowedTime += Time.deltaTime;
		flowSlider.sliderValue = flowedTime / waterFlowDuration;
		if (flowedTime >= waterFlowDuration) {
			endSliding = false;
		}

	}

	void ScoreBarSlide (){
		if (!scorebarSliding) return;
		scorebarSlideTimer -= Time.deltaTime;
		objectManager.scorebar.sliderValue = (scorebarSlideDestinationValue - scorebarSlideStartValue) * (0.2f - scorebarSlideTimer) *5 + scorebarSlideStartValue;
		if (scorebarSlideTimer <= 0) scorebarSliding = false;
	}

	void WaterFlow(){
		if (!flowing || flowPaused)	return;
        flowedTime += Time.deltaTime * (timeMult ? 10 : 1);
		flowSlider.sliderValue = flowedTime / waterFlowDuration;
		if (flowedTime >= waterFlowDuration) {
			SetNextFlow();
		}
	}

	private int scoredigit = 0;

	void AddScore(int score){
		//논리적 점수 더하기
		this.score += score;
		objectManager.scoreText.text = this.score.ToString ();

		if (scoredigit < (int)Mathf.Log10 (this.score)) {
			int[] textSize = {80,80,80,65,55,45,40};
			scoredigit = (int)Mathf.Log10 (this.score);
			objectManager.scoreText.transform.localScale = Vector2.one * textSize[scoredigit];
		}

		scorebarSlideStartValue = scorebarSlideDestinationValue;
		scorebarSlideDestinationValue = (float)this.score / stageInfo.starCondition [2] * 0.8f;
		scorebarSlideTimer = 0.2f;
		scorebarSliding = true;

	}

	bool IsAllsquare(){
		for (int i = 0 ; i < 10; i++){
			for (int j = 0; j < 8; j++){
				if(((tile[i,j].tileType == "p" || tile[i,j].tileType == "op") && !tile[i,j].isFlowed) || tile[i,j].tileType == "em"){
					return false;
				}
			}
		}
		return true;
	}

	void AddScore(int score, Tile targetTile){
		AddScore (score);

		int[] scoreCriteria = {8000, 3000, 1000, 300, 100};
		Color[] colorSet = {
			new Color(254, 51, 76) / 255f, new Color(254, 127, 0) / 255f,
			new Color(208, 100, 254) / 255f, new Color(153, 223, 43) / 255f,
			Color.white
		};
		int[] scaleSet = {90, 70, 60, 40, 40};

		for (int i = 0; i < scoreCriteria.Length; i++){
			if (score >= scoreCriteria[i]){
				GameObject scoreEffect = NGUITools.AddChild (targetTile.gameObject, prefabManager.scoreText);
				UILabel scoreEffectText = scoreEffect.GetComponent<UILabel>();
				scoreEffectText.text = score.ToString();
				scoreEffectText.color = colorSet[i];
				scoreEffect.transform.localScale = Vector2.one * scaleSet[i];
				StartCoroutine (ScoreEffectAnimation(scoreEffect));
				break;
			}
		}
	}

	IEnumerator ScoreEffectAnimation(GameObject obj){
        int randVal = UnityEngine.Random.Range(0,2);
		TweenPosition.Begin (obj, 0.5f, new Vector3 (randVal *2 - 1, 1, 0) * 45);
		TweenAlpha.Begin (obj, 0.5f, 1).from = 0;

		yield return new WaitForSeconds (0.7f);
		TweenAlpha.Begin (obj, 0.5f, 0);

		yield return new WaitForSeconds (0.5f);
		Destroy (obj);

	}

	IEnumerator UpdatePipeRestriction(){
		TweenScale.Begin (objectManager.restrictBox, processingTime / 4, Vector3.one * 1.5f);

		yield return new WaitForSeconds (processingTime / 4);

		objectManager.restrictNumber.text = (stageInfo.limitNum - pipeUsed).ToString ();
		TweenScale.Begin (objectManager.restrictBox, processingTime / 4, Vector3.one);
	}

	void AddCropCount(Tile cropTile){
		if (!cropCount.ContainsKey (cropTile.cropType)) {
			cropCount[cropTile.cropType] = cropTile.cropNum;
		}
		else{
			cropCount[cropTile.cropType] += cropTile.cropNum;
		}
	}

	void AddCropCount(int type, int count){
		if (!cropCount.ContainsKey (type)) {
			cropCount[type] = count;
		}
		else{
			cropCount[type] += count;
		}
	}

	public void SetPreview(int tX, int tY){
		if (!preview) {
			//프리뷰가 없을 시, 새 프리뷰 생성
			preview = NGUITools.AddChild (objectManager.tilePanel, prefabManager.preview[nextPipe[0].pipeNum]);
			preview.transform.localScale = Vector2.one * 126;
		}
		preview.transform.parent = objectManager.tilePanel.transform;
		preview.transform.localPosition = PipeTools.GetPosition (tX, tY);
        preview.transform.localScale = Vector2.one * 126;
	}

	void SetPoint(int tX, int tY, int dir, bool err){
		//포인트 에로우, 혹은 에러의 위치 설정
		objectManager.pointArrow.SetActive (!err);
		objectManager.pointError.SetActive (err);

		GameObject pointer = err ? objectManager.pointError : objectManager.pointArrow;
		pointer.transform.localPosition = PipeTools.GetPosition (tX, tY) - (err ? 63 : 20) * new Vector2 (nextTileX [dir], nextTileY [dir]);
        if (!err)
        {
            objectManager.pointArrow.GetComponent<UISprite>().spriteName = "Arrow_" + dir.ToString();
            objectManager.pointArrow.GetComponent<UISprite>().MakePixelPerfect();
        }
	}

	bool OutOfBound(Vector3 vec){
		int vX = (int)vec.x;
		int vY = (int)vec.y;

		if (vX > 9 || vX < 0 || vY > 7 || vY < 0)
			return true;
		return false;
	}

    ArrayList setupPipe = new ArrayList();

    void SetUpPipe(int[] arr)
    {
        foreach (int i in arr)
            setupPipe.Add(i);
    }

	Pipe GeneratePipe(){
		int selectedNum = -1;

        if (setupPipe.Count > 0)
        {
            selectedNum = (int)setupPipe[0];
            setupPipe.RemoveAt(0);
        }
        else
        {

            //생성할 파이프를 결정
            int randomPipeValue = UnityEngine.Random.Range(0, sumPipeWeight);

            for (int i = 0; i < stageInfo.pipeWeight.Length; i++)
            {
                randomPipeValue -= stageInfo.pipeWeight[i];
                if (randomPipeValue < 0)
                {
                    selectedNum = i;
                    break;
                }
            }


        }
		

		GameObject tempPipe = NGUITools.AddChild (objectManager.nextPanel,prefabManager.pipe[selectedNum]);
		tempPipe.transform.localPosition = Vector3.up * boxPosition [4];
		tempPipe.transform.localScale = Vector3.one * 0.56f;

		return tempPipe.GetComponent<Pipe>();
	}

    void StarEffectSound()
    {
        effectsoundManager.PlayEffectSound(soundManager.starEffect);
    }

	void Unload(){
		Resources.UnloadUnusedAssets ();
	}

    void Vibrate()
    {
        if (PlayerPrefs.HasKey("Vibration") && PlayerPrefs.GetInt("Vibration") == 0) return;
        Handheld.Vibrate();
    }
    
	public Vector3 GetTileVecForArrowInOut(int tX, int tY, int dir){
		int[] nextTileX = {-1, 1, 0, 0};
		int[] nextTileY = {0, 0, 1, -1};		//방향에 따라 다음 타일을 찾기 위한 정보

		tX += nextTileX [dir];
		tY += nextTileY [dir];

		while (true) {
			if (tX == 10) tX = 0;
			if (tX == -1) tX = 9;
			if (tY == 8) tY = 0;
			if (tY == -1) tY = 7;

			if (tile[tX,tY].tileType != "none"){
				return new Vector3(tX, tY, dir);
			}

			tX += nextTileX[dir];
			tY += nextTileY[dir];
		}
	}
}