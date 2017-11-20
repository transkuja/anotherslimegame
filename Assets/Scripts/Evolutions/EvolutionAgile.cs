using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionAgile : EvolutionComponent {

    public override void Start()
    {
        base.Start();
        SetPower(Powers.Agile);
        playerController.stats.AddBuff(new StatBuff(Stats.StatType.GROUND_SPEED, 1.2f, -1));
        playerController.stats.AddBuff(new StatBuff(Stats.StatType.AIR_CONTROL, 1.2f, -1));
        playerController.stats.AddBuff(new StatBuff(Stats.StatType.JUMP_HEIGHT, 1.2f, -1));
        Debug.Log("Start AgileEvolution");
    }
}
