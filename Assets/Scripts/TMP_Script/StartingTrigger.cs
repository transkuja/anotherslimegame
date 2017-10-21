using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingTrigger : MonoBehaviour {

    public Gamemode gameMode;

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() != null)
        {
            GameManager.CurrentGameMode = GameManager.GameModeManager.GetGameModeByName(gameMode);
            SceneManager.LoadScene(1);
        }
    }
}
