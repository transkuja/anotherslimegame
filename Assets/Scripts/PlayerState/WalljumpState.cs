using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalljumpState : PlayerState
{
    float pushForce;
    float pushTime;
    public Vector3 pushDirection;
    float timer;

    public WalljumpState(PlayerControllerHub _playerController) : base(_playerController)
    {
        maxCoolDown = 0.18f;
    }
    public override void OnBegin()
    {
        base.OnBegin();
        curFixedUpdateFct = PushedFromWall;
        pushTime = 0.22f;
        pushForce = 880;
        timer = 0;

        playerController.jumpState.nbJumpMade = 2;

        // Se tourner vers l'exterieur
        playerController.transform.rotation =
           Quaternion.LookRotation(pushDirection, Vector3.up);
        ;

        // Freeze la velocity en xz
        playerController.Rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        playerController.Rb.drag = 14f;
    }
    public override void OnEnd()
    {
    
        base.OnEnd();


        playerController.Rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerController.Rb.drag = 0.0f;

    }
    public void PushedFromWall()
    {
        timer += Time.deltaTime;
        if (timer > pushTime)
        {
            playerController.PlayerState = playerController.freeState;
        }
    }

    public override void Move(Vector3 initialVelocity)
    {
    }
    public override void OnDashPressed()
    {
    }
    public override void OnDownDashPressed()
    {
    }
    public override void OnJumpPressed()
    {
        if(playerController.jumpState.nbJumpMade <= 2)
        {
            JumpManager jm;

            playerController.Rb.constraints = RigidbodyConstraints.FreezeRotation;
            playerController.Rb.drag = 0.0f;

            if (jm = playerController.GetComponent<JumpManager>())
                jm.Jump(JumpManager.JumpEnum.Basic);
            playerController.Rb.AddForce(pushDirection.normalized * pushForce);

            playerController.jumpState.nbJumpMade = 20;
            timer = 0;
            PushedFromWall();
        }
        
    }

}
