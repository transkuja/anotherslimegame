using UnityEngine;

/*
 * Respawn points should be placed properly on entries/exits where we want the player to respawn
 */
public class RespawnPoint : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            other.GetComponent<Player>().respawnPoint = transform;
    }
}
