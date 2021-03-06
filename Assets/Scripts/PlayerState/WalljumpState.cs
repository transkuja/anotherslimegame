﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalljumpState : PlayerState
{
    float pushForce;
    float pushTime;
    public Vector3 pushDirection;
    float timer;

    public WalljumpState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
        // > 0.06 sinon double saut sur le meme mur
        maxCoolDown = 0.07f;
    }
    public override void OnBegin()
    {
        base.OnBegin();
        curFixedUpdateFct = PushedFromWall;
        // delay avant de retomber par défaut < 0.4 sinon sa fait de la merde, je sais pas pourquoi
        pushTime = 0.40f;
        // Force de répulsion > 500
        pushForce = 800;
        timer = 0;

        playerCharacterHub.jumpState.NbJumpMade = 2;

        // Se tourner vers l'exterieur
        playerCharacterHub.transform.rotation =
           Quaternion.LookRotation(pushDirection, Vector3.up);
        ;

        // Freeze la velocity en xz
        playerCharacterHub.Rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        // Friction a la main
        playerCharacterHub.Rb.drag = 25f;
    }
    public override void OnEnd()
    {
    
        base.OnEnd();


        playerCharacterHub.Rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerCharacterHub.Rb.drag = 0.0f;

    }
    public void PushedFromWall()
    {
        timer += Time.deltaTime;
        if (timer > pushTime)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
            playerCharacterHub.Rb.AddForce(pushDirection.normalized * pushForce /5);
        }
    }

    public override void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
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
        if(playerCharacterHub.jumpState.NbJumpMade <= 2)
        {
            JumpManager jm;

            playerCharacterHub.Rb.constraints = RigidbodyConstraints.FreezeRotation;
            playerCharacterHub.Rb.drag = 0.0f;

            if (jm = playerCharacterHub.GetComponent<JumpManager>())
                jm.Jump(JumpManager.JumpEnum.Basic);
            playerCharacterHub.Rb.AddForce(pushDirection.normalized * pushForce);

            playerCharacterHub.jumpState.NbJumpMade = 20;
            timer = 0;
            PushedFromWall();
        }
        
    }

}
