using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour {

    public float speed = 8.0f;
    private Rigidbody rb;
    private float direction = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    public void FixedUpdate()
    {
        rb.AddForce(speed * Vector3.forward * direction, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        direction = -direction;
    }

}
