﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMinigameTrigger : MonoBehaviour
{
    BobBehavior handler = null;
    PNJDefaultMessage defaultmessage = null;

    // TMP
    bool old_is_happy = false;

    public void Start()
    {
        handler = GetComponentInParent<BobBehavior>();
        defaultmessage = GetComponentInParent<PNJDefaultMessage>();
        if (GetComponentInParent<PNJController>() != null)
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

                if ((handler && !handler.IsMinigameStarted() && defaultmessage)
                  || (!handler && defaultmessage))
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

                if (defaultmessage)
                {
                    other.GetComponent<Player>().RefMessage = null;
                    defaultmessage.DestroyUIMessage((int)other.GetComponent<PlayerController>().PlayerIndex);
                }
            }
    }

}
