using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrainedByGhostState : PlayerState
{
    public RestrainedByGhostState(PlayerController _playerController) : base(_playerController)
    {
    }

    public override void Move(Vector3 initialVelocity)
    {
        playerController.GetComponent<Rigidbody>().velocity = playerController.transform.forward * playerController.stats.Get(Stats.StatType.GROUND_SPEED) + Vector3.down;
    }

    public override void OnJumpPressed()
    {
    }

    public override void OnDashPressed()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        
    }
}