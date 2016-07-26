using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour {
	public int pipeNum;
	public bool[] connected = {false};
	public bool[] connectedBefore = {false};

	public UISprite[] pipeSprite;
	public UISprite[] waterJol;

	void Start(){
		if (pipeNum == 2) {
			connected = new bool[2]{false, false};
			connectedBefore = new bool[2]{false, false};
		}
		else{
			connected = new bool[1]{false};
			connectedBefore = new bool[1]{false};
		}
	}

	public void SetConnection(bool op){
		for (int index = 0; index < pipeSprite.Length; index++){
			if (connected[index] && ! connectedBefore[index]) {
				if(!op){
					pipeSprite[index].color = Color.white;
				}
				TweenAlpha.Begin (waterJol[index].gameObject, 0.4f, 1);
			}
			else if (!connected[index] && connectedBefore[index]){
				if (!op){
					pipeSprite[index].color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
				}
				TweenAlpha.Begin (waterJol[index].gameObject, 0.4f, 0);
			}
			connectedBefore[index] = connected[index];
			connected [index] = false;
		}
	}

	public void InitConnection(){
		for (int i = 0; i < pipeSprite.Length; i++) {
			//pipeSprite[i].color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
			pipeSprite[i].color = Color.white * 0.7f;
			connectedBefore[i] = false;
		}
	}

	public void DestroySelf(){
		Destroy (gameObject);
	}
}
