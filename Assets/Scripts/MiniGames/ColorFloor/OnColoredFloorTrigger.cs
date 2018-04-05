using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnColoredFloorTrigger : MonoBehaviour {

    ColorFloorPickupHandler pickupHandler;

    public int currentOwner;
    public bool debug;

    Collider up;
    Collider down;
    Collider left;
    Collider right;

    void InitUp()
    {
        if (transform.parent.GetSiblingIndex() == 0)
        {
            up = null;
        }
        else
        {
            up = transform.parent.parent.GetChild(transform.parent.GetSiblingIndex() - 1).GetChild(transform.GetSiblingIndex()).GetComponentInChildren<Collider>();
        }
    }

    void InitDown()
    {
        if (transform.parent.GetSiblingIndex() == 7)
            down = null;
        else
            down = transform.parent.parent.GetChild(transform.parent.GetSiblingIndex() + 1).GetChild(transform.GetSiblingIndex()).GetComponentInChildren<Collider>();
    }

    void InitLeft()
    {
        if (transform.GetSiblingIndex() == 0)
            left = null;
        else
            left = transform.parent.GetChild(transform.GetSiblingIndex() - 1).GetComponentInChildren<Collider>();
    }

    void InitRight()
    {
        if (transform.GetSiblingIndex() == 7)
            right = null;
        else
            right = transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponentInChildren<Collider>();
    }

    private void Start()
    {
        InitUp();
        InitDown();
        InitRight();
        InitLeft();
    }

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

    public Collider Up
    {
        get
        {
            return up;
        }
    }

    public Collider Down
    {
        get
        {
            return down;
        }
    }

    public Collider Left
    {
        get
        {
            return left;
        }
    }

    public Collider Right
    {
        get
        {
            return right;
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
            currentOwner = (int)pc.playerIndex;
            //pc.GetComponent<Rigidbody>().velocity = new Vector3(pc.GetComponent<Rigidbody>().velocity.x, 0.0f, pc.GetComponent<Rigidbody>().velocity.z);
        }
    }
}
