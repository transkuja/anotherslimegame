using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpulsedState : PlayerState
{

    public float timer;

    public ExpulsedState(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub) : base(_playerCharacterHub, _playerControllerHub)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (AudioManager.Instance != null && AudioManager.Instance.hahahaFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.hahahaFX);
        curFixedUpdateFct = OnExpulseState;
        timer = 0;
        playerCharacterHub.Anim.SetBool("isExpulsed", true);
        playerCharacterHub.Anim.SetFloat("MouvementSpeed", 2);
    }
    public void OnExpulseState()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
        }
    }
    public override void OnEnd()
    {
        playerCharacterHub.Anim.SetBool("isExpulsed", false);
        base.OnEnd();
    }
    public override void Move(Vector3 initialVelocity)
    {
    }
    public override void PushPlayer(Vector3 force)
    {
        //playerController.Rb.velocity = force;
    }
    public override void OnJumpPressed()
    {
    }
    public override void HandleControllerAnim()
    {
    }
    public override void OnDashPressed()
    {
    }
}
