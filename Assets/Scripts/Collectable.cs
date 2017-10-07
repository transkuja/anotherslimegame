using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    CollectableType type;
    [SerializeField]
    int value;

    private void OnTriggerEnter(Collider other)
    {
         PickUp(other.GetComponent<Player>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickUp(collision.gameObject.GetComponent<Player>());
    }

    private void PickUp(Player player)
    {
        if (player)
        {
            if (player.Collectables[(int)type] < Utils.GetMaxValueForCollectable(type))
            {
                player.UpdateCollectableValue(type, value);
                Destroy(this.gameObject);
            }
        }
    }
}
