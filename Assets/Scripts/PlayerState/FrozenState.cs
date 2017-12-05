using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenState : PlayerState {

    public FrozenState(PlayerController _playerController) : base(_playerController)
    {
    }

    public override void OnUpdate() 
    {

    }

    public override void OnFixedUpdate()
    {

    }

    public override Vector3 HandleSpeedWithController()
    {
 
        return Vector3.zero;
    }

    public override void Move(Vector3 initialVelocity)
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

    public override void HandleControllerAnim()
    {
    
    }
}
