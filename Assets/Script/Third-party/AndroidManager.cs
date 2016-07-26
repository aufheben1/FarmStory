using UnityEngine;
using System.Collections;
using System;

public class AndroidManager : MonoBehaviour {
	public string androidFunctionName;
	public string androidParam;
	public bool saveParam;

	public GameObject targetObj;
	public string functionName;
	public bool broadcast;
	public string param;

	private bool clicked = false;

	void Start(){
		if (targetObj == null) targetObj = this.gameObject;
	}

	void SendMsgToAndroid(){
		if (clicked) {
			Debug.Log ("Already processing Communication with android. Please Wait");
			return;
		}

		if (Application.platform != RuntimePlatform.Android) {
			Debug.Log ("You're not using Android Device!");
			return;
		}

		
		clicked = true;

		#if UNITY_ANDROID
		try{
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
				using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")){
					if (androidParam == "" || androidParam == null){
						jo.Call(androidFunctionName);
					}
					else{
						if (saveParam)
							this.param = androidParam;
						jo.Call(androidFunctionName, androidParam);
					}
				}
			}
		}
		catch(Exception e){
			Debug.Log(e.StackTrace);
		}
		#endif
	}
	
	void ResultFromAndroid(string result){
		clicked = false;
		if (result == "fail") {
			if (broadcast)
				targetObj.BroadcastMessage ("PurchaseFail", SendMessageOptions.DontRequireReceiver);
			else
				targetObj.SendMessage ("PurchaseFail", SendMessageOptions.DontRequireReceiver);
			return;
		}
		if (result == "success") {
			if (param == "" || param == null){
				if (broadcast){
					targetObj.BroadcastMessage (functionName, SendMessageOptions.DontRequireReceiver);
				}
				else{
					targetObj.SendMessage (functionName, SendMessageOptions.DontRequireReceiver);
				}
			}
			else{
				if (broadcast){
					targetObj.BroadcastMessage (functionName, param, SendMessageOptions.DontRequireReceiver);
				}
				else{
					targetObj.SendMessage (functionName, param, SendMessageOptions.DontRequireReceiver);	
				}
			}
			return;
		}
	}

}

