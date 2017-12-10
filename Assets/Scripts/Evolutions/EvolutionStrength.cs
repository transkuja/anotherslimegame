using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;


public class EvolutionStrength : EvolutionComponent
{
    public override void  Start()
    {
        base.Start();
        SetPower(Powers.Strength);
        playerController.stats.AddBuff(new StatBuff(Stats.StatType.GROUND_SPEED, 0.85f, -1));

        if (!GetComponent<Player>().evolutionTutoShown[(int)Powers.Strength])
        {
            GetComponent<Player>().evolutionTutoShown[(int)Powers.Strength] = true;
            PopTutoText("Press Y in the air");
        }
    }
}
