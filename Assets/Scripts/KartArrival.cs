using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KartArrival : MonoBehaviour {
    [HideInInspector]
    public int NumberOfLaps;

    [SerializeField]
    CheckPoint lastCheckpoint;

    private void Start()
    {
        ((KartGameMode)GameManager.Instance.CurrentGameMode).Arrival = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        PlayerControllerKart pck = player.GetComponent<PlayerControllerKart>();
        float angle = Vector3.Angle(transform.forward, other.gameObject.GetComponent<Rigidbody>().velocity);
        if (player && !GetComponent<EnnemyController>() && angle < 90.0f && pck)
        {
            if(player.respawnPoint == lastCheckpoint.transform && pck.laps < NumberOfLaps)
            {
                if(++pck.laps < NumberOfLaps)
                    //Don't forget to change this
                    player.CallOnValueChange(PlayerUIStat.Laps, pck.laps + 1);
            }
            if (pck.laps >= NumberOfLaps)
            {
                //The current player finished the race, disable his controls and save the scores
                player.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.FinishedRace;
                GameManager.Instance.CurrentGameMode.PlayerHasFinished(player);
            }
        }
    }
}
