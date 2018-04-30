using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMinigameTrigger : MonoBehaviour {

    public BobBehavior refBobBehavior;

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag =="Player" && other.GetComponent<PlayerControllerHub>() && refBobBehavior != null)
        {
            if (refBobBehavior.IsMinigameStarted() && !DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>("Cowboy"))
            {
                refBobBehavior.WinMinigame();

                for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                {
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>().Rb.drag = 25.0f;
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>().Rb.velocity = Vector3.zero;
                }
 
            }

        }
    }
}
