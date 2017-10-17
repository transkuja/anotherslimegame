using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionComponent : MonoBehaviour {
    Evolution evolution;
    float timer;
    bool isEvolutionInitialized = false;
    bool isTimerInitialized = false;

    public float Timer
    {
        get
        {
            return timer;
        }

        set
        {
            timer = value;
            isTimerInitialized = true;
        }
    }

    protected void SetPower(Powers powerName)
    {
        evolution = GameManager.EvolutionManager.GetEvolutionByPowerName(powerName);
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(true);
        isEvolutionInitialized = true;
        Timer = evolution.Duration;
    }

    void Update()
    {
        if (isTimerInitialized)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                Destroy(this);
            }
        }
    }

    private void OnDestroy()
    {
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(false);
    }
}
