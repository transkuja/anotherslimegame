using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KartArrival : MonoBehaviour {
    [SerializeField]
    public int NumberOfLaps = 3;

    [SerializeField]
    CheckPoint lastCheckpoint;
    Dictionary<Player, int> dicLapsPerPlayer;

    private void Start()
    {
        dicLapsPerPlayer = new Dictionary<Player, int>();
        ((KartGameMode)GameManager.Instance.CurrentGameMode).Arrival = this;
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
                player.CallOnValueChange(PlayerUIStat.Laps, dicLapsPerPlayer[player]);
            }
            else if(player.respawnPoint == lastCheckpoint.transform)
            {
                dicLapsPerPlayer[player]++;
                player.CallOnValueChange(PlayerUIStat.Laps, dicLapsPerPlayer[player]);
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
