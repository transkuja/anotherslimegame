using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMinigameTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().IsUsingAController)
        {
            if (GameManager.CurrentState == GameState.Normal)
            {
                if(!GetComponentInParent<HubMinigameHandler>().hasBeenStarted && !DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(GetComponentInParent<HubMinigameHandler>().id))
                {
                    other.GetComponent<Player>().RefHubMinigameHandler = GetComponentInParent<HubMinigameHandler>();
                    GetComponentInParent<HubMinigameHandler>().CreateUIHubMinigame((int)other.GetComponent<PlayerController>().PlayerIndex);

                }
           
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().IsUsingAController)
            if (GameManager.CurrentState == GameState.Normal)
            {
                other.GetComponent<Player>().RefHubMinigameHandler = null;
                GetComponentInParent<HubMinigameHandler>().DestroyUIMinigame((int)other.GetComponent<PlayerController>().PlayerIndex);
            }
    }

}
