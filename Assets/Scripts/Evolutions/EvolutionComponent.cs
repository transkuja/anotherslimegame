﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
    ;


public class EvolutionComponent : MonoBehaviour {
    Evolution evolution;
    float timer;
    bool isTimerInitialized = false;
    protected PlayerController playerController;
    protected bool isSpecialActionPushedOnce = false;
    protected bool isSpecialActionPushed = false;
    protected bool isSpecialActionReleased = false;

    GameObject affectedPart;
    GameObject activeTutoText;

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

        affectedPart = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild((int)evolution.BodyPart).gameObject;
        if (evolution.BodyPart == BodyPart.Hammer)
            affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(0).GetChild(0).GetChild(0));
        else if (evolution.BodyPart == BodyPart.Staff)
            affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(0).GetChild(0).GetChild(1));
        affectedPart.SetActive(true);

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

    protected void OnDestroy()
    {
        GetComponent<Player>().activeEvolutions--;
        affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh));
        affectedPart.transform.SetSiblingIndex((int)evolution.BodyPart);
        affectedPart.SetActive(false);
    }

    protected void PopTutoText(string _text)
    {
        GameObject tutoText = Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabTutoText, GameManager.UiReference.transform);
        tutoText.transform.position = GetComponent<Player>().cameraReference.GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position) 
                                        + Vector3.up * ((GameManager.Instance.PlayerStart.PlayersReference.Count > 2) ? 40.0f : 80.0f);

        tutoText.GetComponent<Text>().text = _text;
        if (activeTutoText != null)
            activeTutoText.SetActive(false);

        activeTutoText = tutoText;
        Destroy(tutoText, 5.0f);
    }

    public virtual void OnCollisionEnter(Collision coll)
    {

    }
    public virtual void OnCollisionStay(Collision coll)
    {

    }
}
