using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartArrival : MonoBehaviour {
    [SerializeField]
    int NumberOfLaps = 3;

    [SerializeField]
    CheckPoint lastCheckpoint;
    Dictionary<Player, int> dicLapsPerPlayer;

    private void Start()
    {
        dicLapsPerPlayer = new Dictionary<Player, int>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        float angle = Vector3.Angle(transform.forward, other.gameObject.GetComponent<Rigidbody>().velocity);
        if (player && angle < 90.0f )
        {
            
            if(!dicLapsPerPlayer.ContainsKey(player))
            {
                dicLapsPerPlayer.Add(player, 0);
            }
            else if(player.respawnPoint == lastCheckpoint.transform)
            {
                dicLapsPerPlayer[player]++;
                Debug.Log("Lap" + dicLapsPerPlayer[player] + " for " + player.name + "!");
                if (dicLapsPerPlayer[player] >= NumberOfLaps)
                {
                    //The current player finished the race, disable his controls and save the scores
                    player.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.FinishedRace;
                    GameManager.Instance.CurrentGameMode.PlayerHasFinished(player);
                }
            }
        }
    }
}
