using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageSetter : MonoBehaviour {
	public GameObject nguiRoot;
	public Transform tilePanel;

	void Start(){
		StartCoroutine(LoadStage ());
	}

	IEnumerator LoadStage(){
		if (GameObject.Find ("Stage"))
			Destroy (GameObject.Find ("Stage"));

		GameObject stagePref = (GameObject) Resources.Load ("Stage/Stage", typeof(GameObject));
		Instantiate (stagePref);

		TileMaker[,] setupTile = new TileMaker[10, 8];
		Tile[,] tile = new Tile[10,8];

		yield return new WaitForSeconds (1);

		StageMainController stageMainController = GameObject.FindWithTag ("GameController").GetComponent<StageMainController> ();
		//stageMainController.gameObject.AddComponent<StageInfo>(GetComponent<StageInfo>());
		CopyComponent (GetComponent<StageInfo> (), stageMainController.gameObject);
		stageMainController.LoadComponent ();


		List<int> maxRandVal = new List<int>();
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 8; j++){
				setupTile[i,j] = tilePanel.Find ("Tile" + i.ToString() + j.ToString()).GetComponent<TileMaker>();
				if ((setupTile[i,j].tileType == 0 || setupTile[i,j].tileType == 2 || setupTile[i,j].tileType == 3 )&& !maxRandVal.Contains ((int)setupTile[i,j].cropRange.y))
					maxRandVal.Add ((int)setupTile[i,j].cropRange.y);
			}
		}
		maxRandVal.Sort ();

		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 8; j++){
				tile[i,j] = stageMainController.objectManager.tilePanel.transform.Find ("Tile" + i.ToString() + j.ToString()).GetComponent<Tile>();

				switch (setupTile[i,j].tileType) {
				case 0 :
					//빈 타일
					tile[i,j].tileType = "em";

					List<Vector2> outList = new List<Vector2>();
					if (setupTile[i,j].leftOut){
						outList.Add (new Vector2(0,0));
					}
					if (setupTile[i,j].rightOut){
						outList.Add (new Vector2(1,0));
					}
					if (setupTile[i,j].upOut){
						outList.Add (new Vector2(2,0));
					}
					if (setupTile[i,j].downOut){
						outList.Add (new Vector2(3,0));
					}
					if (outList.Count > 0){
						tile[i,j].outVec = new Vector2[outList.Count];
						outList.CopyTo(tile[i,j].outVec);
					}
					/*인아웃 화살표에 대한 로직!*/
					break;
				case 1 :
					//타일이 없음!
					tile[i,j].tileType = "none";
					break;
				case 2 :
					//장애물
					tile[i,j].tileType = "ob";
					break;
				case 3 :
					//장애물 파이프
					tile[i,j].tileType = "op";
					tile[i,j].pipeNum = setupTile[i,j].pipeNum;
					break;
				case 4 :
					tile[i,j].tileType = "s";
					tile[i,j].pipeNum = setupTile[i,j].dir;
					//스타트
					break;
				case 5 :
					tile[i,j].tileType = "e";
					tile[i,j].pipeNum = setupTile[i,j].dir;
					//엔드
					break;
				case 6 :
					tile[i,j].tileType = "i";
					tile[i,j].pipeNum = PipeTools.ReverseDir(setupTile[i,j].dir);
					tile[i,j].outVec = new Vector2[1];
					tile[i,j].outVec[0] = new Vector2(setupTile[i,j].outVec.x, setupTile[i,j].outVec.y);
					//인
					break;
				case 7 :
					tile[i,j].tileType = "o";
					tile[i,j].pipeNum = setupTile[i,j].dir;
					//아웃
					break;
				}
				if ((int)setupTile[i,j].cropRange.y == maxRandVal[0]){
					tile[i,j].cropLevel = 0;
				}
				else if ((int)setupTile[i,j].cropRange.y == maxRandVal[1]){
					tile[i,j].cropLevel = 1;
				} else if ((int)setupTile[i,j].cropRange.y == maxRandVal[2]){
					tile[i,j].cropLevel = 2;
				}
				tile[i,j].cropType = setupTile[i,j].cropType;
				tile[i,j].cropNum = Random.Range ((int)setupTile[i,j].cropRange.x, (int)setupTile[i,j].cropRange.y + 1);
				if (tile[i,j].cropNum == (int)setupTile[i,j].cropRange.y){
					tile[i,j].isCritical = true;
				}
			}
		}

		stageMainController.StartStage ();
		Destroy (nguiRoot);
		Destroy (gameObject);


	}

	Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}
}
