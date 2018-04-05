using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnColoredFloorTrigger : MonoBehaviour {

    ColorFloorPickupHandler pickupHandler;

    public ColorFloorPickupHandler PickupHandler
    {
        get
        {
            if (pickupHandler == null)
                pickupHandler = FindObjectOfType<ColorFloorPickupHandler>();
            return pickupHandler;
        }

        set
        {
            pickupHandler = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponentInParent<PlayerController>() != null)
        {
            PlayerController pc = other.transform.GetComponentInParent<PlayerController>();

            if (transform.childCount > 1)
            {
                MinigamePickUp pickupComponent = transform.GetComponentInChildren<MinigamePickUp>();
                pickupComponent.collectPickup((int)pc.playerIndex);

                Destroy(pickupComponent.gameObject);
                PickupHandler.pickupSpawned--;
            }

            ColorFloorHandler.RegisterFloor((int)pc.playerIndex, GetComponent<Collider>());

            //pc.GetComponent<Rigidbody>().velocity = new Vector3(pc.GetComponent<Rigidbody>().velocity.x, 0.0f, pc.GetComponent<Rigidbody>().velocity.z);
        }
    }
}
