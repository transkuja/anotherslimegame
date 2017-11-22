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
        playerController.GetComponent<Player>().Anim.SetBool("isExpulsed", true);
        playerController.GetComponent<Player>().Anim.SetFloat("MouvementSpeed", 2);
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
        playerController.GetComponent<Player>().Anim.SetBool("isExpulsed", false);
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
