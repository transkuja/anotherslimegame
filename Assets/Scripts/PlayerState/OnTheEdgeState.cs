using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTheEdgeState : PlayerHubState
{
    public OnTheEdgeState(PlayerControllerHub _playerController) : base(_playerController)
    {
    }

    public override void DrawGizmo()
    {
    }

    public override void OnBegin()
    {

    }

    public override void OnEnd()
    {
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnUpdate()
    {

    }

    public override Vector3 HandleSpeedWithController()
    {
        if (playerController.State.ThumbSticks.Left.Y > 0.0f)
            return Vector3.zero;
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
}
