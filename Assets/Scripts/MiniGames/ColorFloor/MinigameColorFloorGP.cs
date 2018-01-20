using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameColorFloorGP : MonoBehaviour {

    ColorFloorPickupHandler pickupHandler;

    private void Start()
    {
        pickupHandler = GetComponent<ColorFloorPickupHandler>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponentInParent<PlayerController>() != null)
        {
            PlayerController pc = collision.transform.GetComponentInParent<PlayerController>();
            Collider thisCollider = collision.contacts[0].thisCollider;
            ColorFloorHandler.RegisterFloor((int)pc.playerIndex, thisCollider);
            thisCollider.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[(int)pc.playerIndex]);
            if (thisCollider.transform.childCount > 0)
            {
                Destroy(thisCollider.transform.GetChild(0).gameObject);
                pickupHandler.pickupSpawned--;
            }
        }
    }

}
