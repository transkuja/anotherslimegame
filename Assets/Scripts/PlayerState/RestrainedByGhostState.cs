using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrainedByGhostState : PlayerState
{
    float maxDuration = 2.0f;
    float timer = 0.0f;

    public void ResetTimer()
    {
        timer = 0.0f;
    }

    public RestrainedByGhostState(PlayerController _playerController) : base(_playerController)
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        timer += Time.deltaTime;
        if(timer > maxDuration)
        {
            timer = 0.0f;
            playerController.PlayerState = playerController.freeState;
        }
    }

    public override void Move(Vector3 initialVelocity)
    {
        // This shouldn't be necessary
        //if(GhostTrail.DicPlayersInTriggers != null && GhostTrail.DicPlayersInTriggers.ContainsKey(playerController.GetComponent<PlayerController>()))
        //{
        //    if(GhostTrail.DicPlayersInTriggers[playerController.GetComponent<PlayerController>()] <= 0)
        //    {
        //        playerController.PlayerState = playerController.freeState;
        //    }
        //}
        
        playerController.Player.Rb.velocity = playerController.transform.forward * playerController.stats.Get(Stats.StatType.GROUND_SPEED) + Vector3.up * playerController.Player.Rb.velocity.y;
    }

    public override void OnJumpPressed()
    {
    }

    public override void OnDashPressed()
    {
    }
}