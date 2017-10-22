using UnityEngine;

public class EscapeModeDoor : MonoBehaviour, IDoor {

    public int requieredAmountKeys = 3;

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            if (other.GetComponent<Player>().Collectables[(int)CollectableType.Key] == requieredAmountKeys) {
                OpenDoor();
            }
        }
    }

    public void OpenDoor()
    {
        transform.parent.parent.gameObject.SetActive(false);
    }
}
