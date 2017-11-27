using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class DashDownState : PlayerState
{
    float timer;
    float maxDashChargeDelay;
    float downDashPower;

    public int nbDashDownMade = 0;
        
    public DashDownState(PlayerController _playerController) : base(_playerController)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        playerController.Rb.velocity = Vector3.zero;

        JumpManager jumpManager = playerController.GetComponent<JumpManager>();
        if (jumpManager != null)
            jumpManager.Stop();
        timer = 0;
        maxDashChargeDelay = .7f;
        downDashPower = 100f;
        curUpdateFct = Levitate;
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
    public void Levitate()
    {
        timer += Time.deltaTime;
        if (timer > maxDashChargeDelay || playerController.State.Buttons.Y == ButtonState.Released)
        {
            curUpdateFct = LaunchDash;
            timer = 0;
        }
    }

    // Apply a speed towards the ground
    public void LaunchDash()
    {
        if (nbDashDownMade < 1)
        {
            Vector3 downPush = Vector3.down * downDashPower;
            playerController.Rb.velocity = downPush; // Override current velocity. 
            timer += Time.deltaTime;
            if (playerController.IsGrounded || timer > 2)
                playerController.PlayerState = playerController.freeState;
        }
    }
    public override void HandleGravity()
    {
    }
    public override void Move(Vector3 initialVelocity)
    {
    }
    public override void OnDownDashPressed()
    {
    }
    public override void OnDashPressed()
    {
    }
}