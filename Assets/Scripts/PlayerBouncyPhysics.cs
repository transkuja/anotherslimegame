using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBouncyPhysics : MonoBehaviour {

    [SerializeField]
    [Range(10.0f, 2000.0f)]
    float bounceStrength = 25.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float bounceDetectionThreshold = 0.2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>()
                && (transform.position.y - collision.transform.position.y) > bounceDetectionThreshold)
        {
            GetComponent<Player>().Rb.velocity += Vector3.up * bounceStrength;
            GetComponent<PlayerController>().canDoubleJump = true;
        }
    }
}
