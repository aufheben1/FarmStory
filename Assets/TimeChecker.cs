using UnityEngine;
using System.Collections;

public class TimeChecker : MonoBehaviour {

    public UILabel targetLabel;
    private UILabel thisLabel;

	void Start () {
        thisLabel = GetComponent<UILabel>();
	
	}
	
	void Update () {
        thisLabel.text = targetLabel.text;	
	}
}
