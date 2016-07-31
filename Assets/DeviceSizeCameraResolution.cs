using UnityEngine;
using System.Collections;

public class DeviceSizeCameraResolution : MonoBehaviour {
    Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        cam.orthographicSize *= 720 / (float)Screen.height;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
