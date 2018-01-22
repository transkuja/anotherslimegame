using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrainedByGhostState : PlayerHubState
{
    float maxDuration = 2.0f;
    float timer = 0.0f;

    public void ResetTimer()
    {
        timer = 0.0f;
    }

    public RestrainedByGhostState(PlayerControllerHub _playerController) : base(_playerController)
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
        playerController.Player.Rb.velocity = playerController.transform.forward * playerController.stats.Get(Stats.StatType.GROUND_SPEED) + Vector3.up * playerController.Player.Rb.velocity.y;
    }

    public override void OnJumpPressed()
    {
    }

    public override void OnDashPressed()
    {
    }
}