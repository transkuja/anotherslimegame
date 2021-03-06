﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedState : PlayerState {
    public Vector3 beforePausingVelocity = Vector3.zero;
    float oldDrag = 0.0f;

    // Underwater state variables for not breaking player state when pause is off
    public bool hasReachedTheSurface = false;
    public bool hasStartedGoingUp = false;
    public float waterLevel;

    public PausedState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        playerCharacterHub.GetComponent<Player>().cameraReference.transform.GetChild(0).GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
        beforePausingVelocity = playerCharacterHub.Rb.velocity;
        playerCharacterHub.Rb.velocity = Vector3.zero;
        playerCharacterHub.Rb.useGravity = false;
        playerCharacterHub.JumpManager.enabled = false;
        oldDrag = playerCharacterHub.Rb.drag;
        playerCharacterHub.Rb.drag = 0.0f;
        playerCharacterHub.Anim.StartPlayback();

    }

    public override void OnEnd()
    {
        base.OnEnd();
        playerCharacterHub.GetComponent<Player>().cameraReference.transform.GetChild(0).GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
        playerCharacterHub.Anim.StopPlayback();
        playerCharacterHub.Rb.velocity = beforePausingVelocity;
        playerCharacterHub.Rb.drag = oldDrag;
        playerCharacterHub.Rb.useGravity = true;
        playerCharacterHub.JumpManager.enabled = true;
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnUpdate()
    {
    }

    public override void HandleGravity()
    {
        playerCharacterHub.Rb.velocity = Vector3.zero;
    }

    public override void OnJumpPressed()
    {
        
    }

    public override void OnDashPressed()
    {
        
    }
    public override void OnDownDashPressed()
    {

    }
}
