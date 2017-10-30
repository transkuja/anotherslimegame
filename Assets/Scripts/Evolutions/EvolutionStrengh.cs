﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

// utils 
//GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>();
//GamePad.GetState(playerIndex).Buttons.A;

public class EvolutionStrengh : EvolutionComponent
{
    [SerializeField]float downDashPower = 100;
    [SerializeField] float maxDashChargeDelay = 0.7f;
    float timer;
    Rigidbody rb;

    // ici je dois savoir tant que x est appuyé. 
    // Quand x est relaché. 

    public void ColorChangeAsupr(Color color)
    {
        Transform slimeBody = null;
        Transform slimeMeshParent = transform.Find("Slime_Test_abilities_02");

        if (slimeMeshParent != null)
            slimeBody = slimeMeshParent.Find("corps");
        if (slimeBody!=null)
        {
            slimeBody.GetComponent<MeshRenderer>().materials[0].color = color;
        }
    }

    public override void  Start()
    {
        base.Start();
        SetPower(Powers.Strengh);
        rb = GetComponent<Rigidbody>();
        playerController.stats.AddBuff(new StatBuff(Stats.StatType.GROUND_SPEED, 0.85f, -1));
    }

    // Prepare caracter for strengh dashing
    public void DashStart()
    {
        // stop mouvement
        ColorChangeAsupr(Color.red);
        if (rb == null)
        {
            Debug.Log("Error : missing Rb on player");
            return;
        }
        rb.velocity = Vector3.zero;

        // stop jump
        JumpManager jumpManager = GetComponent<JumpManager>();
        if (jumpManager != null)
        {
            jumpManager.Stop();
        }
        timer = 0;
    }

    // Force dash after maxDashChargeDelay
    public void Levitate()
    {
        timer += Time.deltaTime;
        if (timer > maxDashChargeDelay)
        {
            LaunchDash();
            GetComponent<PlayerController>().StrenghState = SkillState.Dashing;
        }
    }

    // Apply a speed towards the ground
    public void LaunchDash()
    {
        Vector3 downPush = Vector3.down * downDashPower;
        rb.velocity = downPush; // Override current velocity. 
    }

    /*
     *
     * Cf Player Collision Center
     */
    //public override void OnCollisionEnter(Collision coll)
    //{
    //    base.OnCollisionEnter(coll);
    //    Rigidbody otherRb;
    //    if (GetComponent<PlayerController>().StrenghState == SkillState.Dashing)
    //    {
    //        if (coll.transform.GetComponent<Player>() != null)
    //        {
    //            otherRb = coll.transform.GetComponent<Rigidbody>();
    //            Vector3 direction = coll.contacts[0].point - rb.position;
    //            direction.y = 0;
    //            direction.Normalize();
    //            otherRb.AddForce(direction * expulseDashPower, ForceMode.Impulse);
    //        }
    //    }
       
    //}

    // Specific onCollisionStay
    public override void OnCollisionStay(Collision coll)
    {
        base.OnCollisionStay(coll);
        if (GetComponent<PlayerController>().StrenghState == SkillState.Dashing && playerController.IsGrounded)
        {
            GetComponent<PlayerController>().StrenghState = SkillState.Ready;
            ColorChangeAsupr(Color.white);
        }
    }
}
