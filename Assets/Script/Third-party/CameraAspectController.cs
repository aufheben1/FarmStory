using UnityEngine;
using System.Collections;

public class CameraAspectController : MonoBehaviour {
	public enum size{enlarge, reduce};
	public size sizehandle;
	
	void Awake() {
		float baseScreenAspect = (float)16 / 9;
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float sizeHandler = (sizehandle == size.enlarge ) ? baseScreenAspect / screenAspect  : screenAspect / baseScreenAspect;

		if (camera != null) {
			camera.orthographicSize /=  sizeHandler;
		}
		else{
			transform.localScale *= sizeHandler;
		} 

		Destroy (this);
	}
}
