using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

    public Player owner;

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Player>() != owner)
            other.GetComponent<PlayerController>().PlayerState = other.GetComponent<PlayerController>().restrainedByGhostState;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != owner)
            other.GetComponent<PlayerController>().PlayerState = other.GetComponent<PlayerController>().freeState;
    }
}
