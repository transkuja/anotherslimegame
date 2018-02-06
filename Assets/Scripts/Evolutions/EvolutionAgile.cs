using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionAgile : EvolutionComponent {

    StatBuff groundSpeedBuff = new StatBuff(Stats.StatType.GROUND_SPEED, 1.2f, -1);
    StatBuff airControlBuff = new StatBuff(Stats.StatType.AIR_CONTROL, 1.2f, -1);
    StatBuff jumpHeightBuff = new StatBuff(Stats.StatType.JUMP_HEIGHT, 1.2f, -1);

    public override void Start()
    {
        base.Start();
        SetPower(Powers.Agile);
        playerController.stats.AddBuff(groundSpeedBuff);
        playerController.stats.AddBuff(airControlBuff);
        playerController.stats.AddBuff(jumpHeightBuff);
        Debug.Log("Start AgileEvolution");

        if (!GetComponent<Player>().evolutionTutoShown[(int)Powers.Agile])
        {

            GetComponent<Player>().evolutionTutoShown[(int)Powers.Agile] = true;
        }
    }

    protected override void OnDestroy()
    {
        playerController.stats.RemoveBuff(groundSpeedBuff);
        playerController.stats.RemoveBuff(airControlBuff);
        playerController.stats.RemoveBuff(jumpHeightBuff);
        base.OnDestroy();
    }
}
