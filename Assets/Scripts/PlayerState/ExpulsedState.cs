using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpulsedState : PlayerState
{

    public float timer;

    public ExpulsedState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (AudioManager.Instance != null && AudioManager.Instance.hahahaFX != null)
        {
            if (playerCharacterHub.GetComponent<PNJController>() && playerCharacterHub.GetComponent<PNJController>().myAudioSource != null)
            {
                playerCharacterHub.GetComponent<PNJController>().myAudioSource.PlayOneShot(AudioManager.Instance.hahahaFX, 0.5f);
            }
            else
            {
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.hahahaFX);
            }
        }
     
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
    public override void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
    {
    }
    public override void PushPlayer(Vector3 force)
    {
        //playerController.Rb.velocity = force;
    }
    public override void OnJumpPressed()
    {
    }
    public override void HandleControllerAnim(float x, float y)
    {
    }
    public override void OnDashPressed()
    {
    }
}
