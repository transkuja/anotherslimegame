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

    public int nbDashMade = 0;

    public DashState(PlayerControllerHub _playerController) : base(_playerController)
    {
        maxCoolDown = 0.5f;
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (AudioManager.Instance != null && AudioManager.Instance.dashFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.dashFx, 4);

        if (playerController.dashParticles && playerController.dashParticles.GetComponent<ParticleSystem>())
            playerController.dashParticles.GetComponent<ParticleSystem>().Play();
        playerController.GetComponent<JumpManager>().Stop();
        dashingVelocity = playerController.stats.Get(Stats.StatType.DASH_FORCE);
        dashingMaxTimer = 0.15f;
        dashingTimer = dashingMaxTimer;
        playerController.isGravityEnabled = false;
        playerController.Rb.drag = 15.0f;
        CurFixedUpdateFct = OnDashState;

        //playerController.ChangeDampingValuesCameraFreeLook(0.9f);
    }

    public override void OnEnd()
    {
        playerController.isGravityEnabled = true;
        if (playerController.IsGrounded) nbDashMade = 0;
        playerController.Rb.drag = 0.0f;

        //playerController.ChangeDampingValuesCameraFreeLook(0.0f);
        base.OnEnd();
    }

    public virtual void OnDashState()
{
        //playerController.Player.Rb.AddForce(playerController.transform.forward * dashingVelocity, ForceMode.VelocityChange);
        playerController.Player.Rb.velocity = playerController.transform.forward * dashingVelocity;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        dashingTimer -= Time.deltaTime;
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
