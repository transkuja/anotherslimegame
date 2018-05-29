using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionAgile : EvolutionComponent {

    StatBuff groundSpeedBuff = new StatBuff(Stats.StatType.GROUND_SPEED, 1.2f, -1);
    StatBuff airControlBuff = new StatBuff(Stats.StatType.AIR_CONTROL, 1.2f, -1);
    StatBuff jumpHeightBuff = new StatBuff(Stats.StatType.JUMP_HEIGHT, 1.2f, -1);

    Player playerComponent;

    public override void Start()
    {
        base.Start();
        SetPower(Powers.Agile);
        playerCharacter.stats.AddBuff(groundSpeedBuff);
        playerCharacter.stats.AddBuff(airControlBuff);
        playerCharacter.stats.AddBuff(jumpHeightBuff);

        playerComponent = GetComponent<Player>();
        if (playerComponent.evolutionTutoShown != null && playerComponent.evolutionTutoShown.Length > 0 && !playerComponent.evolutionTutoShown[(int)Powers.Agile] && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            GetComponent<Player>().evolutionTutoShown[(int)Powers.Agile] = true;
        }

        if (feedbackCooldownImg != null)
            feedbackCooldownImg.sprite = ResourceUtils.Instance.spriteUtils.Agile;
    }

    protected override void OnDestroy()
    {
        playerCharacter.stats.RemoveBuff(groundSpeedBuff);
        playerCharacter.stats.RemoveBuff(airControlBuff);
        playerCharacter.stats.RemoveBuff(jumpHeightBuff);
        ParticleSystem.MainModule mainModule = GetComponent<PlayerCharacterHub>().DashParticles.GetComponent<ParticleSystem>().main;
        mainModule.loop = false;
        base.OnDestroy();
    }
}
