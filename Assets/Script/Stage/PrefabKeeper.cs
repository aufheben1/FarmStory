using UnityEngine;
using System.Collections;

public class PrefabKeeper : MonoBehaviour {
	public GameObject explosion;
	public GameObject emptyTile;
	public GameObject waterExplosion;
	public GameObject pointArrow;
	public GameObject pointError;
	public GameObject flowManager;
	public GameObject flashManager;
	public GameObject flashManager2; 
	public GameObject crossFlash;
	public GameObject crossCount;
	public GameObject inoutArrow;
	public GameObject notClear;

	public GameObject itemBandage;
	public GameObject itemHammer;
	public GameObject itemObstacle;

	public GUIText pipeConnected;
	public GameObject[] grass;

	public GameObject textImage;

	public GameObject crossTileBorder;
	public GameObject alert;
	public GameObject[] fly;

	public GameObject pipes;

	public GameObject[] preview;
	public GameObject[] startAnim;
	public GameObject[] endAnim;
	public GameObject[] inAnim;
	public GameObject[] outAnim;

	public GameObject[] pipe;
	public GameObject[] nguiPipe;
	public GameObject[] crack;
	public GameObject[] flashPipe;
	public GameObject flashStEnd;
	int MAX_NUMBER_OF_INOUT = 3;
	
	public void Initialize(string color){
		explosion = (GameObject)Resources.Load ("Effect/Explosion/Explosion", typeof(GameObject));
		emptyTile = (GameObject)Resources.Load ("Tile/Empty/Empty", typeof(GameObject));
		waterExplosion = (GameObject)Resources.Load ("Effect/WaterExplosion/WaterEx", typeof(GameObject));
		pointArrow = (GameObject)Resources.Load ("Effect/Point/Arrow", typeof(GameObject));
		pointError = (GameObject)Resources.Load ("Effect/Point/Error", typeof(GameObject));
		crossFlash = (GameObject)Resources.Load ("Effect/CrossFlash/Flash", typeof(GameObject));
		flowManager = (GameObject)Resources.Load ("Effect/WaterFlow/FlowManager", typeof(GameObject));
		flashManager = (GameObject)Resources.Load ("Effect/WaterFlow/FlashManager", typeof(GameObject));
		flashManager2 = (GameObject)Resources.Load ("Effect/WaterFlow/FlashManager2", typeof(GameObject));
		crossCount = (GameObject)Resources.Load ("Effect/CrossCount/CrossCount", typeof(GameObject));
		pipeConnected = (GUIText)Resources.Load ("Effect/PipeConnected/PipeConnected", typeof(GUIText));
		alert = (GameObject)Resources.Load ("Effect/Alert/newAlert", typeof(GameObject));
		notClear = (GameObject)Resources.Load ("Effect/NotClear/NotClear", typeof(GameObject));
		inoutArrow = (GameObject) Resources.Load ("Arrow/Arrow", typeof(GameObject)); ;
				
		itemBandage = (GameObject)Resources.Load ("Board/Item/Bandage", typeof(GameObject));
		itemHammer = (GameObject)Resources.Load ("Board/Item/Hammer", typeof(GameObject));
		itemObstacle = (GameObject)Resources.Load ("Board/Item/Obstacle", typeof(GameObject));
		
		crossTileBorder = (GameObject)Resources.Load ("Effect/Score/TileBorder", typeof(GameObject));
		
		pipes = (GameObject)Resources.Load ("Pipes", typeof(GameObject));
		textImage = (GameObject)Resources.Load ("Effect/TextImage/TextImage", typeof(GameObject));

		preview = new GameObject[7];
		crack = new GameObject[7];
		flashPipe = new GameObject[7];
		grass = new GameObject[7];
		fly = new GameObject[7];
		startAnim = new GameObject[4];
		endAnim = new GameObject[4];
		inAnim = new GameObject[MAX_NUMBER_OF_INOUT * 4];
		outAnim = new GameObject[MAX_NUMBER_OF_INOUT * 4];
		
		for (int i = 0 ; i < 4 ; i++){
			startAnim[i] = (GameObject)Resources.Load ("Effect/StartEnd/Start" + i, typeof(GameObject));
			endAnim[i] = (GameObject)Resources.Load ("Effect/StartEnd/End" + i, typeof(GameObject));
		}
		
		for (int i = 0; i < MAX_NUMBER_OF_INOUT; i++) {
			for (int j = 0; j < 4 ; j++){
				inAnim[i*4 + j] = (GameObject)Resources.Load ("Effect/InOut/In" + (i+1) + (j), typeof(GameObject));
				outAnim[i*4 + j] = (GameObject)Resources.Load ("Effect/InOut/Out" + (i+1) + (j), typeof(GameObject));
			}
		}
		
		for (int i = 0; i < 7; i++) {
			preview[i] = (GameObject)Resources.Load ("Effect/Preview/" + i, typeof(GameObject));
			crack[i] = (GameObject)Resources.Load ("Effect/Crack/Crack" + i, typeof(GameObject));
			flashPipe[i] = (GameObject)Resources.Load ("Effect/FlashPipe/FlashPipe" + i, typeof(GameObject));
			grass[i] = (GameObject)Resources.Load ("Tile/Grass/Grass" + i, typeof(GameObject));
			fly[i] = (GameObject) Resources.Load ("Interface/FlyFly/Fly" + i, typeof(GameObject)); ;
		}
		flashStEnd = (GameObject)Resources.Load ("Effect/FlashPipe/FlashStEnd", typeof(GameObject));

		pipe = new GameObject[7];
		nguiPipe = new GameObject[7];
		for (int i = 0; i < 7; i++) {
			pipe[i] = (GameObject)Resources.Load ("Pipe/" + color + "/" + i, typeof(GameObject));
			nguiPipe[i] = (GameObject)Resources.Load ("Pipe/NGUI/Pipe" + i, typeof(GameObject));
		}
	}


}
