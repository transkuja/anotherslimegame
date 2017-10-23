using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

// utils 
//GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>();
//GamePad.GetState(playerIndex).Buttons.A;

public class EvolutionStrengh : EvolutionComponent
{
    [SerializeField]float downDashPower = 100;
    [SerializeField] float expulseDashPower = 60;
    bool isDownDashing = false;
    Rigidbody rb;


    public void Start()
    {
        SetPower(Powers.Strengh);
        rb = GetComponent<Rigidbody>();
        Debug.Log("I am Strengh");
    }
  
    public override void SpecialAction()
    {
        base.SpecialAction();
     
        JumpManager jumpManager = GetComponent<JumpManager>();
        if (rb == null)
        {
            Debug.Log("Error : missing Rb on player");
            return;
        }
        isDownDashing = true;
        Vector3 downPush = Vector3.down * downDashPower;
        rb.velocity = downPush; // Override current velocity. 
        //rb.AddForce(downPush, ForceMode.Impulse); // doesn't overide current velocity;
        if (jumpManager!=null)
        {
            jumpManager.Stop();
        }
    }
    public override void OnCollisionEnter(Collision coll)
    {
        base.OnCollisionEnter(coll);
        Rigidbody otherRb;
        if (isDownDashing)
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
        if (playerController.IsGrounded)
        {
            isDownDashing = false;
        }
    }
}
