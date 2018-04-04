using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMinigameTrigger : MonoBehaviour {

    public HubMinigameHandler refMinigameHandler;

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag =="Player" && refMinigameHandler != null)
        {
            refMinigameHandler.WinMinigame();
        }
    }
}
