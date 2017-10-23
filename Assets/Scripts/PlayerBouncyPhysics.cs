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

    [SerializeField]
    [Range(0.0f, 2.5f)]
    float impactForce;

    [SerializeField]
    [Range(0.0f, 40.0f)]
    float impactPropagationThreshold;

    PlayerController playerController;
    Rigidbody rb;

    PlayerController _PlayerController
    {
        get
        {
            if (playerController == null)
                playerController = GetComponent<PlayerController>();
            return playerController;
        }
    }

    Rigidbody _Rb
    {
        get
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            return rb;
        }
    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            if ((transform.position.y - collision.transform.position.y) > bounceDetectionThreshold)
            {
                _Rb.velocity += Vector3.up * bounceStrength;
                _PlayerController.canDoubleJump = true;
            }
            else
            {
                if (_Rb.velocity.magnitude > collision.gameObject.GetComponent<Player>().Rb.velocity.magnitude)
                {
                    if (_Rb.velocity.magnitude > impactPropagationThreshold)
                        collision.gameObject.GetComponent<Player>().Rb.velocity += (_Rb.velocity * impactForce);
                }
                else
                {
                    if (collision.gameObject.GetComponent<Player>().Rb.velocity.magnitude > impactPropagationThreshold)
                        _Rb.velocity += (collision.gameObject.GetComponent<Player>().Rb.velocity * impactForce);
                }

            }
        }
    }
}
