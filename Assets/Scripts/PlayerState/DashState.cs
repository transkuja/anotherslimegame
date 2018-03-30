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
    [Range(0.0f, 2.5f)]
    float impactForce;
    [SerializeField]
    [Range(0.0f, 40.0f)]
    float impactPropagationThreshold;

    public int nbDashMade = 0;

    public DashState(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub) : base(_playerCharacterHub, _playerControllerHub)
    {
        maxCoolDown = 0.5f;
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (AudioManager.Instance != null && AudioManager.Instance.dashFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.dashFx, 4);

        if (playerCharacterHub.DashParticles && playerCharacterHub.DashParticles)
            playerCharacterHub.DashParticles.Play();
        playerCharacterHub.GetComponent<JumpManager>().Stop();
        dashingVelocity = playerCharacterHub.stats.Get(Stats.StatType.DASH_FORCE);
        dashingMaxTimer = 0.15f;
        dashingTimer = dashingMaxTimer;
        playerCharacterHub.IsGravityEnabled = false;
        playerCharacterHub.Rb.drag = 15.0f;
        CurFixedUpdateFct = OnDashState;

        //playerController.ChangeDampingValuesCameraFreeLook(0.9f);
    }

    public override void OnEnd()
    {
        playerCharacterHub.IsGravityEnabled = true;
        if (playerCharacterHub.IsGrounded) nbDashMade = 0;
        playerCharacterHub.Rb.drag = 0.0f;

        //playerController.ChangeDampingValuesCameraFreeLook(0.0f);
        base.OnEnd();
    }

    public virtual void OnDashState()
    {
        playerCharacterHub.Rb.velocity = playerCharacterHub.transform.forward * dashingVelocity;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        dashingTimer -= Time.deltaTime;
        if (dashingTimer <= 0.0f)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;

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
