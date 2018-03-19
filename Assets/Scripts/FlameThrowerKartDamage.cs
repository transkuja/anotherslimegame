using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerKartDamage : MonoBehaviour {

    enum Direction
    {
        Left,
        Right
    }

    [SerializeField]
    float expulsionForce = 150.0f;

    [SerializeField]
    Direction expulseDirection;

    private void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<PlayerControllerKart>())
        {
            other.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.Hit;
            Vector3 expulseDir;

            if (expulseDirection == Direction.Left)
            {
                expulseDir = (transform.localToWorldMatrix * Vector3.left).normalized;
            }
            else
            {
                expulseDir = (transform.localToWorldMatrix * Vector3.right).normalized;
            }

            other.GetComponent<Rigidbody>().AddForce(expulseDir * expulsionForce + transform.up * expulsionForce / 2.0f, ForceMode.Impulse);
        }
        else if (other.GetComponent<AIRabite>())
        {
            other.GetComponent<AIRabite>().CurrentState = AIRabite.RabiteState.Dead;
        }
    }
}
