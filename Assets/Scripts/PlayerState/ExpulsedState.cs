using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpulsedState : PlayerState
{

    public Vector3 repulseForce = Vector3.zero;

    public ExpulsedState(PlayerController _playerController) : base(_playerController)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (AudioManager.Instance != null && AudioManager.Instance.hahahaFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.hahahaFX);
        curFixedUpdateFct = OnExpulseState;
        Debug.Log("EnterExpulseState");
    }
    public void OnExpulseState()
    {
        //Rigidbody _rb = playerController.Player.Rb;
        //_rb.velocity += repulseForce;
        //_rb.GetComponent<Player>().Anim.SetFloat("MouvementSpeed", 3);
        //_rb.GetComponent<Player>().Anim.SetBool("isExpulsed", _rb.GetComponent<PlayerController>().IsGrounded);
    }
    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log("Out of Expulse");
        repulseForce = Vector3.zero;
    }
    public override void Move(Vector3 initialVelocity)
    {
    }
}
