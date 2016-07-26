using UnityEngine;
using System.Collections;

public class StageInfo : MonoBehaviour {
	
	[System.Serializable]
	public class Mission{
		public int cropType;
		public int cropCount;
	}
	
	public enum GameMode{timeAttack, limitedPipe}

	//스테이지 정보를 저장
	public GameMode gameMode = GameMode.limitedPipe;
	public int limitNum = 40;
	public Mission[] mission;
	public int[] starCondition;
	public int[] pipeWeight;


	public int GetSumPipeWeight(){
		int sum = 0;
		foreach (int var in pipeWeight) {
			sum += var;
		}
		return sum;
	}
}
