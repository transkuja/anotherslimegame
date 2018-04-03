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
            if(playerCharacterHub.GetComponent<PlayerControllerHub>())
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), (int)playerCharacterHub.GetComponent<PlayerControllerHub>().playerIndex);
            else
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), -1);
#endif
        }
    }

    public JumpState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
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
        if (playerCharacterHub.IsGrounded)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
        }
    }


    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public void LaunchJump()
    {
        playerCharacterHub.IsGrounded = false;
        JumpManager jm;
        if (jm = playerCharacterHub.GetComponent<JumpManager>())
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
        if (NbJumpMade < playerCharacterHub.stats.Get(Stats.StatType.JUMP_NB))
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
