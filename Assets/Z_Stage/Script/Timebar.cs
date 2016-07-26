using UnityEngine;
using System.Collections;

public class Timebar : MonoBehaviour {
    private UISlider slider;
    private bool sliding = false;
    private float slideTime;
    private float slidedTime = 0;

	void OnEnable() {
        slider = GetComponent<UISlider>();
	}

    void StartSliding(float time)
    {
        slideTime = time;
        sliding = true;
    }

	void Update () {
        if (!sliding) return;
        slidedTime += Time.deltaTime;
        slider.sliderValue = 1 - (slidedTime / slideTime);
        if (slidedTime > slideTime)
        {
            sliding = false;
        }
	}
}
