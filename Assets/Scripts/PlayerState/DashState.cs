using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{

    float dashingMaxTimer;
    float dashingTimer;
    float dashingVelocity;

    // PlayerBouncyPhysics
    [SerializeField]
    [Range(10.0f, 2000.0f)]
    float bounceStrength = 25.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float bounceDetectionThreshold = 0.2f;
    [SerializeField]
    [Range(0.0f, 2.5f)]
    float impactForce;
    [SerializeField]
    [Range(0.0f, 40.0f)]
    float impactPropagationThreshold;

    public DashState(PlayerController _playerController) : base(_playerController)
    {
        maxCoolDown = 0.5f;
    }
    public override void OnBegin()
    {
        base.OnBegin();
        playerController.GetComponent<JumpManager>().Stop();
        dashingVelocity = 100;
        dashingMaxTimer = 0.15f;
        dashingTimer = dashingMaxTimer;
        playerController.isGravityEnabled = false;
        CurFixedUpdateFct = OnDashState;
    }

    public override void OnEnd()
    {
        playerController.isGravityEnabled = true;
        base.OnEnd();
    }

    public virtual void OnDashState()
    {
        playerController.Player.Rb.velocity = playerController.transform.forward * dashingVelocity;
        dashingTimer -= Time.fixedDeltaTime;
        if (dashingTimer <= 0.0f)
        {
            playerController.PlayerState = playerController.freeState;
        }
    }
   

    // override des actions : 
    public override void HandleGravity()
    {
    }
    public override void Move(Vector3 initialVelocity)
    {
    }

    public override void OnDownDashPressed()
    {
    }

    public override void CollisionEnter(Collision collision)
    {
    }
    

    
}
