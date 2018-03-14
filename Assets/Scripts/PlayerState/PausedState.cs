using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedState : PlayerState {
    public Vector3 beforePausingVelocity = Vector3.zero;
    float oldDrag = 0.0f;

    // Underwater state variables for not breaking player state when pause is off
    public bool hasReachedTheSurface = false;
    public bool hasStartedGoingUp = false;
    public float waterLevel;

    public PausedState(PlayerControllerHub _playerController) : base(_playerController)
    {
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        playerController.Player.cameraReference.transform.GetChild(0).GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
        beforePausingVelocity = playerController.Rb.velocity;
        playerController.Rb.velocity = Vector3.zero;
        oldDrag = playerController.Rb.drag;
        playerController.Rb.drag = 0.0f;
        playerController.Player.Anim.StartPlayback();

    }

    public override void OnEnd()
    {
        base.OnEnd();
        playerController.Player.cameraReference.transform.GetChild(0).GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
        playerController.Player.Anim.StopPlayback();
        playerController.Rb.velocity = beforePausingVelocity;
        playerController.Rb.drag = oldDrag;
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnUpdate()
    {
    }

    public override void HandleGravity()
    {
        playerController.Rb.velocity = Vector3.zero;
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
