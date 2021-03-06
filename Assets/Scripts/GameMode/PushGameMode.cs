﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class PushGameMode : GameMode {

    [SerializeField]
    float timer;

    [SerializeField]
    int pointsOnStart = 500;

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);

        Player player;
        for (int i = 0; i < playerReferences.Count; i++)
        {
            player = playerReferences[i].GetComponent<Player>();
            player.UpdateCollectableValue(CollectableType.StrengthEvolution1, 1);
            player.NbLife = 3;
            if( minigameVersion == 0)
                player.UpdateCollectableValue(CollectableType.Points, pointsOnStart);
        }

        LaunchTimer();
    }

    public void LaunchTimer()
    {
        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);

        }
    }

}
