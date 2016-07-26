using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

	[System.Serializable]
	public class Mission{
		public GameObject panel;
		public UILabel goalLabel;
		public UILabel currentLabel;
	}

	public UITexture background;

	public GameObject loading;

	public GameObject tilePanel;
    public GameObject textPanel;
	public GameObject nextPanel;
	public GameObject itemPanel;
	public GameObject topRightAnchor;
    public GameObject windowPanel;
	public GameObject missionWindowPanel;
    public GameObject optionButton;
    public GameObject missionBackground;
	public GameObject missionPanel;
    public UIButtonScale missionButtonScale;
	public GameObject scorePanel;
	public GameObject startButtonPanel;
    public GameObject[] scoreStar;

    public UISlider timebar;
    public Spin secondHand;

	public GameObject[] itemButton;
	public GameObject hammerCover;
	public GameObject obstacleCover;
	public GameObject startButton;
	public GameObject startCover;
	public GameObject hammerHighlight;
	public GameObject resetHighlight;


	public GameObject restrictBox;
	public UILabel restrictNumber;
    public UILabel restrictNumber2;
    public GameObject startButtonHighlighter;

	public Mission[] mission;
    public UILabel missionLabel;
    public GameObject[] centerMission;
	public UISlider scorebar;
	public UILabel scoreText;

	public GameObject pointArrow;
	public GameObject pointError;
	public GameObject missionJolly;
	public UISprite missionJollyFace;

    public GameObject whiteSkin;
	public GameObject graySkin;
    public GameObject successWindow;
	public GameObject failWindow;
    public GameObject pauseWindow;
	public GameObject allsquareStamp;
}
