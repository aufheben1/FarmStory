using UnityEngine;
using System.Collections;

public class TutorialController2 : MonoBehaviour {
	public int[] setUpPipe;
	
	[System.Serializable]
	public class Cover{
		public GameObject coverBox;
		public string tag;
	}
	
	[System.Serializable]
	public class StateCover{
		public Cover[] coverBox;
	}
	
	public Collider[] collider;
	
	public StateCover[] stateCover;
	public GameObject centerCover;
	public GameObject leftCover;
	public GameObject rightCover;
	
	public GameObject clickCover;
	public BoxCollider clickListener;
	
	public GameObject gameController;
	
	public GameObject touch;
	public GameObject missionHighlight;
	private GameObject tempTouch;
	private int tutorialState = 0;
	private bool routineRunning = false;
	
	void SetTutorial() {
		leftCover.SetActive (true);
		rightCover.SetActive (true);
		centerCover.SetActive (true);
		foreach (Collider col in collider) {
			col.enabled = false;
		}
		StartCoroutine (StartNextRoutine ());
	}
	
	void OnSkipButtonClick(){
		if (tutorialState > 0)
			ChangeCoverBox (stateCover [tutorialState - 1], false);
		leftCover.SetActive (false);
		rightCover.SetActive (false);
		centerCover.SetActive (false);
		foreach (Collider col in collider) {
			col.enabled = true;
		}
		clickCover.SetActive (false);
		clickListener.gameObject.SetActive (false);
	}
	
	void Start(){
		gameObject.SendMessage ("SetUpPipe", setUpPipe, SendMessageOptions.DontRequireReceiver);
		collider [0].enabled = false;
	}
	
	void SetClickListener (float pX, float pY, float size, Transform parent){
		clickListener.transform.parent = parent;
		clickListener.transform.localPosition = new Vector3 (pX, pY, 0);
		clickListener.size = Vector3.one * size;
	}
	void SetClickListenerTile(int tX, int tY){
		clickListener.transform.parent = GameObject.Find ("Cover").transform;
		clickListener.gameObject.layer = 9;
		clickListener.transform.localPosition = GetCoordinate (tX, tY);
		clickListener.size = Vector3.one;
		clickListener.transform.localScale = Vector3.one * 1.2f;
	}
	
	Vector3 GetCoordinate(int tX, int tY){
		float tileSize = GetComponent<GameController> ().GetTileSize();
		Vector3 tileBoardPosition = GetComponent<GameController> ().GetTileBoardPosition();
		return new Vector3 ((-4.5f + tX) * tileSize + tileBoardPosition.x, (-3.5f + tY) * tileSize + tileBoardPosition.y, 0);
	}
	
	void SetGameStop(){
		gameController.SendMessage ("PauseGame", SendMessageOptions.DontRequireReceiver);
	}
	
	void SetGameResume(){
		gameController.SendMessage ("ResumeGame", SendMessageOptions.DontRequireReceiver);
	}
	
	
	void ChangeCoverBox(StateCover cover, bool onOff){
		foreach(var obj in cover.coverBox as Cover[]){
			obj.coverBox.SetActive(onOff);
			switch(obj.tag){
			case "c" :
				centerCover.SetActive(!onOff);
				break;
			case "l" :
				leftCover.SetActive(!onOff);
				break;
			case "r" :
				rightCover.SetActive(!onOff);
				break;
			}
		}
	}
	
	IEnumerator StartNextRoutine(){
		if (routineRunning) yield break;
		routineRunning = true;
		
		if (tutorialState > 0)
			ChangeCoverBox (stateCover [tutorialState - 1], false);
		
		ChangeCoverBox (stateCover [tutorialState], true);
		
		switch (tutorialState) {
		case 0 :
			SetClickListenerTile (3,4);
			tempTouch = (GameObject) Instantiate(touch, GetCoordinate (3,4) + new Vector3(0,0,-15), transform.rotation);
			Destroy(GameObject.Find ("Arrow(Clone)"));
			break;
		case 1 :
			Destroy (tempTouch);
			gameController.SendMessage ("OnTileMouseUpAsButton", new Vector2(3,4), SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(0.2f);
			missionHighlight.SetActive(true);
			Destroy(GameObject.Find ("Arrow(Clone)"));
			yield return new WaitForSeconds(2f);
			Destroy (missionHighlight);
			OnSkipButtonClick();
			break;
		}
		
		routineRunning = false;
		tutorialState++;
	}
	

}
