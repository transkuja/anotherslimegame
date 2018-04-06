using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMinigameTrigger : MonoBehaviour
{
    HubMinigameHandler handler = null;
    PNJDefaultMessage defaultmessage = null;

    // TMP
    bool old_is_happy = false;

    public void Start()
    {
        handler = GetComponentInParent<HubMinigameHandler>();
        defaultmessage = GetComponentInParent<PNJDefaultMessage>();
        old_is_happy = GetComponentInParent<PNJController>().isHappy;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().IsUsingAController)
        {
            if (GameManager.CurrentState == GameState.Normal)
            {
                // TMP
                GetComponentInParent<PNJController>().isHappy = false;

                if (handler && !handler.hasBeenStarted && !DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(handler.id))
                {
                    other.GetComponent<Player>().RefHubMinigameHandler = GetComponentInParent<HubMinigameHandler>();
                    handler.CreateUIHubMinigame((int)other.GetComponent<PlayerController>().PlayerIndex);
                }
                else if (
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
                // TMP
                GetComponentInParent<PNJController>().isHappy = old_is_happy;

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
