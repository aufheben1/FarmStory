using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	void Start(){
		iTween.MoveAdd(gameObject, iTween.Hash("x", 10 * Mathf.Cos (30), "y", 10 * Mathf.Sin (30),"time",5.0f, "dalay", 3.0f));
	}
}

