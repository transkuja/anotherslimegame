using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

// utils 
//GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>();
//GamePad.GetState(playerIndex).Buttons.A;
public enum DashState
{
    NONE,CHARGING,DASHING
}

public class EvolutionStrengh : EvolutionComponent
{
    [SerializeField]float downDashPower = 100;
    [SerializeField] float expulseDashPower = 60;
    [SerializeField] float maxDashChargeDelay = 0.7f;
    float timer;
    Rigidbody rb;
    DashState dashState;

    // ici je dois savoir tant que x est appuyé. 
    // Quand x est relaché. 

    public void ColorChangeAsupr(Color color)
    {
        Transform slimeBody = null;
        string slimeName = "Slime_0"+( ((int)playerController.playerIndex)+1);
        Transform slimeMeshParent = transform.Find(slimeName);

        if (slimeMeshParent != null)
            slimeBody = slimeMeshParent.Find("Slime_Body");
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
        Debug.Log("I have Strengh");
    }
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
        playerController.isGravityEnabled = false;
        playerController.canJump = false;
        playerController.canMoveXZ = false;
        timer = 0;
        dashState = DashState.CHARGING;
    }
    public void Levitate()
    {
        timer += Time.deltaTime;
        if (timer > maxDashChargeDelay)
            LauchDash();
    }
    public void LauchDash()
    {
        Vector3 downPush = Vector3.down * downDashPower;
        rb.velocity = downPush; // Override current velocity. 
        playerController.isGravityEnabled = true;
        dashState = DashState.DASHING;
    }
    public override void Update()
    {
        base.Update();
        if (isSpecialActionPushedOnce && dashState != DashState.DASHING)
            DashStart();
        if (isSpecialActionPushed && dashState == DashState.CHARGING)
            Levitate();
        if (isSpecialActionReleased && dashState != DashState.DASHING)
            LauchDash();
    }

    public override void OnCollisionEnter(Collision coll)
    {
        base.OnCollisionEnter(coll);
        Rigidbody otherRb;
        if (dashState == DashState.DASHING)
        {
            if (coll.transform.GetComponent<Player>() != null)
            {
                otherRb = coll.transform.GetComponent<Rigidbody>();
                Vector3 direction = coll.contacts[0].point - rb.position;
                direction.y = 0;
                direction.Normalize();
                otherRb.AddForce(direction * expulseDashPower, ForceMode.Impulse);
            }
        }
       
    }
    public override void OnCollisionStay(Collision coll)
    {
        base.OnCollisionStay(coll);
        if (playerController.IsGrounded)
        {
            dashState = DashState.NONE;
            playerController.canJump = true;
            playerController.canMoveXZ = true;
            ColorChangeAsupr(Color.white);
        }
    }
}
