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
        // This shouldn't be necessary
        //if(GhostTrail.DicPlayersInTriggers != null && GhostTrail.DicPlayersInTriggers.ContainsKey(playerController.GetComponent<PlayerController>()))
        //{
        //    if(GhostTrail.DicPlayersInTriggers[playerController.GetComponent<PlayerController>()] <= 0)
        //    {
        //        playerController.PlayerState = playerController.freeState;
        //    }
        //}
        
        playerController.GetComponent<Rigidbody>().velocity = playerController.transform.forward * playerController.stats.Get(Stats.StatType.GROUND_SPEED) + Vector3.down;
    }

    public override void OnJumpPressed()
    {
    }

    public override void OnDashPressed()
    {
    }
}