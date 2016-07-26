using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour {
	public string itemCode;
	public UILabel itemCount;
	public GameObject saleTag;
	public GameObject gameController;
	public GameObject androidManager;

	private int count;

	void Start(){
		PlayerPrefs.Save ();
		if (!PlayerPrefs.HasKey (itemCode)) {
			PlayerPrefs.SetInt (itemCode, 3);
			PlayerPrefs.Save ();
			SetItemCountLabel ();
		}
		else{
			SetItemCountLabel ();
		}
		this.count = PlayerPrefs.GetInt (itemCode);
	}

	void OnButtonClick(){
		if (count > 0)
			gameController.SendMessage ("On" + itemCode + "ButtonClick", SendMessageOptions.DontRequireReceiver);
		else{
			gameController.SendMessage ("PauseGame", SendMessageOptions.DontRequireReceiver);
			androidManager.GetComponent<AndroidManager>().androidParam = itemCode;
			androidManager.SendMessage("SendMsgToAndroid", SendMessageOptions.DontRequireReceiver);
		}
	}

	void SetItemCountLabel(){
		count = PlayerPrefs.GetInt (itemCode);
		if (count == 0){
			itemCount.text = "";
			saleTag.SetActive(true);
		}
		else{
			itemCount.text = count.ToString();
			saleTag.SetActive(false);
		}
	}
	
	void ItemBought(string code){
		if (itemCode != code) return;
		PlayerPrefs.SetInt (itemCode, 5);
		PlayerPrefs.Save ();
		SetItemCountLabel();
		gameController.SendMessage ("ResumeGame", SendMessageOptions.DontRequireReceiver);
	}

	void PurchaseFail(){
		gameController.SendMessage ("ResumeGame", SendMessageOptions.DontRequireReceiver);
	}

	void ItemUsed(){
		PlayerPrefs.SetInt (itemCode, --count);
		PlayerPrefs.Save ();
		SetItemCountLabel();
	}

}
