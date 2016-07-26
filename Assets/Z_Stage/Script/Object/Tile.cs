using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public Vector2 vec;
	public string tileType;
	public int pipeNum;
	public int[] outDir;
	public bool isFlowed;
	public Vector2[] outVec;
	public int cropLevel;
	public int cropType;
	public int cropNum;

	private int straightRandomValue;

	public bool connected = false;
	public bool connectedBefore = false;
	public bool crossConnected = false;
	public bool crossConnectedBefore = false;
	public bool isCritical = false;


	[HideInInspector]
	public UISprite tile;
	[HideInInspector]
	public UISprite plantSymbol;
	[HideInInspector]
	public UISprite grass;
	[HideInInspector]
	public UISprite plant;

	[HideInInspector]
	public Pipe pipeOnTile;


	void OnEnable(){
		if (transform.Find ("Tile"))
			tile = transform.Find ("Tile").GetComponent<UISprite> ();
		if (transform.Find ("PlantSymbol")){
			plantSymbol = transform.Find ("PlantSymbol").GetComponent<UISprite> ();
			plantSymbol.spriteName = "Base";
		}
		if (transform.Find ("Grass"))
			grass = transform.Find ("Grass").GetComponent<UISprite> ();
		if (transform.Find ("Plant")){
			plant = transform.Find ("Plant").GetComponent<UISprite> ();
			plant.pivot = UIWidget.Pivot.Bottom;
		}
	}

	public void Initialize(int x, int y){
		this.vec = new Vector2 (x, y);
		transform.localPosition = PipeTools.GetPosition(x, y);
		Initialize ();
	}

	public void Initialize(){
		if (tileType == "s" || tileType == "e") {
			tile.spriteName = "Empty_2";
			Destroy (grass.gameObject);
			Destroy (plantSymbol.gameObject);
			Destroy (plant.gameObject);
		}
		else if (tileType == "i"){
			int[] nextAngle = {1,-1,0,2};			//방향에 따라 포인트 화살표의 각도를 정하기 위한 정보
			tile.spriteName = "In";
			tile.transform.rotation = Quaternion.Euler (0, 0, nextAngle[pipeNum] * 90);
			/*인 그림 집어넣기*/
		}
		else if (tileType == "o"){
			int[] nextAngle = {1,-1,0,2};			//방향에 따라 포인트 화살표의 각도를 정하기 위한 정보
			tile.spriteName = "Out";
			tile.transform.rotation = Quaternion.Euler (0, 0, nextAngle[PipeTools.ReverseDir(pipeNum)] * 90);
			/*아웃 그림 집어넣기*/
		}
		else if (tileType == "ob") {
			tile.spriteName = "Obstacle";
			plantSymbol.spriteName = "Base";
		}
		else if (tileType == "op") {
			tile.spriteName = "Op";
			SetPipe (pipeNum);
			GameObject tempObject = NGUITools.AddChild (gameObject, GameObject.FindWithTag("PrefabManager").GetComponent<PrefabManager>().ObstaclePipe[pipeNum]);
			SetPipe (tempObject.GetComponent<Pipe>());
			/*장애물 파이프 instanciate */
		}
		else if (tileType == "none"){
			Destroy(GetComponent<Collider>());
			Destroy(tile);
		}
		else{
			tile.spriteName = "Empty_" + cropLevel.ToString ();
            plantSymbol.pivot = UIWidget.Pivot.Bottom;
			plantSymbol.spriteName = "Symbol_" + PipeTools.cropNameMapper[cropType] + "_" + cropLevel.ToString();
            plantSymbol.MakePixelPerfect();
			/*작물 종류에 따른 타일 로직 필요*/
		}
	}

	IEnumerator GrassPopup(){
		Vector3 plantScale = plant.transform.localScale;
		//plant.transform.localScale = new Vector3 (plantScale.x, 0, 0);
		plant.transform.localScale = Vector3.zero;
		TweenScale.Begin (grass.gameObject, 0.25f, Vector3.one * 180);
		TweenScale.Begin (plant.gameObject, 0.3f, plantScale * 3f);

		yield return new WaitForSeconds (0.25f);
		TweenScale.Begin (grass.gameObject, 0.15f, Vector3.one * 126);

		yield return new WaitForSeconds (0.05f);
		TweenScale.Begin (plant.gameObject, 0.1f, plantScale);

	}

	public void InitConnection(){
		straightRandomValue = (pipeNum < 2 ? Random.Range (0, 2) : 0);
		if (tileType != "p") return;
		pipeOnTile.InitConnection();
		if (plantSymbol.spriteName == "Base") {
			SetPlantSprite();
			if (connectedBefore){
				grass.spriteName = "Grass_" + PipeTools.cropNameMapper[cropType] + "_" + pipeNum.ToString () + "_" + straightRandomValue.ToString();
			}
			else{
				grass.spriteName = "Base";
				plant.color = Color.white * 0.5f;
			}
		}
	}

	public int SetConnection(){
		if (tileType != "p" && tileType !="op") return 0;

		int retVal = 0;
		pipeOnTile.SetConnection (tileType == "op"? true : false);

		if (connected && !connectedBefore && tileType == "p") {

			grass.spriteName = "Grass_" + PipeTools.cropNameMapper[cropType] + "_" + pipeNum.ToString () + "_" + straightRandomValue.ToString();
			if (plantSymbol.spriteName != "Base"){
				plantSymbol.spriteName = "Base";
				SetPlantSprite();
				StartCoroutine(GrassPopup());
				/*자란 풀에 애니메이션 효과*/
			}
            plant.color = Color.white;
			retVal = (isCritical ? 3 : 2);
		}
		else if (!connected && connectedBefore && tileType == "p"){
			grass.spriteName = "Base";
			plant.color = Color.white * 0.5f;
			retVal = 1;
		}

		connectedBefore = connected;
		connected = false;

		if (crossConnected && !crossConnectedBefore && tileType == "p") {
			retVal = 4;
		}
		else if (!crossConnected && crossConnectedBefore && tileType == "p"){
			retVal = 1;
		}
		crossConnectedBefore = crossConnected;
		crossConnected = false;

		return retVal;
	}

	void SetPlantSprite(){
		Vector2[] plantPosition = {
						new Vector2 (33, -55),
						new Vector2 (-35, -52),
						new Vector2 (42, -42),
						new Vector2 (-43, -44),
						new Vector2 (-42, -52),
						new Vector2 (40, -9),
						new Vector2 (30, -60),
						new Vector2 (-37, -9),
						new Vector2 (-28, -60),
				};
		int index = pipeNum + (pipeNum < 2? (pipeNum + straightRandomValue) :2);

		plant.spriteName = "Plant_" + PipeTools.cropNameMapper [cropType] + "_" + pipeNum.ToString () + "_" + straightRandomValue.ToString();
		plant.MakePixelPerfect ();
		plant.transform.localPosition = plantPosition [index];
	}

	public void SetPipe(Pipe targetPipe){
        targetPipe.transform.parent = this.transform;
		pipeOnTile = targetPipe;
		targetPipe.transform.localPosition = Vector3.zero;
		targetPipe.transform.localScale = Vector3.one;
        NGUITools.MarkParentAsChanged(gameObject);
        NGUITools.MarkParentAsChanged(targetPipe.gameObject);
		SetPipe (targetPipe.pipeNum);
	}

	public void SetPipe(int pipeNum){
		int[,] pipeinfo = {{0, 1, 4, 4},
			{4, 4, 2, 3},
			{0, 1, 2, 3},
			{4, 3, 0, 4},
			{4, 2, 4, 0},
			{3, 4, 1, 4},
			{2, 4, 4, 1}};		//pipeNum에 따른 outDir 값
		
		this.pipeNum = pipeNum;
		outDir = new int[4];
		System.Buffer.BlockCopy (pipeinfo, pipeNum * 16, outDir, 0, 16);


		if (plantSymbol.spriteName.Contains ("Base")){
			plantSymbol.spriteName = "Base";
		}

	}

	public void DestroyPipe(){
		Destroy (pipeOnTile.gameObject);
	}

	public void DestroyPipe(float time){
		Destroy (pipeOnTile.gameObject, time);
	}

	public UISlider GetSlider(int dir){
		UISlider slider = GetComponentInChildren<UISlider>();
		if (tileType == "i" || tileType == "o") {
			return slider;
		}
		if (pipeNum == 2) {
			if (dir < 2){
				slider = pipeOnTile.transform.Find("WaterFlow_bot").GetComponent<UISlider>();
				if (dir == 1){
					slider.GetComponentInChildren<UISprite>().invert = !slider.GetComponentInChildren<UISprite>().invert;
				}
			}
			else{
				slider = pipeOnTile.transform.Find("WaterFlow_top").GetComponent<UISlider>();
                slider.GetComponentInChildren<UISprite>().depth = 35 - (int)vec.y;
				if (dir == 3)
					slider.GetComponentInChildren<UISprite>().invert = !slider.GetComponentInChildren<UISprite>().invert;
			}

			return slider;
		}
        slider.GetComponentInChildren<UISprite>().depth = 35 - (int)vec.y;
		for (int i = 0; i < 4; i++) {
			if (outDir[i] != 4){
				if (i != dir)
					slider.GetComponentInChildren<UISprite>().invert = !slider.GetComponentInChildren<UISprite>().invert;
				break;
			}
		}
		 
		return slider;
	}

	public bool IsEnterable(float dir){
		return IsEnterable ((int) dir);
	}

	public bool IsEnterable(int dir){
        if (tileType == "o")
            if (dir == 5)
                return true;
            else            
                return false;
		else if (outDir[dir] == 4 || tileType == "s")
			return false;
		return true;
	}

	public int GetOpenDir(){
		for (int i = 0; i < 4; i++) {
			if (outDir[i] != 4)
				return i;
		}
		return -1;
	}

	public Vector3 GetNextTile(float dir){
		return GetNextTile ((int)dir);
	}

	public Vector3 GetNextTile(int dir){
		//이미 해당 타일엔 들어올 수 있었다고 판단
		int pX = (int)vec.x;
		int pY = (int)vec.y;

		int[] nextTileX = {-1, 1, 0, 0};
		int[] nextTileY = {0, 0, 1, -1};		//방향에 따라 다음 타일을 찾기 위한 정보

		if (tileType == "i"){
			return new Vector3(outVec[0].x, outVec[0].y, 5);
		}
		else if (tileType == "o"){
			return new Vector3(pX + nextTileX[pipeNum], pY + nextTileY[pipeNum], pipeNum);
		}
		else if (outVec.Length != 0){
			for (int i = 0; i < outVec.Length; i++){
				if ((int)(outVec[i].x) == outDir[dir]){
					return (GameObject.FindWithTag("GameController").GetComponent<StageMainController>().GetTileVecForArrowInOut(pX, pY, outDir[dir]));
				}
			}
		}

		dir = outDir [dir];
		pX += nextTileX [dir];
		pY += nextTileY [dir];

		return new Vector3(pX, pY, dir);
	}
}