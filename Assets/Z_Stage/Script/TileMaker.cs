using UnityEngine;
using System.Collections;

public class TileMaker : MonoBehaviour {
	public int tileType;
	public int pipeNum;
	public int dir;
	public Vector2 outVec;
	
	public bool leftOut;
	public bool rightOut;
	public bool upOut;
	public bool downOut;

	public int cropType;
	public Vector2 cropRange;

	public int spCropType;
	public Vector2 spCropRange;

	public UISprite sprite;

	// Use this for initialization
	void OnEnable () {
		sprite = GetComponent<UISprite> ();
	}
}
