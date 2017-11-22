using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other)
    {
        GetComponentInParent<Collectable>().PickUp(other.GetComponent<Player>());
    }
}
