using UnityEngine;
using System.Collections;

public class MapTraveler : MonoBehaviour {
	public float dragSpeed = 0.1f;
	public float zoomSpeed = 0.1f;

	public Camera mainCamera;
	public GameObject panel;

	private Vector3 dragorigin;
	private Vector2 velocity;
	
	private float minResolution = 0.55f;
	private float maxResolution = 1.1f;

	[HideInInspector]
	public float currentResolution;

	void Start(){
		currentResolution = mainCamera.orthographicSize;
	}


	void Update () {
		if (GetComponent<MapController> ().isWindowOn)	return;
		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved) {
			velocity = Input.GetTouch (0).deltaPosition;
		} else if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			currentResolution +=  deltaMagnitudeDiff * zoomSpeed / currentResolution * 1.125f;
			currentResolution = Mathf.Clamp (currentResolution, minResolution * 0.9f, maxResolution * 1.1f);

			mainCamera.orthographicSize = currentResolution;
		}
		panel.transform.Translate (velocity.x * dragSpeed, velocity.y * dragSpeed, 0);
		velocity *= 0.9f;


		panel.transform.position = new Vector3(
			Mathf.Clamp (panel.transform.position.x, -(1920 - 960 * currentResolution)/540f ,(1920 - 960 * currentResolution)/540f),
			Mathf.Clamp (panel.transform.position.y, -(3115 - 540 * currentResolution)/540f,(3115 - 540 * currentResolution)/540f),
			0);

		if (currentResolution > maxResolution){
			currentResolution -= 0.01f;
			mainCamera.orthographicSize -=  0.01f;
		}
		else if (currentResolution < minResolution){
			currentResolution += 0.005f;
			mainCamera.orthographicSize += 0.005f;
		}

		//if (panel.transform.position.x > 
	}
}
