using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

    public PlayerController owner;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if(player && player != owner)
        {
            if(player.PlayerState == player.restrainedByGhostState)
            {
                ((RestrainedByGhostState)(player.PlayerState)).ResetTimer();
            }
            else
            {
                player.PlayerState = player.restrainedByGhostState;
            }
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player && player != owner)
        {
            if (player.PlayerState == player.restrainedByGhostState)
            {
                ((RestrainedByGhostState)(player.PlayerState)).ResetTimer();
            }
            else
            {
                player.PlayerState = player.restrainedByGhostState;
            }
        }
    }
}
