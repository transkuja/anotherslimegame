using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnColoredFloorTrigger : MonoBehaviour {

    enum FloorState { Normal, AnimLocked }

    ColorFloorPickupHandler pickupHandler;

    public int currentOwner = -1;
    public bool hasAnItem = false;

    public bool debug;

    OnColoredFloorTrigger[] neighbors = new OnColoredFloorTrigger[4];
    enum Side { Up, Down, Left, Right }
    FloorState currentState = FloorState.Normal;
    Material material;

    [SerializeField]
    GameObject warningFeedback;

    public bool IsLocked()
    {
        return currentState == FloorState.AnimLocked;
    }

    public void ScoreFromThisFloor()
    {
        currentState = FloorState.AnimLocked;
        StartCoroutine(AnimScore());
        Invoke("ResetState", 1.0f);
    }

    void ResetState()
    {
        if (currentState == FloorState.AnimLocked)
        {
            currentOwner = -1;
            currentState = FloorState.Normal;
        }
    }

    IEnumerator AnimScore()
    {
        float time = 0.0f;
        Color colorToApply = material.GetColor("_EmissionColor");

        while (colorToApply.maxColorComponent < 3.5f)
        {
            time += Time.deltaTime*3;
            colorToApply *= (1 + time);
            material.SetColor("_EmissionColor", colorToApply);
            yield return new WaitForEndOfFrame();
        }
        time = 0.0f;
        while (colorToApply.maxColorComponent > 0.01f)
        {
            time += Time.deltaTime * 5;
            colorToApply *= 1/(1 + time);
            material.SetColor("_EmissionColor", colorToApply);
            yield return new WaitForEndOfFrame();
        }

        currentOwner = -1;
        currentState = FloorState.Normal;
    }

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

    public bool IsPartOfAnEdge()
    {
        if (SameColorNeighbors() < 4)
            return true;

        int availableSides = SameColorNeighbors();
        if (neighbors[(int)Side.Up].Left != null && neighbors[(int)Side.Up].Left.currentOwner == currentOwner) ++availableSides;
        if (neighbors[(int)Side.Up].Right != null && neighbors[(int)Side.Up].Right.currentOwner == currentOwner) ++availableSides;
        if (neighbors[(int)Side.Down].Left != null && neighbors[(int)Side.Down].Left.currentOwner == currentOwner) ++availableSides;
        if (neighbors[(int)Side.Down].Right != null && neighbors[(int)Side.Up].Right.currentOwner == currentOwner) ++availableSides;

        return availableSides < 8;
    }

    private void Start()
    {
        InitUp();
        InitDown();
        InitRight();
        InitLeft();
        currentOwner = -1;
        material = GetComponentInChildren<MeshRenderer>().material;
    }

    public int GetFloorIndex()
    {
        return transform.GetSiblingIndex() + transform.parent.GetSiblingIndex() * 8;
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

            if (hasAnItem)
            {
                MinigamePickUp pickupComponent = transform.GetComponentInChildren<MinigamePickUp>();
                pickupComponent.collectPickup((int)pc.playerIndex);
                ColorFloorPickupHandler.spawnedPickups.Remove(pickupComponent.gameObject);

                Destroy(pickupComponent.gameObject);
                hasAnItem = false;
            }

            ColorFloorHandler.RegisterFloor((int)pc.playerIndex, GetComponent<OnColoredFloorTrigger>());
            currentOwner = (int)pc.playerIndex;
            //pc.GetComponent<Rigidbody>().velocity = new Vector3(pc.GetComponent<Rigidbody>().velocity.x, 0.0f, pc.GetComponent<Rigidbody>().velocity.z);
        }
    }

    public void WarnPlayerSmthgBadIsComing()
    {
        warningFeedback.SetActive(true);
    }
}
