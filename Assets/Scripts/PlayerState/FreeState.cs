using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeState : PlayerState
{
    public FreeState(PlayerControllerHub _playerController) : base(_playerController)
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
        Vector3 xzVelocity = new Vector3(playerController.Rb.velocity.x, 0, playerController.Player.Rb.velocity.z);
        xzVelocity = Vector3.ClampMagnitude(xzVelocity, playerController.stats.Get(Stats.StatType.GROUND_SPEED));
        playerController.Rb.velocity = (playerController.Rb.velocity.y) * Vector3.up + xzVelocity;
    }
}
