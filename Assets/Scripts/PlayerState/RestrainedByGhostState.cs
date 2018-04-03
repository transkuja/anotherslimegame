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

    public RestrainedByGhostState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        timer += Time.deltaTime;
        if(timer > maxDuration)
        {
            timer = 0.0f;
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
        }
    }

    public override void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
    {
        playerCharacterHub.Rb.velocity = playerCharacterHub.transform.forward * playerCharacterHub.stats.Get(Stats.StatType.GROUND_SPEED) + Vector3.up * playerCharacterHub.Rb.velocity.y;
    }

    public override void OnJumpPressed()
    {
    }

    public override void OnDashPressed()
    {
    }
}