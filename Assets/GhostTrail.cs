using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<PlayerController>().enabled = false;
        //Ultra temporary: Need to relieve restrictions when the player hits something
        other.GetComponentInParent<Rigidbody>().velocity = other.transform.forward.normalized * other.GetComponent<PlayerController>().stats.Get(Stats.StatType.GROUND_SPEED);
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PlayerController>().enabled = true;
    }
}
