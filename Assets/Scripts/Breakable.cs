using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.transform.GetComponent<PlayerController>();
        if (player != null && (
                player.PlayerState == player.dashState
                || player.PlayerState == player.downDashState
                )
           )
        {
            // impact
            // pool de morceaux cassés
        }
    }
}
