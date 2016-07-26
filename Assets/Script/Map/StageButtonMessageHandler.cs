using UnityEngine;
using System.Collections;

public class StageButtonMessageHandler : MonoBehaviour {

	void OnButtonClick(){
		int stageNum = int.Parse (GetComponentInChildren<UILabel> ().text);
		GameObject.FindWithTag ("GameController").SendMessage ("RequireWindowOpen", stageNum, SendMessageOptions.DontRequireReceiver);
	}
}
