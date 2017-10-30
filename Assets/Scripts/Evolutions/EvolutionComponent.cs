using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


public class EvolutionComponent : MonoBehaviour {
    Evolution evolution;
    float timer;
    bool isEvolutionInitialized = false;
    bool isTimerInitialized = false;
    protected PlayerController playerController;
    protected bool isSpecialActionPushedOnce = false;
    protected bool isSpecialActionPushed = false;
    protected bool isSpecialActionReleased = false;

    public virtual void Start()
    {
        
    }

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

    public Evolution Evolution
    {
        get
        {
            return evolution;
        }
    }

    protected void SetPower(Powers powerName)
    {
        GetComponent<Player>().activeEvolutions++;
        evolution = GameManager.EvolutionManager.GetEvolutionByPowerName(powerName, (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabEvolution));
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(true);
        isEvolutionInitialized = true;
        Timer = evolution.duration;
        playerController = GetComponent<PlayerController>();
    }

    public virtual void Update()
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
    public virtual void OnCollisionEnter(Collision coll)
    {

    }
    public virtual void OnCollisionStay(Collision coll)
    {

    }
}
