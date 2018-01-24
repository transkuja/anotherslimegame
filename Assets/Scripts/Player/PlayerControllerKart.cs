using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class PlayerControllerKart : PlayerController {

    [SerializeField]
    float forwardSpeed = 5000.0f;
    [SerializeField]
    float turnSpeed = 6000.0f;
    GamePadState previousState;
    Vector3 targetForward;

    void Start () {
        player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();
        targetForward = transform.forward;
        previousState = GamePad.GetState(PlayerIndex);
    }

	void Update () {
        GamePadState state = GamePad.GetState(PlayerIndex);
        rb.AddForce(transform.forward * Time.deltaTime * forwardSpeed * state.Triggers.Right);

        rb.AddForce(-transform.forward * Time.deltaTime * forwardSpeed/2.0f * state.Triggers.Left);

        if(state.Buttons.X == ButtonState.Pressed && previousState.Buttons.X == ButtonState.Released)
        {
            rb.AddForce(transform.forward * 40.0f, ForceMode.Impulse);
        }

        targetForward = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);
        if (targetForward == Vector3.zero)
            targetForward = transform.forward;
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, targetForward, Time.deltaTime * 6.0f * Mathf.Clamp(rb.velocity.magnitude/2.0f, 0.0f, 1.0f)), Vector3.up);
        //transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, rb.velocity.normalized, Time.deltaTime * 10.0f));
        rb.AddForce(Vector3.down * Time.deltaTime * 4000.0f);
        previousState = state;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerControllerKart>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 10.0f, ForceMode.Impulse);
            Rb.AddForce(collision.contacts[0].normal * collision.impulse.magnitude, ForceMode.Impulse);
        }
    }
}
