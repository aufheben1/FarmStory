using UnityEngine;
using System.Collections;
using System.IO;

public class GameController : MonoBehaviour {

	private AudioKeeper audioKeeper;
	private PrefabKeeper prefabKeeper;
	
	//개발자 전용 정보
	[System.Serializable]
	public class dev{
		public Vector3 BasePipeRgb;
		public Vector3 ConnectedPipeRgb;
		public float waterFlowDuration;
		public float waterFlowDurationFast;
	}
	
	//스테이지 정보
	[System.Serializable]
	public class st{
		public string pipeColor;
		public string waterColor;
		public bool isScoreMission;
		public Vector3 startTile;
		public float timeLimit;
		public float flowFasterTime;
		public string missionTypeString;
		public char missionType;
		public int missionNumber;
		public string resttrictTypeString;
		public char restrictType;
		public int restrictNumber;
		public int missionPipe;
		public int missionCrossPipe;
		public int missionMaxPipe;
		public int[] starConditionScore;
		
		public int[] pipeWeight;
	}
	enum TouchMode {Default, Hammer, Obstacle}
	TouchMode touchMode = TouchMode.Default;
	
	public dev devInfo;
	public st stageInfo;

	public GameObject[] mission;

	public UILabel pipeText;
	public UILabel scoreText;
	public UILabel missionText;
	public UILabel pipeLeftText;
	
	public GameObject NGUIInterfece;
	public GameObject scoreEffect;

	//좌표정보
	private float tileSize = 1.2f;
	private Vector3 tileBoardPosition = new Vector3 (0, 0.21f, 0);

	private Vector3[] boxPosition = {
		new Vector3(111, -484, 0), new Vector3(111, -316, 0), 
		new Vector3(111, -229, 0), new Vector3(111, -143, 0), 
		new Vector3(111, -56, 0)};
	/*
	private Vector3[] boxPosition = {
		new Vector3(-8.48f, 0.58f, -6), new Vector3(-8.48f, 2.26f, -6), 
		new Vector3(-8.48f, 3.13f, -6), new Vector3(-8.48f, 4f, -6), 
		new Vector3(-8.48f, 4.87f, -6)	};
	*/	
	/*각종 스위치들*/
	private bool isGameStop = true;
	private bool isTimerOn = true;
	private bool isPipemoving;
	private bool isWaterFlow = false;
	private bool isWindowOn = false;
	
	/*게임 내부 정보*/
	private int fX, fY, dir;
	[HideInInspector]
	public float pipeMovingDuration = 0.2f;
	[HideInInspector]
	public GameObject[] nextPipe;
	private TileKeeper[,] tile;
	private GameObject[,] pipeOnTile;
	private int sumWeight;
	
	/*효과 관련*/
	public GameObject[] itemButton;

	public GameObject[] obstaclePipe;

	public UISlider timebar;
	public UISlider scorebar;

	public GameObject successWindow;
	public GameObject failWindow;
	public GameObject graySkin;
	public GameObject flowButton;

	private GameObject preview;
	private GameObject arrow;
	private GameObject textImage;

	//private GUIText scoreText;
	
	private float flowedTime;
	private Vector3 flowStPosition;
	private Vector3 flowDtPosition;
	private bool movingCurve;
	
	GameObject flowManager;
	
	bool isFasted = false;
	int waterFlowState = 0;
	
	/*물리 계산을 위한 정보*/
	private int[,] pipeinfo = {	{0, 1, 4, 4},
		{4, 4, 2, 3},
		{0, 1, 2, 3},
		{4, 3, 0, 4},
		{4, 2, 4, 0},
		{3, 4, 1, 4},
		{2, 4, 4, 1}};		//pipeNum에 따른 outDir 값
	
	private int[] nextTileX = {-1, 1, 0, 0};
	private int[] nextTileY = {0, 0, 1, -1};		//방향에 따라 파이프 시작점을 찾기위한 정보
	private int[] nextAngle = {2,0,1,3};			//방향에 따라 포인트 화살표의 각도를 정하기 위한 정보
	
	/*스테이지 정보*/
	private int totalPipe;
	private int totalCrossPipe;
	[HideInInspector]
	public int score;
	
	/*기타*/
	public float timeCounter;
	private float flowDuration;
	private float flowMultiplier = 1f;
	private int inoutCount = 0;
	
	bool isChangeableBefore;
	int totalPipeMax = 0;

	//스코어바 관련 함수들
	int scoreDigit = 0;
	float scorebarSliderStartValue;
	float scorebarsliderDestinationValue;
	bool isScorebarSliding = false;
	float scorebarSlideTimer;

	void Start(){
		//GetPreviousData ();
		StartCoroutine (LoadGame ());
	}

	/*
	void GetPreviousData (){
		//기존 게임오브젝트에서 스테이지 관련 데이터를 넘겨받은 후 삭제
		ObjectManager preObjectManager = (ObjectManager)GameObject.Find ("GameController").GetComponent<ObjectManager> ();
		if (preObjectManager == null) {
			stageInfo.missionType = char.Parse (stageInfo.missionTypeString);
			stageInfo.restrictType = char.Parse (stageInfo.resttrictTypeString);
			return;
		}
		devInfo = new dev(); stageInfo = new st ();

		this.devInfo.BasePipeRgb = preObjectManager.devInfo.BasePipeRgb;
		this.devInfo.ConnectedPipeRgb = preObjectManager.devInfo.ConnectedPipeRgb;
		this.devInfo.waterFlowDuration = preObjectManager.devInfo.waterFlowDuration;
		this.devInfo.waterFlowDurationFast = preObjectManager.devInfo.waterFlowDurationFast;

		this.stageInfo.pipeColor = preObjectManager.stageInfo.pipeColor;
		this.stageInfo.waterColor = preObjectManager.stageInfo.waterColor;
		this.stageInfo.startTile = preObjectManager.stageInfo.startTile;

		if (preObjectManager.stageInfo.missionPipe > 0) {
			this.stageInfo.missionType = 'p';
			this.stageInfo.missionNumber = preObjectManager.stageInfo.missionPipe;
		}
		else if (preObjectManager.stageInfo.missionCrossPipe > 0){
			this.stageInfo.missionType = 'c';
			this.stageInfo.missionNumber = preObjectManager.stageInfo.missionCrossPipe;
		}
		else if (preObjectManager.stageInfo.isScoreMission){
			this.stageInfo.missionType = 's';
			this.stageInfo.missionNumber = preObjectManager.stageInfo.starConditionScore[0];
		}
		else if(preObjectManager.stageInfo.isAllsquareMission){
			this.stageInfo.missionType = 'a';
		}

		if (preObjectManager.stageInfo.missionMaxPipe > 0) {
			this.stageInfo.restrictType = 'p';
			this.stageInfo.restrictNumber = preObjectManager.stageInfo.missionMaxPipe;
		}
		else{
			this.stageInfo.restrictType = 't';
			this.stageInfo.restrictNumber = (int) preObjectManager.stageInfo.timeLimit;
		}

		this.stageInfo.timeLimit = preObjectManager.stageInfo.timeLimit;
		this.stageInfo.flowFasterTime = preObjectManager.stageInfo.flowFasterTime;
		this.stageInfo.isScoreMission = preObjectManager.stageInfo.isScoreMission;
		this.stageInfo.missionPipe = preObjectManager.stageInfo.missionPipe;
		this.stageInfo.missionCrossPipe = preObjectManager.stageInfo.missionCrossPipe;
		this.stageInfo.missionMaxPipe = preObjectManager.stageInfo.missionMaxPipe;
		this.stageInfo.starConditionScore = new int[3];
		this.stageInfo.pipeWeight = new int[preObjectManager.stageInfo.pipeWeight.Length];
		System.Buffer.BlockCopy (preObjectManager.stageInfo.starConditionScore, 0, this.stageInfo.starConditionScore, 0, 12);
		System.Buffer.BlockCopy (preObjectManager.stageInfo.pipeWeight, 0, this.stageInfo.pipeWeight, 0, 4 * this.stageInfo.pipeWeight.Length);

		Destroy (preObjectManager.gameObject);
	}


*/
	IEnumerator LoadGame(){

		float routineTime;
		//coroutine 1 : wait for object to be loaded
		routineTime = 0.5f;
		yield return new WaitForSeconds (1);

		//coroutine 2 : 
		routineTime	= 1.25f;
		NGUIInterfece.SetActive (true);
		SetNGUIText ();
		LoadComponent ();
		InitializeTile ();
		prefabKeeper.Initialize (stageInfo.pipeColor);

		MoveInterface (routineTime);
		yield return new WaitForSeconds (routineTime);
		
		/*coroutine 2*/ 
		routineTime = 0.4f;
		SetOnBoard ();						//GUI 텍스트 위치 잡기 및 별 위치 잡기		
		InitializeSpTile ();
		sumWeight = GetSumWeight();			
		
		StartCoroutine (ReadyGo (routineTime));
		nextPipe = new GameObject[5];
		for (int i = 0; i < 5 ; i++){
			for (int j = 0; j < i ; j++){
				TweenPosition.Begin (nextPipe[j], routineTime, boxPosition[4 - i + j]).method = UITweener.Method.EaseIn;
				//iTween.MoveTo(nextPipe[j], iTween.Hash ("position", boxPosition[4 - i + j], "time", routineTime, "easetype", iTween.EaseType.easeInQuad));
			}
			yield return new WaitForSeconds(routineTime);
			nextPipe[i] = GeneratePipe(4);
		}
		nextPipe [0].transform.localScale= Vector3.one * 144;
		
		/*coroutine 3*/
		SetPreview (stageInfo.startTile.x, stageInfo.startTile.y);
		SetPointArrow (stageInfo.startTile);
		Unload ();
		
		isGameStop = false;			//게임 시작
		gameObject.SendMessage ("SetTutorial", SendMessageOptions.DontRequireReceiver);
	}
	
	void SetNGUIText (){
		scoreText.text = "0";
		switch (stageInfo.missionType) {
		case 'p' : 
			mission[0].SetActive(true);
			missionText = mission[0].GetComponentInChildren<UILabel>();
			missionText.text = stageInfo.missionNumber.ToString();
			lastMissionCount = stageInfo.missionNumber;
			break;
		case 'c' :
			mission[1].SetActive(true);
			missionText = mission[1].GetComponentInChildren<UILabel>();
			missionText.text = stageInfo.missionNumber.ToString();
			lastMissionCount = stageInfo.missionNumber;
			break;
		case 's' :
			mission[2].SetActive(true);
			missionText = mission[2].transform.Find("Text").GetComponent<UILabel>();
			missionText.text = stageInfo.missionNumber.ToString();
			break;
		case 'a' :
			mission[3].SetActive(true);
			break;
		}

		if (stageInfo.restrictType == 't')
			Destroy (GameObject.Find ("PipeLeft"));
		else
			pipeLeftText.text = stageInfo.restrictNumber.ToString();
	}

	void LoadComponent(){
		audioKeeper = GetComponentInChildren<AudioKeeper>();
		prefabKeeper = GetComponentInChildren<PrefabKeeper> ();
		prefabKeeper.Initialize (stageInfo.pipeColor);
		empty = (Sprite)Resources.Load ("Tile/Empty/TextureEmpty", typeof(Sprite));
		obstacleTexture = (Sprite)Resources.Load ("Tile/Obstacle/obstacle", typeof(Sprite));
	}

