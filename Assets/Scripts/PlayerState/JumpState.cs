using XInputDotNetPure;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public class JumpState : PlayerState
{

    int nbJump;
    bool hasJumpButtonBeenReleased;
    public JumpState(PlayerController _playerController) : base(_playerController)
    {
        curUpdateFct = OnJump;
    }
   
    public override void OnBegin()
    {
        base.OnBegin();
        hasJumpButtonBeenReleased = false;
        nbJump = 1;
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
    public override void Jump()
    {
       

        if (nbJump < playerController.stats.Get(Stats.StatType.JUMP_NB) && hasJumpButtonBeenReleased)
        {
            nbJump++;
            hasJumpButtonBeenReleased = false;
            if (nbJump > 1)
            {
                if (AudioManager.Instance != null && AudioManager.Instance.youpiFX != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.youpiFX);
            }
            playerController.Jump();
        }
        if (playerController.State.Buttons.A == ButtonState.Released)
            hasJumpButtonBeenReleased = true;
    }

}
