using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{

    float dashingMaxTimer;
    float dashingTimer;
    float dashingVelocity;
    public DashState(PlayerController _playerController) : base(_playerController)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        playerController.GetComponent<JumpManager>().Stop();
        dashingVelocity = 100;
        dashingMaxTimer = 0.15f;
        dashingTimer = dashingMaxTimer;
        playerController.isGravityEnabled = false;
    }

    public override void OnEnd()
    {
        playerController.isGravityEnabled = true;
        base.OnEnd();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        playerController.Player.Rb.velocity = playerController.transform.forward * dashingVelocity;
        dashingTimer -= Time.fixedDeltaTime;
        if (dashingTimer <= 0.0f)
        {
            playerController.PlayerState = new FreeState(playerController);
        }
    }
        // override le movement pour l'interdire : 
    public override void HandleGravity()
    {
    }
    public override void HandleMovementWithController()
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
