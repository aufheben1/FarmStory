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
        //다른 창이 켜져있을 경우 동작하지 않음
        if (GetComponent<MapController> ().isWindowOn)	return;


		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved) {
            //1터치 - 이동 속도
            velocity = Input.GetTouch (0).deltaPosition;
		} else if (Input.touchCount == 2) {
            //2터치 - 확대 축소 속도
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
        //이동 속도에 맞게 패널 이동 + 속도 감소 by 90%
        panel.transform.Translate (velocity.x * dragSpeed, velocity.y * dragSpeed, 0);
		velocity *= 0.9f;

        //경계를 벗어났을 경우 clamp
        panel.transform.localPosition = new Vector3(
            Mathf.Clamp(panel.transform.localPosition.x, -(1920 - 650 * currentResolution), (1920 - 650 * currentResolution)),
            Mathf.Clamp(panel.transform.localPosition.y, -(3115 - 400 * currentResolution), (3115 - 400 * currentResolution)),
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
