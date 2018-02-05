using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class KartGameMode : GameMode {

    bool hasTimerStarted = false;

    float timer;

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);

        LaunchTimer();
    }

    public void LaunchTimer()
    {
        hasTimerStarted = true;
        timer = 0.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
    }

    public override void PlayerHasFinished(Player player)
    {
        GameManager.Instance.ScoreScreenReference.RefreshScores(player, timer, TimeFormat.MinSecMil);
    }
}
