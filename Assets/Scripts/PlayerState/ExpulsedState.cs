using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpulsedState : PlayerState
{

    public float timer;

    public ExpulsedState(PlayerController _playerController) : base(_playerController)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (AudioManager.Instance != null && AudioManager.Instance.hahahaFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.hahahaFX);
        curFixedUpdateFct = OnExpulseState;
        timer = 0;
        //playerController.GetComponent<JumpManager>().Stop();
    }
    public void OnExpulseState()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            playerController.PlayerState = playerController.freeState;
        }
    }
    public override void OnEnd()
    {
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
}
