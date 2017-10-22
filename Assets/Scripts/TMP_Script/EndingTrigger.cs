using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            other.GetComponent<PlayerController>().enabled = false;

            // Making the player to stop in the air 
            other.GetComponent<Rigidbody>().Sleep();

            GameManager.scoreScreenReference.GetComponent<ScoreScreen>().rank++;
            GameManager.scoreScreenReference.GetComponent<ScoreScreen>().RefreshScores(other.GetComponent<Player>());
            GameManager.scoreScreenReference.gameObject.SetActive(true);
        }
    }
}
