using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnColoredFloorTrigger : MonoBehaviour {

    ColorFloorPickupHandler pickupHandler;

    public int currentOwner = -1;
    public bool debug;

    OnColoredFloorTrigger[] neighbors = new OnColoredFloorTrigger[4];
    enum Side { Up, Down, Left, Right }

    void InitUp()
    {
        if (transform.parent.GetSiblingIndex() == 0)
        {
            neighbors[(int)Side.Up] = null;
        }
        else
        {
            neighbors[(int)Side.Up] = transform.parent.parent.GetChild(transform.parent.GetSiblingIndex() - 1).GetChild(transform.GetSiblingIndex()).GetComponentInChildren<OnColoredFloorTrigger>();
        }
    }

    void InitDown()
    {
        if (transform.parent.GetSiblingIndex() == 7)
            neighbors[(int)Side.Down] = null;
        else
            neighbors[(int)Side.Down] = transform.parent.parent.GetChild(transform.parent.GetSiblingIndex() + 1).GetChild(transform.GetSiblingIndex()).GetComponentInChildren<OnColoredFloorTrigger>();
    }

    void InitLeft()
    {
        if (transform.GetSiblingIndex() == 0)
            neighbors[(int)Side.Left] = null;
        else
            neighbors[(int)Side.Left] = transform.parent.GetChild(transform.GetSiblingIndex() - 1).GetComponentInChildren<OnColoredFloorTrigger>();
    }

    void InitRight()
    {
        if (transform.GetSiblingIndex() == 7)
            neighbors[(int)Side.Right] = null;
        else
            neighbors[(int)Side.Right] = transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponentInChildren<OnColoredFloorTrigger>();
    }

    public int SameColorNeighbors()
    {
        int result = 0;
        if (neighbors[(int)Side.Up] != null && neighbors[(int)Side.Up].currentOwner == currentOwner) ++result;
        if (neighbors[(int)Side.Down] != null && neighbors[(int)Side.Down].currentOwner == currentOwner) ++result;
        if (neighbors[(int)Side.Left] != null && neighbors[(int)Side.Left].currentOwner == currentOwner) ++result;
        if (neighbors[(int)Side.Right] != null && neighbors[(int)Side.Right].currentOwner == currentOwner) ++result;
        return result;
    }

    private void Start()
    {
        InitUp();
        InitDown();
        InitRight();
        InitLeft();
        currentOwner = -1;
    }

    public int GetFloorIndex()
    {
        return transform.GetSiblingIndex() + transform.parent.GetSiblingIndex() * 8;
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

    public OnColoredFloorTrigger Up
    {
        get
        {
            return neighbors[(int)Side.Up];
        }
    }

    public OnColoredFloorTrigger Down
    {
        get
        {
            return neighbors[(int)Side.Down];
        }
    }

    public OnColoredFloorTrigger Left
    {
        get
        {
            return neighbors[(int)Side.Left];
        }
    }

    public OnColoredFloorTrigger Right
    {
        get
        {
            return neighbors[(int)Side.Right];
        }
    }

    public OnColoredFloorTrigger[] Neighbors
    {
        get
        {
            return neighbors;
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
