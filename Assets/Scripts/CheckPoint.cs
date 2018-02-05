using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {
    [SerializeField]
    int index = 0;
    [SerializeField]
    bool isStart = false;

    public int Index
    {
        get
        {
            return index;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if(player)
        {
            if(isStart)
                player.respawnPoint = transform;
            else if(player.respawnPoint.GetComponent<CheckPoint>() && player.respawnPoint.GetComponent<CheckPoint>().Index == index-1)
            {
                player.respawnPoint = transform;
            }
        }
    }
}
