using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            if(SceneManager.GetActiveScene().name == "SceneMinigamePush")
            {
                MiniGamePushManager.Singleton.ResetPlayer(other.GetComponent<Player>());
            }
            Respawner.RespawnProcess(other.GetComponent<Player>());
        }
    }
}
