using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class KartGameMode : GameMode {

    bool hasTimerStarted = false;

    float timer;

    float firstFinishTime = -1.0f;

    public KartArrival Arrival;

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this);
        checkRuneObjective = CheckRuneObjectiveForKart;
        LaunchTimer();
    }

    public void LaunchTimer()
    {
        hasTimerStarted = true;
        timer = 0.0f;
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
        GameManager.Instance.ScoreScreenReference.RefreshScores(player, timer, TimeFormat.MinSecMil);
    }

    public bool CheckRuneObjectiveForKart()
    {
        return firstFinishTime <= necessaryTimeForRune;
    }
}
