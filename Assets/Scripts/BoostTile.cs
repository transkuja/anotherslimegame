using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTile : MonoBehaviour {

    [SerializeField]
    float boostForce = 50.0f;

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponentInParent<PlayerControllerKart>())
        {
            other.GetComponentInParent<PlayerControllerKart>().DisableClampingForSeconds(0.4f);
            other.GetComponentInParent<Rigidbody>().AddForce(transform.forward * boostForce, ForceMode.VelocityChange);
        }
    }
}
