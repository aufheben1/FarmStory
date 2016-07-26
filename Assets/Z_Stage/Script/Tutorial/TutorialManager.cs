using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public int[] setUpPipe;
	public GameObject touchAnimation;
	public GameObject touchHighlight;
	public GameObject tilePanel;
    public GameObject textList;

    public GameObject touchSp;
    public GameObject touchAgain;
    public GameObject haceFun;




    [System.Serializable]
    public class State{
        public GameObject[] cover;
        public UIWidget.Pivot pivot;
        public Vector2 listenerPosition;
        public Vector2 listenerScale;
    }

    public State[] state;

    public BoxCollider clickListener;

    private GameObject gameController;
    private float scaleSize;
    private StageMainController controller;

    public GameObject touch;

    
    private int tutorialState = 0;
    private bool routineRunning = false;

    void InitController()
    {
        gameController = GameObject.FindWithTag("GameController");
        controller = gameController.GetComponent<StageMainController>();
        gameController.SendMessage("SetUpPipe", setUpPipe, SendMessageOptions.DontRequireReceiver);
    }

    void SetTutorial(float scaleSize)
    {
        this.scaleSize = scaleSize;
		tilePanel.transform.localScale = Vector3.one * scaleSize;
        textList.transform.localScale = Vector3.one / scaleSize;

		MoveToNextRoutine ();
    }

    IEnumerator StartNextRoutine()
    {
        if (routineRunning) yield break;
        routineRunning = true;

        GameObject targetPipe;

        switch (tutorialState)
        {
        case 0:
			controller.SetPreview (5,3);
			SetTouchAnim (true);
            touchSp.SetActive(true);
			break;

        case 1 :
			SetTouchAnim (false);
            touchSp.SetActive(false);
			yield return new WaitForSeconds(0.5f);
	        targetPipe = controller.nextPipe[0].gameObject;
	        targetPipe.transform.parent = controller.tile[5,3].gameObject.transform;
	        targetPipe.transform.localScale = Vector2.one;
            targetPipe.transform.Find("Sprite_Pipe").GetComponent<UISprite>().depth = 25;
	        TweenPosition.Begin(targetPipe, 1, Vector3.zero);
	        yield return new WaitForSeconds(1f);
            targetPipe.transform.Find("Sprite_Pipe").GetComponent<UISprite>().depth = 20;
	        gameController.SendMessage("DefaultTouch", controller.tile[5, 3], SendMessageOptions.DontRequireReceiver);
	        yield return new WaitForSeconds(1f);
			MoveToNextRoutine(0.2f);
	        break;

		case 2 :
			SetTouchAnim (true);
            touchSp.SetActive(true);
			controller.SetPreview (6,4);
			break;

		case 3 :
			SetTouchAnim (false);
            touchSp.SetActive(false);
			yield return new WaitForSeconds(0.5f);
			targetPipe = controller.nextPipe[0].gameObject;
			targetPipe.transform.parent = controller.tile[6,4].gameObject.transform;
			targetPipe.transform.localScale = Vector2.one;
            targetPipe.transform.Find("Sprite_Pipe").GetComponent<UISprite>().depth = 25;
			TweenPosition.Begin(targetPipe, 1, Vector3.zero);
			yield return new WaitForSeconds(1f);
            targetPipe.transform.Find("Sprite_Pipe").GetComponent<UISprite>().depth = 20;
			gameController.SendMessage("DefaultTouch", controller.tile[6, 4], SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(1f);
			MoveToNextRoutine(0.2f);
			break;

		case 4 :
			SetTouchAnim (true);
            touchAgain.SetActive(true);
			controller.SetPreview (6,4);
			break;
		
		case 5 :
			SetTouchAnim (false);
            touchAgain.SetActive(false);
			yield return new WaitForSeconds(0.5f);
			targetPipe = controller.nextPipe[0].gameObject;
			targetPipe.transform.parent = controller.tile[6,4].gameObject.transform;
			targetPipe.transform.localScale = Vector2.one;
            targetPipe.transform.Find("Sprite_Pipe").GetComponent<UISprite>().depth = 25;
			TweenPosition.Begin(targetPipe, 1, Vector3.zero);
			yield return new WaitForSeconds(1f);
            targetPipe.transform.Find("Sprite_Pipe").GetComponent<UISprite>().depth = 20;
			gameController.SendMessage("DefaultTouch", controller.tile[6, 4], SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(1f);
			MoveToNextRoutine(0.2f);
			break;

		case 6 :
			controller.SetPreview (4,4);
            haceFun.SetActive(true);
            Destroy(haceFun, 1.5f);
            yield return new WaitForSeconds(1.5f);
			EndTutorial();
			break;
        }

        SetCoverBox(tutorialState);
        routineRunning = false;
        tutorialState++;
    }

    void SetCoverBox(int stateNum)
    {
        if (stateNum > 0) {
			foreach (GameObject gObj in state[stateNum - 1].cover) {
				gObj.SetActive (false);
			}
		}
		foreach (GameObject gObj in state[stateNum].cover) {
			gObj.SetActive (true);
		}

		clickListener.transform.localScale = state [stateNum].listenerScale;
		clickListener.transform.localPosition = new Vector3 (state [stateNum].listenerPosition.x, state [stateNum].listenerPosition.y, -1);
    }

	void SetTouchAnim(bool onOff){
		touchAnimation.SetActive (onOff);
		touchHighlight.SetActive (onOff);
		if (onOff) {
			touchAnimation.transform.localPosition = state[tutorialState].listenerPosition;
			touchHighlight.transform.localPosition = state[tutorialState].listenerPosition;
		}
	}

	void MoveToNextRoutine(){
		StartCoroutine (StartNextRoutine ());
	}
	
	void MoveToNextRoutine(float delay){
		StartCoroutine (ReserveRoutine (delay));
	}
	
	IEnumerator ReserveRoutine(float delay){
		yield return new WaitForSeconds(delay);
		StartCoroutine(StartNextRoutine());
	}

	void EndTutorial(){
		SetGameResume ();
		controller.tutorialStage = false;
		Destroy (gameObject);
	}

    void OnSkipButtonClick()
    {
		EndTutorial ();
        //clickListener.gameObject.SetActive(false);
    }

    void SetGameStop()
    {
        gameController.SendMessage("PauseGame", SendMessageOptions.DontRequireReceiver);
    }

    void SetGameResume()
    {
        gameController.SendMessage("ResumeGame", SendMessageOptions.DontRequireReceiver);
    }
}
