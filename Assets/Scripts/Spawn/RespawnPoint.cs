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


public class Respawner
{
    /*
     * Contains specific respawn rules
     */
    public static void RespawnProcess(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Can't respawn player: Trying to respawn a null player!");
            return;
        }
        if (player.respawnPoint == null)
        {
            Debug.LogError("Can't respawn player: Respawn point is null!");
            return;
        }
        player.transform.position = player.respawnPoint.position;
        player.transform.rotation = player.respawnPoint.rotation;
    }
}
