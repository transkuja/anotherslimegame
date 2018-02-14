using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTile : MonoBehaviour {

    [SerializeField]
    float boostForce = 1000.0f;

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponentInParent<PlayerControllerKart>())
        {
            other.GetComponentInParent<Rigidbody>().AddForce(transform.forward * boostForce);
        }
    }
}
