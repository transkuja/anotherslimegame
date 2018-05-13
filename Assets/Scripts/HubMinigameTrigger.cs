using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMinigameTrigger : MonoBehaviour
{
    MinigameTriggerGiverBehavior handler = null;
    PNJMessage defaultmessage = null;

    PNJController pc;

    // TMP
    bool old_is_happy = false;

    public void Start()
    {
        handler = GetComponentInParent<MinigameTriggerGiverBehavior>();
        defaultmessage = GetComponentInParent<PNJMessage>();

        // TMP
        pc = GetComponentInParent<PNJController>();
        if (pc != null)
            old_is_happy = pc.isHappy;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().IsUsingAController)
        {
            if (GameManager.CurrentState == GameState.Normal)
            {
                // TMP
                if (pc != null)
                    pc.isHappy = false;
                //

                if ((handler && !handler.IsMinigameStarted() && defaultmessage)
                  || (!handler && defaultmessage))
                {
                    other.GetComponent<Player>().RefMessage = defaultmessage;
                    defaultmessage.OnEnterTrigger((int)other.GetComponent<PlayerController>().PlayerIndex);
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
                if (pc != null)
                    pc.isHappy = old_is_happy;
                //

                if (defaultmessage)
                {
                    other.GetComponent<Player>().RefMessage = null;
                    defaultmessage.OnExitTrigger((int)other.GetComponent<PlayerController>().PlayerIndex);
                }
            }
    }
}
