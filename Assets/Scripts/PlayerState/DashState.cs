using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState3DDefault
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

    public int nbDashMade = 0;

    public DashState(PlayerController3DDefault _playerController) : base(_playerController)
    {
        maxCoolDown = 0.5f;
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (playerController.dashParticles && playerController.dashParticles.GetComponent<ParticleSystem>())
            playerController.dashParticles.GetComponent<ParticleSystem>().Play();
        playerController.GetComponent<JumpManager>().Stop();
        dashingVelocity = playerController.stats.Get(Stats.StatType.DASH_FORCE);
        dashingMaxTimer = 0.15f;
        dashingTimer = dashingMaxTimer;
        playerController.isGravityEnabled = false;
        CurFixedUpdateFct = OnDashState;

        //playerController.ChangeDampingValuesCameraFreeLook(0.9f);
    }

    public override void OnEnd()
    {
        playerController.isGravityEnabled = true;
        if (playerController.IsGrounded) nbDashMade = 0;
        //playerController.ChangeDampingValuesCameraFreeLook(0.0f);
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
        Vector3 tmp = new Vector3(playerController.Player.Rb.velocity.x, 0.0f, playerController.Player.Rb.velocity.z);
        Vector3 fwd = playerController.transform.forward;

        float dragForceUsed = 0.02f;//(playerController.PreviousPlayerState == playerController.dashState) ? dragForceDash : dragForce;

        if (tmp.sqrMagnitude > 7.0f)// && Vector3.Dot(playerController.transform.forward, tmp) > 0)
        {
            if ((tmp.x > 0 && playerController.Player.Rb.velocity.x - tmp.x * fwd.x * dragForceUsed < 0)
            || (tmp.x < 0 && playerController.Player.Rb.velocity.x - tmp.x * fwd.x * dragForceUsed > 0)
            || (tmp.z > 0 && playerController.Player.Rb.velocity.z - tmp.z * fwd.z * dragForceUsed < 0)
            || (tmp.z < 0 && playerController.Player.Rb.velocity.z - tmp.z * fwd.z * dragForceUsed > 0))
                playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
            else
                playerController.Player.Rb.velocity -= (tmp.normalized * dragForceUsed * tmp.sqrMagnitude) * ((Time.deltaTime / 0.05f));

            tmp = new Vector3(playerController.Player.Rb.velocity.x, 0.0f, playerController.Player.Rb.velocity.z);

            if (Vector3.Dot(fwd, tmp) < 0)
                playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;

        }
        else
        {
            playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
        }
    }

    public override void OnDownDashPressed()
    {
    }

    public override void CollisionEnter(Collision collision)
    {
    }
    

    
}
