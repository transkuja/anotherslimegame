using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSprite : MonoBehaviour {
    float t;

	void Update () {
        if (t > 2 * Mathf.PI)
            t -= 2 * Mathf.PI;
        t += Time.deltaTime * 2.5f;

        transform.localScale = new Vector3(Mathf.Sin(t), 1, 1);
	}
}
