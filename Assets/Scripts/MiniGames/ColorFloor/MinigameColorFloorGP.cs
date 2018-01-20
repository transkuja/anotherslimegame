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
            if (thisCollider.transform.childCount > 0)
            {
                ColorFloorPickUp pickupComponent = thisCollider.transform.GetChild(0).GetComponent<ColorFloorPickUp>();
                if (pickupComponent.pickupType == ColorFloorPickUpType.Score)
                    ColorFloorHandler.ScorePoints((int)pc.playerIndex);

                Destroy(pickupComponent.gameObject);
                pickupHandler.pickupSpawned--;
            }

            ColorFloorHandler.RegisterFloor((int)pc.playerIndex, thisCollider);
            thisCollider.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[(int)pc.playerIndex]);
        }
    }

}
