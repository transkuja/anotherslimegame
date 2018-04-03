using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class DashDownState : PlayerState
{
    float timer;
    float maxDashChargeDelay;
    float downDashPower;

    public int nbDashDownMade = 0;
        
    public DashDownState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        playerCharacterHub.Rb.velocity = Vector3.zero;

        JumpManager jumpManager = playerCharacterHub.GetComponent<JumpManager>();
        if (jumpManager != null)
            jumpManager.Stop();
        timer = 0;
        maxDashChargeDelay = .7f;
        downDashPower = 100f;
        curUpdateFct = Levitate;
    }
    public override void OnEnd()
    {
        if (playerCharacterHub.IsGrounded) nbDashDownMade = 0;
        base.OnEnd();

    }
    public void Levitate()
    {
        timer += Time.deltaTime;
        if (timer > maxDashChargeDelay || (playerCharacterHub.GetComponent<PlayerControllerHub>() && playerCharacterHub.GetComponent<PlayerControllerHub>().State.Buttons.Y == ButtonState.Released))
        {
            curUpdateFct = OnDash;
            timer = 0;
            if (playerCharacterHub.DashParticles && playerCharacterHub.DashParticles)
                playerCharacterHub.DashParticles.Play();
        }
    }
    // TMP debug pour que ça marche , A refaire en moins lourd

    // Apply a speed towards the ground
    public void OnDash()
    {
        Vector3 downPush = Vector3.down * downDashPower;
        playerCharacterHub.Rb.velocity = downPush; // Override current velocity. 
        timer += Time.deltaTime;
        if (playerCharacterHub.IsGrounded || timer > 2)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
        }

        // TMP debug pour que ça marche , A refaire en moins lourd
        Collider[] coll = Physics.OverlapSphere(playerCharacterHub.transform.position - Vector3.up, 2f);
        if (coll!=null)
        for (int i = 0;i < coll.Length;i++)
        {
            if (coll[i].CompareTag("HardBreakable"))
                {
                    coll[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    coll[i].gameObject.transform.parent = null;
                    coll[i].GetComponent<Rigidbody>().velocity = playerCharacterHub.Rb.velocity*1.2f;
                }
        }

    }
    public override void HandleGravity()
    {
    }
    public override void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
    {
    }
    public override void OnDownDashPressed()
    {
    }
    public override void OnDashPressed()
    {
    }
}