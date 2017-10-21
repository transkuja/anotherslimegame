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
            if (timer != 0.0f)
                isTimerInitialized = true;
        }
    }

    protected void SetPower(Powers powerName)
    {
        GetComponent<Player>().activeEvolutions++;
        evolution = GameManager.EvolutionManager.GetEvolutionByPowerName(powerName, (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabEvolution));
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(true);
        isEvolutionInitialized = true;
        Timer = evolution.duration;
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
        GetComponent<Player>().activeEvolutions--;
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(false);
    }
}
