using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    [SerializeField]
    Vector3 torque = new Vector3(0.1f, 0.8f, 0.1f);
    [SerializeField]
    float rotationSpeed = 50.0f;
    //public float timerRotation = 0.0f;
    //public float[] vectorActions;
    Rigidbody rb;
    // Use this for initialization

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if(GameManager.CurrentState == GameState.Normal && !GameManager.Instance.isTimeOver)
            rb.AddRelativeTorque(torque * Time.deltaTime * rotationSpeed, ForceMode.VelocityChange);
        //transform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.Self);
        //transform.Rotate(Vector3.right * 2 * Time.deltaTime, Space.Self);
    }
}
