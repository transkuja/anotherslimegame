using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    [SerializeField]
    Vector3 torque = new Vector3(0.1f, 0.8f, 0.1f);
    [SerializeField]
    float rotationSpeed = 50.0f;
    Rigidbody rb;
    // Use this for initialization

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (GameManager.CurrentState == GameState.Normal && !GameManager.Instance.isTimeOver)
        {
            rb.AddRelativeTorque(torque * Time.deltaTime * rotationSpeed, ForceMode.VelocityChange);
        }
    }
}
