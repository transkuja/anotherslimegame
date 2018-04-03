using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeState : PlayerState
{
    public FreeState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Vector3 xzVelocity = new Vector3(playerCharacterHub.Rb.velocity.x, 0, playerCharacterHub.Rb.velocity.z);
        xzVelocity = Vector3.ClampMagnitude(xzVelocity, playerCharacterHub.stats.Get(Stats.StatType.GROUND_SPEED));
        playerCharacterHub.Rb.velocity = (playerCharacterHub.Rb.velocity.y) * Vector3.up + xzVelocity;
    }
}
