using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {
    [SerializeField]
    int index = 0;
    [SerializeField]
    bool isStart = false;
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if(player)
        {
            if(isStart)
                player.respawnPoint = transform;
            else
            {
                player.respawnPoint = transform;
            }
        }
    }
}