	void InitializeTile(){
		//float sizeHandler = 16f / 9f / ((float)Screen.width / Screen.height);
		//tileSize = 1.2f * sizeHandler;

		/*타일을 컴포넌트화 하여 저장*/
		GameObject.Find ("TileBoardInterface").transform.position = tileBoardPosition;
		Sprite empty = (Sprite) Resources.Load ("Tile/Empty/TextureEmpty", typeof(Sprite));
		pipeOnTile = new GameObject[10, 8];
		tile = new TileKeeper[10, 8];
		for (int i = 0; i < 10; i++) { 
			for (int j = 0; j < 8; j++) {
				tile [i, j] = (TileKeeper)GameObject.Find ("Tile" + (i * 10 + j + 1)).GetComponent ("TileKeeper");
				tile[i,j].SetGameManager(gameObject);
				if (tile [i, j].tileType == "s" || tile [i, j].tileType == "e" || tile [i, j].tileType == "i" || tile [i, j].tileType == "o")
						tile [i, j].gameObject.GetComponent<SpriteRenderer> ().sprite = empty;
				tile [i, j].transform.position = GetCoordinate (i, j, 1);
				//tile[i,j].transform.localScale *= sizeHandler;
				tile [i, j].transform.rotation = Quaternion.Euler (0, 0, 0);
				tile [i, j].vec = new Vector2 (i, j);
			}	
		}
		Resources.UnloadAsset (empty);
	}

	void MoveInterface(float time){
		Destroy (GameObject.Find("Loading"));
		iTween.MoveFrom (GameObject.Find ("TileBoardInterface"), iTween.Hash("x", 0, "y", 25, "time", 0.75f, "easetype",iTween.EaseType.linear, "delay", 0.5f));

		if (stageInfo.restrictType == 'p') {
			Destroy(itemButton[1]);
			itemButton[1] = itemButton[3];
		}
		else{
			Destroy (itemButton[3]);
		}
		
		for (int i = 0; i < 3; i++)
			itemButton [i].transform.localScale = Vector3.zero;

		string[] interfaceName = {"Item", "Mission", "ScorePanel", "Next"};
		TweenRotation.Begin (GameObject.Find ("Timebar"), 0.75f, Quaternion.Euler(0,0,0));
		foreach (string name in interfaceName) {
			TweenPosition.Begin(GameObject.Find (name), 0.75f, Vector3.zero);
		}
		GameObject.Find ("Next").GetComponent<TweenPosition> ().delay = 0.3f;
	}

	void SetOnBoard(){
		GameObject tempObject;

		/*별 위치이동 및 버튼 효과*/
		float zeroPositionX = -34f;
		float fullPositionX = -156f;

		for (int i = 0; i < 3; i++){
			tempObject = GameObject.Find ("Star" + i);
			TweenPosition.Begin (tempObject, 0.7f, new Vector3((fullPositionX - zeroPositionX) * stageInfo.starConditionScore[i]/stageInfo.starConditionScore[2] + zeroPositionX, -251, 0));
			TweenScale.Begin (itemButton[i],0.7f, Vector3.one);
			itemButton[i].GetComponent<TweenScale>().method = UITweener.Method.BounceIn;
			Destroy (itemButton[i].GetComponent<TweenScale>(), 1.5f);
		}

		if (stageInfo.restrictType == 'p') {
			flowButtonState = flowButtonMode.stop;
			flowButton.SendMessage ("SetButtonStateStop",SendMessageOptions.DontRequireReceiver);
		}
		else{
			flowButtonState = flowButtonMode.flowNormal;
			flowButton.SendMessage ("SetButtonStatePlay",SendMessageOptions.DontRequireReceiver);
		}


		textImage = (GameObject)Instantiate (prefabKeeper.textImage);
	}

	IEnumerator ReadyGo(float time){
		GameObject ready;
		GameObject go;
		
		ready = textImage.transform.FindChild ("Ready").gameObject;
		go = textImage.transform.FindChild ("Go").gameObject;
		
		yield return new WaitForSeconds (time * 5 - 1.4f);
		ready.gameObject.SetActive (true);
		audioKeeper.PlayEffectsound ("readyGo");
		
		yield return new WaitForSeconds (0.8f);
		ready.gameObject.SetActive (false);
		go.gameObject.SetActive (true);
		
		yield return new WaitForSeconds (0.8f);
		go.gameObject.SetActive (false);
		for (int i = 0; i < 3; i++)
			itemButton [i].AddComponent<UIButtonScale> ();
	}

	void InitializeSpTile(){
		GameObject tempTile ;
		int tempX = 0; int tempY = 0; int tempZ = 0;
		Color tC = new Color(1f,1f,1f,0.7f);
		for (int i = 0; i < 10; i++) {
			for(int j = 0; j < 8; j++){
				if (tile[i,j].specialOut.Length ==4){
					for (int k = 0; k < 4; k++){
						if (tile[i,j].specialOut[k].x != -1f && IsOutOfBound(i + nextTileX[k], j + nextTileY[k]) && tile[i,j].tileType != "i"){
							Instantiate (prefabKeeper.inoutArrow, 
							             GetCoordinate (i,j,7) + new Vector3(nextTileX[k],nextTileY[k],0) * tileSize * 0.5f, 
							             Quaternion.Euler (0,0, 90 * (1 + nextAngle[k])));
						}
					}
				}
				switch (tile[i,j].tileType){
				case "op" :
					Instantiate(obstaclePipe[tile[i,j].pipeNum], GetCoordinate (i,j,5), transform.rotation);
					System.Buffer.BlockCopy(pipeinfo, tile[i,j].pipeNum * 16, tile[i,j].outDir, 0, 16); break;
				case "s" :
					startTile = new Vector2(i,j);
					tempTile = (GameObject)Instantiate (prefabKeeper.startAnim[(int)stageInfo.startTile.z]);
					Destroy ((GameObject)Instantiate (prefabKeeper.emptyTile, GetCoordinate (i,j,1),transform.rotation), 0.5f);
					tempTile.GetComponent<Renderer>().material.color = tC;
					StartCoroutine(RestoreColor(tempTile));
					iTween.MoveTo (tempTile,iTween.Hash ("position", GetCoordinate(i,j,1),"time",0.5f,"easetype", iTween.EaseType.easeOutQuart));
					Destroy (tile[i,j].gameObject);
					tile[i,j] = (TileKeeper) tempTile.GetComponent ("TileKeeper"); 	
					tile[i,j].vec = new Vector2(i,j);
					break;
				case "e" :
					endTile = new Vector2(i,j);
					for (int k = 0 ; k < 4 ; k++)
						if (tile[i,j].outDir[k] != 4f)
							tempX = k;
					tempTile = (GameObject)Instantiate (prefabKeeper.endAnim[tempX]);
					Destroy ((GameObject)Instantiate (prefabKeeper.emptyTile, GetCoordinate (i,j,1),transform.rotation), 0.5f);
					tempTile.GetComponent<Renderer>().material.color = tC;
					StartCoroutine(RestoreColor(tempTile));
					iTween.MoveTo (tempTile,iTween.Hash ("position", GetCoordinate(i,j,1),"time",0.5f,"easetype", iTween.EaseType.easeOutQuart));
					//tempTile = (GameObject)Instantiate (prefabKeeper.endAnim[k], GetCoordinate(i,j,1), transform.rotation);
					Destroy (tile[i,j].gameObject);
					tile[i,j] = (TileKeeper) tempTile.GetComponent ("TileKeeper");
					 
					tile[i,j].vec = new Vector2(i,j);
					break;
				case "i" :
					tempZ = tile[i,j].specialOut.Length - 1;
					tempX = (int)tile[i,j].specialOut[tempZ].x;
					tempY = (int)tile[i,j].specialOut[tempZ].y;
					if (tile[i,j].specialOut[tempZ].z != -1f) tempZ = (int)tile[i,j].specialOut[tempZ].z;
					
					tempTile = (GameObject)Instantiate (prefabKeeper.outAnim[inoutCount * 4 + tempZ], GetCoordinate (tempX, tempY, 1), transform.rotation);
					Destroy (tile[tempX,tempY].gameObject);
					tile[tempX,tempY] = (TileKeeper) tempTile.GetComponent ("TileKeeper"); 
					
					tempZ = tile[i,j].specialOut.Length - 1;
					tempX = i - nextTileX[tempZ];
					tempY = j - nextTileY[tempZ];
					
					tempTile = (GameObject)Instantiate (prefabKeeper.inAnim[inoutCount * 4 + tempZ], GetCoordinate (i, j, 1), transform.rotation);
					if (tile[tempX,tempY].specialOut.Length == 0){
						tile[tempX,tempY].specialOut = new Vector3[4];
						for (int k = 0 ; k < 4 ; k++)
							tile[tempX,tempY].specialOut[k].x = -1f;
					}
					if (tile[i,j].specialOut[tempZ].z == -1f)
						tile[tempX,tempY].specialOut[tempZ] = new Vector3(tile[i,j].specialOut[tempZ].x + nextTileX[tempZ], tile[i,j].specialOut[tempZ].y + nextTileY[tempZ], tempZ);
					else{
						tile[tempX,tempY].specialOut[tempZ] = new Vector3(tile[i,j].specialOut[tempZ].x + nextTileX[(int)tile[i,j].specialOut[tempZ].z], tile[i,j].specialOut[tempZ].y+ nextTileY[(int)tile[i,j].specialOut[tempZ].z], tile[i,j].specialOut[tempZ].z);
					}
					
					Destroy (tile[i,j].gameObject);
					tile[i,j] = (TileKeeper) tempTile.GetComponent ("TileKeeper"); 
					
					inoutCount++; break;
				}
			}
		}
	}

	void Unload(){
		Resources.UnloadUnusedAssets ();
	}
	
	IEnumerator TakeScreenShot(){
		yield return new WaitForEndOfFrame ();
		
		byte[] imageByte;
		Texture2D tex = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, true);
		tex.ReadPixels (new Rect (0, 0, Screen.width, Screen.height),0 ,0, true);
		tex.Apply ();
		imageByte = tex.EncodeToPNG ();
		DestroyImmediate (tex);
		File.WriteAllBytes ("mnt/sdcard/DCIM/number1" + ".png" , imageByte);
		
