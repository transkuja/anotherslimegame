using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;


public class EvolutionStrength : EvolutionComponent
{
    StatBuff groundSpeedBuff = new StatBuff(Stats.StatType.GROUND_SPEED, 0.8f, -1);
    StatBuff airControlBuff = new StatBuff(Stats.StatType.AIR_CONTROL, 0.8f, -1);
    StatBuff jumpHeightBuff = new StatBuff(Stats.StatType.JUMP_HEIGHT, 0.8f, -1);

    public override void  Start()
    {
        base.Start();
        SetPower(Powers.Strength);
        playerCharacter.stats.AddBuff(groundSpeedBuff);
        playerCharacter.stats.AddBuff(airControlBuff);
        playerCharacter.stats.AddBuff(jumpHeightBuff);

        Player playerComponent = GetComponent<Player>();
        if (playerComponent.evolutionTutoShown != null && playerComponent.evolutionTutoShown.Length > 0 && !playerComponent.evolutionTutoShown[(int)Powers.Strength] && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            playerComponent.evolutionTutoShown[(int)Powers.Strength] = true;
            if ((int)GetComponent<PlayerController>().playerIndex == Controls.keyboardIndex)
                Utils.PopTutoText("Press", KeyboardControlType.Stomp, "in the air to stomp", playerComponent);
            else
                Utils.PopTutoText("Press", ControlType.Y, "in the air to stomp", playerComponent);
        }

        if (feedbackCooldownImg != null)
            feedbackCooldownImg.sprite = ResourceUtils.Instance.spriteUtils.Strength;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerCharacter.stats.RemoveBuff(groundSpeedBuff);
        playerCharacter.stats.RemoveBuff(airControlBuff);
        playerCharacter.stats.RemoveBuff(jumpHeightBuff);

        transform.GetChild((int)BodyPart.Body).GetChild(1).localScale = Vector3.one;
        if(GetComponent<PlayerCharacterHub>().StrengthParticles && GetComponent<PlayerCharacterHub>().StrengthParticles.GetComponent<StrengthParticlesHandler>())
            GetComponent<PlayerCharacterHub>().StrengthParticles.GetComponent<StrengthParticlesHandler>().enabled = false;
    }
}
