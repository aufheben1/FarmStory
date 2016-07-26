using UnityEngine;
using System.Collections;

public class AndroidBackButtonManager : MonoBehaviour {

	bool isAndroid = false;
	bool timer = false;
	float counter = 2;

	void Start () {
		if (Application.platform == RuntimePlatform.Android) {
			isAndroid = true;
		}
	}

	void Update () {
		if (isAndroid && !timer) {
			if (Input.GetKey(KeyCode.Escape)){
				timer = true;
				counter = 1;
				switch(Application.loadedLevelName){
				case "Map" :
					GameObject temp = GameObject.FindWithTag("GameController");
					if (temp != null){
                        temp.SendMessage("OnBackButtonClick");
					}
					break;
				case "Title" :
					Application.Quit ();
					break;
				default :
                    GameObject temp2 = GameObject.FindWithTag("GameController");
                    if (temp2 != null)
                    {
                        temp2.SendMessage("OnBackButtonClick");
                    }
					break;
				}
			}
		}
		if (isAndroid && timer) {
			counter -= Time.deltaTime;
			if (counter <= 0)
				timer = false;
		}
	}
}