		System.IO.Directory.GetFiles("mnt/sdcard/DCIM/");
	}

	IEnumerator RestoreColor(GameObject obj){
		yield return new WaitForSeconds (0.5f);
		//Color tC = Color.black;
		obj.GetComponent<Renderer>().material.color = Color.white;
	}
	
	int GetSumWeight(){
		/*파이프 가중치들의 총합을 구함*/
		int sum = 0;
		for (int i = 0; i < stageInfo.pipeWeight.Length ; i++)
			sum += stageInfo.pipeWeight[i];
		return sum;
	}
	
	void SetPreview (int tX, int tY){
		PipeKeeper tempPipe = (PipeKeeper)nextPipe [0].GetComponent ("PipeKeeper");
		preview = (GameObject)Instantiate (prefabKeeper.preview[tempPipe.pipeNum], GetCoordinate (tX, tY, 7), tempPipe.transform.rotation);
	}
	
	void SetPreview (float tX, float tY){
		PipeKeeper tempPipe = (PipeKeeper)nextPipe [0].GetComponent ("PipeKeeper");
		preview = (GameObject)Instantiate (prefabKeeper.preview[tempPipe.pipeNum], GetCoordinate (tX, tY, 7), tempPipe.transform.rotation);
	}
	
	GameObject GeneratePipe(int code){
		int selectedNum = -1;
		if (setupPipe.Count > 0) {
			selectedNum = (int)setupPipe[0];
			setupPipe.RemoveAt(0);
		}
		else{
			/*새로운 파이프 1개를 code 가 해당하는 위치에 생성*/
			int rndPipeNum = Random.Range (0, sumWeight);
					
			/*생성할 파이프 결정*/
			for (int i = 0; i < stageInfo.pipeWeight.Length; i++) {
				rndPipeNum -= stageInfo.pipeWeight[i];
				if (rndPipeNum <0){
					selectedNum = i;
					break;
				}
			}
		}

		/*해당 파이프 생성*/
		GameObject tempPipe = NGUITools.AddChild (nextPipeParent, prefabKeeper.nguiPipe[selectedNum]);

		TweenPosition.Begin (tempPipe, 0, boxPosition [code]);
		//tempPipe.transform.position += boxPosition [code] / 540f;
		tempPipe.transform.localScale = Vector3.one * 70;
		//GameObject tempPipe = (GameObject) Instantiate(prefabKeeper.nguiPipe[selectedNum]);
		//tempPipe.transform.parent = nextPipeParent.transform;
		//GameObject tempPipe = (GameObject) Instantiate(prefabKeeper.pipe[selectedNum], boxPosition[code], transform.rotation);
				
		return tempPipe;
	}
	
	void ClearBorder(Vector2 vec){
		if (isPipemoving) return;
		int x = (int)vec.x;
		int y = (int)vec.y;
		//if (!tile [x, y].isClicked)	return;
		if (tile [x, y].tileType == "p")
			pipeOnTile [x, y].transform.localScale = Vector3.one;
		ChangeColor (tile [x, y].gameObject, Vector3.one);
		tile [x, y].isClicked = false;
	}
	
	void SetBorder(Vector2 vec){
		if (isPipemoving) return;
		int x = (int)vec.x;
		int y = (int)vec.y;

		/*터치모드가 아이템 사용이 아닐 시*/
		if (touchMode == TouchMode.Default)
			preview.transform.position = GetCoordinate (x, y, 7);
		
		if (tile [x, y].isClicked) return;
		if (tile [x, y].tileType == "p")
			pipeOnTile [x, y].transform.localScale = Vector3.one * 0.9f;
		ChangeColor (tile [x, y].gameObject, Vector3.one * 0.5f);
		//tile [x, y].isClicked = true;
		
	}
	
	void Update(){
		ScorebarSlide ();
		if (isGameStop)	return;
		WaterFlow ();
		Timer ();
	}

	void ScorebarSlide(){
		if (!isScorebarSliding) return;
		scorebarSlideTimer -= Time.deltaTime;
		scorebar.sliderValue = (scorebarsliderDestinationValue - scorebarSliderStartValue) * (0.2f - scorebarSlideTimer) / 0.2f + scorebarSliderStartValue;
		if (scorebarSlideTimer <= 0)	isScorebarSliding = false;
	}

	int flowedCount = 0;

	enum flowButtonMode {flowNormal, flowFast, stop};
	private flowButtonMode flowButtonState;
	private bool flowButtonTimerRunning = false;

	IEnumerator FlowButtonEnableTimer(){
		yield return new WaitForSeconds (0.5f);
		flowButtonTimerRunning = false;
	}
	
	void FlowButtonTouched(){
		if (flowButtonTimerRunning) return;
		switch (flowButtonState) {
		case flowButtonMode.flowNormal :
			flowMultiplier = 0.1f;
			flowButtonState = flowButtonMode.flowFast;
			if (isWaterFlow)
				audioKeeper.PlayEffectsound("waterStart");
			flowButton.SendMessage("SetButtonStateFast", SendMessageOptions.DontRequireReceiver);
			break;
		case flowButtonMode.flowFast :
			if (stageInfo.restrictType == 'p'){
				isWaterFlow = false;
				flowButtonState = flowButtonMode.stop;
				flowButton.SendMessage("SetButtonStateStop", SendMessageOptions.DontRequireReceiver);
			}
			else{
				flowMultiplier = 1f;
				flowButtonState = flowButtonMode.flowNormal;
				flowButton.SendMessage("SetButtonStatePlay", SendMessageOptions.DontRequireReceiver);
			}
			break;
		case flowButtonMode.stop :
			if (!waterFlowStarted)
				StartWaterFlow();
			else
				isWaterFlow = true;
				flowMultiplier = 0.1f;
			/*
			 * 물 시작 안했을 경우엔 시작, 물 시작했을 경우엔 다시 시작, 물 속도는 빠르게!
			 */
			flowButtonState = flowButtonMode.flowFast;
			flowButton.SendMessage("SetButtonStateFast", SendMessageOptions.DontRequireReceiver);
			break;
		}
		flowButtonTimerRunning = true;
		StartCoroutine (FlowButtonEnableTimer());
	}

	private bool waterFlowStarted = false;

	void Vibrate(float time){
		StartCoroutine (cVibrate (time));
	}

	IEnumerator cVibrate(float time){
		yield return new WaitForSeconds (time);
		Vibrate ();
	}

	void Vibrate(){
		if (PlayerPrefs.HasKey ("Vibration") && PlayerPrefs.GetInt ("Vibration") == 0) return;
		Handheld.Vibrate ();
	}

	IEnumerator InitializeWaterFlow(){
		dir = (int)stageInfo.startTile.z;
		fX = (int)stageInfo.startTile.x - nextTileX[dir];
		fY = (int)stageInfo.startTile.y - nextTileY[dir];
				
		flowedTime = 0f;
		flowDuration = devInfo.waterFlowDuration / 3;
		
		audioKeeper.PlayBGM ("waterBGM");
		audioKeeper.PlayEffectsound ("waterStart");
		AnimatorControl (tile [fX, fY].gameObject, true);
		Vibrate ();
		
		flowManager = (GameObject)Instantiate (prefabKeeper.flowManager, 
		                                       GetCoordinate (fX, fY, 6) + 
		                                       new Vector3 (nextTileX [dir] * tileSize * 0.5f, nextTileY [dir] * tileSize * 0.5f, 0),
		                                       transform.rotation);
		yield return new WaitForSeconds (1.75f);
		GetNextWaterDt ();
		waterFlowStarted = true;
		isWaterFlow = true;
	}

	IEnumerator OnResetButtonClick(){
		if (isPipemoving) yield break;
		int numToReset = (stageInfo.restrictNumber - pipeUsed >= 5 ? 5 : stageInfo.restrictNumber - pipeUsed);
		isPipemoving = true;
		for (int i = 0; i < numToReset; i++) {
			Destroy(nextPipe[i]);
		}
		for (int i = 0; i < 5 ; i++){
			for (int j = 0; j < i ; j++){
				TweenPosition.Begin (nextPipe[j], 0.4f, boxPosition[4 - i + j]).method = UITweener.Method.EaseIn;
				//iTween.MoveTo(nextPipe[j], iTween.Hash ("position", boxPosition[4 - i + j], "time", 0.4f, "easetype", iTween.EaseType.easeInQuad));
			}
			yield return new WaitForSeconds(0.4f);
			if (i < numToReset)
				nextPipe[i] = GeneratePipe(4);
		}
		//nextPipe [0].transform.localScale= Vector3.one * 1.19f;
		nextPipe [0].transform.localScale= Vector3.one * 144;

		AfterPipeMoved ();
		isPipemoving = false;
		itemButton [1].SendMessage ("ItemUsed", SendMessageOptions.DontRequireReceiver);
	}

	void OnBandageButtonClick(){
		if (!isWaterFlow || isGameStop) return;
		GameObject tempObj = (GameObject)Instantiate (prefabKeeper.itemBandage);
		iTween.MoveTo (tempObj, iTween.Hash ("time", 0.2f, "position", flowManager.transform.position+ new Vector3(0,0,-3)));
		audioKeeper.PlayEffectsound ("bandFire");
		audioKeeper.PlayEffectsound ("bandUsed", 5.2f);
		//iTween.MoveBy (tempObj, iTween.Hash ("time", 0.4f, "x",3,"y",10, "delay", 5));
		//iTween.MoveTo (tempObj, iTween.Hash ("time", 0.2f, "position", GameObject.Find ("Band").transform.position + new Vector3(0,0,-6),"delay", 5));
		AnimatorControl (tempObj, true, "Bandage_E", 0.2f);
		Destroy (tempObj, 5.7f);
		itemButton [1].SendMessage ("ItemUsed", SendMessageOptions.DontRequireReceiver);
		//GameObject tempObj = (GameObject)Instantiate ();
		/*밴드 효과 인스텐시에이트*/
		PauseWater (5.7f);
		Vibrate (5.7f);
	}

	void OnHammerButtonClick(){
		if (isGameStop) return;
		ChangeTouchMode (TouchMode.Hammer);
		//GameObject tempHammer = (GameObject)Instantiate (prefabKeeper.itemHammer, GetCoordinate();
	}

	void OnObstacleButtonClick(){
		if (isGameStop)	return;
		ChangeTouchMode (TouchMode.Obstacle);
	}
	
	void PauseWater(float time){
		StartCoroutine (cPauseWater (time));
	}
	
	IEnumerator cPauseWater(float time){
		isWaterFlow = false;
		yield return new WaitForSeconds (time);
		isWaterFlow = true;
	}

	void GetNextWaterDt(){
		if (waterFlowState == 0) {
			flowedCount ++;

			if (tile[fX,fY].tileType == "p") Instantiate(prefabKeeper.grass[tile[fX,fY].pipeNum], GetCoordinate (fX,fY,7),transform.rotation);
			//tile[fX,fY].gameObject.GetComponent<SpriteRenderer>().sprite = grass;
			dir = tile[fX, fY].outDir[dir];
			if(tile[fX, fY].specialOut.Length == 4 && tile[fX, fY].specialOut[dir].x != -1f){
				//인아웃일 경우
				int tempX, tempY, tempZ;
				tempX = (int)tile[fX,fY].specialOut[dir].x;
				tempY = (int)tile[fX,fY].specialOut[dir].y;
				tempZ = dir;
				if (tile[fX,fY].specialOut[dir].z != -1f)
					tempZ = (int)tile[fX,fY].specialOut[dir].z;
				
				audioKeeper.PlayEffectsound("waterIn");
				if(!IsOutOfBound (fX+nextTileX[dir], fY+nextTileY[dir])){
					AnimatorControl(tile[fX+nextTileX[dir], fY+nextTileY[dir]].gameObject, true);
					AnimatorControl(tile[tempX - nextTileX[tempZ],tempY - nextTileY[tempZ]].gameObject, true);
					PauseWater (2f);
				}
				fX = tempX;	fY = tempY;	dir = tempZ;
				flowManager = (GameObject) Instantiate (prefabKeeper.flowManager, 
				                                        GetCoordinate (fX, fY, 6) +
				                                        new Vector3(-nextTileX[dir] * tileSize * 0.5f, -nextTileY[dir] * tileSize * 0.5f, 0),
				                                        transform.rotation);
			}
			else if (flowedCount == 1){
				fX = (int) stageInfo.startTile.x;
				fY = (int) stageInfo.startTile.y;
				dir = (int) stageInfo.startTile.z;
			}
			else{
				Debug.Log (fX + " , " + fY);
				fX += nextTileX[dir];
				fY += nextTileY[dir];
			}
			//좌표를 다음으로 우선 이동
			
			if(!IsEnterable(fX,fY,dir)){
				isGameStop = true;
				Destroy (preview);
				StartCoroutine (FailByConnection(fX,fY,dir));
				return;
			}
			if(IsSuccess (fX,fY)){
				isGameStop = true;
				Vibrate ();
				Destroy (preview);
				if ((stageInfo.missionType == 'p' && totalPipe >= stageInfo.missionNumber)||
				    (stageInfo.missionType == 'c' && totalCrossPipe >= stageInfo.missionNumber)||
				    (stageInfo.missionType == 'a' && CheckAllSquare ())||
				    (stageInfo.missionType == 's')
				    )
					StartCoroutine(Success (fX,fY,dir));
				else
					StartCoroutine (FailByMission(fX,fY,dir));
				
				return;
			}
			Destroy (Instantiate (alertPref, GetCoordinate (fX, fY, 4), transform.rotation), 6);
			isChangeableBefore = tile[fX,fY].isChangeable;
			tile[fX,fY].isChangeable = false;
		}
		
		movingCurve = false;
		//물 흐르는 좌표 계산
		flowStPosition = flowManager.transform.position;
		if (tile [fX, fY].pipeNum < 2)
			flowDtPosition = GetCoordinate (fX, fY, 6) + 
				new Vector3 (nextTileX [dir], nextTileY [dir], 0) * tileSize * (2 * waterFlowState - 1) / 6;
		else if (tile [fX, fY].pipeNum == 2)
			flowDtPosition = GetCoordinate (fX, fY, 6) +
				new Vector3 (nextTileX [dir] * tileSize * (-0.2f + waterFlowState * 0.35f), 
				             nextTileY [dir] * tileSize * (2 * waterFlowState - 1) / 6, 0);
		else {
			switch(waterFlowState){
			case 0: 
				flowDtPosition = GetCoordinate (fX, fY, 6) +
					new Vector3(nextTileX[dir], nextTileY[dir],0) * tileSize / -6f;
				break;
			case 1: 
				movingCurve = true;
				flowDtPosition = GetCoordinate (fX, fY, 6) +
					new Vector3(-nextTileX[dir] + nextTileX[tile[fX,fY].outDir[dir]], -nextTileY[dir] + nextTileY[tile[fX,fY].outDir[dir]],0) * tileSize / 6f;
				break;
			case 2:
				flowDtPosition = GetCoordinate (fX, fY, 6) +
					new Vector3(nextTileX[tile[fX,fY].outDir[dir]], nextTileY[tile[fX,fY].outDir[dir]],0) * tileSize * 0.5f;
				break;
			}
		}
		
		//스코어 및 십자 파이프 효과들
		if (waterFlowState == 1) {
			if (tile[fX,fY].pipeNum == 2 & dir < 2){
				flowManager = (GameObject) Instantiate(prefabKeeper.flowManager,
				                                       GetCoordinate (fX,fY,6) + new Vector3(nextTileX[dir], nextTileY[dir],0) * tileSize /5f,
				                                       transform.rotation);
				flowStPosition = flowManager.transform.position;
				flowDtPosition = flowStPosition;
			}
			if (!isChangeableBefore && tile [fX, fY].pipeNum == 2 && tile [fX, fY].tileType == "p") {
				AddScore (1000, fX, fY);
			} else if (tile [fX, fY].tileType == "op") {
				AddScore (1000, fX, fY);
			} else {
				AddScore (300, fX, fY);
			}
		}
		
		
		
		waterFlowState = (waterFlowState + 1) % 3;
	}
	
	void WaterFlow(){
		if (!isWaterFlow) return;
		flowedTime += Time.deltaTime / flowMultiplier;
		stageInfo.flowFasterTime -= Time.deltaTime;
		
		if(!isFasted && stageInfo.flowFasterTime < 0){
			isFasted = true;
			devInfo.waterFlowDuration = devInfo.waterFlowDurationFast;
		}
		
		if (flowedTime >= flowDuration) {
			GetNextWaterDt(); 	//다음 타일 계산
			flowDuration = devInfo.waterFlowDuration / 3;
			flowedTime = 0;
		}
		
		
		//flowedTime / flowDuration 비교
		if (movingCurve) {
			int[,] rad = {{0,0,3,1}, {0,0,3,1},{0,2,0,0},{0,2,0,0}};
			int[,] vrad = {{0,0,-1,1}, {0,0, 1, -1}, {1,-1,0,0},{-1,1,0,0}};
			float f = (rad[dir,tile[fX,fY].outDir[dir]] + vrad[dir,tile[fX,fY].outDir[dir]] * flowedTime/ flowDuration) * Mathf.PI / 2;
			flowManager.transform.position = flowDtPosition + new Vector3(Mathf.Cos (f),Mathf.Sin (f),0) * tileSize / 6 ;
			//커브 무빙 공식
		}
		else
			flowManager.transform.position = flowStPosition + (flowDtPosition - flowStPosition) * flowedTime /flowDuration ; //직선 무빙 공식
		
		
		
	}
	


	public GameObject alertPref;
	void Timer(){
		if (!isTimerOn)	return;
		if (stageInfo.restrictType == 'p') return;
		timeCounter += Time.deltaTime / flowMultiplier;

		timebar.sliderValue = 1 - timeCounter / stageInfo.timeLimit;
		//timebarCap.transform.position = new Vector3 (-6.0433f, 4.621f - 9.52f * timeCounter / stageInfo.timeLimit, -3);
		//timebarMask.transform.position = new Vector3 (-6.0433f, 4.621f - 4.76f * timeCounter / stageInfo.timeLimit, -2);
		//timebarMask.transform.localScale = new Vector3 (0.25f, 9.52f * timeCounter / stageInfo.timeLimit, 0.2f);
		
		if (timeCounter >= stageInfo.timeLimit){
			isTimerOn = false;
			StartWaterFlow ();

			//StartCoroutine(WaterFlow());
		}
	}

	void StartWaterFlow(){
		if (stageInfo.restrictType == 'p') {
			flowButtonHighlight.SetActive (false);
		}
		//★★얼러트 시작?
		StartCoroutine( InitializeWaterFlow());
	}

	int pipeUsed = 0;

	void TimeMultiply(){
		flowMultiplier = 0.1f;
		if (!isTimerOn)
			audioKeeper.PlayEffectsound("waterStart");
	}

	void TimeNormalize(){
		flowMultiplier = 1f;
	}

	void OnTileMouseDown(Vector2 vec){
		if (isGameStop) return;
		int vX = (int)vec.x;
		int vY = (int)vec.y;

		/*
		if (tile [vX, vY].tileType == "s" || tile [vX, vY].tileType == "e") {
			if (stageInfo.restrictType == 't' || isWaterFlow)
				TimeMultiply ();
		} 

		else */
		if (tile[vX,vY].isChangeable)
			SetBorder (vec);
	}

	void OnTileMouseExit(Vector2 vec){
		if (isGameStop) return;
		int vX = (int)vec.x;
		int vY = (int)vec.y;

		/*
		if (tile [vX, vY].tileType == "s" || tile [vX, vY].tileType == "e") {
			if (stageInfo.restrictType == 't' || isWaterFlow)
				TimeNormalize ();
		} else*/
		if (tile [vX, vY].isChangeable) 
			ClearBorder (vec);
		
	}

    bool tiletouchable = true;

    IEnumerator tileTouchControl()
    {
        yield return new WaitForSeconds(0.1f);
        tiletouchable = true;
    }

	void OnTileMouseUpAsButton(Vector2 vec){
        if (!tiletouchable) return;
        tiletouchable = false;
        StartCoroutine(tileTouchControl());
		if (isGameStop) return;
		int vX = (int)vec.x;
		int vY = (int)vec.y;

		if (tile [vX, vY].tileType != "s" && tile [vX, vY].tileType != "e") {
			ClearBorder (vec);
			switch (touchMode){
			case TouchMode.Default : 
				StartCoroutine(TileTouched (vec));
				break;
			case TouchMode.Hammer :
				StartCoroutine (HammerCrash(vec));
				break;
			case TouchMode.Obstacle :
				StartCoroutine (ObstacleGenerate(vec));
				break;
			}
		}
	}

	public GameObject nextPipeParent;

	public GameObject pipeLeftTextEffect;

	public ArrayList setupPipe = new ArrayList();

	void SetUpPipe(int[] arr){
		foreach (int i in arr) {
			setupPipe.Add (i);
		}
	}

	IEnumerator PipeLeftTextControl(){
		pipeLeftTextEffect.SetActive (true);
		TweenScale.Begin (pipeLeftTextEffect, pipeMovingDuration, Vector3.one * 169).from = Vector3.one * 400;
		TweenScale.Begin (pipeLeftText.transform.parent.gameObject, pipeMovingDuration / 2, Vector3.one * 1.5f);
		yield return new WaitForSeconds (pipeMovingDuration / 2);
		pipeLeftText.text = (stageInfo.restrictNumber - pipeUsed).ToString ();
		TweenScale.Begin (pipeLeftText.transform.parent.gameObject, pipeMovingDuration / 2, Vector3.one);
		yield return new WaitForSeconds (pipeMovingDuration / 2);
		pipeLeftTextEffect.SetActive (false);
	}


	IEnumerator TileTouched(Vector2 vec){
		if (isGameStop) yield break;
		
		int tX = (int)vec.x;
		int tY = (int)vec.y;	
		
		//파이프가 이미 이동중일 시, 변경불가 타일일 시 실행종료
		if (isPipemoving) {
			yield break;
		}
		if (!tile [tX, tY].isChangeable) {
			audioKeeper.PlayEffectsound("blocked");
			yield break;
		}

		//파이프 이동 루틴 시작
		isPipemoving = true;
		pipeUsed++;			//사용 파이프 개수 +1
		lastX = tX;			//각종 효과를 위한 타일 좌표 저장
		lastY = tY;

		Destroy (preview);	//프리뷰 삭제
		
		//해당 좌표 파이프 생성 후, ngui 파이프 삭제
		int pipeNum = nextPipe [0].GetComponent<PipeKeeper> ().pipeNum;
		Destroy (nextPipe [0]);
		GameObject tempPipe = (GameObject) Instantiate (prefabKeeper.pipe[pipeNum], GetCoordinate (tX,tY,6), transform.rotation);

		if (tile [tX, tY].tileType == "p") {
			//기존에 파이프가 있었을 시
			Destroy (pipeOnTile [tX, tY]);		//기존 파이프 삭제
			tile[tX,tY].isLinked = false;

			//파이프 삭제 효과
			Instantiate (prefabKeeper.explosion, GetCoordinate (tX, tY, 7), prefabKeeper.explosion.transform.rotation);
			CrashSound();

			//크랙 삭제
			Destroy (tile [tX, tY].transform.FindChild ("Crack").gameObject);
		} else {
			//처음 파이프를 놓을 시
			AddScore (50, tX, tY);		//점수 추가
			audioKeeper.PlayEffectsound("tileClicked");
		}

		//크랙 생성
		GameObject tempk = (GameObject)Instantiate (prefabKeeper.crack [pipeNum], GetCoordinate (tX, tY, 2), transform.rotation);
		tempk.transform.parent = tile [tX, tY].transform;
		tempk.name = "Crack";
						
		//타일 논리적 정보 쿄체
		pipeOnTile [tX, tY] = tempPipe;
		tile [tX, tY].pipeNum = pipeNum;
		tile [tX, tY].tileType = "p";
		tile [tX, tY].isLinked = true;
		System.Buffer.BlockCopy(pipeinfo, pipeNum * 16, tile[tX,tY].outDir, 0, 16);

		// 파이프 개수제한 일 시, 개수 표시 변경 및 효과
		if (stageInfo.restrictType == 'p') {
			StartCoroutine(PipeLeftTextControl());
		}

		// 파이프 개수 모두 소진 시, 물 흐르기 시작
		if (stageInfo.restrictType == 'p' && stageInfo.restrictNumber == pipeUsed && !waterFlowStarted){
			Destroy(arrow);
			FlowButtonTouched();
			StartWaterFlow();
			yield break;
		}

		//다음 타일 프리뷰 생성 및 이동
		pipeNum = nextPipe [1].GetComponent<PipeKeeper> ().pipeNum;
		preview = (GameObject)Instantiate (prefabKeeper.preview[pipeNum], new Vector3(-8, 1, -7), tempPipe.transform.rotation);
		iTween.MoveTo (preview, iTween.Hash("position", GetCoordinate(tX,tY,7), "time", pipeMovingDuration, "easetype", iTween.EaseType.linear));

		int leftNext = (stageInfo.restrictType == 'p' && stageInfo.restrictNumber - pipeUsed < 5) ? stageInfo.restrictNumber - pipeUsed  : 5;

		//넥스트 파이프들 이동 
		for (int i = 1; i < 5; i++) {
			TweenPosition.Begin (nextPipe[i], pipeMovingDuration, boxPosition[i - 1]).method = UITweener.Method.EaseIn;
		}

		//넥스트 파이프 논리적으로 한칸씩 밀어내기
		for (int i = 1; i < 5; i++) {
			nextPipe [i - 1] = nextPipe [i];
		}

		//넥스트 파이프 크기! 
		nextPipe [0].transform.localScale = Vector3.one * 144f;

		//파이프 이동 후 논리적 계산 및 효과들
		AfterPipeMoved ();

		iTween.ScaleFrom (tile [tX, tY].gameObject, Vector3.one * 0.8f, 0.2f);

		//대기시간
		yield return new WaitForSeconds (pipeMovingDuration);

		//넥스트 파이프 생성, 개수제한 미션이고, 남은 개수가 5개 미만일 시 생성 안함
		if (stageInfo.restrictType != 'p' || stageInfo.restrictNumber - pipeUsed >= 5)
			nextPipe [4] = GeneratePipe (4);
		else
			nextPipe[4] = new GameObject();

		//파이프 이동완료, 스위치 끄기
		isPipemoving = false;
	}

	void ChangeTouchMode(TouchMode mode){
		if (touchMode == mode) {
			switch (mode){
			case TouchMode.Hammer:
				itemButton[2].GetComponentInChildren<UISprite>().color = Color.white;
				break;
			case TouchMode.Obstacle :
				itemButton[0].GetComponentInChildren<UISprite>().color = Color.white;
				break;
			}
			touchMode = TouchMode.Default;
			return;
		}
		switch (touchMode) {
		case TouchMode.Hammer :
			itemButton[2].GetComponentInChildren<UISprite>().color = Color.white;
			break; 
		case TouchMode.Obstacle :
			itemButton[0].GetComponentInChildren<UISprite>().color = Color.white;
			break; 
		}
		switch (mode){
		case TouchMode.Hammer :
			itemButton[2].GetComponentInChildren<UISprite>().color = new Color(0.5f, 0.5f, 0.5f);
			break;
		case TouchMode.Obstacle :
			itemButton[0].GetComponentInChildren<UISprite>().color = new Color(0.5f, 0.5f, 0.5f) ;
			break;
		}
		touchMode = mode;
	}

	Sprite empty;
	Sprite obstacleTexture;

	IEnumerator HammerCrash(Vector2 vec){

		isPipemoving = true;
		ChangeTouchMode (TouchMode.Default);

		int vX = (int)vec.x;
		int vY = (int)vec.y;

		if (tile [vX, vY].tileType == "p" && tile[vX,vY].isChangeable) {
			Destroy (pipeOnTile [vX, vY], pipeMovingDuration);
			Destroy (tile [vX, vY].transform.FindChild ("Crack").gameObject, pipeMovingDuration);
		}
		else if (tile [vX, vY].tileType == "em" && !tile [vX, vY].isChangeable)
			;
		else {
			isPipemoving = false;
			yield break;
		}
		Destroy (Instantiate(prefabKeeper.itemHammer, GetCoordinate (vX - 1.2f, vY + 0.5f, 8),transform.rotation), 0.5f);
		tile [vX, vY].tileType = "em";
		tile [vX, vY].isChangeable = true;
		tile [vX, vY].outDir = new int[]{4,4,4,4};
		AfterPipeMoved();
		itemButton [2].SendMessage ("ItemUsed", SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (pipeMovingDuration);
		tile[vX,vY].GetComponent<SpriteRenderer>().sprite = empty;
		isPipemoving = false;
	}
	
	IEnumerator ObstacleGenerate(Vector2 vec){
		ChangeTouchMode (TouchMode.Default);

		int vX = (int)vec.x;
		int vY = (int)vec.y;

		if (tile [vX, vY].tileType == "p" && tile[vX,vY].isChangeable) {
			Destroy (pipeOnTile [vX, vY], pipeMovingDuration);
			Destroy (tile [vX, vY].transform.FindChild ("Crack").gameObject, pipeMovingDuration);
		}

		if ((tile[vX,vY].tileType =="p" || tile [vX, vY].tileType == "em" )&& tile [vX, vY].isChangeable) {
			isPipemoving = true;
			Destroy (Instantiate (prefabKeeper.itemObstacle, GetCoordinate(vX, vY, 7), transform.rotation), 0.5f);
			tile [vX, vY].isChangeable = false;
			tile [vX, vY].outDir = new int[]{4,4,4,4};
			AfterPipeMoved();
			itemButton [0].SendMessage ("ItemUsed", SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(pipeMovingDuration);
			tile[vX,vY].GetComponent<SpriteRenderer>().sprite = obstacleTexture;
			tile[vX,vY].tileType = "em";
			isPipemoving = false;
		}
		 
		yield break;
	}
	
	bool IsOutOfBound(int rX, int rY){
		if (rX < 0 || rX >= 10 || rY < 0 || rY >= 8)
			return true;
		return false;
	}
	
	bool IsEnterable(int rX,int rY,int dir){
		if (IsOutOfBound (rX,rY))
			return false;
		else if (tile [rX, rY].outDir [dir] == 4)
			return false;
		return true;
	}
	
	bool IsSuccess(int x, int y){
		if (tile [x, y].tileType == "e")
			return true;
		return false;
	}
	
	void CrashSound(){
		audioKeeper.PlayEffectsound (audioKeeper.crash[Random.Range (0,9)]);
	}
	
	bool connectedToEnd = false;
	int lastMissionCount;

	void AfterPipeMoved(){
		/*파이프가 놓여진 후 포인트 애로우 표시, 연결된 타일 및 인아웃 색 변경*/
		int tX = (int)stageInfo.startTile.x;
		int tY = (int)stageInfo.startTile.y;
		int dir = (int)stageInfo.startTile.z;
		
		totalPipe = 0;
		totalCrossPipe = 0;
		
		/*연결타일의 이전 연결여부 저장 및 현재 연결여부 초기화*/
		for (int i = 0; i < 10; i++) {
			for (int j = 0 ; j < 8; j++){
				tile[i,j].isLinkedBefore = tile[i,j].isLinked;
				tile[i,j].isLinked = false;
			}
		}
		
		/*trace*/
		while (true) {
			if (!IsEnterable (tX, tY, dir)){
				SetEndConnection(false);
				break;
			}
			if (IsSuccess (tX, tY)){
				SetEndConnection (true);
				break;
			}
			
			totalPipe++;
			if (tile[tX,tY].isLinked) totalCrossPipe++;
			
			/*해당 타일 연결여부 확인*/
			tile[tX,tY].isLinked = true;
			
			/*다음 타일로 trace*/
			dir = tile [tX, tY].outDir [dir];
			if (tile [tX, tY].specialOut.Length == 4 && tile [tX, tY].specialOut[dir].x != -1f ) {
				/*인아웃 타일일 경우*/
				if (!IsOutOfBound (tX + nextTileX[dir], tY + nextTileY[dir]))
					tile[tX + nextTileX[dir], tY + nextTileY[dir]].isLinked = true; //in 타일에 적용
				int tempX, tempY;
				tempX = (int)tile [tX, tY].specialOut [dir].x;
				tempY = (int)tile [tX, tY].specialOut [dir].y;
				if (tile [tX, tY].specialOut [dir].z != -1f)
					dir = (int)tile [tX, tY].specialOut [dir].z;
				tX = tempX;
				tY = tempY;
				
				if (!IsOutOfBound (tX - nextTileX[dir], tY - nextTileY[dir]))
					tile[tX - nextTileX[dir], tY - nextTileY[dir]].isLinked = true; //out 타일에 적용
			} else {
				/*일반 타일일 경우 더함*/
				tX += nextTileX [dir];
				tY += nextTileY [dir];
			}
		}
		
		if (arrow != null)
			Destroy (arrow);
		/*화살표, 금지표시 결정 후 instantiate*/
		if (!IsOutOfBound (tX, tY) && tile [tX, tY].isChangeable && tile [tX, tY].tileType == "em") 
			SetPointArrow (tX, tY, dir);
		else if (!IsOutOfBound (tX, tY) && IsEnterable (tX,tY,dir) && tile [tX, tY].tileType == "e" ) 
			;
		else 
			SetPointError (tX, tY, dir);
		
		
		for (int i = 0; i < 10; i++) {
			for (int j = 0 ; j < 8; j++){
				if (tile[i,j].isLinkedBefore != tile[i,j].isLinked){
					if (tile[i,j].isLinked)
					switch(tile[i,j].tileType){
						case "p" : ChangeColor (pipeOnTile[i,j], devInfo.ConnectedPipeRgb); break;
						case "i" : InOutBoarder(i,j,true); break;
						case "o" : InOutBoarder(i,j,true); break;
					}
					else
					switch(tile[i,j].tileType){
						case "p" : ChangeColor (pipeOnTile[i,j], devInfo.BasePipeRgb); break;
						case "i" : InOutBoarder(i,j,false); break;
						case "o" : InOutBoarder(i,j,false); break;
					}
				}
			}
		}

		if (stageInfo.missionType == 'p' ) {
			int currentMissionCount = Mathf.Max ((stageInfo.missionNumber - totalPipe), 0);
			if (currentMissionCount < lastMissionCount){
				lastMissionCount = currentMissionCount;
				StartCoroutine(MissionTextEffectOnPipeMission());
			}
			else{
				lastMissionCount = currentMissionCount;
				missionText.text = lastMissionCount.ToString();
			}

		}
		else if (stageInfo.missionType =='c') {
			int currentMissionCount = Mathf.Max (stageInfo.missionNumber - totalCrossPipe, 0);
			if (currentMissionCount < lastMissionCount){
				lastMissionCount = currentMissionCount;
				StartCoroutine (MissionTextEffectOnCrossMission());
			}
			else{
				lastMissionCount = currentMissionCount;
				missionText.text = lastMissionCount.ToString();
			}

		}

		if (totalPipe > totalPipeMax) {
			FlashPipe();
			if (totalPipe > totalPipeMax + 1){
				/*고져스 + 양쪽 퍼지기*/
				EnableForSecond(textImage.transform.FindChild("Gorgeous").gameObject, 1f);
				FlashForwardBackward();
			}
			else if (totalPipe / 5 > totalPipeMax / 5 && totalPipe % 5 == 0){
				EnableForSecond(textImage.transform.FindChild("Quintuple").gameObject, 1f);
				StartCoroutine(FlashThree(tX,tY,dir));
			}
			else{
				//StartCoroutine(WaterFlash (false));
				StartCoroutine(FlashBackward (tX,tY,dir,false));
			} 
			
			
			totalPipeMax = totalPipe;
			pipeText.text = totalPipe.ToString() + " pipes";
			EnableForSecond( pipeText.gameObject, 1);
			/*
			pipeConnected.gameObject.SetActive(true);
			pipeConnected.text = totalPipe.ToString() + " pipes";
			pipeConnected.transform.position = GetCoordinate (lastX , lastY, 0);
			SetGUIPosition(pipeConnected.gameObject);
			iTween.MoveBy (pipeConnected.gameObject, iTween.Hash ("y", 0.05, "time", 0.5f));
			pipeConnected.gameObject.transform.FindChild("Shadow").guiText.text = totalPipe.ToString() + " pipes";
			StartCoroutine (GUIHide(pipeConnected, 0.5f));*/
		} 
	}

	void FlyBall(int tX, int tY){
		GameObject fly = GameObject.Find ("Fly");
		GameObject flyTarget = NGUITools.AddChild (fly, prefabKeeper.fly[tile[tX,tY].pipeNum]);
		float flyTime = Random.Range (0.1f, 0.4f);
		flyTarget.transform.localScale = Vector3.one * 121;
		flyTarget.transform.position = GetCoordinate(tX,tY,0) / 5.4f;
		TweenPosition.Begin (flyTarget, flyTime, new Vector3 (873, 128, 0));
		flyTarget.GetComponent<TweenPosition> ().method = UITweener.Method.EaseIn;
		TweenScale.Begin (flyTarget, flyTime, Vector3.one * 80);
		Destroy (flyTarget, flyTime);
	}

	IEnumerator MissionTextEffectOnCrossMission(){
		int tX = (int)stageInfo.startTile.x;
		int tY = (int)stageInfo.startTile.y;
		int dir = (int)stageInfo.startTile.z;
		int flyCount = 0;

		/*연결타일의 이전 연결여부 저장 및 현재 연결여부 초기화*/
		for (int i = 0; i < 10; i++) {
			for (int j = 0 ; j < 8; j++){
				tile[i,j].isLinked = false;
			}
		}
		
		while (true) {
			if (!IsEnterable (tX,tY,dir))
				break;
			if (IsSuccess (tX, tY))
				break;

			if (tile[tX,tY].isLinked ){
				FlyBall (tX,tY);
				flyCount++;
			}
			tile[tX,tY].isLinked = true;

			dir = tile[tX,tY].outDir[dir];
			if (tile[tX,tY].specialOut.Length == 4 && tile[tX,tY].specialOut[dir].x != -1f){
				if (!IsOutOfBound (tX + nextTileX[dir], tY + nextTileY[dir]))
					tile[tX + nextTileX[dir], tY + nextTileY[dir]].isLinked = true; //in 타일에 적용
				int tempX, tempY;
				tempX = (int)tile [tX, tY].specialOut [dir].x;
				tempY = (int)tile [tX, tY].specialOut [dir].y;
				if (tile [tX, tY].specialOut [dir].z != -1f)
					dir = (int)tile [tX, tY].specialOut [dir].z;
				tX = tempX;
				tY = tempY;
				
				if (!IsOutOfBound (tX - nextTileX[dir], tY - nextTileY[dir]))
					tile[tX - nextTileX[dir], tY - nextTileY[dir]].isLinked = true; //out 타일에 적용
			}
			else{
				tX += nextTileX[dir];
				tY += nextTileY[dir];
			}
		}

		int preMissionCount = int.Parse (missionText.text);
		int counterCount = Mathf.Min (flyCount, 3);
		GameObject missionIcon = missionText.transform.parent.gameObject;
		for (int i = 0; i < counterCount; i++) {
			TweenScale.Begin (missionIcon, 0.1f / counterCount, new Vector2(1.5f, 1.5f));
			yield return new WaitForSeconds(0.1f / counterCount);
			missionText.text = Mathf.Max ((preMissionCount - (i + 1) * flyCount / counterCount), 0).ToString();
			TweenScale.Begin (missionIcon, 0.1f / counterCount, new Vector2(1, 1));
			yield return new WaitForSeconds(0.1f / counterCount);
		}
	}



	IEnumerator MissionTextEffectOnPipeMission(){
		int tX = (int)stageInfo.startTile.x;
		int tY = (int)stageInfo.startTile.y;
		int dir = (int)stageInfo.startTile.z;
		bool fly = false;
		int flyCount = 0;
		while (true) {
			if (!IsEnterable (tX,tY,dir))
				break;
			if (IsSuccess (tX, tY))
				break;
			if (fly || tX == lastX && tY == lastY){
				fly = true;
				FlyBall(tX,tY);
				flyCount ++;
			}
			
			dir = tile[tX,tY].outDir[dir];
			if (tile[tX,tY].specialOut.Length == 4 && tile[tX,tY].specialOut[dir].x != -1f){
				int tempX, tempY;
				tempX = (int) tile[tX,tY].specialOut[dir].x;
				tempY = (int) tile[tX,tY].specialOut[dir].y;
				if (tile [tX, tY].specialOut [dir].z != -1f)
					dir = (int)tile [tX, tY].specialOut [dir].z;
				tX = tempX;
				tY = tempY;
			}
			else{
				tX += nextTileX[dir];
				tY += nextTileY[dir];
			}
		}

		int preMissionCount = int.Parse (missionText.text);
		int counterCount = Mathf.Min (flyCount, 3);
		GameObject missionIcon = missionText.transform.parent.Find ("Icon_Pipe").gameObject;
		for (int i = 0; i < counterCount; i++) {
			TweenScale.Begin (missionText.gameObject, 0.1f / counterCount, new Vector2(105, 105));
			TweenScale.Begin (missionIcon, 0.1f / counterCount, new Vector2(105, 133));
			yield return new WaitForSeconds(0.1f / counterCount);
			missionText.text = Mathf.Max ((preMissionCount - (i + 1) * flyCount / counterCount), 0).ToString();
			TweenScale.Begin (missionText.gameObject, 0.1f / counterCount, new Vector2(70, 70));
			TweenScale.Begin (missionIcon, 0.1f / counterCount, new Vector2(70, 89));
			yield return new WaitForSeconds(0.1f / counterCount);
		}
	}

	int lastX, lastY;
	
	bool stEndBorderColorGreen = false;
	bool stEndBorderColorRed = false;
	
	Vector2 startTile;
	Vector2 endTile;

	public GameObject flowButtonHighlight;

	void SetStEndBorder(bool res){
		tile [(int)startTile.x, (int)startTile.y].transform.FindChild ("Green0").gameObject.SetActive (res);
		tile [(int)startTile.x, (int)startTile.y].transform.FindChild ("Green1").gameObject.SetActive (res);
		tile [(int)endTile.x, (int)endTile.y].transform.FindChild ("Green0").gameObject.SetActive (res);
		tile [(int)endTile.x, (int)endTile.y].transform.FindChild ("Green1").gameObject.SetActive (res);
		if (stageInfo.restrictType == 'p' && !waterFlowStarted) {
			flowButtonHighlight.SetActive(res);
		}

	}
	
	bool IsMissionComplete(){
		if (stageInfo.missionType == 'p' && stageInfo.missionNumber > totalPipe)
			return false;
		if (stageInfo.missionType == 'c' && stageInfo.missionNumber > totalCrossPipe)
			return false;
		return true;
	}
	
	void SetEndConnection(bool res){
		if (res && !IsMissionComplete ())
			res = false;
		if (res == connectedToEnd) return;
	
		SetStEndBorder (res);
		connectedToEnd = res;
	}

	void PauseGame(){
		isGameStop = true;
	}

	void ResumeGame(){
		isGameStop = false;
	}


	IEnumerator FlashForward(int tX, int tY, int dir){
		float movetime = 0.1f;
		dir = tile [tX, tY].outDir [dir];
		GameObject flashManager = (GameObject)Instantiate (prefabKeeper.flashManager, 
		                                                   GetCoordinate (tX, tY, 4) + new Vector3 (nextTileX [dir], nextTileY [dir], 0) * tileSize * 0.5f,
		                                                   transform.rotation);
		for (int i = 0; i < 3; i++) {
			if (tile [tX, tY].specialOut.Length == 4 && tile [tX, tY].specialOut [dir].x != -1.0f)
				break;
			else{
				tX += nextTileX[dir];
				tY += nextTileY[dir];
			}
			
			if(!IsEnterable(tX,tY,dir)) { Destroy(flashManager, 2.5f); yield break; }
			if(IsSuccess(tX,tY)) { Destroy(flashManager, 2.5f); yield break; }
			
			if (tile[tX,tY].pipeNum <= 2){
				iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4) + new Vector3(nextTileX[dir], nextTileY[dir], 0) * tileSize * 0.5f, "time", movetime, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(movetime);
			}
			else{
				iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4), "time", movetime / 2, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(movetime / 2);
				iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4) + new Vector3(nextTileX[tile[tX,tY].outDir[dir]], nextTileY[tile[tX,tY].outDir[dir]], 0) * tileSize * 0.5f, "time", movetime / 2, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(movetime / 2);
			}
			
			dir = tile[tX,tY].outDir[dir];
			
		}
		Destroy(flashManager, 2.5f);
	}
	
	IEnumerator FlashBackward(int tX, int tY, int dir, bool broad){
		float movetime = 0.1f;
		
		GameObject prefab;
		if (broad)
			prefab = prefabKeeper.flashManager2;
		else
			prefab = prefabKeeper.flashManager;
		
		GameObject flashManager = (GameObject)Instantiate (prefab, 
		                                                   GetCoordinate (tX, tY, 4) - new Vector3 (nextTileX [dir], nextTileY [dir], 0) * tileSize * 0.5f,
		                                                   transform.rotation);
		
		for (int i = 0; i < 3; i++) {
			tX -= nextTileX [dir];
			tY -= nextTileY [dir];
			
			if (IsOutOfBound(tX,tY))
				break;
			if (tile[tX,tY].tileType != "p")
				break;
			
			for (int j = 0; j < 4; j++){
				if (tile[tX,tY].outDir[j] == dir){
					dir = j; break;
				}
			}
			if (tile[tX,tY].pipeNum <= 2){
				iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4) - new Vector3(nextTileX[dir], nextTileY[dir], 0) * tileSize * 0.5f, "time", movetime, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(movetime);
			}
			else{
				iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4), "time", movetime / 2, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(movetime / 2);
				iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4) - new Vector3(nextTileX[dir], nextTileY[dir], 0) * tileSize * 0.5f, "time", movetime / 2, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(movetime / 2);
			}
		}
		Destroy(flashManager, 2.5f);
		
	}
	
	void FlashForwardBackward(){
		int tX = (int)stageInfo.startTile.x;
		int tY = (int)stageInfo.startTile.y;
		int dir = (int)stageInfo.startTile.z;
		
		while (true) {
			if (!IsEnterable (tX,tY,dir))
				return;
			if (IsSuccess (tX, tY))
				return;
			if (tX == lastX && tY == lastY){
				StartCoroutine(FlashForward(tX, tY, dir));
				StartCoroutine(FlashBackward(tX, tY, dir,false));
				/*startflash forward backward*/
			}
			
			dir = tile[tX,tY].outDir[dir];
			if (tile[tX,tY].specialOut.Length == 4 && tile[tX,tY].specialOut[dir].x != -1f){
				int tempX, tempY;
				tempX = (int) tile[tX,tY].specialOut[dir].x;
				tempY = (int) tile[tX,tY].specialOut[dir].y;
				if (tile [tX, tY].specialOut [dir].z != -1f)
					dir = (int)tile [tX, tY].specialOut [dir].z;
				tX = tempX;
				tY = tempY;
			}
			else{
				tX += nextTileX[dir];
				tY += nextTileY[dir];
			}
		}
	}
	
	void FlashPipe(){
		int tX = (int)stageInfo.startTile.x;
		int tY = (int)stageInfo.startTile.y;
		int dir = (int)stageInfo.startTile.z;
		
		Destroy ((GameObject)Instantiate(prefabKeeper.flashStEnd, GetCoordinate(tX-nextTileX[dir],tY-nextTileY[dir],7),transform.rotation), 0.3f);
		
		/*trace*/
		while (true) {
			if (!IsEnterable (tX, tY, dir))
				break;
			if (IsSuccess (tX, tY)){
				Destroy ((GameObject)Instantiate(prefabKeeper.flashStEnd, GetCoordinate(tX,tY,7),transform.rotation), 0.3f);
				break;
			}

			Destroy((GameObject)Instantiate(prefabKeeper.flashPipe[tile[tX,tY].pipeNum], GetCoordinate(tX,tY,7),transform.rotation), 0.3f);
			/*다음 타일로 trace*/
			dir = tile [tX, tY].outDir [dir];
			if (tile [tX, tY].specialOut.Length == 4 && tile [tX, tY].specialOut[dir].x != -1f ) {
				/*인아웃 타일일 경우*/
				if (!IsOutOfBound (tX + nextTileX[dir], tY + nextTileY[dir]))
					Destroy ((GameObject)Instantiate(prefabKeeper.flashStEnd, GetCoordinate(tX+nextTileX[dir],tY+nextTileY[dir],7),transform.rotation), 0.3f);
				int tempX, tempY;
				tempX = (int)tile [tX, tY].specialOut [dir].x;
				tempY = (int)tile [tX, tY].specialOut [dir].y;
				if (tile [tX, tY].specialOut [dir].z != -1f)
					dir = (int)tile [tX, tY].specialOut [dir].z;
				tX = tempX;
				tY = tempY;
				
				if (!IsOutOfBound (tX - nextTileX[dir], tY - nextTileY[dir]))
					Destroy ((GameObject)Instantiate(prefabKeeper.flashStEnd, GetCoordinate(tX-nextTileX[dir],tY-nextTileY[dir],7),transform.rotation), 0.3f);
			} else {
				/*일반 타일일 경우 더함*/
				tX += nextTileX [dir];
				tY += nextTileY [dir];
			}
			
		}
	}
	
	IEnumerator FlashThree(int tX, int tY, int dir){
		StartCoroutine(FlashBackward (tX, tY, dir, true));
		yield return new WaitForSeconds (0.5f);
		StartCoroutine(FlashBackward (tX, tY, dir, true));
		yield return new WaitForSeconds (0.5f);
		StartCoroutine(FlashBackward (tX, tY, dir, true));
	}
	
	IEnumerator WaterFlash(bool test/*int dur, int endNum*/){
		int tX = (int) stageInfo.startTile.x;
		int tY = (int) stageInfo.startTile.y;
		int tDir = (int) stageInfo.startTile.z;
		int fullCount = totalPipe;
		int counter = 0;
		
		float movetime = 0.1f;
		
		GameObject prefab;
		if (test)
			prefab = prefabKeeper.flashManager2;
		else
			prefab = prefabKeeper.flashManager;
		
		GameObject flashManager = flowManager;
		if (fullCount < 4)
			flashManager = (GameObject) Instantiate(prefab, GetCoordinate (tX,tY,4) + new Vector3(nextTileX[tDir], nextTileY[tDir], 0) * -tileSize * 0.5f, transform.rotation);
		
		while (true) {
			if(!isGameStop){
				if(!IsEnterable(tX,tY,tDir)) { Destroy(flashManager, 2.5f); yield break; }
				if(IsSuccess(tX,tY)) { Destroy(flashManager, 2.5f); yield break; }
				
				if (counter == fullCount -4)
					flashManager = (GameObject) Instantiate(prefab, GetCoordinate (tX,tY,4) + new Vector3(nextTileX[tDir], nextTileY[tDir], 0) * -tileSize * 0.5f, transform.rotation);
				if (counter >= fullCount - 4){
					if (tile[tX,tY].pipeNum <= 2){
						iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4) + new Vector3(nextTileX[tDir], nextTileY[tDir], 0) * tileSize * 0.5f, "time", movetime, "easetype", iTween.EaseType.linear));
						yield return new WaitForSeconds(movetime);
					}
					else{
						iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4), "time", movetime / 2, "easetype", iTween.EaseType.linear));
						yield return new WaitForSeconds(movetime / 2);
						iTween.MoveTo (flashManager, iTween.Hash("position", GetCoordinate(tX,tY,4) + new Vector3(nextTileX[tile[tX,tY].outDir[tDir]], nextTileY[tile[tX,tY].outDir[tDir]], 0) * tileSize * 0.5f, "time", movetime / 2, "easetype", iTween.EaseType.linear));
						yield return new WaitForSeconds(movetime / 2);
					}
				}
				tDir = tile[tX,tY].outDir[tDir];
				counter++;
				if (tile [tX, tY].specialOut.Length == 4 && tile [tX, tY].specialOut [tDir].x != -1.0f){
					int tempX, tempY;
					tempX = (int) tile[tX,tY].specialOut[tDir].x;
					tempY = (int) tile[tX,tY].specialOut[tDir].y;
					if (tile[tX,tY].specialOut[tDir].z != -1f)
						tDir = (int)tile[tX,tY].specialOut[tDir].z;
					tX = tempX;
					tY = tempY;
					Destroy(flashManager, 2.5f);
					flashManager = (GameObject) Instantiate (prefab, GetCoordinate(tX, tY, 6) + new Vector3(nextTileX[tDir], nextTileY[tDir], 0) * tileSize * -0.5f,transform.rotation);
				}
				else{
					tX += nextTileX[tDir];
					tY += nextTileY[tDir];
				}
			}
		}
		
	}
	
	IEnumerator GUIHide(GUIText b, float a){
		yield return new WaitForSeconds (a);
		b.gameObject.SetActive (false);
	}
	
	void InOutBoarder (int tx, int ty, bool lkd){
		tile[tx,ty].gameObject.transform.FindChild("boarder1").gameObject.SetActive (lkd);
		tile[tx,ty].gameObject.transform.FindChild("boarder2").gameObject.SetActive (lkd);
	}
	
	IEnumerator Success(int tx, int tY, int dir){
		audioKeeper.PlayEffectsound ("waterEnd");
		AnimatorControl (tile [tx, tY].gameObject, true);
		yield return new WaitForSeconds (2.5f);
		bool isAllSquare = CheckAllSquare();
		Debug.Log ("올스퀘어 : " + isAllSquare);
		/*올스퀘어 일 시 올스퀘어 효과*/
		yield return new WaitForSeconds (0.3f);
		Debug.Log ("미션 파이프의 개수: " + stageInfo.missionPipe + "개, 미션 십자파이프의 개수 : " + stageInfo.missionCrossPipe + "개");
		Debug.Log ("사용된 파이프의 개수: " + totalPipe + "개, 사용된 십자파이프의 개수 : " + totalCrossPipe + "개");
		Debug.Log("성공!");
		StartCoroutine(CrossCheck(isAllSquare));
	}
	
	public bool CheckAllSquare(){
		for (int i = 0; i < 10; i++)
			for (int j = 0; j < 8; j++)
				if (tile [i, j].isChangeable)
					return false;
		return true;
		
	}
	
	IEnumerator FailByConnection(int tX, int tY, int dir){
		isGameStop = true;
		audioKeeper.GetComponent<AudioSource>().Pause();

		Destroy (arrow);
		GameObject tempObj;
		audioKeeper.PlayEffectsound ("waterExplosion");
		audioKeeper.PlayEffectsound ("fail");
		tempObj = (GameObject) Instantiate (prefabKeeper.waterExplosion, GetCoordinate (tX, tY, 9) + new Vector3( - tileSize * nextTileX[dir] * 0.5f,  - tileSize * nextTileY[dir] * 0.5f, 0),transform.rotation);
		yield return new WaitForSeconds (2.5f);
		Destroy (tempObj);
		tempObj = (GameObject) Instantiate (prefabKeeper.pointError, GetCoordinate (tX, tY, 8) + new Vector3( -tileSize * nextTileX[dir] * 0.5f,  - tileSize * nextTileY[dir] * 0.5f, 0),transform.rotation);
		audioKeeper.PlayEffectsound ("failConnect");
		tempObj.transform.localScale *= 1.5f;
		yield return new WaitForSeconds (2.5f);
		Destroy (tempObj);

		audioKeeper.PlayBGM ("failBGM");
		failWindow.transform.parent.gameObject.SetActive (true);
		failWindow.SetActive (true);
		graySkin.SetActive (true);

		//AnimatorControl (failWindow.transform.parent.gameObject, true);
		/*
		tempObj = (GameObject) Instantiate (prefabKeeper.failWindow, new Vector3 (0, 0, -12), transform.rotation);
		GameObject tempObj1 = tempObj.transform.FindChild("Failed").gameObject;
		tempObj1.transform.position = new Vector3 (0.5f, 0.55f, 0);
		tempObj1.guiText.fontSize = (int)(Screen.width * 0.05);
		*/
		
		isWindowOn = true;
		/*BMG 변경!*/
		Debug.Log ("실패");
	}
	
	IEnumerator FailByMission(int tX, int tY, int dir){
		isGameStop = true;
		audioKeeper.GetComponent<AudioSource>().Pause();
		GameObject tempObj;
		audioKeeper.PlayEffectsound ("waterExplosion");
		audioKeeper.PlayEffectsound ("fail");
		tempObj = (GameObject) Instantiate (prefabKeeper.waterExplosion, GetCoordinate (tX, tY, 9) + new Vector3( - tileSize * nextTileX[dir] * 0.5f,  - tileSize * nextTileY[dir] * 0.5f, 0),transform.rotation);
		yield return new WaitForSeconds (2.5f);
		audioKeeper.PlayEffectsound ("failConnect");
		yield return new WaitForSeconds (2.5f);
		Destroy (tempObj);

		audioKeeper.PlayBGM ("failBGM");
		failWindow.transform.parent.gameObject.SetActive (true);
		failWindow.SetActive (true);
		graySkin.SetActive (true);
		failWindow.SendMessage ("SetMissionIcon", stageInfo.missionType, SendMessageOptions.DontRequireReceiver);

		isWindowOn = true;
		Debug.Log ("실패");
	}


	void AnimatorControl(GameObject obj, bool en, string name, float delay){
		StartCoroutine (cAnimatorControl (obj, en, name, delay));
	}
	
	IEnumerator cAnimatorControl(GameObject obj, bool en, string name, float delay){
		yield return new WaitForSeconds (delay);
		AnimatorControl (obj, en, name);
	}
	public void AnimatorControl(GameObject obj, bool en){
		Animator anim = obj.GetComponent<Animator> ();
		anim.enabled = en;
	}
	
	public void AnimatorControl(GameObject obj, bool en, string name){
		Animator anim = obj.GetComponent<Animator> ();
		anim.enabled = en;
		anim.Play (name);
	}

	IEnumerator CrossCheck(bool isAS){
		for (int i = 0; i < 10; i++){
			for (int j = 0; j < 8; j++) {
				if (tile[i,j].isChangeable){
					if (tile[i,j].tileType == "p")
						Instantiate (prefabKeeper.notClear ,GetCoordinate(i,j,7),transform.rotation);
				}
			}
		}
			
		
		int tX = (int)stageInfo.startTile.x;
		int tY = (int)stageInfo.startTile.y;
		int dir = (int)stageInfo.startTile.z;
		GameObject tempObj;
		totalCrossPipe = 0;
		
		/*연결타일의 이전 연결여부 저장 및 현재 연결여부 초기화*/
		for (int i = 0; i < 10; i++) {
			for (int j = 0 ; j < 8; j++){
				tile[i,j].isLinked = false;
			}
		}

		/*trace*/
		while (true) {
			if (!IsEnterable (tX, tY, dir))
				break;
			if (IsSuccess (tX, tY))
				break;
			
			if (tile[tX,tY].isLinked) {
				totalCrossPipe++;
				Instantiate(prefabKeeper.crossCount,GetCoordinate (tX,tY,9),Quaternion.Euler (0,0,0));
				if (totalCrossPipe >= 5){
					AddScore(8000,tX, tY);
				}
				else{
					AddScore(2000 + totalCrossPipe * 1000, tX, tY);
				}
				audioKeeper.PlayEffectsound("blocked");
				/*점수 빠바방! 효과*/
				yield return new WaitForSeconds(0.15f);
				
			}
			
			/*해당 타일 연결여부 확인*/
			tile[tX,tY].isLinked = true;
			
			
			/*다음 타일로 trace*/
			dir = tile [tX, tY].outDir [dir];
			if (tile [tX, tY].specialOut.Length == 4 && tile [tX, tY].specialOut[dir].x != -1f ) {
				/*인아웃 타일일 경우*/
				int tempX, tempY;
				tempX = (int)tile [tX, tY].specialOut [dir].x;
				tempY = (int)tile [tX, tY].specialOut [dir].y;
				if (tile [tX, tY].specialOut [dir].z != -1f)
					dir = (int)tile [tX, tY].specialOut [dir].z;
				tX = tempX;
				tY = tempY;
				
			} else {
				/*일반 타일일 경우 더함*/
				tX += nextTileX [dir];
				tY += nextTileY [dir];
			}
		}

		yield return new WaitForSeconds (1f);
		if (isAS) {
			Debug.Log ("올스퀘어 달성으로 " + (score * 2) + "점 추가");
			AddScore (score * 2);
			PlayerPrefs.SetInt ((Application.loadedLevel - 2).ToString() + "AS", 1);
		}
		
		int pipeScore = score;
		int cleanScore;
		int totalScore;
		
		if (score < stageInfo.starConditionScore [0]) {
			StartCoroutine (FailByMission (fX,fY,dir));
			yield break;
		} else if (isAS) {
			audioKeeper.PlayEffectsound ("successConnect");
			Handheld.Vibrate ();
			GameObject textParent = textImage.transform.FindChild ("AllSquare").gameObject; //굿잡!
			textParent.transform.FindChild("-1").gameObject.SetActive(true);
			for (int i = 0 ; i < 10 ; i++){
				textParent.transform.FindChild(i.ToString()).gameObject.SetActive(true);
				iTween.MoveFrom (textParent.transform.FindChild(i.ToString()).gameObject, iTween.Hash ("y", 10, "time", 0.2f, "easetype", iTween.EaseType.easeInOutElastic));
				//textParent.transform.FindChild(i.ToString()).gameObject.SetActive(true);
				yield return new WaitForSeconds(0.1f);
			}

		}
		else{
			audioKeeper.PlayEffectsound ("successConnect");
			textImage.transform.FindChild ("GoodJob").gameObject.SetActive(true); //굿잡!
			yield return new WaitForSeconds(1);
		}
		yield return new WaitForSeconds(1);
		audioKeeper.PauseBGM ();
		yield return new WaitForSeconds(2);

		/*클린 점수 계산*/
		int emptyCount = 0;
		int pipeCount = 0;
		for (int i = 0; i < 10; i++){
			for (int j = 0; j < 8; j++) {
				if (tile[i,j].isChangeable){
					emptyCount ++;
					if (tile[i,j].tileType == "p")
						pipeCount ++;
				}
			}
		}
		
		if (emptyCount == 0){
			Debug.Log("클린율 100%로 " + score +  "점 획득!");
			cleanScore = score;
			AddScore (score);
		}
		else{
			Debug.Log("클린율 " + ((float)(emptyCount - pipeCount) * 100 / emptyCount) + "%로 " + (int) (score * (float)((float)(emptyCount - pipeCount) / emptyCount)) + "점 획득!");
			cleanScore = (int) (score * (float)((float)(emptyCount - pipeCount) / emptyCount));
			AddScore((int) (score * (float)((float)(emptyCount - pipeCount) / emptyCount)));
		}
		
		totalScore = cleanScore + pipeScore;
		
		int tmp = PlayerPrefs.GetInt ("MaxStage");
		if (tmp < Application.loadedLevel - 1)
			PlayerPrefs.SetInt ("MaxStage", Application.loadedLevel - 1);
		
		if (PlayerPrefs.HasKey ((Application.loadedLevel - 2) + "score")) {
			if (PlayerPrefs.GetInt ((Application.loadedLevel - 2) + "score") < score){
				PlayerPrefs.SetInt ((Application.loadedLevel - 2) + "score", score);
			}
		}
		else
			PlayerPrefs.SetInt((Application.loadedLevel - 2).ToString() + "score", score);
		PlayerPrefs.Save();

		audioKeeper.PlayBGM ("successBGM");
		successWindow.transform.parent.gameObject.SetActive (true);
		successWindow.SetActive (true);
		graySkin.SetActive (true);
				
		for (int i = 0; i < 3; i++) {
			if (score >= stageInfo.starConditionScore[2 - i]){
				successWindow.SendMessage("ShowStar", 3 - i, SendMessageOptions.DontRequireReceiver);
				if (!PlayerPrefs.HasKey ((Application.loadedLevel - 2) + "star"))
					PlayerPrefs.SetInt ((Application.loadedLevel - 2) + "star", 3-i);
				else if (PlayerPrefs.GetInt ((Application.loadedLevel - 2) + "star", 3-i) < 3-i)
					PlayerPrefs.SetInt ((Application.loadedLevel - 2) + "star", 3-i);
				break;
			}
		}
		successWindow.SendMessage("ShowScore", new Vector3(pipeScore, cleanScore, totalScore), SendMessageOptions.DontRequireReceiver);

		isWindowOn = true;
		
		PlayerPrefs.Save();
		
	}
	
	void EnableForSecond(GameObject obj, float time){
		StartCoroutine (cEnableForSecond (obj, time));
	}
	
	IEnumerator cEnableForSecond(GameObject obj, float time){
		obj.SetActive (true);
		yield return new WaitForSeconds (time);
		obj.SetActive (false);
	}
	
	void SetPointArrow(int tX, int tY, int dir){
		arrow = (GameObject)Instantiate(prefabKeeper.pointArrow, 
		                                GetCoordinate(tX,tY,8) + new Vector3 (-nextTileX [dir] * tileSize / 3f, 
		                                      -nextTileY [dir] * tileSize / 3f, 0),Quaternion.Euler (0,0, nextAngle[dir] * 90f));
	}

	void StarEffectSound(){
		audioKeeper.PlayEffectsound ("starEffect");
	}

	void SetPointArrow(Vector3 vec){
		float tX = vec.x;
		float tY = vec.y;
		int dir = (int)vec.z;
		arrow = (GameObject)Instantiate(prefabKeeper.pointArrow,GetCoordinate(tX,tY,8) + new Vector3 (-nextTileX [dir] * tileSize / 3f, -nextTileY [dir] * tileSize / 3f, 0),Quaternion.Euler (0,0, nextAngle[dir] * 90f));
	}
	
	void SetPointError(int tX, int tY, int dir){
		Destroy (arrow);
		arrow = (GameObject)Instantiate(prefabKeeper.pointError, GetCoordinate(tX,tY,8) + new Vector3 (-nextTileX [dir] * tileSize / 3f, -nextTileY [dir] * tileSize / 3f, 0),Quaternion.Euler (0,0,0));
	} 
	
	void ChangeColor(GameObject obj, Vector3 rgbColor){
		Color tempColor = obj.GetComponent<Renderer>().material.color;
		tempColor.r = rgbColor.x;
		tempColor.g = rgbColor.y;
		tempColor.b = rgbColor.z;
		obj.GetComponent<Renderer>().material.color = tempColor;
	}

	void AddScore(int score){
		//점수 더하기(논리적)
		this.score += score;
		scoreText.text = this.score.ToString ();
		
		//점수 텍스트 크기 조정
		if (scoreDigit < (int)(Mathf.Log10 (this.score))) {
			int[] scoreTextSize = {80, 80, 80, 65, 55, 45, 40};
			scoreDigit = (int)Mathf.Log10 (this.score);
			scoreText.transform.localScale = Vector3.one * scoreTextSize[scoreDigit];
		}
		
		//점수바 효과
		scorebarSliderStartValue = scorebarsliderDestinationValue;
		scorebarsliderDestinationValue = (float)this.score / stageInfo.starConditionScore [2] * 0.8f;
		scorebarSlideTimer = 0.2f;
		isScorebarSliding = true;
	}
	
	void AddScore(int score, int tX, int tY){
		//논리적 점수 더하기, 점수 텍스트 크기 조정, 점수바 효과
		AddScore (score);
		//점수 표시
		int[] standardScore = {8000, 3000, 1000, 300, 50};
		Color[] colorSet = {new Color (254, 51, 76, 0) / 255f, new Color (254, 127, 0, 0) / 255f,
			new Color(208, 100, 254, 0) / 255f, new Color(153, 223, 43, 0) / 255f,
			new Color(228, 203, 127, 0) / 255f};
		int[] scaleSet = {90, 70, 60, 60, 60};
		
		for (int i = 0; i < standardScore.Length; i++) {
			if (score >= standardScore[i]){
				GameObject scoreEffect = NGUITools.AddChild(this.scoreEffect.transform.parent.gameObject,this.scoreEffect);
				UILabel scoreEffectText = scoreEffect.GetComponent<UILabel>();
				scoreEffectText.text = score.ToString();
				scoreEffectText.color = colorSet[i];
				scoreEffect.transform.localScale = Vector3.one * scaleSet[i];
				scoreEffect.transform.position = GetCoordinate(tX,tY,0) / 5.4f;
				StartCoroutine (ScoreAnimation (scoreEffectText.gameObject, tX, tY));
				break;
			}
		}
	}
	
	IEnumerator ScoreAnimation(GameObject obj, int tX, int tY){
		Destroy (obj, 1.2f);
		TweenPosition.Begin (obj, 0.5f, GetCoordinate(tX,tY + 0.5f,0) * 100);
		TweenAlpha.Begin (obj, 0.5f, 1);
		
		yield return new WaitForSeconds (0.5f);
		TweenPosition.Begin (obj, 1f, GetCoordinate(tX,tY + 1,0) * 100);
		yield return new WaitForSeconds (0.2f);
		TweenAlpha.Begin (obj, 0.5f, 0);
	}

	public float GetTileSize(){
		return tileSize;
	}
	public Vector3 GetTileBoardPosition(){
		return tileBoardPosition;
	}

	Vector3 GetCoordinate (int x, int y, int depth){
		/*타일좌표를 기준으로 게임내 좌표 변환*/
		return new Vector3 ((-4.5f + x) * tileSize + tileBoardPosition.x, (-3.5f + y) * tileSize + tileBoardPosition.y, -depth);
	}
	
	Vector3 GetCoordinate (float x, float y, int depth){
		return new Vector3 ((-4.5f + x) * tileSize + tileBoardPosition.x, (-3.5f + y) * tileSize + tileBoardPosition.y, -depth);
	}
}