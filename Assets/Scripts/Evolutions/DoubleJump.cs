using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour {

    Evolution evolution = GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.DoubleJump);
    float timer;

	void Start () {
        timer = evolution.Duration;
	}
	
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
            Destroy(this);
	}
}
