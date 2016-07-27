using UnityEngine;
using System.Collections;

public class SetOptionEnable : MonoBehaviour {

	public GameObject stopSign;
	public string optionName;
	
	void Start () {
		SetStopsign ();
    }

	void SetStopsign(){
        if (PlayerPrefs.GetInt(optionName) == 0)
        {
            stopSign.SetActive(true);
        }
        else
            stopSign.SetActive(false);
	}
}
