using UnityEngine;
using System.Collections;

public class SecondFollow : MonoBehaviour {
	public GameController gameController;

	private float maxTime = 40f;
	void Update(){
		GetComponent<UILabel> ().text = (40f - gameController.timeCounter).ToString ("n2") + "s";
		float pX = -583 + 1185 * (1 - gameController.timeCounter / maxTime);
		transform.localPosition = new Vector3 (pX, 90, 0);

	}
}
