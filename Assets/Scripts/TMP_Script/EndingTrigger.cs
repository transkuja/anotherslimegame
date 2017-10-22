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

            GameManager.Instance.ScoreScreenReference.rank++;
            GameManager.Instance.ScoreScreenReference.RefreshScores(other.GetComponent<Player>());
            GameManager.Instance.ScoreScreenReference.gameObject.SetActive(true);
        }
    }
}
