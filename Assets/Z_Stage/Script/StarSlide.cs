using UnityEngine;
using System.Collections;

public class StarSlide : MonoBehaviour {
    public GameObject twinkle;
	private float stTime = 0;
	private float endTime = 0.5f;
	private UISlider slider;

	// Use this for initialization
	void Start () {
		slider = GetComponent<UISlider> ();
        Instantiate(twinkle, transform.position, transform.rotation);
	}	
	
	// Update is called once per frame
	void Update () {
		stTime += Time.deltaTime;
		slider.sliderValue = stTime / endTime;
		if (stTime > endTime) {
            
			Destroy (this);
		}
	}
}
