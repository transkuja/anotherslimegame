using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameColorFloorGP : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponentInParent<PlayerController>() != null)
        {
            PlayerController pc = collision.transform.GetComponentInParent<PlayerController>();
            ColorFloorHandler.RegisterFloor((int)pc.playerIndex, collision.contacts[0].thisCollider);
            collision.contacts[0].thisCollider.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[(int)pc.playerIndex]);
        }
    }


    // TODO: keep lists in a scene specific script
    //RegisterSelfToPlayer(int _playerIndex)

    //UnregisterSelfToPlayer(int _playerIndex)
}
