﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;


public class EvolutionStrength : EvolutionComponent
{
    StatBuff groundSpeedBuff = new StatBuff(Stats.StatType.GROUND_SPEED, 0.85f, -1);

    public override void  Start()
    {
        base.Start();
        SetPower(Powers.Strength);
        playerController.stats.AddBuff(groundSpeedBuff);
        Player playerComponent = GetComponent<Player>();
        if (!playerComponent.evolutionTutoShown[(int)Powers.Strength] && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            playerComponent.evolutionTutoShown[(int)Powers.Strength] = true;
            Utils.PopTutoText("Press Y in the air", playerComponent);
        }
    }

    protected override void OnDestroy()
    {
        playerController.stats.RemoveBuff(groundSpeedBuff);
        base.OnDestroy();
    }
}
