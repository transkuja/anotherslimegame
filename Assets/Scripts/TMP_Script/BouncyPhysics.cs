using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPhysics : MonoBehaviour {
    [SerializeField]
    [Range(10.0f, 2000.0f)]
    float bounceStrength = 50.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float bounceDetectionThreshold = 0.2f;

    int evolutionMultiplier = 1;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            if ((collision.transform.position.y - transform.position.y) > bounceDetectionThreshold)
            {
                //Powers.DoubleJump change to agility
                evolutionMultiplier = (collision.gameObject.GetComponent<Player>().GetComponent<EvolutionComponent>() != null && collision.gameObject.GetComponent<Player>().GetComponent<EvolutionComponent>().Evolution.Id == (int)Powers.DoubleJump) ?  2 :  1;
                collision.gameObject.GetComponent<Player>().Rb.velocity += Vector3.up * bounceStrength* evolutionMultiplier;
                collision.gameObject.GetComponent<Player>().CanDoubleJump = true;
            }
        }
    }
}
