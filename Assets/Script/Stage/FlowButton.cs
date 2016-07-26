using UnityEngine;
using System.Collections;

public class FlowButton : MonoBehaviour {
	public UISprite buttonSprite;
	public Spin handSpin;


	private string activeButtonImage = "button_stop";
	private string passiveButtonImage = "button_start";

	void SetButtonStateStop(){
		buttonSprite.spriteName = passiveButtonImage;
		handSpin.rotationsPerSecond = new Vector3 (0, 0, 0);
	}

	void SetButtonStatePlay(){
		buttonSprite.spriteName = passiveButtonImage;
		handSpin.rotationsPerSecond = new Vector3 (0, 0, -0.15f);
	}

	void SetButtonStateFast(){
		buttonSprite.spriteName = activeButtonImage;
		handSpin.rotationsPerSecond = new Vector3 (0, 0, -1);
	}

}
