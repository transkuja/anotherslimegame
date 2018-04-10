using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMinigameTrigger : MonoBehaviour {

    public HubMinigameHandler refMinigameHandler;

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag =="Player" && other.GetComponent<PlayerControllerHub>() && refMinigameHandler != null)
        {
            if (refMinigameHandler.GetComponent<HubMinigameHandler>().hasBeenStarted && !DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(refMinigameHandler.GetComponent<HubMinigameHandler>().id))
            {
                refMinigameHandler.WinMinigame();

                for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                {
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>().Rb.drag = 25.0f;
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>().Rb.velocity = Vector3.zero;
                }
 
            }

        }
    }
}
