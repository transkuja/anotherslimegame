using XInputDotNetPure;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public class JumpState : PlayerState
{

    bool hasJumpButtonBeenReleased;
    public JumpState(PlayerController _playerController) : base(_playerController)
    {
        curUpdateFct = OnJump;
    }
   
    public override void OnBegin()
    {
        base.OnBegin();
        hasJumpButtonBeenReleased = false;
        LauchJump();
    }
   
    public override void OnEnd()
    {
        base.OnEnd();
    }
    public void OnJump()
    {
        if (playerController.IsGrounded)
        {
            playerController.PlayerState = new FreeState(playerController);
        }
    }


    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }
     public void LauchJump()
    {
        playerController.IsGrounded = false;
        JumpManager jm;
        if (jm = playerController.GetComponent<JumpManager>())
            jm.Jump(JumpManager.JumpEnum.Basic);
        else
            Debug.LogError("No jump manager attached to player!");
        playerController.chargeFactor = 0;
        playerController.NbJump++;
    }


    public override void OnJumpPressed()
    {
        if (playerController.NbJump < playerController.stats.Get(Stats.StatType.JUMP_NB) && hasJumpButtonBeenReleased)
        {
            hasJumpButtonBeenReleased = false;
            if (playerController.NbJump > 1)
            {
                if (AudioManager.Instance != null && AudioManager.Instance.youpiFX != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.youpiFX);
            }
            LauchJump();
        }
        if (playerController.State.Buttons.A == ButtonState.Released)
            hasJumpButtonBeenReleased = true;
    }

}
