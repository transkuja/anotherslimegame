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
        Player playerComponent = GetComponent<Player>();
        if (playerComponent.evolutionTutoShown != null && playerComponent.evolutionTutoShown.Length > 0 && !playerComponent.evolutionTutoShown[(int)Powers.Strength] && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            playerComponent.evolutionTutoShown[(int)Powers.Strength] = true;
            Utils.PopTutoText("Press Y in the air", playerComponent);
        }

        if (feedbackCooldownImg != null)
            feedbackCooldownImg.sprite = ResourceUtils.Instance.spriteUtils.Strength;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
