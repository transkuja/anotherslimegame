using UWPAndXInput;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public class JumpState : PlayerHubState
{

    bool hasJumpButtonBeenReleased;
    public int nbJumpMade = 0;

    public JumpState(PlayerControllerHub _playerController) : base(_playerController)
    {
        curUpdateFct = OnJump;
    }
   
    public override void OnBegin()
    {
        base.OnBegin();
        hasJumpButtonBeenReleased = false;
        LaunchJump();
    }
   
    public override void OnEnd()
    {
        base.OnEnd();
    }
    public void OnJump()
    {
        if (playerController.IsGrounded)
        {
            playerController.PlayerState = playerController.freeState;
        }
    }


    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public void LaunchJump()
    {
        playerController.IsGrounded = false;
        JumpManager jm;
        if (jm = playerController.GetComponent<JumpManager>())
            jm.Jump(JumpManager.JumpEnum.Basic);
        else
            Debug.LogError("No jump manager attached to player!");

        nbJumpMade++;
    }

   
    public override void OnJumpPressed()
    {
        // obligé de faire un check ici( jump--> dash--> jump--> dash-->jump ...)
        
        if (hasJumpButtonBeenReleased)
        {
            hasJumpButtonBeenReleased = false;
            if (playerController.wallJumpState.WallJumpTest())
            {
                playerController.PlayerState = playerController.wallJumpState;
            }
            else if (nbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
            {
                if (nbJumpMade > 1)
                {
                    if (AudioManager.Instance != null && AudioManager.Instance.youpiFX != null)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.youpiFX);
                }

                LaunchJump();
            }
        }
        if (playerController.State.Buttons.A == ButtonState.Released)
            hasJumpButtonBeenReleased = true;
    }

}
