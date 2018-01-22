using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class DashDownState : PlayerHubState
{
    float timer;
    float maxDashChargeDelay;
    float downDashPower;

    public int nbDashDownMade = 0;
        
    public DashDownState(PlayerControllerHub _playerController) : base(_playerController)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        playerController.Rb.velocity = Vector3.zero;

        JumpManager jumpManager = playerController.GetComponent<JumpManager>();
        if (jumpManager != null)
            jumpManager.Stop();
        timer = 0;
        maxDashChargeDelay = .7f;
        downDashPower = 100f;
        curUpdateFct = Levitate;
    }
    public override void OnEnd()
    {
        if (playerController.IsGrounded) nbDashDownMade = 0;
        base.OnEnd();

    }
    public void Levitate()
    {
        timer += Time.deltaTime;
        if (timer > maxDashChargeDelay || playerController.State.Buttons.Y == ButtonState.Released)
        {
            curUpdateFct = OnDash;
            timer = 0;
            if (playerController.dashParticles && playerController.dashParticles.GetComponent<ParticleSystem>())
                playerController.dashParticles.GetComponent<ParticleSystem>().Play();
        }
    }
    // TMP debug pour que ça marche , A refaire en moins lourd

    // Apply a speed towards the ground
    public void OnDash()
    {
        Vector3 downPush = Vector3.down * downDashPower;
        playerController.Rb.velocity = downPush; // Override current velocity. 
        timer += Time.deltaTime;
        if (playerController.IsGrounded || timer > 2)
        {
            playerController.PlayerState = playerController.freeState;
        }

        // TMP debug pour que ça marche , A refaire en moins lourd
        Collider[] coll = Physics.OverlapSphere(playerController.transform.position - Vector3.up, 2f);
        if (coll!=null)
        for (int i = 0;i < coll.Length;i++)
        {
            if (coll[i].CompareTag("HardBreakable"))
                {
                    coll[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    coll[i].gameObject.transform.parent = null;
                    coll[i].GetComponent<Rigidbody>().velocity = playerController.Rb.velocity*1.2f;
                }
        }

    }
    public override void HandleGravity()
    {
    }
    public override void Move(Vector3 initialVelocity)
    {
    }
    public override void OnDownDashPressed()
    {
    }
    public override void OnDashPressed()
    {
    }
}