﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportState : PlayerState {
    public Vector3 beforePausingVelocity = Vector3.zero;
    float oldDrag = 0.0f;

    // Underwater state variables for not breaking player state when pause is off
    public bool hasReachedTheSurface = false;
    public bool hasStartedGoingUp = false;
    public float waterLevel;

    public TeleportState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
        maxCoolDown = 0.5f;
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        // Face happy
        playerCharacterHub.GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Winner;
        Vector3 direction = playerCharacterHub.GetComponent<Player>().cameraReference.transform.GetChild(0).position - playerCharacterHub.transform.position;
        direction -= direction.y * Vector3.up;

        playerCharacterHub.transform.LookAt(playerCharacterHub.transform.position + direction);
        // play teleport particles
        playerCharacterHub.TeleportParticles.Play();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        playerCharacterHub.GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Neutral;
        playerCharacterHub.TeleportParticles.Stop();
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnUpdate()
    {
    }

    public override void HandleGravity()
    {

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

    public override Vector3 HandleSpeed(float x, float y)
    {
        return Vector3.zero;
    }
}
