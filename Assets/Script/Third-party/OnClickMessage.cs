using UnityEngine;
using System.Collections;

public class OnClickMessage : MonoBehaviour {

	public GameObject target;
	public string functionName;
	public string param;

	void Start () {
		if (target == null)
			target = this.gameObject;
	}

	void OnMouseUpAsButton(){
		Debug.Log ("Good");
		if (param == null || param == "") {
			target.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
		}
		else{
			target.SendMessage(functionName, param, SendMessageOptions.DontRequireReceiver);
		}
	}
}
