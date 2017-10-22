using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            // TODO : Handle multiple player -> pause
            GameManager.ChangeState(GameState.Paused);
            GameManager.scoreScreenReference.GetComponent<ScoreScreen>().RefreshScores();
            GameManager.scoreScreenReference.gameObject.SetActive(true);
        }
    }
}
