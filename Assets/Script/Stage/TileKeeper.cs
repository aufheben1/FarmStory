using UnityEngine;
using System.Collections;

public class TileKeeper : MonoBehaviour {
	public string tileType;
	public int pipeNum;				
	public bool  isChangeable;	
	public int[] outDir;
	public Vector2 vec;
	public Vector3[] specialOut;

	public bool isLinked = false;
	public bool isLinkedBefore = false;
	public bool isClicked = false;

	GameObject gameManager;

	void Start(){
		gameManager = GameObject.FindWithTag ("GameController");
	}

	public void SetGameManager(GameObject obj){
		gameManager = obj;
	}

	void OnMouseDown(){
		gameManager.SendMessage ("OnTileMouseDown", vec, SendMessageOptions.DontRequireReceiver);
		/*

		if (tileType == "s" || tileType == "e") {
			gameManager.SendMessage ("FlowMult", true, SendMessageOptions.DontRequireReceiver);
		}

		if (!isChangeable) return;

		gameManager.SendMessage("SetBorder", vec, SendMessageOptions.DontRequireReceiver);
		*/
	}

	/*
	void OnMouseEnter(){
		GameObject gameManager = GameObject.FindWithTag("GameController");
		if (!isChangeable) return;
		//transform.localScale *= 0.95f;
		gameManager.SendMessage("SetBorder", vec, SendMessageOptions.DontRequireReceiver);
	}
	*/
	void OnMouseUpAsButton(){
		gameManager.SendMessage ("OnTileMouseUpAsButton", vec, SendMessageOptions.DontRequireReceiver);
		/*
		if (tileType == "s" || tileType == "e") 
				SpeedDiv ();
		else {
				ClearBorder ();
				gameManager.SendMessage ("TileTouched", vec, SendMessageOptions.DontRequireReceiver);
		}
		*/
	}

	void OnMouseExit(){
		gameManager.SendMessage ("OnTileMouseExit", vec, SendMessageOptions.DontRequireReceiver);
		/*
		if (tileType == "s" || tileType == "e")
			SpeedDiv ();
		ClearBorder ();
		*/
	}

	void SpeedDiv(){
		gameManager.SendMessage ("FlowMult", false, SendMessageOptions.DontRequireReceiver);
	}

	void ClearBorder(){
		gameManager.SendMessage ("ClearBorder",vec, SendMessageOptions.DontRequireReceiver);
	}

	
	public void  DestroySelf (){
		Destroy(gameObject);
	}
}
