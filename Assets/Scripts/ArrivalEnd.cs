using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivalEnd : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player!=null)
        {
            GameManager.Instance.CurrentGameMode.PlayerHasFinished(player);
        }
    }
}
