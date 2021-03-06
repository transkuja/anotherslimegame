﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class KartGameMode : GameMode {

    [SerializeField]
    PlayerControllerKart.DrivingCondition defaultDrivingCondition = PlayerControllerKart.DrivingCondition.Normal;

    public int NumberOfLaps = 5;
    int currentPtsIndex = 4;
    public float maxTime = 75f;
    float playersLeft;

    float timer;

    public float firstFinishTime = -1.0f;

    [HideInInspector]
    private KartArrival arrival;

    public KartArrival Arrival
    {
        get
        {
            return arrival;
        }

        set
        {
            arrival = value;
            arrival.NumberOfLaps = NumberOfLaps;
        }
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);
        playersLeft = playerReferences.Count;
        for (int i = 0; i < playerReferences.Count; i++)
        {
            Player player = playerReferences[i].GetComponent<Player>();
            if (player != null)
            {
                player.OnDeathEvent += OnPlayerDeath;
                player.CallOnValueChange(PlayerUIStat.Laps, 1);
            }
            PlayerControllerKart pk = playerReferences[i].GetComponent<PlayerControllerKart>();
            if (pk)
                pk.CurrentCondition = defaultDrivingCondition;
        }
        checkRuneObjective = CheckRuneObjectiveForKart;
        LaunchTimer();
    }

    public void LaunchTimer()
    {
        timer = 0.0f;
        GameManager.Instance.GameFinalTimer = maxTime;
        GameManager.Instance.LaunchFinalTimer();
    }

    protected override void Update()
    {
        base.Update();
        if (GameManager.CurrentState != GameState.Normal)
            return;
        timer += Time.deltaTime;
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
    }

    public override void PlayerHasFinished(Player player)
    {
        if(Mathf.Approximately(firstFinishTime, -1.0f))
        {
            firstFinishTime = timer;
        }

        player.FinishTime = timer;
        player.HasFinishedTheRun = true;
        playersLeft--;
        if(playersLeft <= 0)
        {
            GameManager.Instance.ScoreScreenReference.RankKartPlayers();
        }
    }

    public bool CheckRuneObjectiveForKart()
    {
        return firstFinishTime >= 0f && firstFinishTime <= necessaryTimeForRune;
    }

    public void OnPlayerDeath(Player player)
    {
        player.transform.position = player.respawnPoint.position;
        player.transform.rotation = player.respawnPoint.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
