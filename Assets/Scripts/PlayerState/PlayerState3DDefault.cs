using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3DDefault : PlayerState<PlayerController3DDefault>
{

    public PlayerState3DDefault(PlayerController3DDefault _playerController) : base(_playerController)
    {
        playerController = _playerController;
    }

    public virtual void HandleGravity()
    {
        if (playerController.isGravityEnabled)
        {
            float gravity = playerController.GetComponent<JumpManager>().GetGravity();
            playerController.Player.Rb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
        }
    }
    public virtual void Move(Vector3 initialVelocity)
    {

    }
    public virtual void OnDashPressed()
    {
        if (playerController.dashState.nbDashMade < 1)
        {
            playerController.PlayerState = playerController.dashState;
            if (playerController.jumpState.nbJumpMade > 0)
                playerController.dashState.nbDashMade++;
        }
    }
    public virtual void OnDownDashPressed()
    {
        if (playerController.downDashState.nbDashDownMade < 1 && !playerController.IsGrounded)
        {
            playerController.PlayerState = playerController.downDashState;
            if (playerController.jumpState.nbJumpMade > 0)
                playerController.downDashState.nbDashDownMade++;
        }
    }
}
