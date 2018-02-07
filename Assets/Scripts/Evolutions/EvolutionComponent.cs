using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
    ;


public class EvolutionComponent : MonoBehaviour {
    Evolution evolution;
    float timer;
    protected PlayerControllerHub playerController;
    protected bool isSpecialActionPushedOnce = false;
    protected bool isSpecialActionPushed = false;
    protected bool isSpecialActionReleased = false;

    GameObject affectedPart;

    public virtual void Start()
    {
        timer = -1;
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
        evolution = GameManager.EvolutionManager.GetEvolutionByPowerName(powerName);
        if((int)evolution.BodyPart < transform.GetChild((int)PlayerChildren.SlimeMesh).childCount)
            affectedPart = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild((int)evolution.BodyPart).gameObject;
        if (evolution.BodyPart == BodyPart.Hammer)
            affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(0).GetChild(0).GetChild(0));
        else if (evolution.BodyPart == BodyPart.Staff)
            affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(0).GetChild(0).GetChild(1));
        else if(evolution.BodyPart == BodyPart.Ghost)
        {
            //Change player appearence
            ((EvolutionGhost)(this)).SetGhostVisual();
            //Then do Nothing
            affectedPart = transform.gameObject;
        }
        affectedPart.SetActive(true);

        playerController = GetComponent<PlayerControllerHub>();
    }

    public virtual void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                Destroy(this);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        GetComponent<Player>().activeEvolutions--;
        affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh));
        affectedPart.transform.SetSiblingIndex((int)evolution.BodyPart);
        affectedPart.SetActive(false);
    }

    public virtual void OnCollisionEnter(Collision coll)
    {

    }
    public virtual void OnCollisionStay(Collision coll)
    {

    }
}
