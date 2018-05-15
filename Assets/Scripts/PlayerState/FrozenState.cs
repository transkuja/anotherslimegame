using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenState : PlayerState {
    public float maxFrozenTime = 5.0f;
    float timer = 0.0f;
    public FrozenState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }

    public override void OnBegin()
    {
        base.OnBegin();
        timer = 0.0f;
    }

    public override void OnUpdate() 
    {
        timer += Time.deltaTime;
        if(timer >= maxFrozenTime)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
        }
    }

    public override void OnFixedUpdate()
    {

    }

    public override Vector3 HandleSpeed(float x, float y)
    {
 
        return Vector3.zero;
    }

    public override void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
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

    public override void PushPlayer(Vector3 force)
    {
    
    }

    public override void HandleControllerAnim(float x, float y)
    {
    
    }
}
