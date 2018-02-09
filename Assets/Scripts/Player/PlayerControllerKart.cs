using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class PlayerControllerKart : PlayerController {

    [SerializeField]
    float forwardSpeed = 20000.0f;
    [SerializeField]
    float maxVelocityMagnitude = 20.0f;
    [SerializeField]
    float turnSpeed = 200.0f;
    [SerializeField]
    float dashForce = 100.0f;

    [SerializeField]
    CheckPoint LastCheckpoint;

    GamePadState previousState;
    Vector3 targetForward;

    bool DisableInputs = false;

    void Start () {
        player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();
        targetForward = transform.forward;
        previousState = GamePad.GetState(PlayerIndex);
    }

	void Update () {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.up, -transform.up, out hit))
        {
            if(hit.collider.GetComponent<DeathZone>())
            {
                rb.AddForce(-transform.up * 300.0f + rb.velocity * Time.deltaTime);
                return;
            }
        }
        if (DisableInputs)
            return;
        GamePadState state = GamePad.GetState(PlayerIndex);
        // -1 if we're reversing, 1 if going forward
        float directionFactor = Vector3.Dot(rb.velocity, transform.forward) < -0.2f ? -1.0f : 1.0f;
        // Clamp Rotation speed relative to maxVelocity
        float velocityRatio = directionFactor * ((rb.velocity.magnitude > maxVelocityMagnitude) ? 1.0f : (rb.velocity.magnitude / maxVelocityMagnitude));

        transform.Rotate(Vector3.up * velocityRatio * state.ThumbSticks.Left.X * Time.deltaTime * turnSpeed);

        //rb.AddForce(transform.right * rb.velocity.magnitude * state.ThumbSticks.Left.X);

        rb.AddForce(transform.forward * Time.deltaTime * forwardSpeed * state.Triggers.Right);

        rb.AddForce(-transform.forward * Time.deltaTime * forwardSpeed/2.0f * state.Triggers.Left);

        //rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocityMagnitude);

        if (state.Buttons.X == ButtonState.Pressed && previousState.Buttons.X == ButtonState.Released)
        {
            rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
        }
        targetForward = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);
        if (targetForward == Vector3.zero)
            targetForward = transform.forward;
        //transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, targetForward, Time.deltaTime * 6.0f * Mathf.Clamp(rb.velocity.magnitude/2.0f, 0.0f, 1.0f)), Vector3.up);
        //transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, rb.velocity.normalized, Time.deltaTime * 10.0f));

       
        //float directionFactor = Mathf.Round((state.Triggers.Right) + (state.Triggers.Left * -1));

        rb.AddForce(Vector3.down * Time.deltaTime * 20000.0f);


        previousState = state;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerControllerKart>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 50.0f, ForceMode.Impulse);
            //Rb.AddForce(collision.contacts[0].normal * collision.impulse.magnitude * 2.0f, ForceMode.Impulse);
        }
    }

    public void DisableControls()
    {
        //StartCoroutine(Disable());

        DisableInputs = true;
    }

    //IEnumerator Disable()
    //{
    //    DisableInputs = true;
    //    for(int i = 0; i < 30; i++)
    //    {
    //        rb.AddForce(transform.forward * forwardSpeed/3.0f * Time.deltaTime);
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    enabled = false;
    //}
}
