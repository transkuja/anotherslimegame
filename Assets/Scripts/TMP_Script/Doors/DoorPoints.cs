using UnityEngine;

public class DoorPoints : MonoBehaviour, IDoor {

    public GameObject doorToOPen;
    public int requieredAmountPoints = 270;

    private Color myColor;

    public void Start()
    {
        myColor = doorToOPen.GetComponent<MeshRenderer>().material.color;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null && doorToOPen.activeSelf)
        {
            if (other.GetComponent<Player>().Collectables[(int)CollectableType.Points] >= requieredAmountPoints)
            {
                other.GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, -requieredAmountPoints);
                OpenDoor();
            } else
            {
                ChangeColor(Color.red);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null && doorToOPen.activeSelf)
        {
            ChangeColor(myColor);
        }
    }

    public void OpenDoor()
    {
        doorToOPen.SetActive(false);
    }

    public void ChangeColor(Color c)
    {
        doorToOPen.GetComponent<MeshRenderer>().material.color = c;
    }
}
