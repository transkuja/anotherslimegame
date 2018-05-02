using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePointsFeedback : MonoBehaviour {

    public float timeBeforeDestruction;
    public float ascendingSpeed;

	void Start () {
        Destroy(gameObject, timeBeforeDestruction);
	}

    private void Update()
    {
        transform.GetChild(0).position = transform.GetChild(0).position.x * Vector3.right + (transform.GetChild(0).position.y + Time.deltaTime * ascendingSpeed) * Vector3.up;
    }

}
