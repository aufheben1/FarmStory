using UnityEngine;
using System.Collections;

public class StoneFly : MonoBehaviour {
	void mStart () {
        Vector3 startPosition = transform.localPosition;
        float randX = Random.Range(0f, 2f);
        float randY = Random.Range(0f, 2f);

        TweenPosition.Begin(gameObject, Random.Range(0.1f, 0.5f), startPosition + new Vector3(randX - 1, randY *0.5f, 0)  * 80);
        TweenScale.Begin(gameObject, Random.Range(0.1f, 0.5f), transform.localScale * Random.Range(1f, 2f));

        Destroy(gameObject, 0.5f);
	}
}
