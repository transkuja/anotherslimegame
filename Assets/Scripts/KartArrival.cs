using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KartArrival : MonoBehaviour {
    [SerializeField]
    public int NumberOfLaps = 3;

    [SerializeField]
    CheckPoint lastCheckpoint;

    private void Start()
    {
        ((KartGameMode)GameManager.Instance.CurrentGameMode).Arrival = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        float angle = Vector3.Angle(transform.forward, other.gameObject.GetComponent<Rigidbody>().velocity);
        if (player && angle < 90.0f )
        {
            if(player.respawnPoint == lastCheckpoint.transform && player.NbPoints < 3)
            {
                player.NbPoints++;
                player.CallOnValueChange(PlayerUIStat.Points, player.NbPoints);
            }
            if (player.NbPoints >= NumberOfLaps)
            {
                //The current player finished the race, disable his controls and save the scores
                player.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.FinishedRace;
                GameManager.Instance.CurrentGameMode.PlayerHasFinished(player);
            }
        }
    }
}
