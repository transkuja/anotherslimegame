using UnityEngine;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            RespawnProcess(other.GetComponent<Player>());
        }
    }

    /*
     * Contains specific respawn rules
     */
    void RespawnProcess(Player player)
    {
        player.transform.position = player.respawnPoint.position;
        player.transform.rotation = player.respawnPoint.rotation;
    }

}
