using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KartArrival : MonoBehaviour {
    [SerializeField]
    public int NumberOfLaps = 3;

    [SerializeField]
    CheckPoint lastCheckpoint;
    int[] playerLaps;

    private void Start()
    {
        playerLaps = new int[4];
        ((KartGameMode)GameManager.Instance.CurrentGameMode).Arrival = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        float angle = Vector3.Angle(transform.forward, other.gameObject.GetComponent<Rigidbody>().velocity);
        if (player && angle < 90.0f )
        {
            if(player.respawnPoint == lastCheckpoint.transform)
            {
                playerLaps[player.ID]++;
                player.CallOnValueChange(PlayerUIStat.Laps, playerLaps[player.ID]);
                if (playerLaps[player.ID] >= NumberOfLaps)
                {
                    //The current player finished the race, disable his controls and save the scores
                    player.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.FinishedRace;
                    GameManager.Instance.CurrentGameMode.PlayerHasFinished(player);
                }
            }
        }
    }
}
