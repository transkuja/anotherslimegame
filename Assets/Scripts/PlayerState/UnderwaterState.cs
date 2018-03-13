using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterState : PlayerState
{
    public bool hasReachedTheSurface = false;
    public float waterLevel;
    float waterTolerance;
    public UnderwaterState(PlayerControllerHub _playerController) : base(_playerController)
    {
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        waterTolerance = playerController.GetComponent<SphereCollider>().radius;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        hasReachedTheSurface = false;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    // No dash underwater
    public override void OnDashPressed()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (playerController.transform.position.y > waterLevel - waterTolerance)
            hasReachedTheSurface = true;
    }

    public override void OnJumpPressed()
    {
        if (hasReachedTheSurface)
        {
            // TODO: cancel anim
            hasReachedTheSurface = false;
            base.OnJumpPressed();
        }
    }

    // Disable gravity when underwater
    public override void HandleGravity()
    {
        if (!hasReachedTheSurface)
            playerController.Player.Rb.AddForce(30 * Vector3.up);
        else
            playerController.Player.Rb.velocity = new Vector3(playerController.Player.Rb.velocity.x, 0, playerController.Player.Rb.velocity.z);
    }
}
