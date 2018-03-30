using UWPAndXInput;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public class JumpState : PlayerState
{
    private int nbJumpMade = 0;

    public int NbJumpMade
    {
        get
        {
            return nbJumpMade;
        }

        set
        {
            nbJumpMade = value;
#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), (int)playerController.playerIndex);
#endif
        }
    }

    public JumpState(PlayerControllerHub _playerController) : base(_playerController)
    {
        curUpdateFct = OnJump;
    }
   
    public override void OnBegin()
    {
        base.OnBegin();
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
        {
            jm.Jump(JumpManager.JumpEnum.Basic);
            if (AudioManager.Instance != null && AudioManager.Instance.jumpFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.jumpFx);
        }
        else
            Debug.LogError("No jump manager attached to player!");

        NbJumpMade++;
    }

   
    public override void OnJumpPressed()
    {
        if (NbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
        {
            LaunchJump();

            if (NbJumpMade > 1)
            {
                if (AudioManager.Instance != null && AudioManager.Instance.youpiFX != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.youpiFX);
            }
        }
    }

}
