using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMinigameTrigger : MonoBehaviour {
    HubMinigameHandler handler = null;
    PNJDefaultMessage defaultmessage = null;
    public void Start()
    {
        handler = GetComponentInParent<HubMinigameHandler>();
        defaultmessage = GetComponentInParent<PNJDefaultMessage>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().IsUsingAController)
        {
            if (GameManager.CurrentState == GameState.Normal)
            {
                if (handler && !handler.hasBeenStarted && !DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(handler.id))
                {
                    other.GetComponent<Player>().RefHubMinigameHandler = GetComponentInParent<HubMinigameHandler>();
                    handler.CreateUIHubMinigame((int)other.GetComponent<PlayerController>().PlayerIndex);
                } else if (
                    (handler && !handler.hasBeenStarted && defaultmessage)
                    || (!handler && defaultmessage)
                )
                {
                    other.GetComponent<Player>().RefMessage = defaultmessage;
                    defaultmessage.CreateUIMessage((int)other.GetComponent<PlayerController>().PlayerIndex);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().IsUsingAController)
            if (GameManager.CurrentState == GameState.Normal)
            {
                if (handler && !DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(handler.id))
                {
                    other.GetComponent<Player>().RefHubMinigameHandler = null;
                    handler.DestroyUIMinigame((int)other.GetComponent<PlayerController>().PlayerIndex);
                }
                else if (defaultmessage)
                {
                    other.GetComponent<Player>().RefMessage = null;
                    defaultmessage.DestroyUIMessage((int)other.GetComponent<PlayerController>().PlayerIndex);
                }
            }
    }

}
